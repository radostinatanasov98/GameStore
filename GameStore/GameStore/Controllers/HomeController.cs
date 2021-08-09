﻿namespace GameStore.Controllers
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

            var games = new List<GameHomePageViewModel>();

            if (this.data.Reviews.Count() > 0)
            {
                foreach (var gameId in gameIdsByRatingQuery)
                {
                    games.Add(this.data
                        .Games
                        .Where(g => g.Id == gameId)
                        .Select(g => new GameHomePageViewModel
                        {
                            CoverImageUrl = g.CoverImageUrl,
                            GameId = g.Id
                        })
                        .First());
                }
            }
            else
            {
                games = this.data
                    .Games
                    .Take(6)
                    .Select(g => new GameHomePageViewModel
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
                .Select(g => new GameHomePageViewModel
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

            if (this.User.Identity.IsAuthenticated)
            {
                becomeUserType.IsClient = IsUserClient();
                becomeUserType.IsPublisher = IsUserPublisher();
            }

            var model = new HomePageViewModel
            {
                BecomeUserType = becomeUserType,
                TopRatedGames = games,
                LatestGames = latestGames
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
