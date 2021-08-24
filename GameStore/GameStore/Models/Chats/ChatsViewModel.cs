namespace GameStore.Models.Chats
{
    using System.Collections.Generic;

    public class ChatsViewModel
    {
        public int Id { get; init; }

        public IEnumerable<FriendChatsViewModel> Friends { get; init; } = new List<FriendChatsViewModel>();
    }
}
