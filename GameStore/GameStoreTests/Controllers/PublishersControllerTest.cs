namespace GameStore.Tests.Controllers
{
    using GameStore.Controllers;
    using GameStore.Data.Models;
    using GameStore.Models.Publishers;
    using MyTested.AspNetCore.Mvc;
    using System.Linq;
    using Xunit;

    public class PublishersControllerTest
    {
        [Fact]
        public void BecomeShouldReturnViewIfUserIsAuthenticatedAndNotChosenAType()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Publishers/Become")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<PublishersController>(c => c.Become())
            .Which()
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View();

        [Fact]
        public void BecomeShouldRedirectToErrorPageIfUserHasChosenAType()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Publishers/Become")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<PublishersController>(c => c.Become())
            .Which()
            .WithData(GetClient())
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void BecomePostShouldCreateDatabaseEntryAndRedirectToAddGamePage()
            => MyController<PublishersController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Become(new BecomePublisherFormModel 
                {
                    Name = "test",
                    PictureUrl= "https://www.clipartkey.com/mpngs/m/152-1520367_user-profile-default-image-png-clipart-png-download.png"
                }))
            .ShouldHave()
            .ValidModelState()
            .Data(data => data
                .WithSet<Publisher>(p => p
                    .First().DisplayName == "test"))
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Games/Add");

        [Fact]
        public void BecomePostShouldRedirectToErrorPageIfUserHasAlreadyChosenAType()
            => MyController<PublishersController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.Become(new BecomePublisherFormModel
                {
                    Name = "test",
                }))
            .ShouldHave()
            .Data(data => data
                .WithSet<Publisher>(p => p
                    .Count() == 0))
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
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
    }
}
