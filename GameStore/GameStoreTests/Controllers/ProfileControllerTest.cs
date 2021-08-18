namespace GameStore.Tests.Controllers
{
    using GameStore.Controllers;
    using GameStore.Data.Models;
    using GameStore.Models.Clients;
    using MyTested.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    using static Data.DataConstants.Client;

    public class ProfileControllerTest
    {
        [Fact]
        public void MainShouldReturnViewWithCorrectModelIfUserIsClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                    .WithData(GetClient())
                    .WithData(GetGenre())
                    .WithData(GetPegiRating())
                    .WithData(GetGames())
                    .WithData(GetClientGames()))
                .Calling(c => c.Main(1))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ClientProfileViewModel>(m => m.Games.Count() == 5 &&
                    m.ReviewsCount == 0 &&
                    m.AvarageRating == -1 &&
                    m.Friends.Count() == 0));

        [Fact]
        public void MainShouldRedirectToErrorPageIfUserIsNotClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Main(1))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void MainShouldRedirectToErrorPageIfProfileDoesNotExist()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.Main(32))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void MainPostShouldSendFriendRequestAndRedirectToIndex()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(new Client 
                        {
                            Id = 2,
                            DisplayName = "testName2",
                            UserId = "otherUserId"
                        }))
                .Calling(c => c.MainPost(2))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<ClientRelationship>(cr => cr.Count() == 2))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Profile/Main?ProfileId=2");

        [Fact]
        public void MainPostShouldRedirectToErrorPageIfUserIsNotClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.MainPost(2))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void MainPostShouldRedirectToErrorPageIfAlreadyAreFriendsOrIfRequestHasAlreadyBeenSent()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(new Client
                        {
                            Id = 2,
                            DisplayName = "testName2",
                            UserId = "otherUserId"
                        })
                        .WithData(GetClientRelationships()))
                .Calling(c => c.MainPost(2))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<ClientRelationship>(cr => cr.Count() == 2))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void AccpetShouldSetBothClientRelationshipEntitiesAreFriendsColumnToTrue()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(new Client
                        {
                            Id = 2,
                            DisplayName = "testName2",
                            UserId = "otherUserId"
                        })
                        .WithData(GetClientRelationships()))
                .Calling(c => c.Accept(1))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<ClientRelationship>(cr => cr.Count() == 2 &&
                    cr.First(cr => cr.Id == 1).AreFriends == true &&
                    cr.First(cr => cr.Id == 2).AreFriends == true))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Profile/Main?ProfileId=1");

        [Fact]
        public void AcceptShouldRedirectToErrorPageIfUserIsNotClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Accept(1))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void AcceptShouldRedirectToErrorPageIfClientIsNotRecipientOfTheRequest()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(new Client
                        {
                            Id = 2,
                            DisplayName = "testName2",
                            UserId = "otherUserId"
                        })
                        .WithData(GetClientRelationships()))
                .Calling(c => c.Accept(2))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<ClientRelationship>(cr => cr.Count() == 2 &&
                    cr.First(cr => cr.Id == 1).AreFriends == false &&
                    cr.First(cr => cr.Id == 2).AreFriends == false))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void DeclineShouldRemoveBothClientRelationshipEntriesFromTheDatabase()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(new Client
                        {
                            Id = 2,
                            DisplayName = "testName2",
                            UserId = "otherUserId"
                        })
                        .WithData(GetClientRelationships()))
                .Calling(c => c.Decline(1))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<ClientRelationship>(cr => cr.Count() == 0))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Profile/Main?ProfileId=1");

        [Fact]
        public void DeclineShouldRedirectToErrorPageIfUserIsNotAClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Decline(1))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void DeclineShouldRedirectToErrorPageIFClientIsNotRecipientOfRequest()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient())
                        .WithData(new Client
                        {
                            Id = 2,
                            DisplayName = "testName2",
                            UserId = "otherUserId"
                        })
                        .WithData(GetClientRelationships()))
                .Calling(c => c.Decline(2))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void EditShouldReturnViewWithCorrectModel()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.Edit(1, GetEditProfileFormModel()))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<EditProfileFormModel>());

        [Fact]
        public void EditShouldRedirectToErrorPageIfUserIsNotClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Edit(1, GetEditProfileFormModel()))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void EditShouldRedirectToErrorPageIfClientIsNotProfileOwner()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.Edit(2, GetEditProfileFormModel()))
            .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void EditPostShouldUpdateDatabaseEntryAndRedirectToProfilePage()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.Edit(new EditProfileFormModel
                {
                    AreFriendsPrivate = true,
                    AreGamesPrivate = true,
                    ProfileId = 1,
                    PictureUrl = "myNewUrl",
                    Description = "myNewDescription"
                }))
            .ShouldHave()
            .ValidModelState()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<Client>(client => client
                    .First(c => c.Id == 1).ProfilePictureUrl == "myNewUrl" &&
                    client.First(c => c.Id == 1).Description == "myNewDescription"))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Profile/Main?ProfileId=1");

        [Fact]
        public void EditPostShouldRedirectToErrorPageIfUserIsNotAClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.Edit(new EditProfileFormModel
                {
                    AreFriendsPrivate = true,
                    AreGamesPrivate = true,
                    ProfileId = 1,
                    PictureUrl = "myNewUrl",
                    Description = "myNewDescription"
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
        public void EditPostShouldRedirectToErrorPageIfClientIsNotOwnerOfProfile()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.Edit(new EditProfileFormModel
                {
                    AreFriendsPrivate = true,
                    AreGamesPrivate = true,
                    ProfileId = 2,
                    PictureUrl = "myNewUrl",
                    Description = "myNewDescription"
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
        public void RemoveProfilePictureShouldEditDatabaseEntryAndRedirectToProfilePage()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.RemoveProfilePicture(1))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<Client>(client => client
                    .First().ProfilePictureUrl == DefaultProfilePictureUrl))
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Profile/Main?ProfileId=1");

        [Fact]
        public void RemoveProfilePictureShouldRedirectToErrorPageIfUserIsNotClient()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId")))
                .Calling(c => c.RemoveProfilePicture(1))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        [Fact]
        public void RemoveProfilePictureShouldRedirectToErrorPageIfClientIsNotOwnerOfAccount()
            => MyController<ProfileController>
                .Instance(controller => controller
                    .WithUser(user => user
                        .WithIdentifier("testId"))
                        .WithData(GetClient()))
                .Calling(c => c.RemoveProfilePicture(2))
            .ShouldHave()
            .ValidModelState()
            .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .Redirect("/Home/Error");

        private static IEnumerable<ClientRelationship> GetClientRelationships()
        {
            yield return new ClientRelationship
            {
                ClientId = 1,
                FriendId = 2,
                AreFriends = false,
                HasFriendRequest = true
            };

            yield return new ClientRelationship
            {
                ClientId = 2,
                FriendId = 1,
                AreFriends = false,
                HasFriendRequest = false
            };
        }

        private static EditProfileFormModel GetEditProfileFormModel()
            => new EditProfileFormModel
            {
                AreFriendsPrivate = false,
                AreGamesPrivate = false,
                ProfileId = 1,
                PictureUrl = null,
                Description = null
            };

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

        private static PegiRating GetPegiRating()
            => new PegiRating();

        private static Genre GetGenre()
            => new Genre { Name = "test" };

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
    }
}
