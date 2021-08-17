namespace GameStore.Tests.Controllers
{
    using GameStore.Tests.Mocks;
    using GameStore.Controllers;
    using Xunit;
    using System.Linq;
    using GameStore.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using GameStore.Models.Games;
    using System;
    using GameStore.Data;
    using MyTested.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class GamesControllerTest
    {
        [Fact]
        public void GetAllShouldDisplayAllGames()
            => MyMvc
                .Pipeline()
                .ShouldMap("/Games/All")
                .To<GamesController>(c => c.All(null, null, null))
                .Which(controller => controller
                    .WithData(GetPublisher())
                    .WithData(GetGenre())
                    .WithData(GetPegiRating())
                    .WithData(GetGames()))
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AllGamesViewModel>()
                    .Passing(m => m.Games.Count() == 5));

        [Fact]
        public void GetAllShouldDisplayGamesBySearchQuery()
        => MyMvc
        .Pipeline()
        .ShouldMap("/Games/All?SearchQuery=specialTest")
        .To<GamesController>(c => c.All("specialTest", null, null))
        .Which(controller => controller
            .WithData(GetPublisher())
            .WithData(GetGenre())
            .WithData(GetPegiRating())
            .WithData(GetGames())
            .WithData(new Game
            {
                PublisherId = 1,
                Description = "test",
                RecommendedRequirementsId = 1,
                MinimumRequirementsId = 1,
                Name = "specialTest",
                PegiRatingId = 1,
                Price = 39.99M,
                DateAdded = DateTime.UtcNow,
                CoverImageUrl = null,
                TrailerUrl = "https://www.youtube.com/embed/bjN-3EyXNyE"
            }))
        .ShouldReturn()
        .View(view => view
            .WithModelOfType<AllGamesViewModel>()
            .Passing(m => m.Games.Count() == 1 &&
                     m.Games.First().Name == "specialTest"));

        [Fact]
        public void GetAllShouldDisplayGamesBySortQuery()
        => MyMvc
        .Pipeline()
        .ShouldMap("/Games/All?SortQuery=Name")
        .To<GamesController>(c => c.All(null, "Name", null))
        .Which(controller => controller
            .WithData(GetPublisher())
            .WithData(GetGenre())
            .WithData(GetPegiRating())
            .WithData(GetGames())
            .WithData(new Game
            {
                PublisherId = 1,
                Description = "test",
                RecommendedRequirementsId = 1,
                MinimumRequirementsId = 1,
                Name = "1First",
                PegiRatingId = 1,
                Price = 39.99M,
                DateAdded = DateTime.UtcNow,
                CoverImageUrl = null,
                TrailerUrl = "https://www.youtube.com/embed/bjN-3EyXNyE"
            }))
        .ShouldReturn()
        .View(view => view
            .WithModelOfType<AllGamesViewModel>()
            .Passing(m => m.Games.Count() == 6 &&
                     m.Games.First().Name == "1First"));

        [Fact]
        public void AddShouldBeAcessableByPublishersAndReturnView()
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Games/Add")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .To<GamesController>(c => c.Add())
                .Which(controller => controller
                    .WithData(GetPublisher()))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddGameFormModel>());

        [Fact]
        public void AddShouldRedirectNonPublisherUsersToLoginPage()
        => MyMvc
        .Pipeline()
        .ShouldMap(request => request
            .WithPath("/Games/Add")
            .WithUser())
        .To<GamesController>(c => c.Add())
        .Which()
        .ShouldHave()
        .ActionAttributes(attributes => attributes
            .RestrictingForAuthorizedRequests())
        .AndAlso()
        .ShouldReturn()
        .Redirect("/Home/Error");

        [Fact]
        public void DetailsShouldDisplayDetailsOfCorrectGame()
            => MyMvc
                .Pipeline()
                .ShouldMap("/Games/Details?GameId=6")
                .To<GamesController>(c => c.Details(6))
                .Which(controller => controller
                    .WithData(GetPublisher())
                    .WithData(GetGenre())
                    .WithData(GetPegiRating())
                    .WithData(GetGames())
                    .WithData(new Review
                    {
                        Rating = 5,
                        GameId = 6
                    })
                    .WithData(new GameGenre 
                    {
                        GameId = 6,
                        GenreId = 1
                    })
                    .WithData(new Game
                    {
                        PublisherId = 1,
                        Description = "test",
                        RecommendedRequirementsId = 1,
                        MinimumRequirementsId = 1,
                        Name = "SixthGame",
                        PegiRatingId = 1,
                        Price = 39.99M,
                        DateAdded = DateTime.UtcNow,
                        CoverImageUrl = null,
                        TrailerUrl = "https://www.youtube.com/embed/bjN-3EyXNyE",
                    }))
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<GameDetailsViewModel>()
                    .Passing(m => m.Name == "SixthGame" && m.Price == 39.99M));

        [Fact]
        public void DetailsShouldRedirectToErrorPageIfProvidedGameIdIsInvalid()
            => MyMvc
                .Pipeline()
                .ShouldMap("/Games/Details?GameId=76")
                .To<GamesController>(c => c.Details(76))
                .Which(controller => controller
                    .WithData(GetPublisher())
                    .WithData(GetGenre())
                    .WithData(GetPegiRating())
                    .WithData(GetGames())
                .ShouldReturn()
                .Redirect("/Home/Error"));

        private static Publisher GetPublisher()
        {
            var publisher = new Publisher
            {
                UserId = "testId",
                DisplayName = "testName"
            };

            return publisher;
        }

        private static PegiRating GetPegiRating()
            => new PegiRating();

        private static Genre GetGenre()
            => new Genre { Name = "test" };

        private static IEnumerable<Game> GetGames()
            => Enumerable.Range(0, 5).Select(g => new Game
            {
                PublisherId = 1,
                Description = "test",
                RecommendedRequirementsId = 1,
                MinimumRequirementsId = 1,
                Name = "test",
                PegiRatingId = 1,
                Price = 39.99M,
                DateAdded = DateTime.UtcNow,
                CoverImageUrl = null,
                TrailerUrl = "https://www.youtube.com/embed/bjN-3EyXNyE"
            });
    }
}
