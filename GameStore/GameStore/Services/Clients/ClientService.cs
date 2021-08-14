namespace GameStore.Services.Clients
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Clients;
    using GameStore.Services.Games;
    using GameStore.Services.Reviews;
    using GameStore.Services.ShoppingCart;
    using System.Collections.Generic;
    using System.Linq;
    using static GameStore.Data.DataConstants.Client;

    public class ClientService : IClientService
    {
        private readonly GameStoreDbContext data;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IGamesService gamesService;
        private readonly IReviewService reviewService;

        public ClientService(GameStoreDbContext data)
        {
            this.data = data;
            this.shoppingCartService = new ShoppingCartService(data);
            this.gamesService = new GamesService(data);
            this.reviewService = new ReviewService(data);
        }

        public void BecomeClient(BecomeClientFormModel inputModel, string userId)
        {
            var client = CreateClient(inputModel, userId);

            this.data.Clients.Add(client);

            this.data.ShoppingCarts.Add(this.shoppingCartService.CreateShoppingCart(client));

            this.data.SaveChanges();
        }

        public Client GetClientByUserId(string userId)
            => this.data.Clients.FirstOrDefault(c => c.UserId == userId);

        public Client GetClientById(int profileId)
            => this.data.Clients.FirstOrDefault(c => c.Id == profileId);

        public int GetClientId(string userId)
            => this.data.Clients.First(c => c.UserId == userId).Id;

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

        public bool RelationCheck(ClientRelationship relationship)
            => relationship switch
            {
                null => false,
                _ => relationship.AreFriends
            };

        public ClientRelationship GetRelationship(int clientId, int profileId)
            => this.data.ClientRelationships.FirstOrDefault(cr => cr.ClientId == clientId && cr.FriendId == profileId);

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

        public ClientProfileViewModel GetClientProfileViewModel(int clientId, int profileId, int? relationId, bool hasRelation, Client profile)
            => new ClientProfileViewModel
            {
                RelationId = relationId,
                ClientId = clientId,
                ProfileId = profileId,
                AreFriends = hasRelation,
                Username = profile.Name,
                Games = this.gamesService.SortHoverModelByProfile(this.GetOwnedGameIds(profileId)),
                AreGamesPrivate = profile.AreGamesPrivate,
                AreFriendsPrivate = profile.AreFriendsPrivate,
                Description = profile.Description,
                Friends = this.GetFriendsAndRequests(profileId, clientId),
                ProfilePictureUrl = profile.ProfilePictureUrl,
                Reviews = this.reviewService.SortByUser(profile.Name),
                ReviewsCount = this.data.Reviews.Where(r => r.ClientId == profileId).Count(),
                AvarageRating = this.data.Reviews.Any(r => r.ClientId == profileId) ? this.data.Reviews.Where(r => r.ClientId == profileId).Average(r => r.Rating) : -1
            };

        public int? GetRelationId(bool hasRelation, ClientRelationship relationship)
            => hasRelation ? relationship.Id : null;

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

        public bool IsFriendRequestValid(int clientId, int profileId)
            => clientId == profileId || this.data.ClientRelationships.Any(cr => cr.ClientId == clientId && cr.FriendId == profileId);

        public ClientRelationship GetRelationshipById(int id)
            => this.data.ClientRelationships.First(cr => cr.Id == id);

        public void AcceptFriendRequest(ClientRelationship relationship)
        {
            var friendRelationship = this.GetRelationship(relationship.FriendId, relationship.ClientId);

            relationship.AreFriends = true;
            relationship.HasFriendRequest = false;
            friendRelationship.AreFriends = true;

            this.data.SaveChanges();
        }

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

        public void RemoveProfilePicture(int profileId)
        {
            this.GetClientById(profileId).ProfilePictureUrl = DefaultProfilePictureUrl;

            this.data.SaveChanges();
        }

        public bool ClientOwnsGame(int clientId, int gameId)
            => this.data.ClientGames.Any(cg => cg.ClientId == clientId && cg.GameId == gameId);

        public List<int> GetOwnedGameIds(int profileId)
            => this.data
            .ClientGames
            .Where(cg => cg.ClientId == profileId)
            .Select(cg => cg.GameId)
            .ToList();      
    }
}
