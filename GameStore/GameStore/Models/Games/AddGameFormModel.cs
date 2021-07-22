using System.Collections.Generic;

namespace GameStore.Models.Games
{
    public class AddGameFormModel
    {
        public string Name { get; init; }

        public string Description { get; set; }

        public string CoverImageUrl { get; init; }

        public decimal Price { get; init; }

        public string PegiRating { get; init; }

        public string CPU { get; init; }

        public string GPU { get; init; }

        public string RAM { get; init; }

        public string VRAM { get; init; }

        public string Storage { get; init; }

        public string OS { get; init; }

        public IEnumerable<PegiRatingViewModel> PegiRatings { get; set; }
    }
}
