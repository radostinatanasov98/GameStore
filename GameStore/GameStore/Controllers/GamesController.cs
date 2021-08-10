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

    public class GamesController : Controller
    {
        private readonly GameStoreDbContext data;

        public GamesController(GameStoreDbContext data)
            => this.data = data;

        public IActionResult All(string searchQuery, string sortQuery, string searchByQuery)
        {
            var games = new List<GameListingViewModel>();


            var gamesQuery = this.data.Games.ToList();

            if (searchQuery != null)
            {
                var tokens = searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                if (searchByQuery == "" || searchByQuery == "Name")
                {
                    foreach (var token in tokens)
                    {
                        var game = gamesQuery
                            .Where(g => g.Name.Contains(token))
                            .Select(g => new GameListingViewModel
                            {
                                Id = g.Id,
                                Name = g.Name,
                                CoverImageUrl = g.CoverImageUrl,
                                PegiRating = g.PegiRating.Name,
                                Genres = GetGameGenreNames(g, this.data),
                                DateAdded = g.DateAdded.ToString(),
                                Rating = this.data.Reviews.Any(r => r.GameId == g.Id) ? this.data.Reviews.Where(r => r.GameId == g.Id).Average(r => r.Rating) : 0
                            })
                            .FirstOrDefault();

                        if (game != null && !games.Any(g => g.Id == game.Id)) games.Add(game);
                    }
                }

                if (searchByQuery == "Genre")
                {
                    foreach (var currentGame in gamesQuery)
                    {
                        bool isGenre = true;

                        foreach (var token in tokens)
                        {
                            var genre = this.data.Genres.FirstOrDefault(g => g.Name.ToLower() == token.ToLower());
                            isGenre = genre != null && this.data.GameGenres.Any(gg => gg.GameId == currentGame.Id && gg.GenreId == genre.Id);
                            if (isGenre == false) break;
                        }

                        if (isGenre)
                        {


                            var game = new GameListingViewModel
                            {
                                Id = currentGame.Id,
                                Name = currentGame.Name,
                                CoverImageUrl = currentGame.CoverImageUrl,
                                PegiRating = this.data.PegiRatings.First(pr => pr.Id == currentGame.PegiRatingId).Name,
                                Genres = GetGameGenreNames(currentGame, this.data),
                                DateAdded = currentGame.DateAdded.ToString(),
                                Rating = this.data.Reviews.Any(r => r.GameId == currentGame.Id) ? this.data.Reviews.Where(r => r.GameId == currentGame.Id).Average(r => r.Rating) : 0
                            };

                            games.Add(game);
                        }
                    }
                }
            }
            else
            {
                games = gamesQuery
                    .Select(g => new GameListingViewModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                        CoverImageUrl = g.CoverImageUrl,
                        PegiRating = this.data.PegiRatings.First(pr => pr.Id == g.PegiRatingId).Name,
                        Genres = GetGameGenreNames(g, this.data),
                        DateAdded = g.DateAdded.ToString(),
                        Rating = this.data.Reviews.Any(r => r.GameId == g.Id) ? this.data.Reviews.Where(r => r.GameId == g.Id).Average(r => r.Rating) : 0
                    })
                    .ToList();
            }

            games = sortQuery switch
            {
                "Name" => games.OrderBy(gq => gq.Name).ToList(),
                "Rating" => games.OrderByDescending(gq => gq.Rating).ToList(),
                "Newest" => games.OrderByDescending(gq => gq.DateAdded).ToList(),
                "Oldest" => games.OrderBy(gq => gq.DateAdded).ToList(),
                _ => games.OrderBy(gq => gq.Id).ToList(),
            };

            var model = new AllGamesViewModel
            {
                Games = games,
                SearchQuery = searchQuery,
                Genres = GetGenres()
            };

            return View(model);
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
                DateAdded = DateTime.UtcNow,
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
            var gameQuery = this.data
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

            return View(gameQuery);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPost(int GameId)
        {
            if (!IsUserClient()) return BadRequest();

            var shoppingCartQuery = this.data
                .ShoppingCarts
                .FirstOrDefault(sc => sc.Client.UserId == this.User.GetId());

            var client = this.data.Clients.First(c => c.UserId == this.User.GetId());

            var userOwnsGame = this.data
                .ClientGames
                .Any(cg => cg.ClientId == client.Id && cg.GameId == GameId);

            if (userOwnsGame) return BadRequest();
            if (shoppingCartQuery.ShoppingCartProducts.Any(scp => scp.GameId == GameId)) return BadRequest();

            this.data
                .ShoppingCartProducts
                .Add(new ShoppingCartProduct
                {
                    GameId = GameId,
                    ShoppingCartId = shoppingCartQuery.Id
                });

            this.data.SaveChanges();

            return Redirect("/Clients/ShoppingCart");
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
            return View(new PostReviewFormModel
            {
                Ratings = new List<int> { 1, 2, 3, 4, 5 }
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult PostReview(int GameId, PostReviewFormModel model)
        {
            if (!IsUserClient()) return BadRequest();

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