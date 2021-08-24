namespace GameStore.Areas.Admin.Models.Reviews
{
    public class AllReviewsForAdminViewModel
    {
        public int Id { get; init; }

        public int GameId { get; init; }

        public string Caption { get; init; }

        public string Content { get; init; }

        public string Username { get; init; }
    }
}
