namespace GameStore.Services.Reviews
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Games;
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

        public List<ReviewViewModel> GetReviewsForViewModel(bool isAdmin, int clientId)
            => this.data
                    .Reviews
                    .Where(r => r.Content != null && r.Caption != null)
                    .Select(r => new ReviewViewModel
                    {
                        Id = r.Id,
                        Username = this.data.Clients.FirstOrDefault(c => c.Id == r.ClientId).DisplayName,
                        ClientId = r.ClientId,
                        Caption = r.Caption,
                        Content = r.Content,
                        Rating = r.Rating,
                        GameId = r.GameId,
                        GameName = this.data.Games.FirstOrDefault(g => g.Id == r.GameId).Name,
                        CanEdit = isAdmin || clientId == r.ClientId
                    })
                    .OrderByDescending(r => r.ClientId == clientId)
                    .ToList();

        public void Edit(int clientId, int gameId, PostReviewFormModel model)
        {
            var review = this.data
                .Reviews
                .First(r => r.ClientId == clientId && r.GameId == gameId);

            review.Rating = model.Rating;
            review.Caption = model.Caption;
            review.Content = model.Content;

            this.data.SaveChanges();
        }

        public void Remove(int clientId, int gameId)
        {
            var review = this.data
                .Reviews
                .First(r => r.ClientId == clientId && r.GameId == gameId);

            this.data.Remove(review);

            this.data.SaveChanges();
        }

        public List<ReviewViewModel> SortByUser(List<ReviewViewModel> reviews, string username)
            => reviews
                .Where(r => r.Username == username)
                .ToList();


        public List<ReviewViewModel> SortByGame(List<ReviewViewModel> reviews, int gameId)
            => reviews
            .Where(r => r.GameId == gameId)
            .ToList();

        public bool HasReviewed(int? clientId, int gameId)
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

        public bool IsOwner(int clientId, int reviewId)
        {
            return this.data.Reviews.Any(r => r.Id == reviewId && r.ClientId == clientId);
        }

        public AllReviewsViewModel GetAllReviewsModel(List<ReviewViewModel> reviews,
            int clientId,
            int gameId,
            string name)
            => new AllReviewsViewModel
            {
                Name = name,
                GameId = gameId,
                Reviews = reviews,
                HasReviewed = this.HasReviewed(clientId, gameId),
                Owned = this.data.ClientGames.Any(cg => cg.ClientId == clientId && cg.GameId == gameId)
            };
    }
}
