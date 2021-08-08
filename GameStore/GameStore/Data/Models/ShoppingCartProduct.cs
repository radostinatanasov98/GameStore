namespace GameStore.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ShoppingCartProduct
    {
        [Key]
        [Required]
        public int Id { get; init; }

        [Required]
        public int ShoppingCartId { get; init; }

        public ShoppingCart ShoppingCart { get; init; }

        [Required]
        public int GameId { get; init; }

        public Game Game { get; init; }
    }
}
