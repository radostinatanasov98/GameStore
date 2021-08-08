namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class GamesController : Controller
    {
        private readonly GameStoreDbContext data;

        public GamesController(GameStoreDbContext data)
            => this.data = data;

        public IActionResult All()
        {
            var gamesQuery = this.data
                    .Games
                    .Select(g => new GameListingViewModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                        CoverImageUrl = g.CoverImageUrl,
                        PegiRating = g.PegiRating.Name,
                        Genres = g.GameGenres
                            .Where(gg => gg.GameId == g.Id)
                            .Select(gg => gg.Genre.Name)
                            .ToList()
                    })
                    .ToList();

            return View(gamesQuery);
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!IsUserPublisher())
            {
                return BadRequest();
            }

            return View(new AddGameFormModel
            {
                PegiRatings = this.GetPegiRatings(),
                Genres = this.GetGenres()
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(AddGameFormModel game)
        {
            if (!IsUserPublisher())
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                game.PegiRatings = this.GetPegiRatings();
                game.Genres = this.GetGenres();

                return View(game);
            }

            var trailerUrlTokens = game.TrailerUrl.Split("watch?v=");

            var embedUrl = trailerUrlTokens[0] + "embed/" + trailerUrlTokens[1];

            var validGame = new Game
            {
                Name = game.Name,
                Description = game.Description,
                CoverImageUrl = game.CoverImageUrl,
                TrailerUrl = embedUrl,
                Price = game.Price,
                PegiRatingId = game.PegiRatingId,
                MinimumRequirements = new Requirements
                {
                    CPU = game.MinimumCPU,
                    GPU = game.MinimumGPU,
                    RAM = game.MinimumRAM,
                    VRAM = game.MinimumVRAM,
                    Storage = game.MinimumStorage,
                    OS = game.MinimumOS
                },
                RecommendedRequirements = new Requirements
                {
                    CPU = game.RecommendedCPU,
                    GPU = game.RecommendedGPU,
                    RAM = game.RecommendedRAM,
                    VRAM = game.RecommendedVRAM,
                    Storage = game.RecommendedStorage,
                    OS = game.RecommendedOS
                },
                PublisherId = this.data
                                  .Publishers
                                  .Where(p => p.UserId == this.User.GetId())
                                  .FirstOrDefault()
                                  .Id
            };

            this.data.Games.Add(validGame);

            foreach (var genreId in game.GenreIds)
            {
                var genre = this.data
                    .Genres
                    .Where(g => g.Id == genreId)
                    .FirstOrDefault();

                this.data.GameGenres.Add(new GameGenre
                {
                    Game = validGame,
                    Genre = genre
                });
            }

            this.data.SaveChanges();

            return Redirect("All");
        }

        public IActionResult Details(int GameId)
        {
            var gamesQuery = this.data
                    .Games
                    .Where(g => g.Id == GameId)
                    .Select(g => new GameDetailsViewModel
                    {
                        Id = GameId,
                        Name = g.Name,
                        PublisherName = g.Publisher.Name,
                        Price = g.Price,
                        Description = g.Description,
                        CoverImageUrl = g.CoverImageUrl,
                        TrailerUrl = g.TrailerUrl,
                        PegiRating = g.PegiRating.Name,
                        MinimumRequirementsId = g.MinimumRequirementsId,
                        RecommendedRequirementsId = g.RecommendedRequirementsId,
                        Genres = g.GameGenres.Select(gg => gg.Genre.Name)
                    })
                    .FirstOrDefault();

            return View(gamesQuery);
        }

        [Authorize]
        public IActionResult Remove(int GameId)
        {
            var gamePublisher = this.data.Publishers.Where(p => p.Games.Any(g => g.Id == GameId)).FirstOrDefault();

            if (gamePublisher.UserId != this.User.GetId()) return BadRequest();

            var game = this.data
                    .Games
                    .Where(g => g.Id == GameId)
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

        [Authorize]
        public IActionResult Purchase()
        {
            if (!IsClient()) return BadRequest();

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Purchase(PurchaseFormModel model)
        {
            if (!IsClient()) return BadRequest();

            var gameQuery = this.data
                .Games
                .Where(g => g.Id == model.GameId)
                .FirstOrDefault();

            if (gameQuery == null) return BadRequest();

            var clientQuery = this.data
                .Clients
                .Where(c => c.UserId == this.User.GetId())
                .FirstOrDefault();

            if (clientQuery == null) return BadRequest();

            this.data
                .ClientGames
                .Add(new ClientGame
                {
                    Client = clientQuery,
                    Game = gameQuery
                });

            this.data.SaveChanges();

            return Redirect("/Clients/Library");
        }

        public IActionResult Reviews(int GameId)
        {
            var model = new AllReviewsViewModel
            {
                Name = this.data.Games.FirstOrDefault(g => g.Id == GameId).Name,
                GameId = GameId,
                Reviews = this.data
                            .Reviews
                            .Where(r => r.GameId == GameId)
                            .Select(r => new ReviewViewModel
                            {
                                Username = this.data.Clients.FirstOrDefault(c => c.UserId == this.User.GetId()).Name,
                                Caption = r.Caption,
                                Content = r.Content,
                                Rating = r.Rating
                            })
                            .ToList(),
            };

            return View(model);
        }

        public IActionResult PostReview(int GameId)
        {
            return View( new PostReviewFormModel 
            { 
                Ratings = new List<int> { 1, 2, 3, 4, 5}
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult PostReview(int GameId, PostReviewFormModel model)
        {
            if (!IsClient()) return BadRequest();

            var clientId = this.data
                .Clients
                .FirstOrDefault(c => c.UserId == this.User.GetId())
                .Id;

            var ownsGame = this.data
                .ClientGames
                .Any(cg => cg.ClientId == clientId && cg.GameId == GameId);

            if (!ownsGame) return BadRequest();

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

        private bool IsClient()
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
    }
}