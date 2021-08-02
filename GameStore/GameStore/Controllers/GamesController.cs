﻿namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Games;
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

        public IActionResult Remove(int Id)
        {
            var game = this.data
                    .Games
                    .Where(g => g.Id == Id)
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

        private bool IsUserPublisher()
            => data.Publishers.Any(p => p.UserId == this.User.GetId());

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