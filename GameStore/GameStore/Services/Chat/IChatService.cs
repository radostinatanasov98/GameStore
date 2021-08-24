namespace GameStore.Services.Chat
{
    using GameStore.Models.Chats;
    using System.Collections.Generic;


    public interface IChatService
    {
        public List<FriendChatsViewModel> GetFriendsForChat(int clientId);

        public ChatViewModel GetChat(int clientId, int friendId);

        public void SendMessage(int clientId, int friendId, string message);

        public ChatsViewModel GetChatsViewModel(int clientId, List<FriendChatsViewModel> friends);
    }
}
