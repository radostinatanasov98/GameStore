namespace GameStore.Services.Reviews
{
    using GameStore.Data.Models;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;

    public interface IReviewService
    {
        public List<ReviewViewModel> GetReviewsForViewModel();

        public List<ReviewViewModel> SortByUser(string username);

        public List<ReviewViewModel> SortByGame(int gameId);

        public bool HasReviewed(int clientId, int gameId);

        public void CreateReview(string content,
            string caption,
            int rating,
            int clientId,
            int gameId);
    }
}
