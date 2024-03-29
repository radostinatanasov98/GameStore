﻿namespace GameStore.Models.Games
{
    using GameStore.Models.PegiRatings;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class AddGameFormModel
    {
        [Required]
        [StringLength(Game.NameMaxLength, MinimumLength = Game.NameMinLength)]
        public string Name { get; init; }

        [Required]
        [StringLength(Game.DescriptionMaxLength, MinimumLength = Game.DescriptionMinLength)]
        public string Description { get; set; }

        [Required(ErrorMessage = Shared.InvalidUrlErrorMessage)]
        [RegularExpression(Shared.ImageUrlRegularExpression)]
        public string CoverImageUrl { get; init; }

        [Required(ErrorMessage = "Trailer link should be in format: 'https://www.youtube.com/watch?v=dusPBwR8MWM'")]
        [RegularExpression(Game.TrailerUrlRegularExpression)]
        public string TrailerUrl { get; init; }

        public decimal Price { get; init; }

        public int PegiRatingId { get; init; }

        [Required]
        [StringLength(Requirements.CPUMaxLength, MinimumLength = Requirements.CPUMinLength)]
        public string MinimumCPU { get; init; }

        [Required]
        [StringLength(Requirements.GPUMaxLength, MinimumLength = Requirements.GPUMinLength)]
        public string MinimumGPU { get; init; }

        [Required(ErrorMessage = Requirements.MemoryErrorMessage)]
        [RegularExpression(Requirements.MemoryRegularExpression,
            ErrorMessage = Requirements.MemoryErrorMessage)]
        public string MinimumRAM { get; init; }

        [Required(ErrorMessage = Requirements.MemoryErrorMessage)]
        [RegularExpression(Requirements.MemoryRegularExpression,
            ErrorMessage = Requirements.MemoryErrorMessage)]
        public string MinimumVRAM { get; init; }

        [Required(ErrorMessage = Requirements.StorageErrorMEssage)]
        [RegularExpression(Requirements.StorageRegularExpression,
            ErrorMessage = Requirements.StorageErrorMEssage)]
        public string MinimumStorage { get; init; }

        [Required]
        [StringLength(Requirements.OSMaxLength, MinimumLength = Requirements.OSMinLength)]
        public string MinimumOS { get; init; }

        [Required]
        [StringLength(Requirements.CPUMaxLength, MinimumLength = Requirements.CPUMinLength)]
        public string RecommendedCPU { get; init; }

        [Required]
        [StringLength(Requirements.GPUMaxLength, MinimumLength = Requirements.GPUMinLength)]
        public string RecommendedGPU { get; init; }

        [Required(ErrorMessage = Requirements.MemoryErrorMessage)]
        [RegularExpression(Requirements.MemoryRegularExpression,
            ErrorMessage = Requirements.MemoryErrorMessage)]
        public string RecommendedRAM { get; init; }

        [Required(ErrorMessage = Requirements.MemoryErrorMessage)]
        [RegularExpression(Requirements.MemoryRegularExpression,
            ErrorMessage = Requirements.MemoryErrorMessage)]
        public string RecommendedVRAM { get; init; }

        [Required(ErrorMessage = Requirements.StorageErrorMEssage)]
        [RegularExpression(Requirements.StorageRegularExpression,
            ErrorMessage = Requirements.StorageErrorMEssage)]
        public string RecommendedStorage { get; init; }

        [Required]
        [StringLength(Requirements.OSMaxLength, MinimumLength = Requirements.OSMinLength)]
        public string RecommendedOS { get; init; }
        
        [Required(ErrorMessage = "Games must have at least 1 genre.")]
        public IEnumerable<int> GenreIds { get; set; }

        public IEnumerable<PegiRatingViewModel> PegiRatings { get; set; }

        public IEnumerable<GenreViewModel> Genres { get; set; }
    }
}
