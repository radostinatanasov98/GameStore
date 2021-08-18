namespace GameStore.Models.Reviews
{
    using System.Collections.Generic;

    public class AllReviewsViewModel
    {
        public string Name { get; init; }

        public int GameId { get; init; }

        public bool HasReviewed { get; init; }

        public bool Owned { get; init; }

        public IEnumerable<ReviewViewModel> Reviews { get; init; } = new List<ReviewViewModel>();
    }
}
