namespace GameStore.Services.Reviews
{
    using GameStore.Data.Models;
    using GameStore.Models.Reviews;
    using System.Collections.Generic;

    public interface IReviewService
    {
        public List<ReviewViewModel> GetReviewsForViewModel(Client profile);
    }
}
