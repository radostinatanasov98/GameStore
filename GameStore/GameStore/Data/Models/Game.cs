namespace GameStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static DataConstants.Game;

    public class Game
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; init; }

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public string CoverImageUrl { get; set; }

        [Required]
        public string TrailerUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime DateAdded { get; init; }

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

        public IEnumerable<ShoppingCartProduct> ShoppingCartProducts { get; init; } = new List<ShoppingCartProduct>();

        public IEnumerable<Review> Reviews { get; init; } = new List<Review>();

        public IEnumerable<ClientGame> ClientGames { get; init; } = new List<ClientGame>();

        public IEnumerable<GameGenre> GameGenres { get; init; } = new List<GameGenre>();
    }
}