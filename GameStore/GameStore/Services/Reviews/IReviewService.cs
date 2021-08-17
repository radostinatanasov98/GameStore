namespace GameStore.Services.Reviews
{
    using GameStore.Data.Models;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;

    public interface IReviewService
    {
        public List<ReviewViewModel> GetReviewsForViewModel(bool isAdmin, int? clientId);

        public List<ReviewViewModel> SortByUser(List<ReviewViewModel> reviews, string username);

        public List<ReviewViewModel> SortByGame(List<ReviewViewModel> reviews, int gameId);

        public void Edit(int clientId, int gameId, PostReviewFormModel model);

        public void Remove(int clientId, int gameId);

        public bool HasReviewed(int? clientId, int gameId);

        public bool IsOwner(int clientId, int reviewId);

        public void CreateReview(string content,
            string caption,
            int rating,
            int clientId,
            int gameId);
    }
}
