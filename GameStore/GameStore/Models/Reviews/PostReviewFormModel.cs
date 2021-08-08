namespace GameStore.Models.Reviews
{
    using System.Collections.Generic;

    public class PostReviewFormModel
    {
        public string Caption { get; init; }

        public int Rating { get; init; }

        public string Content { get; init; }

        public int GameId { get; init; }

        public int UserId { get; init; }

        public IEnumerable<int> Ratings { get; init; } = new List<int>();
    }
}
