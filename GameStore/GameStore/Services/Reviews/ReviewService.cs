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

        public List<ReviewViewModel> GetReviewsForViewModel()
            => this.data
                    .Reviews
                    .Where(r => r.Content != null && r.Caption != null)
                    .Select(r => new ReviewViewModel
                    {
                        Username = this.data.Clients.FirstOrDefault(c => c.Id == r.ClientId).Name,
                        Caption = r.Caption,
                        Content = r.Content,
                        Rating = r.Rating,
                        GameId = r.GameId
                    })
                    .ToList();

        public List<ReviewViewModel> SortByUser(string username)
            => this.GetReviewsForViewModel()
            .Where(r => r.Username == username)
            .ToList();

        public List<ReviewViewModel> SortByGame(int gameId)
            => this.GetReviewsForViewModel()
            .Where(r => r.GameId == gameId)
            .ToList();

        public bool HasReviewed(int clientId, int gameId)
            => this.data
            .Reviews
            .Any(r => r.ClientId == clientId && r.GameId == gameId);

        public void CreateReview(string content, string caption, int rating, int clientId, int gameId)
        {
            this.data
                .Reviews
                .Add(new Review
                {
                    Content = content,
                    Caption = caption,
                    Rating = rating,
                    ClientId = clientId,
                    GameId = gameId
                });

            this.data.SaveChanges();
        }
    }
}
