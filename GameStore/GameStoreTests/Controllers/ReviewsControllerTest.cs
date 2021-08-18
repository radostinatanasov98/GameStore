namespace GameStore.Tests.Controllers
{
    using GameStore.Controllers;
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using MyTested.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ReviewsControllerTest
    {
        [Fact]
        public void AllShouldReturnViewWithCorrectModel()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetReviews())
                        .WithData(GetClient())
                        .WithData(GetPegiRating())
                        .WithData(GetGenre())
                        .WithData(GetGames()))
                .Calling(c => c.All(1))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<AllReviewsViewModel>(m => m
                    .Reviews.Count() == 6));

        [Fact]
        public void WriteShouldReturnViewWithCorrectModel()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(GetClientGame()))
                .Calling(c => c.Write(1))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<PostReviewFormModel>(m => m
                    .Ratings.Count() == 5));

        [Fact]
        public void WriteShouldRedirectToErrorPageIfUserIsNotAClient()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Write(1))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void WriteShouldRedirectToErrorPageIfClientDoesNotOwnTheGame()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.Write(1))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void WritePostShouldCreateDatabaseEntryAndRedirectToAllReviewsPage()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(GetClientGame()))
                .Calling(c => c.Write(1,
                    new PostReviewFormModel
                    {
                        Rating = 5,
                        Caption = "newCaption",
                        Content = "newContent"
                    }))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<Review>(review => review
                    .First().Rating == 5 &&
                    review.First().Caption == "newCaption" &&
                    review.First().Content == "newContent"))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Reviews/All?GameId=1");

        [Fact]
        public void WritePostShouldUpdateDatabaseEntryAndRedirectToAllReviewsPage()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(GetClientGame())
                        .WithData(GetReview()))
                .Calling(c => c.Write(1,
                    new PostReviewFormModel
                    {
                        Rating = 5,
                        Caption = "newCaption",
                        Content = "newContent"
                    }))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<Review>(review => review
                    .First().Rating == 5 &&
                    review.First().Caption == "newCaption" &&
                    review.First().Content == "newContent" &&
                    review.Count() == 1))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Reviews/All?GameId=1");

        [Fact]
        public void WritePostShouldRedirectToErrorPageIfUserIsNotClient()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Write(1,
                    new PostReviewFormModel
                    {
                        Rating = 5,
                        Caption = "newCaption",
                        Content = "newContent"
                    }))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void WritePostShouldRedirectToErrorPageIfClientDoesNotOwnTheGame()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(GetReview()))
                .Calling(c => c.Write(1,
                    new PostReviewFormModel
                    {
                        Rating = 5,
                        Caption = "newCaption",
                        Content = "newContent"
                    }))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void WritePostShouldReturnViewIfModelStateIsNotValid()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(GetClientGame()))
                .Calling(c => c.Write(1,
                    new PostReviewFormModel
                    {
                        Rating = 5,
                        Caption = null,
                        Content = "newContent"
                    }))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<PostReviewFormModel>(m => m
                    .Ratings.Count() == 5 &&
                    m.Caption == null &&
                    m.Content == "newContent"));

        [Fact]
        public void RemoveShouldRemoveEntryFromDatabaseAndRedirectToAllReviewsPage()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(GetClientGame())
                        .WithData(GetReview()))
                .Calling(c => c.Remove(1, 1))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<Review>(review => review.Count() == 0))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Reviews/All?GameId=1");

        [Fact]
        public void RemoveShouldRedirectToErrorPageIfUserIsNotClient()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClientGame())
                        .WithData(GetReview()))
                .Calling(c => c.Remove(1, 1))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void RemoveShouldRedirectToErrorPageIfClientIsNotOwnerOfReview()
            => MyController<ReviewsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(GetClientGame())
                        .WithData(new Review 
                        { 
                            GameId = 1,
                            ClientId = 2
                        }))
                .Calling(c => c.Remove(1, 1))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        private static Client GetClient()
        {
            var client = new Client
            {
                Id = 1,
                UserId = "testId",
                DisplayName = "testName",
                ShoppingCartId = 1,
                ProfilePictureUrl = "randomTestUrl"
            };

            return client;
        }

        private static Review GetReview()
            => new Review
            {
                ClientId = 1,
                GameId = 1,
                Rating = 3,
                Caption = "oldCaption",
                Content = "oldContent"
            };

        private static IEnumerable<Review> GetReviews()
            => Enumerable.Range(0, 6).Select(r => new Review
            {
                GameId = 1,
                Rating = 4,
                ClientId = 1,
                Caption = "test",
                Content = "test"
            });

        private static PegiRating GetPegiRating()
            => new PegiRating();

        private static Genre GetGenre()
            => new Genre { Name = "test" };

        private static ClientGame GetClientGame()
            => new ClientGame { ClientId = 1, GameId = 1};

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
