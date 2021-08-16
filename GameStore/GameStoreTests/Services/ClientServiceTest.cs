namespace GameStore.Tests.Services
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Clients;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using GameStore.Services.Clients;
    using GameStore.Tests.Mocks;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static GameStore.Data.DataConstants.Client;

    public class ClientServiceTest
    {
        private const int gameId = 1;
        private const int clientId = 1;
        private const string userId = "testId";

        [Fact]
        public void AcceptFriendRequestShouldUpdateEntriesCorrectlyInDatabase()
        {
            // Arrange
            const int firstClientId = 1;
            const int secondClientId = 2;

            using var data = DatabaseMock.Instance;

            var request = new ClientRelationship
            {
                ClientId = firstClientId,
                FriendId = secondClientId,
                AreFriends = false,
                HasFriendRequest = true
            };

            data.ClientRelationships.AddRange(request, new ClientRelationship
            {
                ClientId = secondClientId,
                FriendId = firstClientId,
                AreFriends = false,
                HasFriendRequest = false
            });
            data.SaveChanges();

            var clientService = new ClientService(data);
            // Act
            clientService.AcceptFriendRequest(request);

            // Assert
            Assert.True(data.ClientRelationships.All(cr => cr.AreFriends == true));
        }

        [Fact]
        public void BecomeClientShouldAddCorrectEntryInDatabase()
        {
            // Arrange
            const string userId = "testId";
            var model = new BecomeClientFormModel
            {
                Name = "test",
                Description = "test description",
                ProfilePictureUrl = "",
                AreFriendsPrivate = false,
                AreGamesPrivate = true
            };

            using var data = DatabaseMock.Instance;

            var clientService = new ClientService(data);
            // Act
            clientService.BecomeClient(model, userId);
            var client = data.Clients.FirstOrDefault(c => c.UserId == userId);
            var shoppingCart = data.ShoppingCarts.FirstOrDefault(sc => sc.Client == client);

            // Assert
            Assert.NotNull(client);
            Assert.NotNull(shoppingCart);
            Assert.True(data.Clients.Any(c => c.UserId == userId));
            Assert.True(data.ShoppingCarts.Any(sc => sc.Client == client));
            Assert.True(client.DisplayName == model.Name);
            Assert.True(client.ProfilePictureUrl == model.ProfilePictureUrl);
            Assert.True(client.Description == model.Description);
            Assert.True(client.AreFriendsPrivate == model.AreFriendsPrivate);
            Assert.True(client.AreGamesPrivate == model.AreGamesPrivate);
        }

        [Fact]
        public void ClientOwnsGameShouldReturnTrueIfClientGamesContainsClientAndGame()
        {
            // Arrange
            using var data = DatabaseMock.Instance;
            ArrangeClientOwnsTestsDatabase(data);

            var clientService = new ClientService(data);
            // Act
            var result = clientService.ClientOwnsGame(clientId, gameId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ClientOwnsGameShouldReturnFalseIfClientGamesContainsClientAndGame()
        {
            // Arrange
            using var data = DatabaseMock.Instance;
            ArrangeClientOwnsTestsDatabase(data);

            var clientService = new ClientService(data);
            // Act
            var result = clientService.ClientOwnsGame(2, gameId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DeclineFriendRequestRemovesCorrectEntriesFromDatabase()
        {
            // Arrange
            const int firstClientId = 1;
            const int secondClientId = 2;

            using var data = DatabaseMock.Instance;

            var request = new ClientRelationship
            {
                ClientId = firstClientId,
                FriendId = secondClientId,
                AreFriends = false,
                HasFriendRequest = true
            };

            data.ClientRelationships.AddRange(request, new ClientRelationship
            {
                ClientId = secondClientId,
                FriendId = firstClientId,
                AreFriends = false,
                HasFriendRequest = false
            });

            data.ClientRelationships.AddRange(request, new ClientRelationship
            {
                ClientId = 3,
                FriendId = 4,
                AreFriends = false,
                HasFriendRequest = false
            });

            data.ClientRelationships.AddRange(request, new ClientRelationship
            {
                ClientId = 4,
                FriendId = 3,
                AreFriends = false,
                HasFriendRequest = false
            });
            data.SaveChanges();

            var clientService = new ClientService(data);
            // Act
            clientService.DeclineFriendRequest(request);

            // Assert
            Assert.False(data.ClientRelationships.Any(cr => cr.ClientId == 1 && cr.ClientId == 2));
        }

        [Fact]
        public void EditProfileShouldUpdateCorrectEntryInDatabase()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var client = new Client
            {
                Id = clientId,
                Description = "Empty",
                ProfilePictureUrl = "Empty",
                AreFriendsPrivate = false,
                AreGamesPrivate = false
            };

            data.Clients.Add(client);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            clientService.EditProfile(new EditProfileFormModel
            {
                ProfileId = clientId,
                Description = "Changed",
                PictureUrl = "Changed",
                AreFriendsPrivate = true,
                AreGamesPrivate = true
            });

            // Assert
            Assert.Equal("Changed", client.Description);
            Assert.Equal("Changed", client.ProfilePictureUrl);
            Assert.True(client.AreFriendsPrivate);
            Assert.True(client.AreGamesPrivate);
        }

        [Fact]
        public void GetClientByIdReturnsCorrectClient()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var client = new Client
            {
                Id = clientId
            };

            data.Clients.Add(client);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetClientById(clientId);

            // Assert
            Assert.Equal(clientId, result.Id);
        }

        [Fact]
        public void GetClientIdShouldReturnCorrectIdWhenUserIsClient()
        {
            // Arrange
            const string userId = "testId";
            const int id = 1;

            using var data = DatabaseMock.Instance;

            data.Clients.Add(new Client { UserId = userId, Id = id });
            data.SaveChanges();

            var clientService = new ClientService(data);
            // Act
            var result = clientService.GetClientId(userId);

            // Assert
            Assert.Equal(result, id);
        }

        [Fact]
        public void GetClientProfileViewModelShouldReturnCorrectModel()
        {
            // Arrange
            int? relationId = null;

            using var data = DatabaseMock.Instance;

            var client = new Client
            {
                Id = clientId,
                DisplayName = "Peter",
                UserId = userId,
                Description = null,
                AreGamesPrivate = true,
                AreFriendsPrivate = true,
                ProfilePictureUrl = null,
            };

            data.Clients.Add(client);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetClientProfileViewModel(clientId,
                clientId,
                relationId,
                false,
                client,
                new List<GameHoverViewModel>(),
                new List<ReviewViewModel>());

            // Assert
            Assert.Equal(-1, result.AvarageRating);
            Assert.Equal(0, result.ReviewsCount);
            Assert.False(result.Games.Any());
            Assert.False(result.Reviews.Any());
            Assert.Null(result.RelationId);
            Assert.Equal(clientId, result.ClientId);
            Assert.Equal(clientId, result.ProfileId);
            Assert.Equal(client.DisplayName, result.Username);
            Assert.Equal(client.Description, result.Description);
            Assert.Equal(client.ProfilePictureUrl, result.ProfilePictureUrl);
            Assert.True(result.AreGamesPrivate);
            Assert.True(result.AreFriendsPrivate);
            Assert.False(result.AreFriends);
        }

        private void ArrangeClientOwnsTestsDatabase(GameStoreDbContext data)
        {
            const int clientId = 1;
            const int gameId = 1;

            data.ClientGames
                .Add(new ClientGame
                {
                    Client = new Client
                    {
                        Id = clientId
                    },
                    Game = new Game
                    {
                        Id = gameId
                    }
                });

            data.SaveChanges();
        }

        [Fact]
        public void GetFriendsAndRequestsShouldReturnCorrectFriendsViewModelList()
        {
            // Arrange
            const int profileId = 2;
            using var data = DatabaseMock.Instance;

            data.Clients.Add(new Client
            {
                Id = 1
            });

            data.Clients.Add(new Client
            {
                Id = 2
            });

            data.ClientRelationships.Add(new ClientRelationship
            {
                ClientId = 1,
                FriendId = 2,
                AreFriends = true,
                HasFriendRequest = false
            });

            data.ClientRelationships.Add(new ClientRelationship
            {
                ClientId = 2,
                FriendId = 1,
                AreFriends = true,
                HasFriendRequest = false
            });

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetFriendsAndRequests(clientId, profileId);

            // Assert
            Assert.True(result.Count() == 1);
        }

        [Fact]
        public void GetOwnedGameIdsShouldReturnCorrectIds()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.ClientGames.Add(new ClientGame
            {
                GameId = 1,
                ClientId = clientId
            });
            data.ClientGames.Add(new ClientGame
            {
                GameId = 2,
                ClientId = clientId
            });

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetOwnedGameIds(clientId);

            // Assert
            Assert.True(result.Count() == 2);
            Assert.Equal(new List<int>() { 1, 2 }, result);
        }

        [Fact]
        public void GetRelationshipShouldReturnCorrectRelationship()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.ClientRelationships.Add(new ClientRelationship
            {
                ClientId = clientId,
                FriendId = 2
            });

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetRelationship(clientId, 2);

            // Assert
            Assert.Equal(clientId, result.ClientId);
            Assert.Equal(2, result.FriendId);
        }

        [Fact]
        public void GetRelationshipByIdShouldReturnCorrectRelationship()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.ClientRelationships.Add(new ClientRelationship
            {
                Id = 1,
                ClientId = clientId,
                FriendId = 2,
                AreFriends = false,
                HasFriendRequest = true
            });

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetRelationshipById(1);

            // Assert
            Assert.Equal(1, result.ClientId);
            Assert.Equal(2, result.FriendId);
            Assert.Equal(1, result.Id);
            Assert.False(result.AreFriends);
            Assert.True(result.HasFriendRequest);
        }

        [Fact]
        public void GetRelationIdShouldReturnCorrectIdIfClientsAreFriends()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var clientRelationship = new ClientRelationship
            {
                Id = 1,
                AreFriends = true
            };

            data.ClientRelationships.Add(clientRelationship);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetRelationId(true, clientRelationship);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void GetRelationIdShouldReturnNullIfClientsAreNotFriends()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var clientRelationship = new ClientRelationship
            {
                Id = 1,
                AreFriends = false
            };

            data.ClientRelationships.Add(clientRelationship);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.GetRelationId(false, clientRelationship);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void IsFriendRequestInvalidShouldReturnTrueIfSuchClientRelationshipExistsOrClientOwnsTheProfile()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.ClientRelationships.Add(new ClientRelationship
            {
                ClientId = 1,
                FriendId = 2,
            });

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var result = clientService.IsFriendRequestInvalid(1, 2);
            var resultForOwnProfile = clientService.IsFriendRequestInvalid(1, 1);

            // Assert
            Assert.True(result);
            Assert.True(resultForOwnProfile);
        }

        [Fact]
        public void IsFriendRequestInvalidShouldReturnFalseIfNoSuchClientRelationshipExists()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var clientService = new ClientService(data);

            // Act
            var result = clientService.IsFriendRequestInvalid(1, 2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void RelationCheckShouldReturnCorrectValuesDependingOnRelationship()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var firstRelation = new ClientRelationship
            {
                Id = 1,
                AreFriends = true
            };

            var secondRelation = new ClientRelationship
            {
                Id = 2,
                AreFriends = false
            };

            data.ClientRelationships.AddRange(firstRelation, secondRelation);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            var areFriends = clientService.RelationCheck(firstRelation);
            var areNotFriends = clientService.RelationCheck(secondRelation);
            var nonExistentRelation = clientService.RelationCheck(null);


            // Assert
            Assert.True(areFriends);
            Assert.False(areNotFriends);
            Assert.False(nonExistentRelation);
        }

        [Fact]
        public void RemoveProfilePictureShouldUpdateCorrectEntryInDatabase()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var client = new Client
            {
                Id = clientId,
                ProfilePictureUrl = "Default"
            };

            data.Clients.Add(client);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            clientService.RemoveProfilePicture(clientId);

            // Assert
            Assert.Equal(DefaultProfilePictureUrl, client.ProfilePictureUrl);
        }

        [Fact]
        public void SendFriendRequestShouldCreateCorrectEntriesInDatabase()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var firstRelation = new ClientRelationship
            {
                Id = 1,
                ClientId = 1,
                FriendId = 2,
                AreFriends = false,
                HasFriendRequest = false
            };

            var secondRelation = new ClientRelationship
            {
                Id = 2,
                ClientId = 2,
                FriendId = 1,
                AreFriends = false,
                HasFriendRequest = true
            };

            data.AddRange(firstRelation, secondRelation);

            data.SaveChanges();

            var clientService = new ClientService(data);

            // Act
            clientService.SendFriendRequest(1, 2);

            // Assert
            Assert.Equal(1, firstRelation.ClientId);
            Assert.Equal(2, firstRelation.FriendId);
            Assert.False(firstRelation.AreFriends);
            Assert.False(firstRelation.HasFriendRequest);
            Assert.Equal(2, secondRelation.ClientId);
            Assert.Equal(1, secondRelation.FriendId);
            Assert.False(secondRelation.AreFriends);
            Assert.True(secondRelation.HasFriendRequest);
        }
    }
}
