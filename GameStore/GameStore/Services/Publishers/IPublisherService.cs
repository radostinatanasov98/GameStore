namespace GameStore.Services.Publishers
{
    using GameStore.Data.Models;

    public interface IPublisherService
    {
        public int GetPublisherId(string userId);

        public Publisher GetPublisherByGameId(int gameId);

    }
}
