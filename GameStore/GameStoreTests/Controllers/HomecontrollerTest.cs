namespace GameStore.Tests.Controllers
{
    using GameStore.Controllers;
    using GameStore.Data.Models;
    using GameStore.Models.Home;
    using MyTested.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HomecontrollerTest
    {
        [Fact]
        public void IndexShouldReturnCorrectAmountOfGames()
            => MyMvc
                .Pipeline()
                .ShouldMap("/")
                .To<HomeController>(c => c.Index())
                .Which(controller => controller
                    .WithData(GetPublisher())
                    .WithData(GetGenre())
                    .WithData(GetPegiRating())
                    .WithData(GetGames()))
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<HomePageViewModel>()
                    .Passing(m => m.TopRatedGames.Count() == 5 && m.LatestGames.Count() == 5));

        [Fact]
        public void ErrorShouldReturnView()
        => MyController<HomeController>
            .Instance()
            .Calling(c => c.Error())
            .ShouldReturn()
            .View();

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
