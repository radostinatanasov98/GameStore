namespace GameStore.Controllers
{
    using GameStore.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using GameStore.Infrastructure;
    using GameStore.Services.Users;
    using GameStore.Services.Clients;
    using System.Linq;
    using GameStore.Services.Reviews;
    using GameStore.Services.Games;
    using GameStore.Models.Clients;

    public class ProfileController : Controller
    {
        private readonly GameStoreDbContext data;
        private readonly IUserService userService;
        private readonly IClientService clientService;
        private readonly IGamesService gamesService;
        private readonly IReviewService reviewService;

        public ProfileController(GameStoreDbContext data)
        {
            this.data = data;
            this.userService = new UserService(data);
            this.clientService = new ClientService(data);
            this.gamesService = new GamesService(data);
            this.reviewService = new ReviewService(data);
        }

        [Authorize]
        public IActionResult Main(int profileId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("Error");

            var profile = this.clientService.GetClientById(profileId);

            if (profile == null) return BadRequest();

            var relationship = this.clientService.GetRelationship(this.clientService.GetClientId(this.User.GetId()), profileId);
            var hasRelation = this.clientService.RelationCheck(relationship);
            int? relationId = this.clientService.GetRelationId(relationship);

            var model = this.clientService.GetClientProfileViewModel(
                this.clientService.GetClientId(this.User.GetId()),
                profileId,
                relationId,
                hasRelation,
                profile,
                this.gamesService.GetHoverModelForProfile(this.clientService.GetOwnedGameIds(profileId)),
                this.reviewService.SortByUser(profile.Name));

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Main")]
        public IActionResult MainPost(int profileId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("Error");

            var client = this.clientService.GetClientByUserId(this.User.GetId());

            if (this.clientService.IsFriendRequestInvalid(client.Id, profileId)) return Redirect("Error");

            this.clientService.SendFriendRequest(client.Id, profileId);

            return Redirect("~/");
        }

        [Authorize]
        public IActionResult Accept(int requestId)
        {
            var clientRelationship = this.clientService.GetRelationshipById(requestId);

            if (this.clientService.GetClientId(this.User.GetId()) != clientRelationship.ClientId) return Redirect("Error");

            this.clientService.AcceptFriendRequest(clientRelationship);

            return Redirect("/Games/All");
        }

        [Authorize]
        public IActionResult Decline(int requestId)
        {
            var clientRelationship = this.clientService.GetRelationshipById(requestId);

            if (this.clientService.GetClientId(this.User.GetId()) != clientRelationship.ClientId) return Redirect("Error");

            this.clientService.DeclineFriendRequest(clientRelationship);

            return Redirect("/Games/All");
        }

        [Authorize]
        public IActionResult Edit(int profileId, EditProfileFormModel model)
        {
            if (profileId != this.clientService.GetClientId(this.User.GetId())) return Redirect("Error");

            model.ProfileId = profileId;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(EditProfileFormModel inputModel)
        {
            if (inputModel.ProfileId != this.clientService.GetClientId(this.User.GetId())) return Redirect("Error");

            this.clientService.EditProfile(inputModel);

            return Redirect("/Profile/Main?ProfileId=" + inputModel.ProfileId);
        }

        [Authorize]
        public IActionResult RemoveProfilePicture(int profileId)
        {
            if (this.clientService.GetClientId(this.User.GetId()) != profileId) return Redirect("Error");

            this.clientService.RemoveProfilePicture(profileId);

            return Redirect("/Profile/Main?ProfileId=" + profileId);
        }
    }
}
