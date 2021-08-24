namespace GameStore.Services.Chat
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Chats;
    using GameStore.Models.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChatService : IChatService
    {
        private readonly GameStoreDbContext data;

        public ChatService(GameStoreDbContext data)
            => this.data = data;

        public ChatViewModel GetChat(int clientId, int friendId)
        {
            var friend = this.data.Clients.First(c => c.Id == friendId);

            var messagesByClient = this.data
                .Messages
                .Where(m => m.SenderId == clientId && m.RecipientId == friendId)
                .Select(m => new MessageViewModel
                {
                    Content = m.Content,
                    TimeStamp = m.TimeStamp.ToString("HH:mm dd-MM-yy"),
                    OwnerId = clientId
                })
                .ToList();

            var messagesByFriend = this.data
                .Messages
                .Where(m => m.SenderId == friendId && m.RecipientId == clientId)
                .Select(m => new MessageViewModel
                {
                    Content = m.Content,
                    PictureUrl = friend.ProfilePictureUrl,
                    TimeStamp = m.TimeStamp.ToString("HH:mm dd-MM-yy"),
                    OwnerId = friendId
                })
                .ToList();

            var result = new List<MessageViewModel>();

            messagesByClient.ForEach(mvm => result.Add(mvm));
            messagesByFriend.ForEach(mvm => result.Add(mvm));

            result = result.OrderBy(m => m.TimeStamp).ToList();

            return new ChatViewModel 
            {
                FriendName = friend.DisplayName,
                ProfilePictureUrl = this.data.Clients.First(c => c.Id == clientId).ProfilePictureUrl,
                Messages = result,
                ClientId = clientId,
                FriendId = friendId
            };
        }

        public ChatsViewModel GetChatsViewModel(int clientId, List<FriendChatsViewModel> friends)
            => new ChatsViewModel
            {
                Id = clientId,
                Friends = friends
            };

        public List<FriendChatsViewModel> GetFriendsForChat(int clientId)
        {
            var friendIds = this.data
                .ClientRelationships
                .Where(cr => cr.ClientId == clientId && cr.AreFriends)
                .Select(cr => cr.FriendId)
                .ToList();

            return this.data
                .Clients
                .Where(c => friendIds.Contains(c.Id))
                .Select(c => new FriendChatsViewModel
                {
                    ImageUrl = c.ProfilePictureUrl,
                    FriendId = c.Id,
                    Username = c.DisplayName
                })
                .ToList();
        }

        public void SendMessage(int clientId, int friendId, string message)
        {
            this.data
                .Messages
                .Add(new Message
                {
                    Content = message,
                    SenderId = clientId,
                    RecipientId = friendId,
                    TimeStamp = DateTime.UtcNow
                });

            this.data.SaveChanges();
        }
    }
}

