namespace GameStore.Models.Games
{
    using System.Collections.Generic;

    public class GameDetailsViewModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public string CoverImageUrl { get; init; }

        public string TrailerUrl { get; init; }

        public decimal Price { get; init; }
        
        public int MinimumRequirementsId { get; init; }

        public int RecommendedRequirementsId { get; init; }

        public string PublisherName { get; init; }

        public string PegiRating { get; init; }

        public IEnumerable<string> Genres { get; init; } = new List<string>();
    }
}
