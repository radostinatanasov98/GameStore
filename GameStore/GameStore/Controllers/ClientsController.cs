namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Clients;
    using GameStore.Models.Reviews;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using Services.Users;
    using static Data.DataConstants.Client;
    using Services.Clients;
    using Services.Games;
    using Services.ShoppingCart;
    using Services.Reviews;

    public class ClientsController : Controller
    {
        private readonly GameStoreDbContext data;
        private readonly IUserService userService;
        private readonly IClientService clientService;
        private readonly IGamesService gamesService;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IReviewService reviewService;

        public ClientsController(GameStoreDbContext data)
        {
            this.data = data;
            userService = new UserService(data);
            clientService = new ClientService(data);
            gamesService = new GamesService(data);
            shoppingCartService = new ShoppingCartService(data);
            reviewService = new ReviewService(data);
        }

        [Authorize]
        public IActionResult Become()
        {
            var userId = GetUserId();

            if (this.userService.IsUserClient(userId) || this.userService.IsUserPublisher(userId))
            {
                return BadRequest();
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Become(BecomeClientFormModel inputModel)
        {
            if (!this.User.Identity.IsAuthenticated) Redirect("Error");

            var userId = GetUserId();

            if (this.userService.IsUserClient(userId) || this.userService.IsUserPublisher(userId))
            {
                return Redirect("Error");
            }

            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            this.clientService.BecomeClient(inputModel, userId);
            
            return Redirect("/Games/All");
        }

        [Authorize]
        public IActionResult Library()
        {
            if (!this.User.Identity.IsAuthenticated) Redirect("Error");

            var userId = GetUserId();

            if (!userService.IsUserClient(userId)) return Redirect("Error");

            var gamesQuery = this.gamesService.GetGamesForLibraryView(this.User.GetId());

            return View(gamesQuery);
        }

        [Authorize]
        public IActionResult ShoppingCart()
        {
            var userId = GetUserId();

            if (!this.userService.IsUserClient(userId)) return Redirect("Error");

            var clientQuery = clientService.GetClientByUserId(userId);

            var shoppingCartProductsQuery = this.shoppingCartService.GetShoppingCartProducts(clientQuery.ShoppingCartId);

            var gamesQuery = this.gamesService.GetGamesForShoppingCartView(shoppingCartProductsQuery);

            var shoppingCartViewModel = this.shoppingCartService.GetShoppingCartViewModel(gamesQuery);

            return View(shoppingCartViewModel);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ShoppingCart")]
        public IActionResult ShoppingCartPost()
        {
            var clientQuery = this.clientService.GetClientByUserId(GetUserId());

            var shoppingCartProductsQuery = this.shoppingCartService.GetProducts(clientQuery.ShoppingCartId);

            this.shoppingCartService.Purchase(shoppingCartProductsQuery, clientQuery.Id);

            return Redirect("/Clients/Library");
        }

        public IActionResult ShoppingCartRemove(int gameId)
        {
            var clientQuery = this.clientService.GetClientByUserId(GetUserId());

            var productQuery = this.shoppingCartService.GetProduct(gameId, clientQuery.ShoppingCartId);

            if (productQuery == null) return Redirect("Error");

            this.shoppingCartService.RemoveProdutc(productQuery);

            return Redirect("/Clients/ShoppingCart");
        }

        [Authorize]
        public IActionResult Profile(int profileId)
        {
            if (!this.userService.IsUserClient(GetUserId())) return Redirect("Error");

            var profile = this.clientService.GetClientById(profileId);

            if (profile == null) return BadRequest();

            var relationship = this.clientService.GetRelationship(GetClientId(), profileId);
            var hasRelation = this.clientService.RelationCheck(relationship);
            int? relationId = this.clientService.GetRelationId(hasRelation, relationship);

            var model = this.clientService.GetClientProfileViewModel(GetClientId(), profileId, relationId, hasRelation, profile);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Profile")]
        public IActionResult ProfilePost(int profileId)
        {
            if (!this.userService.IsUserClient(GetUserId())) return Redirect("Error");

            var client = this.clientService.GetClientByUserId(this.User.GetId());

            if (this.clientService.IsFriendRequestValid(client.Id, profileId)) return Redirect("Error");

            this.clientService.SendFriendRequest(client.Id, profileId);

            return Redirect("~/");
        }

        [Authorize]
        public IActionResult Accept(int requestId)
        {
            var clientRelationship = this.clientService.GetRelationshipById(requestId);

            if (GetClientId() != clientRelationship.ClientId) return Redirect("Error");

            this.clientService.AcceptFriendRequest(clientRelationship);

            return Redirect("/Games/All");
        }

        // TRANSFER TO SERVICES
        [Authorize]
        public IActionResult Decline(int? requestId)
        {
            if (requestId == null) return BadRequest();

            var clientRelationship = this.data.ClientRelationships.First(cr => cr.Id == requestId);

            if (GetClientId() != clientRelationship.ClientId) return BadRequest();

            clientRelationship.AreFriends = false;
            clientRelationship.HasFriendRequest = false;

            var friendRelationship = this.data.ClientRelationships.First(cr => cr.ClientId == clientRelationship.FriendId);

            this.data.Remove(clientRelationship);
            this.data.Remove(friendRelationship);
            this.data.SaveChanges();

            return Redirect("/Games/All");
        }

        [Authorize]
        public IActionResult Edit(int profileId, EditProfileFormModel model)
        {
            if (profileId != GetClientId()) return BadRequest();

            model.ProfileId = profileId;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(EditProfileFormModel inputModel)
        {
            if (inputModel.ProfileId != GetClientId()) return BadRequest();
            var profile = this.data
                .Clients
                .FirstOrDefault(c => c.Id == inputModel.ProfileId);

            if (inputModel.PictureUrl != null) profile.ProfilePictureUrl = inputModel.PictureUrl;

            if (inputModel.Description != null) profile.Description = inputModel.Description;

            profile.AreFriendsPrivate = inputModel.AreFriendsPrivate;
            profile.AreGamesPrivate = inputModel.AreGamesPrivate;

            this.data.SaveChanges();

            return Redirect("/Clients/Profile?ProfileId=" + inputModel.ProfileId);
        }

        [Authorize]
        public IActionResult RemoveProfilePicture(int profileId)
        {
            if (GetClientId() != profileId) return BadRequest();

            this.data
                .Clients
                .First(c => c.Id == GetClientId())
                .ProfilePictureUrl = DefaultProfilePictureUrl;

            this.data.SaveChanges();

            return Redirect("/Clients/Profile?ProfileId=" + profileId);
        }

        private string GetUserId()
            => this.User.GetId();
        private bool IsUserPublisher()
            => this.data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => this.data.Clients.Any(p => p.UserId == this.User.GetId());

        private int GetClientId()
            => this.data.Clients.First(c => c.UserId == this.User.GetId()).Id;
    }
}
