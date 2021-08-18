using System.Collections.Generic;

namespace GameStore.Models.Games
{
    public class GameListingViewModel
    {
        public int Id { get; init; }

        public string PublisherName { get; init; }

        public string Name { get; init; }

        public double Rating { get; init; }

        public string CoverImageUrl { get; init; }

        public string PegiRating { get; init; }

        public string DateAdded { get; init; }

        public IEnumerable<string> Genres { get; init; } = new List<string>();
    }
}
