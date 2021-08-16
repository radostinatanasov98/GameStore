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

    public class Gamescontroller
    {
        [Fact]
        public void GetAllShouldDisplayAllGames()
        {
            // Arrange
            var data = this.GetData();

            var gamesController = new GamesController(data);

            // Act
            var result = gamesController.All(null, null, null);

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = viewResult.Model;

            var indexViewModel = Assert.IsType<AllGamesViewModel>(model);

            Assert.Equal(5, indexViewModel.Games.Count());
        }

        [Fact]
        public void AddShouldBeAcessedOnlyByPublishers()
        {

        }

        private GameStoreDbContext GetData()
        {
            var data = DatabaseMock.Instance;

            data.Clients.Add(new Client
            {
                Id = 1,
                ShoppingCartId = 1,
                Description = "empty",
                AreFriendsPrivate = true,
                AreGamesPrivate = true,
                DisplayName = "test",
                ProfilePictureUrl = "none",
                UserId = "testId"
            });

            data.SaveChanges();

            data.Publishers.Add(new Publisher
            {
                Id = 1,
                DisplayName = "test",
                UserId = "testId1"
            });

            data.SaveChanges();

            data.Genres.Add(new Genre
            {
                Id = 1,
                Name = "test"
            });

            data.SaveChanges();

            data.PegiRatings.Add(new PegiRating { Id = 1, Name = "test" });

            data.SaveChanges();

            for (int i = 0; i < 5; i++)
            {
                data.Games.Add(new Game
                {
                    Id = i + 1,
                    DateAdded = DateTime.UtcNow,
                    Description = $"test{i}",
                    TrailerUrl = "https://www.youtube.com/watch?v=oHfGhuidwBg",
                    Price = 39.99M,
                    CoverImageUrl = $"none{i}",
                    MinimumRequirements = null,
                    MinimumRequirementsId = i + 1,
                    RecommendedRequirements = null,
                    RecommendedRequirementsId = i + 2,
                    Name = $"test{i}",
                    PegiRating = data.PegiRatings.First(pr => pr.Id == 1),
                    PegiRatingId = 1,
                    PublisherId = 1,
                    Publisher = data.Publishers.First(p => p.Id == 1),
                });
            }

            data.GameGenres.Add(new GameGenre
            {
                GameId = 1,
                GenreId = 1
            });

            data.SaveChanges();

            data.ClientGames.Add(new ClientGame
            {
                Client = data.Clients.First(c => c.Id == 1),
                ClientId = 1,
                Game = data.Games.First(g => g.Id == 1),
                GameId = 1
            });

            data.SaveChanges();

            return data;
        }
    }
}
