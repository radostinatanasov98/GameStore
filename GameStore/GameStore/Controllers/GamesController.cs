namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Infrastructure;
    using GameStore.Models.Games;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using Services.Games;
    using GameStore.Services.Users;
    using GameStore.Services.Requirements;
    using GameStore.Services.Publishers;
    using GameStore.Services.ShoppingCart;
    using GameStore.Services.Clients;

    public class GamesController : Controller
    {
        private readonly GameStoreDbContext data;
        private readonly IGamesService gamesService;
        private readonly IUserService userService;
        private readonly IRequirementsService requirementsService;
        private readonly IPublisherService publisherService;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IClientService clientService;

        public GamesController(GameStoreDbContext data,
            IGamesService gamesService,
            IUserService userService,
            IRequirementsService requirementsService,
            IPublisherService publisherService,
            IShoppingCartService shoppingCartService,
            IClientService clientService)
        {
            this.data = data;
            this.gamesService = gamesService;
            this.userService = userService;
            this.requirementsService = requirementsService;
            this.publisherService = publisherService;
            this.shoppingCartService = shoppingCartService;
            this.clientService = clientService;
        }

        public IActionResult All(string searchQuery, string sortQuery, string searchByQuery)
            => View(
                this.gamesService.CreateAllGamesViewModel(
                this.gamesService.HandleSortQuery(sortQuery, this.gamesService.HandleSearchQueries(searchQuery, searchByQuery)),
                this.gamesService.GetGenres()
                ));

        [Authorize]
        public IActionResult Add()
        {
            if (!this.userService.IsUserPublisher(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View(this.gamesService.CreateAddGameFormModel());
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(AddGameFormModel inputModel)
        {
            if (!this.userService.IsUserPublisher(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (!ModelState.IsValid)
            {
                inputModel.PegiRatings = this.gamesService.GetPegiRatings();
                inputModel.Genres = this.gamesService.GetGenres();

                return View(inputModel);
            }

            this.gamesService.CreateGame(
                inputModel,
                this.requirementsService.CreateRequirements(
                    inputModel.MinimumCPU,
                    inputModel.MinimumGPU,
                    inputModel.MinimumRAM,
                    inputModel.MinimumVRAM,
                    inputModel.MinimumStorage,
                    inputModel.MinimumOS),
                this.requirementsService.CreateRequirements(
                    inputModel.RecommendedCPU,
                    inputModel.RecommendedGPU,
                    inputModel.RecommendedRAM,
                    inputModel.RecommendedVRAM,
                    inputModel.RecommendedStorage,
                    inputModel.RecommendedOS),
                this.publisherService.GetPublisherId(this.User.GetId()));

            return RedirectToAction(nameof(GamesController.All), "Games");
        }

        public IActionResult Details(int gameId)
        {
            if (!this.gamesService.GameExists(gameId))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var isClient = this.User.Identity.IsAuthenticated ?
                this.userService.IsUserClient(this.User.GetId()) : false;
            var clientId = isClient ?
                this.data.Clients.First(c => c.UserId == this.User.GetId()).Id : -1;
            
            return View(this.gamesService.GetGameDetailsViewModel(gameId, clientId));
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPost(int gameId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var shoppingCartQuery = this.shoppingCartService.GetShoppingCart(this.User.GetId());
            var client = this.clientService.GetClientByUserId(this.User.GetId());

            if (this.clientService.ClientOwnsGame(client.Id, gameId))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (this.shoppingCartService.GetProduct(gameId, client.ShoppingCartId) != null)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            this.shoppingCartService.AddShoppingCartProduct(shoppingCartQuery.Id, gameId);

            return RedirectToAction(nameof(ClientsController.ShoppingCart), "Clients");
        }

        [Authorize]
        public IActionResult Remove(int gameId)
        {
            if (!this.userService.IsUserPublisher(this.User.GetId()) && !this.User.IsAdmin())
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var game = this.gamesService.GetGameById(gameId);

            if (game == null)
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var publisherId = this.data.Games.FirstOrDefault(g => g.Id == gameId).PublisherId;
            var publisher = this.data.Publishers.FirstOrDefault(p => p.Id == publisherId);

            if (publisher.UserId != this.User.GetId() && !this.User.IsAdmin())
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var minRequirements = this.requirementsService.GetRequirementsById(game.MinimumRequirementsId);
            var recRequirements = this.requirementsService.GetRequirementsById(game.RecommendedRequirementsId);
            this.gamesService.RemoveGame(game, minRequirements, recRequirements);

            return RedirectToAction(nameof(GamesController.All), "Games");
        }
    }
}