namespace GameStore.Tests.Services
{
    using GameStore.Data.Models;
    using GameStore.Services.Users;
    using GameStore.Tests.Mocks;
    using Xunit;

    public class UserServiceTest
    {
        private const string userId = "testId";

        [Fact]
        public void IsUserClientShouldReturnTrueIfUserIsClient()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Clients
                .Add(new Client
                {
                    UserId = userId
                });

            data.SaveChanges();

            var userService = new UserService(data);

            //Act
            var result = userService.IsUserClient(userId);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserClientShouldReturnFalseIfUserIsNotClient()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var userService = new UserService(data);

            //Act
            var result = userService.IsUserClient(userId);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUserPublisherShouldReturnTrueIfUserIsClient()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Publishers
                .Add(new Publisher
                {
                    UserId = userId
                });

            data.SaveChanges();

            var userService = new UserService(data);

            //Act
            var result = userService.IsUserPublisher(userId);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserPublisherShouldReturnFalseIfUserIsNotClient()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var userService = new UserService(data);

            //Act
            var result = userService.IsUserPublisher(userId);

            //Assert
            Assert.False(result);
        }
    }
}
