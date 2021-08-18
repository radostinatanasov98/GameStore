using GameStore.Data;
using GameStore.Data.Models;
using GameStore.Models.Publishers;
using System.Collections.Generic;
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

        public void CreatePublisher(BecomePublisherFormModel model, string userId)
        {
            var validPublisher = new Publisher
            {
                DisplayName = model.Name,
                UserId = userId,
                PictureUrl = model.PictureUrl
            };

            this.data.Publishers.Add(validPublisher);

            this.data.SaveChanges();
        }

        public void EditLogo(EditLogoFormModel model)
        {
            this.data
                .Publishers
                .First(p => p.Id == model.Id)
                .PictureUrl = model.Url;

            this.data.SaveChanges();
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

        public List<PublisherViewModel> GetPublishers()
            => this.data
            .Publishers
            .Select(p => new PublisherViewModel
            {
                Name = p.DisplayName,
                PictureUrl = p.PictureUrl
            })
            .ToList();
    }
}
