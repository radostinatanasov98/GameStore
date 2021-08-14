using GameStore.Data;
using GameStore.Data.Models;
using System.Linq;

namespace GameStore.Services.Publishers
{
    public class PublisherService : IPublisherService
    {
        private readonly GameStoreDbContext data;

        public PublisherService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public Publisher GetPublisherByGameId(int gameId)
        {
            var publisherId = this.data.Games.First(g => g.Id == gameId).PublisherId;
            return this.data.Publishers.First(p => p.Id == publisherId);
        }

        public int GetPublisherId(string userId)
            => this.data
            .Publishers
            .Where(p => p.UserId == userId)
            .FirstOrDefault()
            .Id;


    }
}
