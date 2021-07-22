namespace GameStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Game
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(25)]
        public string Name { get; init; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(25)]
        public string CoverImageUrl { get; init; }

        public decimal Price { get; init; }

        [Required]
        public int MinimumRequirementsId { get; init; }

        public Requirements MinimumRequirements { get; init; }


        [Required]
        public int RecommendedRequirementsId { get; init; }

        public Requirements RecommendedRequirements { get; init; }

        [Required]
        public int PublisherId { get; init; }

        public Publisher Publisher { get; init; }

        public int PegiRatingId { get; init; }

        public PegiRating PegiRating { get; init; }

        public IEnumerable<Review> Reviews { get; init; } = new List<Review>();

        public IEnumerable<ClientGame> ClientGames { get; init; } = new List<ClientGame>();

        public IEnumerable<GameGenre> GameGenres { get; init; } = new List<GameGenre>();
    }
}