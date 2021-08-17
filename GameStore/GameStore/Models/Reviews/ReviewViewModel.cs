namespace GameStore.Models.Reviews
{
    public class ReviewViewModel
    {
        public int Id { get; init; }

        public string Username { get; init; }
        
        public int ClientId { get; init; }

        public string Caption { get; init; }

        public string Content { get; init; }

        public int Rating { get; init; }

        public int GameId { get; init; }

        public string GameName { get; init; }

        public bool CanEdit { get; init; }
    }
}
