namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
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
            if (!IsUserClient()) return BadRequest();

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
            var publisherId = this.data.Games.First(g => g.Id == gameId).PublisherId;
            var publisher = this.data.Publishers.First(p => p.Id == publisherId);

            if (publisher.UserId != this.User.GetId()) return BadRequest();

            var game = this.data
                    .Games
                    .Where(g => g.Id == gameId)
                    .FirstOrDefault();

            var minRequirements = this.data
                .Requirements
                .Where(r => r.Id == game.MinimumRequirementsId)
                .FirstOrDefault();

            var recRequirements = this.data
                .Requirements
                .Where(r => r.Id == game.RecommendedRequirementsId)
                .FirstOrDefault();

            this.data.Games.Remove(game);
            this.data.Requirements.Remove(minRequirements);
            this.data.Requirements.Remove(recRequirements);
            this.data.SaveChanges();

            return Redirect("/Games/All");
        }

        public IActionResult Reviews(int GameId)
        {
            var model = new AllReviewsViewModel
            {
                Name = this.data.Games.FirstOrDefault(g => g.Id == GameId).Name,
                GameId = GameId,
                Reviews = this.data
                            .Reviews
                            .Where(r => r.GameId == GameId && r.Content != null && r.Caption != null)
                            .Select(r => new ReviewViewModel
                            {
                                Username = this.data.Clients.FirstOrDefault(c => c.Id == r.ClientId).Name,
                                Caption = r.Caption,
                                Content = r.Content,
                                Rating = r.Rating
                            })
                            .ToList(),
            };

            return View(model);
        }

        public IActionResult PostReview(int GameId)
            => View(new PostReviewFormModel
            {
                Ratings = new List<int> { 1, 2, 3, 4, 5 }
            });

        [Authorize]
        [HttpPost]
        public IActionResult PostReview(int GameId, PostReviewFormModel model)
        {
            if (!IsUserClient()) return BadRequest();

            if ((model.Caption == null && model.Content != null) || (model.Caption != null && model.Content == null)) return BadRequest();

            var clientId = this.data
                .Clients
                .FirstOrDefault(c => c.UserId == this.User.GetId())
                .Id;

            var ownsGame = this.data
                .ClientGames
                .Any(cg => cg.ClientId == clientId && cg.GameId == GameId);

            var alreadyReviewed = this.data
                .Reviews
                .Any(r => r.ClientId == clientId && r.GameId == GameId);

            if (!ownsGame || alreadyReviewed) return BadRequest();

            var review = new Review
            {
                Caption = model.Caption,
                Content = model.Content,
                Rating = model.Rating,
                ClientId = clientId,
                GameId = GameId
            };

            this.data.Reviews.Add(review);

            this.data.SaveChanges();

            return Redirect("/Games/Reviews?GameId=" + GameId);
        }

        private bool IsUserPublisher()
            => data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => this.data.Clients.Any(p => p.UserId == this.User.GetId());

        private IEnumerable<PegiRatingViewModel> GetPegiRatings()
            => this.data
            .PegiRatings
            .Select(pr => new PegiRatingViewModel
            {
                Id = pr.Id,
                Name = pr.Name
            })
            .ToList();

        private IEnumerable<GenreViewModel> GetGenres()
            => this.data
            .Genres
            .Select(g => new GenreViewModel
            {
                Id = g.Id,
                Name = g.Name
            })
            .ToList();

        private static IEnumerable<string> GetGameGenreNames(Game game, GameStoreDbContext data)
        {
            var genreIds = data
                .GameGenres
                .Where(gg => gg.GameId == game.Id)
                .Select(gg => gg.GenreId)
                .ToList();

            var genres = new List<string>();

            foreach (var id in genreIds)
            {
                genres.Add(data.Genres.First(g => g.Id == id).Name);
            }

            return genres;
        }
    }
}