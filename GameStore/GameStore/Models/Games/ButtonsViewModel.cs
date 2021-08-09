namespace GameStore.Models.Games
{
    public class ButtonsViewModel
    {
        public bool IsPublisher { get; init; }

        public bool IsOwned { get; set; }

        public int GameId { get; init; }

        public decimal Price { get; init; }
    }
}
