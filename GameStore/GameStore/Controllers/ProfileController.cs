namespace GameStore.Controllers
{
    using GameStore.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using GameStore.Infrastructure;
    using GameStore.Services.Users;
    using GameStore.Services.Clients;
    using GameStore.Services.Reviews;
    using GameStore.Services.Games;
    using GameStore.Models.Clients;

    public class ProfileController : Controller
    {
        private readonly IUserService userService;
        private readonly IClientService clientService;
        private readonly IGamesService gamesService;
        private readonly IReviewService reviewService;

        public ProfileController(GameStoreDbContext data)
        {
            this.userService = new UserService(data);
            this.clientService = new ClientService(data);
            this.gamesService = new GamesService(data);
            this.reviewService = new ReviewService(data);
        }

        [Authorize]
        public IActionResult Main(int profileId)
        {
            bool isAdmin = this.User.IsAdmin();

            if (!this.userService.IsUserClient(this.User.GetId()) && !isAdmin) return Redirect("/Home/Error");

            var profile = this.clientService.GetClientById(profileId);

            if (profile == null) return Redirect("/Home/Error");

            var clientId = this.clientService.GetClientId(this.User.GetId());

            var relationship = this.clientService.GetRelationship(clientId, profileId);
            var hasRelation = this.clientService.RelationCheck(relationship);
            int? relationId = this.clientService.GetRelationId(hasRelation, relationship);

            var reviews = this.reviewService.GetReviewsForViewModel(isAdmin, clientId);

            var model = this.clientService.GetClientProfileViewModel(
                this.clientService.GetClientId(this.User.GetId()),
                profileId,
                relationId,
                hasRelation,
                profile,
                this.gamesService.GetHoverModelForProfile(this.clientService.GetOwnedGameIds(profileId)),
                this.reviewService.SortByUser(reviews, profile.DisplayName));

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Main")]
        public IActionResult MainPost(int profileId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");

            var client = this.clientService.GetClientByUserId(this.User.GetId());

            if (this.clientService.IsFriendRequestInvalid(client.Id, profileId)) return Redirect("/Home/Error");

            this.clientService.SendFriendRequest(client.Id, profileId);

            return Redirect("/Profile/Main?ProfileId=" + profileId);
        }

        [Authorize]
        public IActionResult Accept(int requestId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");

            var clientRelationship = this.clientService.GetRelationshipById(requestId);

            if (this.clientService.GetClientId(this.User.GetId()) != clientRelationship.ClientId) return Redirect("/Home/Error");

            this.clientService.AcceptFriendRequest(clientRelationship);

            return Redirect("/Profile/Main?ProfileId=" + clientRelationship.ClientId);
        }

        [Authorize]
        public IActionResult Decline(int requestId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");

            var clientRelationship = this.clientService.GetRelationshipById(requestId);

            if (this.clientService.GetClientId(this.User.GetId()) != clientRelationship.ClientId) return Redirect("/Home/Error");

            this.clientService.DeclineFriendRequest(clientRelationship);

            return Redirect("/Profile/Main?ProfileId=" + clientRelationship.ClientId);
        }

        [Authorize]
        public IActionResult Edit(int profileId, EditProfileFormModel model)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");

            if (profileId != this.clientService.GetClientId(this.User.GetId())) return Redirect("/Home/Error");

            model.ProfileId = profileId;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(EditProfileFormModel inputModel)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");
            if (inputModel.ProfileId != this.clientService.GetClientId(this.User.GetId())) return Redirect("/Home/Error");

            this.clientService.EditProfile(inputModel);

            return Redirect("/Profile/Main?ProfileId=" + inputModel.ProfileId);
        }

        [Authorize]
        public IActionResult RemoveProfilePicture(int profileId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("/Home/Error");
            if (this.clientService.GetClientId(this.User.GetId()) != profileId) return Redirect("/Home/Error");

            this.clientService.RemoveProfilePicture(profileId);

            return Redirect("/Profile/Main?ProfileId=" + profileId);
        }

        [Authorize]
        public IActionResult All()
        {
            return View(this.clientService.GetClientsForAllView());
        }
    }
}
