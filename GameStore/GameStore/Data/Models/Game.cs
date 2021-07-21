namespace GameStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Game
    {
        [Required]
        [Key]
        public string Id { get; init; } = Guid.NewGuid().ToString();

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
        public string MinimumRequirementsId { get; init; }

        public Requirements MinimumRequirements { get; init; }


        [Required]
        public string RecommendedRequirementsId { get; init; }

        public Requirements RecommendedRequirements { get; init; }

        [Required]
        public string PublisherId { get; init; }

        public Publisher Publisher { get; init; }

        public string PegiRatingId { get; init; }

        public PegiRating PegiRating { get; init; }

        public IEnumerable<Review> Reviews { get; init; } = new List<Review>();

        public IEnumerable<ClientGame> ClientGames { get; init; } = new List<ClientGame>();

        public IEnumerable<GameGenre> GameGenres { get; init; } = new List<GameGenre>();
    }
}