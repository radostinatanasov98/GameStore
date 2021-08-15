namespace GameStore.Services.Clients
{
    using GameStore.Data.Models;
    using GameStore.Models.Clients;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;

    public interface IClientService
    {
        public void AcceptFriendRequest(ClientRelationship relationship);

        public void BecomeClient(BecomeClientFormModel inputModel, string userId);

        public bool ClientOwnsGame(int clientId, int gameId);

        public void DeclineFriendRequest(ClientRelationship relationship);

        public void EditProfile(EditProfileFormModel inputModel);

        public Client GetClientById(int profileId);

        public Client GetClientByUserId(string userId);

        public int GetClientId(string userId);

        public ClientProfileViewModel GetClientProfileViewModel(int clientId,
            int profileId,
            int? relationId,
            bool hasRelation,
            Client profile,
            List<GameHoverViewModel> games,
            List<ReviewViewModel> reviews);

        public List<FriendsViewModel> GetFriendsAndRequests(int profileId, int clientId);

        public List<int> GetOwnedGameIds(int profileId);

        public ClientRelationship GetRelationship(int clientId, int profileId);

        public ClientRelationship GetRelationshipById(int id);

        public int? GetRelationId(bool hasRelation, ClientRelationship profile);

        public bool IsFriendRequestInvalid(int clientId, int profileId);

        public bool RelationCheck(ClientRelationship relationship);

        public void RemoveProfilePicture(int profileId);

        public void SendFriendRequest(int clientId, int profileId);
    }
}
