namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ShoppingCart
    {
        [Key]
        [Required]
        public int Id { get; init; }

        [Required]
        public int ClientId { get; init; }

        public Client Client { get; init; }

        public IEnumerable<ShoppingCartProduct> ShoppingCartProducts { get; set; } = new List<ShoppingCartProduct>();
    }
}
