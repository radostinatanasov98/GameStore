namespace GameStore.Tests.Services
{
    using GameStore.Data.Models;
    using GameStore.Models.Reviews;
    using GameStore.Services.Reviews;
    using GameStore.Tests.Mocks;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ReviewServiceTest
    {
        public const string caption = "testCaption";
        public const string content = "testContent";
        public const string username = "testUser";

        [Fact]
        public void GetReviewsForViewModelShouldReturnCorrectModels()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            for (int i = 0; i < 5; i++)
            {
                data.Reviews.Add(new Review
                {
                    Caption = caption,
                    Content = content,
                    Rating = i,
                    GameId = i
                });
            }

            data.SaveChanges();

            var reviewService = new ReviewService(data);

            // Act
            var result = reviewService.GetReviewsForViewModel(false, -1);

            // Assert
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(caption, result[i].Caption);
                Assert.Equal(content, result[i].Content);
                Assert.Equal(i, result[i].Rating);
                Assert.Equal(i, result[i].GameId);
                Assert.Null(result[i].Username);
            }
        }

        [Fact]
        public void SortByUserShouldReturnProperModels()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            for (int i = 0; i < 5; i++)
            {
                data.Reviews.Add(new Review
                {
                    Client = i < 3 ? new Client { DisplayName = "test"} : null,
                    Caption = caption,
                    Content = content,
                    Rating = i,
                    GameId = i
                });
            }

            data.SaveChanges();

            var reviewService = new ReviewService(data);

            // Act
            var reviews = reviewService.GetReviewsForViewModel(false, -1);
            var result = reviewService.SortByUser(reviews, "test");

            // Assert
            Assert.True(result.Count == 3);
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(caption, result[i].Caption);
                Assert.Equal(content, result[i].Content);
                Assert.Equal(i, result[i].Rating);
                Assert.Equal(i, result[i].GameId);
                Assert.Equal("test", result[i].Username);
            }
        }

        [Fact]
        public void SortByGameShouldReturnProperModels()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            for (int i = 0; i < 5; i++)
            {
                data.Reviews.Add(new Review
                {
                    GameId = i < 3 ? 1 : 2,
                    Caption = caption,
                    Content = content,
                    Rating = i
                });
            }

            data.SaveChanges();

            var reviewService = new ReviewService(data);

            // Act
            var reviews = reviewService.GetReviewsForViewModel(false, -1);
            var result = reviewService.SortByGame(reviews, 1);

            // Assert
            Assert.True(result.Count == 3);
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(caption, result[i].Caption);
                Assert.Equal(content, result[i].Content);
                Assert.Equal(i, result[i].Rating);
                Assert.Equal(1, result[i].GameId);
                Assert.Null(result[i].Username);
            }
        }

        [Fact]
        public void HasReviewedShouldReturnTrueIfReviewsContainReviewWithGivenParams()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Reviews
                .Add(new Review
                {
                    ClientId = 1,
                    GameId = 1
                });

            data.SaveChanges();

            var reviewService = new ReviewService(data);

            // Act
            var result = reviewService.HasReviewed(1, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasReviewedShouldReturnFalseIfReviewsDoNotContainReviewWithGivenParams()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Reviews
                .Add(new Review
                {
                    ClientId = 1,
                    GameId = 0
                });

            data.SaveChanges();

            var reviewService = new ReviewService(data);

            // Act
            var result = reviewService.HasReviewed(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateReviewShouldAddProperEntityInDatabase()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var reviewService = new ReviewService(data);

            // Act
            reviewService.CreateReview(content,
                caption,
                5,
                1,
                2);

            var review = data.Reviews.First();

            // Assert
            Assert.Equal(content, review.Content);
            Assert.Equal(caption, review.Caption);
            Assert.Equal(5, review.Rating);
            Assert.Equal(1, review.ClientId);
            Assert.Equal(2, review.GameId);
        }
    }
}
