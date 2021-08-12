namespace GameStore.Models.Clients
{
    public class FriendsViewModel
    {
        public string Username { get; init; }

        public string ProfilePictureUrl { get; init; }

        public int FriendId { get; init; }

        public int ClientId { get; init; }

        public int OwnerId { get; init; }

        public int Id { get; init; }

        public bool AreFriends { get; init; }

        public bool HasRequest { get; init; }
    }
}
