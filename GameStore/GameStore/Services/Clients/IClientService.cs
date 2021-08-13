namespace GameStore.Services.Clients
{
    using GameStore.Data.Models;
    using GameStore.Models.Clients;
    using System.Collections.Generic;

    public interface IClientService
    {
        public void BecomeClient(BecomeClientFormModel inputModel, string userId);

        public int GetClientId(string userId);

        public Client GetClientById(int profileId);

        public Client GetClientByUserId(string userId);

        public bool RelationCheck(ClientRelationship relationship);

        public ClientRelationship GetRelationship(int clientId, int profileId);

        public ClientRelationship GetRelationshipById(int id);

        public int? GetRelationId(bool hasRelation, ClientRelationship profile);

        public List<FriendsViewModel> GetFriendsAndRequests(int profileId, int clientId);

        public ClientProfileViewModel GetClientProfileViewModel(int clientId, int profileId, int? relationId, bool hasRelation, Client profile);

        public void SendFriendRequest(int clientId, int profileId);

        public bool IsFriendRequestValid(int clientId, int profileId);

        public void AcceptFriendRequest(ClientRelationship relationship);

        public void DeclineFriendRequest(ClientRelationship relationship);

        public void EditProfile(EditProfileFormModel inputModel);

        public void RemoveProfilePicture(int profileId);

        public bool ClientOwnsGame(int clientId, int gameId);
    }
}
