namespace GameStore.Models.Chats
{
    using GameStore.Models.Messages;
    using System.Collections.Generic;

    public class ChatViewModel
    {
        public int ClientId { get; init; }

        public int FriendId { get; init; }

        public string FriendName { get; init; }

        public string ProfilePictureUrl { get; init; }

        public string Message { get; init; }

        public IEnumerable<MessageViewModel> Messages { get; init; } = new List<MessageViewModel>();
    }
}
