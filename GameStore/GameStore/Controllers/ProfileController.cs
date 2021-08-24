namespace GameStore.Controllers
{
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

        public ProfileController(IUserService userService,
            IClientService clientService,
            IGamesService gamesService,
            IReviewService reviewService)
        {
            this.userService = userService;
            this.clientService = clientService;
            this.gamesService = gamesService;
            this.reviewService = reviewService;
        }

        [Authorize]
        public IActionResult Main(int profileId)
        {
            bool isAdmin = this.User.IsAdmin();

            if (!this.userService.IsUserClient(this.User.GetId()) && !isAdmin)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var profile = this.clientService.GetClientById(profileId);

            if (profile == null)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var clientId = isAdmin ? -1 : this.clientService.GetClientId(this.User.GetId());
            var relationship = this.clientService.GetRelationship(clientId, profileId);
            var hasRelation = this.clientService.RelationCheck(relationship);
            int? relationId = this.clientService.GetRelationId(hasRelation, relationship);
            var reviews = this.reviewService.GetReviewsForViewModel(isAdmin, clientId);
            var model = this.clientService.GetClientProfileViewModel(
                clientId,
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
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var client = this.clientService.GetClientByUserId(this.User.GetId());

            if (this.clientService.IsFriendRequestInvalid(client.Id, profileId))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            this.clientService.SendFriendRequest(client.Id, profileId);

            return RedirectToAction(nameof(ProfileController.Main), "Profile", new { profileId = profileId });
        }

        [Authorize]
        public IActionResult Accept(int requestId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var clientRelationship = this.clientService.GetRelationshipById(requestId);

            if (this.clientService.GetClientId(this.User.GetId()) != clientRelationship.ClientId)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            this.clientService.AcceptFriendRequest(clientRelationship);

            return RedirectToAction(nameof(ProfileController.Main), "Profile", new { profileId = clientRelationship.ClientId });
        }

        [Authorize]
        public IActionResult Decline(int requestId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var clientRelationship = this.clientService.GetRelationshipById(requestId);

            if (this.clientService.GetClientId(this.User.GetId()) != clientRelationship.ClientId)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            this.clientService.DeclineFriendRequest(clientRelationship);

            return RedirectToAction(nameof(ProfileController.Main), "Profile", new { profileId = clientRelationship.ClientId });
        }

        [Authorize]
        public IActionResult Edit(int profileId, EditProfileFormModel model)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (profileId != this.clientService.GetClientId(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            model.ProfileId = profileId;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(EditProfileFormModel inputModel)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (inputModel.ProfileId != this.clientService.GetClientId(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            this.clientService.EditProfile(inputModel);

            return RedirectToAction(nameof(ProfileController.Main), "Profile", new { profileId = inputModel.ProfileId });
        }

        [Authorize]
        public IActionResult RemoveProfilePicture(int profileId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (this.clientService.GetClientId(this.User.GetId()) != profileId)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            this.clientService.RemoveProfilePicture(profileId);

            return RedirectToAction(nameof(ProfileController.Main), "Profile", new { profileId = profileId });
        }

        [Authorize]
        public IActionResult All()
            => View(this.clientService.GetClientsForAllView());

    }
}
