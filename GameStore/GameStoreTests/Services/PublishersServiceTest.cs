namespace GameStore.Tests.Services
{
    using GameStore.Data.Models;
    using GameStore.Services.Publishers;
    using GameStore.Tests.Mocks;
    using Xunit;

    public class PublishersServiceTest
    {
        [Fact]
        public void GetPublisherIdShoulReturnCorrectIdWhenUserIsPublisher()
        {
            // Arrange
            const string userId = "testId";
            const int id = 1;

            using var data = DatabaseMock.Instance;

            data.Publishers.Add(new Publisher { UserId = userId, Id = id });
            data.SaveChanges();

            var publishersService = new PublisherService(data);

            // Act
            var result = publishersService.GetPublisherId(userId);

            // Assert
            Assert.Equal(result, id);
        }

        [Fact]
        public void GetPublisherByGameIdReturnsCorrectPublisher()
        {
            // Arrange
            const int gameId = 1;
            var publisher = new Publisher { Name = "test", Id = 1};

            using var data = DatabaseMock.Instance;

            data.Games.Add(new Game { Publisher = publisher, Id = gameId });
            data.SaveChanges();

            var publisherService = new PublisherService(data);

            // Act
            var result = publisherService.GetPublisherByGameId(gameId);

            // Assert
            Assert.Equal(result, publisher);
        }
    }
}
