namespace GameStore.Services.Publishers
{
    using GameStore.Data.Models;
    using GameStore.Models.Publishers;

    public interface IPublisherService
    {
        public Publisher GetPublisherByGameId(int gameId);

        public int GetPublisherId(string userId);

        public void CreatePublisher(BecomePublisherFormModel model, string userId);
    }
}
