﻿namespace GameStore.Services.Clients
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Clients;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;
    using System.Linq;
    using static GameStore.Data.DataConstants.Client;

    public class ClientService : IClientService
    {
        private readonly GameStoreDbContext data;

        public ClientService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public void AcceptFriendRequest(ClientRelationship relationship)
        {
            var friendRelationship = this.GetRelationship(relationship.FriendId, relationship.ClientId);

            relationship.AreFriends = true;
            relationship.HasFriendRequest = false;
            friendRelationship.AreFriends = true;

            this.data.SaveChanges();
        }

        public void BecomeClient(BecomeClientFormModel inputModel, string userId)
        {
            var client = this.CreateClient(inputModel, userId);

            this.data.Clients.Add(client);

            this.data.ShoppingCarts
                .Add(new ShoppingCart
                {
                    Client = client
                });

            this.data.SaveChanges();
        }

        public bool ClientOwnsGame(int clientId, int gameId)
            => this.data.ClientGames.Any(cg => cg.ClientId == clientId && cg.GameId == gameId);

        public void DeclineFriendRequest(ClientRelationship relationship)
        {
            var friendRelationship = this.GetRelationship(relationship.FriendId, relationship.ClientId); ;

            relationship.AreFriends = false;
            relationship.HasFriendRequest = false;

            this.data.Remove(relationship);
            this.data.Remove(friendRelationship);
            this.data.SaveChanges();
        }

        public void EditProfile(EditProfileFormModel inputModel)
        {
            var profile = this.GetClientById(inputModel.ProfileId);

            if (inputModel.PictureUrl != null) profile.ProfilePictureUrl = inputModel.PictureUrl;

            if (inputModel.Description != null) profile.Description = inputModel.Description;

            profile.AreFriendsPrivate = inputModel.AreFriendsPrivate;
            profile.AreGamesPrivate = inputModel.AreGamesPrivate;

            this.data.SaveChanges();
        }

        public Client GetClientById(int profileId)
            => this.data.Clients.FirstOrDefault(c => c.Id == profileId);

        public Client GetClientByUserId(string userId)
            => this.data.Clients.FirstOrDefault(c => c.UserId == userId);

        public int GetClientId(string userId)
            => this.data.Clients.First(c => c.UserId == userId).Id;

        public ClientProfileViewModel GetClientProfileViewModel(int clientId,
            int profileId,
            int? relationId,
            bool hasRelation,
            Client profile,
            List<GameHoverViewModel> games,
            List<ReviewViewModel> reviews)
            => new ClientProfileViewModel
            {
                RelationId = relationId,
                ClientId = clientId,
                ProfileId = profileId,
                AreFriends = hasRelation,
                Username = profile.Name,
                Games = games,
                AreGamesPrivate = profile.AreGamesPrivate,
                AreFriendsPrivate = profile.AreFriendsPrivate,
                Description = profile.Description,
                Friends = this.GetFriendsAndRequests(profileId, clientId),
                ProfilePictureUrl = profile.ProfilePictureUrl,
                Reviews = reviews,
                ReviewsCount = this.data.Reviews.Where(r => r.ClientId == profileId).Count(),
                AvarageRating = this.data.Reviews.Any(r => r.ClientId == profileId) ? this.data.Reviews.Where(r => r.ClientId == profileId).Average(r => r.Rating) : -1
            };

        public List<FriendsViewModel> GetFriendsAndRequests(int profileId, int clientId)
            => this.data
                    .ClientRelationships
                    .Where(cr => cr.ClientId == profileId && (cr.HasFriendRequest || cr.AreFriends))
                    .Select(cr => new FriendsViewModel
                    {
                        Id = cr.Id,
                        FriendId = cr.FriendId,
                        ClientId = cr.ClientId,
                        OwnerId = clientId,
                        HasRequest = cr.HasFriendRequest,
                        AreFriends = cr.AreFriends,
                        ProfilePictureUrl = this.data.Clients.First(c => c.Id == cr.FriendId).ProfilePictureUrl,
                        Username = this.data.Clients.First(c => c.Id == cr.FriendId).Name
                    })
                    .ToList();

        public List<int> GetOwnedGameIds(int profileId)
            => this.data
            .ClientGames
            .Where(cg => cg.ClientId == profileId)
            .Select(cg => cg.GameId)
            .ToList();

        public ClientRelationship GetRelationship(int clientId, int profileId)
            => this.data.ClientRelationships.FirstOrDefault(cr => cr.ClientId == clientId && cr.FriendId == profileId);

        public ClientRelationship GetRelationshipById(int id)
            => this.data.ClientRelationships.First(cr => cr.Id == id);

        public int? GetRelationId(ClientRelationship relationship)
            => relationship.AreFriends ? relationship.Id : null;

        public bool IsFriendRequestInvalid(int clientId, int profileId)
            => clientId == profileId || this.data.ClientRelationships.Any(cr => cr.ClientId == clientId && cr.FriendId == profileId);

        public bool RelationCheck(ClientRelationship relationship)
            => relationship switch
            {
                null => false,
                _ => relationship.AreFriends
            };

        public void RemoveProfilePicture(int profileId)
        {
            this.GetClientById(profileId).ProfilePictureUrl = DefaultProfilePictureUrl;

            this.data.SaveChanges();
        }

        public void SendFriendRequest(int clientId, int profileId)
        {
            var clientRelationship = new ClientRelationship
            {
                ClientId = clientId,
                FriendId = profileId,
                AreFriends = false,
                HasFriendRequest = false,
            };

            var friendRelationship = new ClientRelationship
            {
                ClientId = profileId,
                FriendId = clientId,
                AreFriends = false,
                HasFriendRequest = true
            };

            this.data.ClientRelationships.AddRange(clientRelationship, friendRelationship);

            this.data.SaveChanges();
        }

        private Client CreateClient(BecomeClientFormModel inputModel, string userId)
            => new Client
            {
                Name = inputModel.Name,
                UserId = userId,
                ProfilePictureUrl = inputModel.ProfilePictureUrl == null ? DefaultProfilePictureUrl : inputModel.ProfilePictureUrl,
                Description = inputModel.Description,
                AreFriendsPrivate = inputModel.AreFriendsPrivate,
                AreGamesPrivate = inputModel.AreGamesPrivate
            };
    }
}
