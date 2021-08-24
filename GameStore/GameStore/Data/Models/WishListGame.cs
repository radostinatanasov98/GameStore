namespace GameStore.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class WishListGame
    {
        [Key]
        [Required]
        public int Id { get; init; }

        public int GameId { get; init; }

        public Game Game { get; init; }

        public int ClientId { get; init; }

        public Client Client { get; init; }
    }
}
