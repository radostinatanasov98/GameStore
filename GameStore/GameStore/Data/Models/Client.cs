namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants.Shared;
    using static DataConstants.Client;

    public class Client
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string DisplayName { get; init; }

        [Required]
        public string UserId { get; init; }

        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        [RegularExpression(ImageUrlRegularExpression)]
        public string ProfilePictureUrl { get; set; }

        public bool AreFriendsPrivate { get; set; }

        public bool AreGamesPrivate { get; set; }

        public int ShoppingCartId { get; init; }

        public ShoppingCart ShoppingCart { get; init; }

        public IEnumerable<WishListGame> WishListGames { get; init; } = new List<WishListGame>();

        public IEnumerable<ClientGame> ClientGames { get; init; } = new List<ClientGame>();

        public IEnumerable<Review> Reviews { get; init; } = new List<Review>();
    }
}
