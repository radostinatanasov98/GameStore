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

    public class ClientsController : Controller
    {
        private readonly GameStoreDbContext data;
        private readonly IUserService userService;
        private readonly IClientService clientService;
        private readonly IGamesService gamesService;
        private readonly IShoppingCartService shoppingCartService;

        public ClientsController(GameStoreDbContext data,
            IUserService userService,
            IClientService clientService,
            IGamesService gamesService,
            IShoppingCartService shoppingCartService)
        {
            this.data = data;
            this.userService = userService;
            this.clientService = clientService;
            this.gamesService = gamesService;
            this.shoppingCartService = shoppingCartService;
        }

        [Authorize]
        public IActionResult Become()
        {
            var userId = GetUserId();

            if (this.userService.IsUserClient(userId) || this.userService.IsUserPublisher(userId))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Become(BecomeClientFormModel inputModel)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var userId = GetUserId();

            if (this.userService.IsUserClient(userId) || this.userService.IsUserPublisher(userId))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            this.clientService.BecomeClient(inputModel, userId);

            return RedirectToAction(nameof(GamesController.All), "Games");
        }

        [Authorize]
        public IActionResult Library()
        {
            var userId = GetUserId();

            if (!userService.IsUserClient(userId))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var client = this.data.Clients.First(c => c.UserId == userId);

            var gamesQuery = this.gamesService.GetGamesForLibraryView(client.Id);

            return View(gamesQuery);
        }

        [Authorize]
        public IActionResult ShoppingCart()
        {
            var userId = GetUserId();

            if (!this.userService.IsUserClient(userId))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

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
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var clientQuery = this.clientService.GetClientByUserId(this.User.GetId());

            var shoppingCartProductsQuery = this.shoppingCartService.GetProducts(clientQuery.ShoppingCartId);

            this.shoppingCartService.Purchase(shoppingCartProductsQuery, clientQuery.Id);

            return RedirectToAction(nameof(ClientsController.Library), "Clients");
        }

        [Authorize]
        public IActionResult ShoppingCartRemove(int gameId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var clientQuery = this.clientService.GetClientByUserId(GetUserId());

            var productQuery = this.shoppingCartService.GetProduct(gameId, clientQuery.ShoppingCartId);

            if (productQuery == null) return RedirectToAction(nameof(HomeController.Error));

            this.shoppingCartService.RemoveProduct(productQuery);

            return RedirectToAction(nameof(ClientsController.ShoppingCart), "Clients");
        }

        [Authorize]
        public IActionResult WishList(int clientId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()) && this.User.IsAdmin())
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var model = new WishListViewModel
            {
                OwnerId = clientId,
                Games = this.gamesService.WishedGames(clientId)
            };

            return View(model);
        }

        [Authorize]
        public IActionResult WishListAdd(int gameId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var clientId = this.clientService.GetClientId(this.User.GetId());

            this.clientService.AddGameToWishList(clientId, gameId);

            return RedirectToAction(nameof(ClientsController.WishList), "Clients", new { clientId = clientId });
        }

        [Authorize]
        public IActionResult Gift(int clientId, int gameId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (this.clientService.GetClientId(this.User.GetId()) == clientId)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            this.clientService.Gift(clientId, gameId);

            return RedirectToAction(nameof(ClientsController.WishList), "Clients", new { clientId = clientId });
        }

        private string GetUserId()
            => this.User.GetId();
    }
}
