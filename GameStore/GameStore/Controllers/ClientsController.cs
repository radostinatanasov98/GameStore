namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Infrastructure;
    using GameStore.Models.Clients;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using Services.Users;
    using Services.Clients;
    using Services.Games;
    using Services.ShoppingCart;
    using GameStore.Services.Reviews;

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
            this.userService = new UserService(data);
            this.clientService = new ClientService(data);
            this.gamesService = new GamesService(data);
            this.shoppingCartService = new ShoppingCartService(data);
            this.reviewService = new ReviewService(data);
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

        private string GetUserId()
            => this.User.GetId();

        private int GetClientId()
            => this.data.Clients.First(c => c.UserId == this.User.GetId()).Id;
    }
}
