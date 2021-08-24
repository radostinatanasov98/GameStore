namespace GameStore.Tests.Services
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Services.Games;
    using GameStore.Tests.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class GamesServiceTest
    {

        [Fact]
        public void GetGamesForAllViewShouldReturnProperModels()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetGamesForAllView();

            // Assert
            Assert.True(result.Count == 5);
            for (int i = 0; i < 5; i++)
            {
                Assert.True(result[i].Name == $"test{i}");
                Assert.True(result[i].CoverImageUrl == $"none{i}");
                Assert.True(result[i].Id == i + 1);
            }
        }

        [Fact]
        public void GetGamesForHoverModelShouldReturnCorrectModels()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetGamesForHoverModel();

            // Assert
            Assert.True(result.Count == 5);
            for (int i = 0; i < 5; i++)
            {
                Assert.True(result[i].Name == $"test{i}");
                Assert.True(result[i].CoverImageUrl == $"none{i}");
            }
        }

        [Fact]
        public void GetHoverModelForProfileShouldReturnCorrectModels()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetHoverModelForProfile(data.ClientGames
                .Where(cg => cg.ClientId == 1).Select(cg => cg.GameId).ToList());

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0].Name == "test0");
            Assert.True(result[0].CoverImageUrl == "none0");
        }

        [Fact]
        public void GetGamesForLibraryViewShouldreturnCorrectModels()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetGamesForLibraryView(1);

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0].Name == "test0");
            Assert.True(result[0].CoverImageUrl == "none0");
        }

        [Fact]
        public void GetGamesForShoppingCartViewShouldReturnCorrectModels()
        {
            // Arrange
            var data = this.GetData();

            data.ShoppingCartProducts.Add(new ShoppingCartProduct { GameId = 1, ShoppingCartId = 1 });
            data.SaveChanges();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetGamesForShoppingCartView(data.ShoppingCartProducts.Where(scp => scp.ShoppingCartId == 1));

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0].Name == "test0");
            Assert.True(result[0].ImageUrl == "none0");
        }

        [Fact]
        public void GetGamesListingSortedByNameShouldReturnProperModel()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetGamesListingSortedByName(new List<string>() { "test1", "test2" });

            // Assert
            Assert.True(result.Count == 2);
            Assert.True(result[0].Name == "test1");
            Assert.True(result[1].Name == "test2");
        }

        [Fact]
        public void GetGamesListingByGenreShouldReturnProperModels()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetGamesListingByGenre(new List<string>() { "test" });

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0].Name == $"test{0}");
            Assert.True(result[0].CoverImageUrl == $"none{0}");
            Assert.True(result[0].Id == 1);
        }

        [Fact]
        public void HandleSearchQueriesShouldReturnProperModels()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.HandleSearchQueries("test0", null);

            // Assert
            Assert.True(result.Count == 1);
            Assert.True(result[0].Name == $"test{0}");
            Assert.True(result[0].CoverImageUrl == $"none{0}");
            Assert.True(result[0].Id == 1);
        }

        [Fact]
        public void HandleSortQueryShouldReturnProperModels()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.HandleSortQuery("Name", gamesService.GetGamesForAllView());

            // Assert
            Assert.True(result.Count == 5);
            Assert.True(result[0].Name == $"test{0}");
            Assert.True(result[0].CoverImageUrl == $"none{0}");
            Assert.True(result[0].Id == 1);
        }

        [Fact]
        public void CreateAllGamesViewModelShouldReturnProperViewModel()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.CreateAllGamesViewModel(null, null, null, 1, 6);

            // Assert
            Assert.True(result.Games.Count() == 5);
            Assert.True(result.Genres.Count() == 1);
        }

        [Fact]
        public void GetPegiRatingsShouldReturnCorrectViewModel()
        {
            // Arrange
            using var data = this.GetData();

            var gamesService = new GamesService(data);
            // Act
            var result = gamesService.GetPegiRatings();

            // Assert
            Assert.True(result.Count == 1);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("test", result[0].Name);
        }

        [Fact]
        public void CreateAddGameFormModelShouldReturnCorrectViewModel()
        {
            // Arrange
            using var data = this.GetData();

            var gamesService = new GamesService(data);
            // Act
            var result = gamesService.CreateAddGameFormModel();

            // Assert
            Assert.True(result.PegiRatings.Count() == 1);
            Assert.True(result.Genres.Count() == 1);
            Assert.Equal("test", result.PegiRatings.First().Name);
            Assert.Equal("test", result.Genres.First().Name);
        }

        [Fact]
        public void CreateGameShouldCreateCorrectDataEntries()
        {
            // Arrange
            using var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            gamesService.CreateGame(new AddGameFormModel
            {
                Description = "specialDescriptionForThisTest",
                TrailerUrl = "https://www.youtube.com/watch?v=oHfGhuidwBg",
                Price = 39.99M,
                CoverImageUrl = "test",
                Name = "specialNameForThisTest",
                PegiRatingId = 1,
                GenreIds = new List<int>() { 1 },
                MinimumCPU = "test",
                MinimumGPU = "test",
                MinimumOS = "test",
                MinimumRAM = "test",
                MinimumStorage = "test",
                MinimumVRAM = "test",
                RecommendedCPU = "test",
                RecommendedGPU = "test",
                RecommendedOS = "test",
                RecommendedRAM = "test",
                RecommendedStorage = "test",
                RecommendedVRAM = "test"
            }, null, null, 1);

            var result = data.Games.FirstOrDefault(g => g.Name == "specialNameForThisTest");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Name == "specialNameForThisTest");
            Assert.Null(result.MinimumRequirements);
            Assert.Null(result.RecommendedRequirements);
            Assert.Equal(1, result.PegiRatingId);
            Assert.Equal("https://www.youtube.com/embed/oHfGhuidwBg", result.TrailerUrl);
            Assert.Equal(39.99M, result.Price);
        }

        [Fact]
        public void GetGameByIdShouldReturnCorrectGame()
        {
            // Arrange
            var data = this.GetData();

            var gamesService = new GamesService(data);

            // Act
            var result = gamesService.GetGameById(1);

            // Assert
            Assert.True(result.Name == "test0");
            Assert.True(result.CoverImageUrl == "none0");
            Assert.True(result.Id == 1);
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
