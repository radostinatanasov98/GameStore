namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Infrastructure;
    using GameStore.Models;
    using GameStore.Models.Games;
    using GameStore.Models.Home;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class HomeController : Controller
    {
        private readonly GameStoreDbContext data;

        public HomeController(GameStoreDbContext data)
        {
            this.data = data;
        }

        public IActionResult Index()
        {
            var gameIdsByRatingQuery = this.data
                .Reviews
                .OrderByDescending(r => r.Rating)
                .Select(r => r.GameId)
                .Take(6)
                .ToList();

            var games = new List<GameHoverViewModel>();

            if (this.data.Reviews.Count() > 0)
            {
                foreach (var gameId in gameIdsByRatingQuery)
                {
                    if (!games.Any(g => g.GameId == gameId))
                    {
                        games.Add(this.data
                            .Games
                            .Where(g => g.Id == gameId)
                            .Select(g => new GameHoverViewModel
                            {
                                CoverImageUrl = g.CoverImageUrl,
                                GameId = g.Id,
                                Name = this.data.Games.First(gm => gm.Id == g.Id).Name
                            })
                            .First());
                    }
                }
            }
            else
            {
                games = this.data
                    .Games
                    .Take(6)
                    .Select(g => new GameHoverViewModel
                    {
                        CoverImageUrl = g.CoverImageUrl,
                        GameId = g.Id,
                        Name = this.data.Games.First(gm => gm.Id == g.Id).Name
                    })
                    .ToList();
            }

            var latestGames = this.data
                .Games
                .OrderByDescending(g => g.DateAdded)
                .Take(6)
                .Select(g => new GameHoverViewModel
                {
                    CoverImageUrl = g.CoverImageUrl,
                    GameId = g.Id,
                    Name = this.data.Games.First(gm => gm.Id == g.Id).Name
                })
                .ToList();

            var becomeUserType = new BecomePublisherOrClientHomeViewModel
            {
                IsClient = false,
                IsPublisher = false,
            };

            var reccommendedGames = new List<GameHoverViewModel>();

            if (this.User.Identity.IsAuthenticated)
            {
                becomeUserType.IsClient = IsUserClient();
                becomeUserType.IsPublisher = IsUserPublisher();

                var genres = new Dictionary<int, int>();

                if (IsUserClient())
                {
                    var client = this.data
                        .Clients
                        .First(c => c.UserId == this.User.GetId());

                    var gameGenres = this.data
                        .GameGenres
                        .ToList();

                    foreach (var gameGenre in gameGenres)
                    {
                        var currentGenre = this.data.Genres.First(g => g.Id == gameGenre.GameId);

                        if (this.data.ClientGames.Any(cg => cg.ClientId == client.Id && cg.GameId == gameGenre.GameId))
                        {
                            if (!genres.ContainsKey(currentGenre.Id)) genres.Add(currentGenre.Id, 0);

                            genres[currentGenre.Id]++;
                        }
                    }
                }

                var topThreeGenres = genres.OrderByDescending(g => g.Value).Take(3).Select(g => g.Key).ToList();

                if (topThreeGenres.Count() == 3)
                {
                    var currentGenre = topThreeGenres[0];

                    for (int i = 0; i < 6; i++)
                    {
                        if (i > 1 && i <= 3) currentGenre = topThreeGenres[1];
                        if (i > 3 && i <= 5) currentGenre = topThreeGenres[2];

                        var allGamesFromCurrentGenre = this.data
                            .Games
                            .Where(g => g.GameGenres.Any(gg => gg.GenreId == currentGenre))
                            .Select(g => new GameHoverViewModel
                            {
                                CoverImageUrl = g.CoverImageUrl,
                                GameId = g.Id,
                                Name = this.data.Games.First(gm => gm.Id == g.Id).Name
                            })
                            .ToList();

                        foreach (var game in allGamesFromCurrentGenre)
                        {
                            if (!reccommendedGames.Any(rg => rg.GameId == game.GameId))
                            {
                                reccommendedGames.Add(game);
                                break;
                            }
                        }
                    }
                }
                else if (topThreeGenres.Count() == 2)
                {
                    var currentGenre = topThreeGenres[0];

                    for (int i = 0; i < 6; i++)
                    {
                        if (i > 3) currentGenre = topThreeGenres[1];

                        var allGamesFromCurrentGenre = this.data
                            .Games
                            .Where(g => g.GameGenres.Any(gg => gg.GenreId == currentGenre))
                            .Select(g => new GameHoverViewModel
                            {
                                CoverImageUrl = g.CoverImageUrl,
                                GameId = g.Id,
                                Name = this.data.Games.First(gm => gm.Id == g.Id).Name
                            })
                            .ToList();

                        foreach (var game in allGamesFromCurrentGenre)
                        {
                            if (!reccommendedGames.Any(rg => rg.GameId == game.GameId))
                            {
                                reccommendedGames.Add(game);
                                break;
                            }
                        }
                    }
                }
                else if (topThreeGenres.Count() == 1)
                {
                    var currentGenre = topThreeGenres[0];

                    for (int i = 0; i < 6; i++)
                    {
                        var allGamesFromCurrentGenre = this.data
                            .Games
                            .Where(g => g.GameGenres.Any(gg => gg.GenreId == currentGenre))
                            .Select(g => new GameHoverViewModel
                            {
                                CoverImageUrl = g.CoverImageUrl,
                                GameId = g.Id,
                                Name = this.data.Games.First(gm => gm.Id == g.Id).Name
                            })
                            .ToList();

                        foreach (var game in allGamesFromCurrentGenre)
                        {
                            if (!reccommendedGames.Any(rg => rg.GameId == game.GameId))
                            {
                                reccommendedGames.Add(game);
                                break;
                            }
                        }
                    }
                }
            }

            var model = new HomePageViewModel
            {
                BecomeUserType = becomeUserType,
                TopRatedGames = games,
                LatestGames = latestGames,
                ReccommendedGames = reccommendedGames
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool IsUserPublisher()
            => this.data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => this.data.Clients.Any(p => p.UserId == this.User.GetId());
    }
}
