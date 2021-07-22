namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(25)]
        public string Name { get; init; }

        [Required]
        public string UserId { get; init; }

        public IEnumerable<ClientGame> ClientGames { get; init; } = new List<ClientGame>();

        public IEnumerable<Review> Reviews { get; init; } = new List<Review>();
    }
}
