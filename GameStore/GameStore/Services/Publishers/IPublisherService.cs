namespace GameStore.Services.Publishers
{
    using GameStore.Data.Models;
    using GameStore.Models.Publishers;
    using System.Collections.Generic;

    public interface IPublisherService
    {
        public Publisher GetPublisherByGameId(int gameId);

        public int GetPublisherId(string userId);

        public void CreatePublisher(BecomePublisherFormModel model, string userId);

        public List<PublisherViewModel> GetPublishers();

        public void EditLogo(EditLogoFormModel model);
    }
}
