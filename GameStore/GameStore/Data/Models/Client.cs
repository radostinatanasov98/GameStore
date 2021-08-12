namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Client;

    public class Client
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; init; }

        [Required]
        public string UserId { get; init; }

        public string Description { get; set; }

        public string ProfilePictureUrl { get; set; }

        public bool AreFriendsPrivate { get; init; }

        public bool AreGamesPrivate { get; init; }

        public int ShoppingCartId { get; init; }

        public ShoppingCart ShoppingCart { get; init; }

        public IEnumerable<ClientGame> ClientGames { get; init; } = new List<ClientGame>();

        public IEnumerable<Review> Reviews { get; init; } = new List<Review>();

        public IEnumerable<ClientRelationship> Friends { get; init; } = new List<ClientRelationship>();
    }
}
