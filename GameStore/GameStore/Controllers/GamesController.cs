namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Infrastructure;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using Services.Games;
    using GameStore.Services.Users;
    using GameStore.Services.Requirements;
    using GameStore.Services.Publishers;
    using GameStore.Services.ShoppingCart;
    using GameStore.Services.Clients;
    using GameStore.Services.Reviews;

    public class GamesController : Controller
    {
        private readonly GameStoreDbContext data;
        private readonly IGamesService gamesService;
        private readonly IUserService userService;
        private readonly IRequirementsService requirementsService;
        private readonly IPublisherService publisherService;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IClientService clientService;
        private readonly IReviewService reviewService;

        public GamesController(GameStoreDbContext data)
        {
            this.data = data;
            this.gamesService = new GamesService(data);
            this.userService = new UserService(data);
            this.requirementsService = new RequirementsService(data);
            this.publisherService = new PublisherService(data);
            this.shoppingCartService = new ShoppingCartService(data);
            this.clientService = new ClientService(data);
            this.reviewService = new ReviewService(data);
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
            if (!this.userService.IsUserPublisher(this.User.GetId())) return Redirect("Error");

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
            => View(this.gamesService.GetGameDetailsViewModel(gameId));

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
            if (this.userService.IsUserPublisher(this.User.GetId())) return Redirect("Error");

            var game = this.gamesService.GetGameById(gameId);

            if (game == null) return Redirect("Error");

            var publisherId = this.data.Games.FirstOrDefault(g => g.Id == gameId).PublisherId;
            var publisher = this.data.Publishers.FirstOrDefault(p => p.Id == publisherId);

            if (publisher.UserId != this.User.GetId()) return BadRequest();

            var minRequirements = this.requirementsService.GetRequirementsById(game.MinimumRequirementsId);
            var recRequirements = this.requirementsService.GetRequirementsById(game.RecommendedRequirementsId);

            this.gamesService.RemoveGame(game, minRequirements, recRequirements);

            return Redirect("/Games/All");
        }

        public IActionResult Reviews(int gameId)
        {
            var model = new AllReviewsViewModel
            {
                Name = this.gamesService.GetGameById(gameId).Name,
                GameId = gameId,
                Reviews = this.reviewService.SortByGame(gameId)
            };

            return View(model);
        }

        public IActionResult PostReview()
            => View(new PostReviewFormModel
            {
                Ratings = new List<int> { 1, 2, 3, 4, 5 }
            });

        [Authorize]
        [HttpPost]
        public IActionResult PostReview(int gameId, PostReviewFormModel model)
        {
            if (!this.userService.IsUserClient(this.User.GetId())) return Redirect("Error");

            if ((model.Caption == null && model.Content != null) || (model.Caption != null && model.Content == null))
            {
                model.Ratings = new List<int> { 1, 2, 3, 4, 5 };

                return View(model);
            }

            var clientId = this.clientService.GetClientByUserId(this.User.GetId()).Id;

            var ownsGame = this.clientService.ClientOwnsGame(clientId, gameId);

            var alreadyReviewed = this.reviewService.HasReviewed(clientId, gameId);

            if (!ownsGame || alreadyReviewed) return Redirect("Error");

            this.reviewService.CreateReview(model.Content,
                model.Caption,
                model.Rating,
                clientId,
                gameId);

            return Redirect("/Games/Reviews?GameId=" + gameId);
        }
    }
}