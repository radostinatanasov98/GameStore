namespace GameStore.Models.Games
{
    public class AddGameFormModel
    {
        public string Name { get; init; }

        public string Description { get; set; }

        public string CoverImageUrl { get; init; }

        public decimal Price { get; init; }

        public string PegiRating { get; init; }
    }
}
