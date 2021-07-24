namespace GameStore.Models.Games
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class AddGameFormModel
    {
        [Required]
        [MaxLength(Game.NameMaxLength)]
        public string Name { get; init; }

        [Required]
        [MinLength(Game.DescriptionMinLength)]
        [MaxLength(Game.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public string CoverImageUrl { get; init; }

        [Required]
        public string TrailerUrl { get; init; }

        public decimal Price { get; init; }

        public int PegiRatingId { get; init; }

        [Required]
        [MaxLength(Requirements.CPULength)]
        public string MinimumCPU { get; init; }

        [Required]
        [MaxLength(Requirements.GPULength)]
        public string MinimumGPU { get; init; }

        public int MinimumRAM { get; init; }

        public int MinimumVRAM { get; init; }

        public int MinimumStorage { get; init; }

        [Required]
        [MaxLength(Requirements.OSLength)]
        public string MinimumOS { get; init; }

        [Required]
        [MaxLength(Requirements.CPULength)]
        public string RecommendedCPU { get; init; }

        [Required]
        [MaxLength(Requirements.GPULength)]
        public string RecommendedGPU { get; init; }

        public int RecommendedRAM { get; init; }

        public int RecommendedVRAM { get; init; }

        public int RecommendedStorage { get; init; }

        [Required]
        [MaxLength(Requirements.OSLength)]
        public string RecommendedOS { get; init; }

        public IEnumerable<PegiRatingViewModel> PegiRatings { get; set; }
    }
}
