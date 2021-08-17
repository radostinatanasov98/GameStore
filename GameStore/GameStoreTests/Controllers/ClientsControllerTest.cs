namespace GameStore.Tests.Controllers
{
    using GameStore.Controllers;
    using GameStore.Data.Models;
    using GameStore.Models.Clients;
    using GameStore.Models.Games;
    using GameStore.Models.ShoppingCart;
    using MyTested.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ClientsControllerTest
    {
        [Fact]
        public void BecomeShouldReturnViewIfUserIsAuthenticatedAndHasNotChosenAType()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/Become")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.Become())
            .Which()
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View();

        [Fact]
        public void BecomeShouldNotBeAccesedByUsersWhichHaveAType()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/Become")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.Become())
            .Which()
            .WithData(GetPublisher())
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void BecomePostShouldRedirectToErrorPageIfUserIsNotAClient()
            => MyController<ClientsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                    .WithData(GetPublisher()))
                .Calling(c => c.Become(new BecomeClientFormModel()))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .Redirect("/Home/Error");

        [Fact]
        public void BecomePostShouldCreateANewClientIfModelStateIsValid()
            => MyController<ClientsController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Become(new BecomeClientFormModel
                {
                    Name = "Test",
                    AreFriendsPrivate = true,
                    AreGamesPrivate = true
                }))
                .ShouldHave()
                .ValidModelState()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .Redirect("/Games/All");

        [Fact]
        public void LibraryShouldReturnViewIfUserIsClientAndDoesntHaveGames()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/Library")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.Library())
            .Which()
            .WithData(GetClient())
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<GameListingViewModel>>());

        [Fact]
        public void LibraryShouldReturnViewWithCorrectAmountOfGamesIfUserIsClientAndHasPurchasedGames()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/Library")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.Library())
            .Which()
            .WithData(GetClient())
            .WithData(GetClientGames())
            .WithData(GetPegiRating())
            .WithData(GetGenre())
            .WithData(GetGames())
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<GameListingViewModel>>()
                .Passing(g => g.Count() == 5));

        [Fact]
        public void LibraryShouldRedirectToErrorPageIfUserIsNotAClient()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/Library")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.Library())
            .Which()
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void ShoppingCartShouldReturnViewIfUserIsClientAndItIsEmpty()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/ShoppingCart")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.ShoppingCart())
            .Which()
            .WithData(GetClient())
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ShoppingCartViewModel>());

        [Fact]
        public void ShoppingCartShouldReturnViewIfUserIsClientAndItHasGames()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/ShoppingCart")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.ShoppingCart())
            .Which()
            .WithData(GetClient())
            .WithData(new Publisher
            {
                Id = 1,
                DisplayName = "test",
                UserId = "Test123"
            })
            .WithData(GetPegiRating())
            .WithData(GetGenre())
            .WithData(GetGames())
            .WithData(GetShoppingCart())
            .WithData(GetShoppingCartProducts())
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ShoppingCartViewModel>()
                .Passing(scvw => scvw.Games.Count() == 5));

        [Fact]
        public void ShoppingCartShouldRedirectToErrorPageIfUserIsNotClient()
                        => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/ShoppingCart")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.ShoppingCart())
            .Which()
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void ShoppingCartPostShouldRedirectToLibraryPageUponPurchase()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/ShoppingCart")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.ShoppingCartPost())
            .Which()
            .WithData(GetClient())
            .WithData(new Publisher
            {
                Id = 1,
                DisplayName = "test",
                UserId = "Test123"
            })
            .WithData(GetPegiRating())
            .WithData(GetGenre())
            .WithData(GetGames())
            .WithData(GetShoppingCart())
            .WithData(GetShoppingCartProducts())
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<ClientGame>(cg => cg.Count() == 5))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Clients/Library");

        [Fact]
        public void ShoppingCartPostShouldRedirectToErrorPageIfUserIsNotClient()
            => MyMvc
            .Pipeline()
            .ShouldMap(request => request
                    .WithPath("/Clients/ShoppingCart")
                    .WithUser(user => user
                        .WithIdentifier("testId")))
            .To<ClientsController>(c => c.ShoppingCartPost())
            .Which()
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        private static Publisher GetPublisher()
        {
            var publisher = new Publisher
            {
                UserId = "testId",
                DisplayName = "testName"
            };

            return publisher;
        }

        private static Client GetClient()
        {
            var client = new Client
            {
                UserId = "testId",
                DisplayName = "testName",
                ShoppingCartId = 1
            };

            return client;
        }

        private static IEnumerable<ClientGame> GetClientGames()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new ClientGame
                {
                    ClientId = 1,
                    GameId = i + 1
                };
            }
        }

        private static PegiRating GetPegiRating()
         => new PegiRating 
            {
                Name = "test"
            };

        private static Genre GetGenre()
            => new Genre
            {
                Name = "test" 
            };

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
                CoverImageUrl = "TestImage",
                TrailerUrl = "https://www.youtube.com/embed/bjN-3EyXNyE"
            });

        private static IEnumerable<ShoppingCartProduct> GetShoppingCartProducts()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new ShoppingCartProduct
                {
                    ShoppingCartId = 1,
                    GameId = i + 1
                };
            }
        }

        private static ShoppingCart GetShoppingCart()
        {
            var shoppingCart = new ShoppingCart
            {
                ClientId = 1,
                Id = 1
            };

            return shoppingCart;
        }
    }
}

