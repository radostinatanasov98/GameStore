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

        public GamesController(GameStoreDbContext data)
        {
            this.data = data;
            this.gamesService = new GamesService(data);
            this.userService = new UserService(data);
            this.requirementsService = new RequirementsService(data);
            this.publisherService = new PublisherService(data);
            this.shoppingCartService = new ShoppingCartService(data);
            this.clientService = new ClientService(data);
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
            if (!this.userService.IsUserPublisher(this.User.GetId())) return Redirect("/Home/Error");

            return View(this.gamesService.CreateAddGameFormModel());
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(AddGameFormModel inputModel)
        {
            if (!this.userService.IsUserPublisher(this.User.GetId())) return Redirect("Error");

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

            return Redirect("All");
        }

        public IActionResult Details(int gameId)
        {
            if (!this.gamesService.GameExists(gameId)) return Redirect("/Home/Error");

            return View(this.gamesService.GetGameDetailsViewModel(gameId));
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPost(int gameId)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("Error");

            var shoppingCartQuery = this.shoppingCartService.GetShoppingCart(this.User.GetId());

            var client = this.clientService.GetClientByUserId(this.User.GetId());

            if (this.clientService.ClientOwnsGame(client.Id, gameId)) return Redirect("Error");
            if (this.shoppingCartService.GetShoppingCart(this.User.GetId()).ShoppingCartProducts.Any(scp => scp.GameId == gameId)) return Redirect("Error");

            this.shoppingCartService.AddShoppingCartProduct(shoppingCartQuery.Id, gameId);

            return Redirect("/Clients/ShoppingCart");
        }

        [Authorize]
        public IActionResult Remove(int gameId)
        {
            if (this.userService.IsUserPublisher(this.User.GetId()) && !this.User.IsAdmin()) return BadRequest();

            var game = this.gamesService.GetGameById(gameId);

            if (game == null) return Redirect("Error");

            var publisherId = this.data.Games.FirstOrDefault(g => g.Id == gameId).PublisherId;
            var publisher = this.data.Publishers.FirstOrDefault(p => p.Id == publisherId);

            if (publisher.UserId != this.User.GetId() && !this.User.IsAdmin()) return BadRequest();

            var minRequirements = this.requirementsService.GetRequirementsById(game.MinimumRequirementsId);
            var recRequirements = this.requirementsService.GetRequirementsById(game.RecommendedRequirementsId);

            this.gamesService.RemoveGame(game, minRequirements, recRequirements);

            return Redirect("/Games/All");
        }
    }
}