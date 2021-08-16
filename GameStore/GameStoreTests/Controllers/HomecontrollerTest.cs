namespace GameStore.Tests.Controller
{
    using GameStore.Controllers;
    using GameStore.Data.Models;
    using GameStore.Models.Home;
    using GameStore.Tests.Mocks;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using Xunit;

    public class HomecontrollerTest
    {
        [Fact]
        public void ErrorShouldReturnView()
        {
            // Arrange
            var homeController = new HomeController(null);

            // Act
            var result = homeController.Error();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void GetIndexShoulTakeCorrectAmountOfGames()
        {
            // Arrange
            var data = DatabaseMock.Instance;
            data.Games.AddRange(Enumerable.Range(0, 8).Select(g => new Game()));
            data.SaveChanges();

            var homeController = new HomeController(data);

            // Act
            var result = homeController.Index();

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = viewResult.Model;

            var indexViewModel = Assert.IsType<HomePageViewModel>(model);

            Assert.Equal(6, indexViewModel.TopRatedGames.Count());
            Assert.Equal(6, indexViewModel.LatestGames.Count());
        }
    }
}
