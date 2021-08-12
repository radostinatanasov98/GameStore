namespace GameStore.Services.Reviews
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;
    using System.Linq;

    public class ReviewService : IReviewService
    {
        private readonly GameStoreDbContext data;

        public ReviewService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public List<ReviewViewModel> GetReviewsForViewModel(Client profile)
            => this.data
                    .Reviews
                    .Where(r => r.ClientId == profile.Id && r.Content != null && r.Caption != null)
                    .Select(r => new ReviewViewModel
                    {
                        Username = profile.Name,
                        Caption = r.Caption,
                        Content = r.Content,
                        Rating = r.Rating
                    })
                    .ToList();
    }
}
