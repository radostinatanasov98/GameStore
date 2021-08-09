namespace GameStore.Models.Games
{
    public class GameShoppingCartViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public string Publisher { get; init; }

        public string PegiRating { get; init; }

        public decimal Price { get; init; }

        public string ImageUrl { get; init; }
    }
}
