namespace GameStore.Services.Publishers
{
    using GameStore.Data.Models;

    public interface IPublisherService
    {
        public Publisher GetPublisherByGameId(int gameId);

        public int GetPublisherId(string userId);
    }
}
