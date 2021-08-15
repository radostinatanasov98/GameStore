namespace GameStore.Tests.Controller
{
    using GameStore.Controllers;
    using GameStore.Tests.Mocks;
    using Microsoft.AspNetCore.Mvc;
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
        public void IndexShouldReturnViewWithProperModel()
        {
            // Arrange
            var homeController = new HomeController(null);
            var gameService = GamesServiceMock.Instance;

            // Act

            // Assert
        }
    }
}
