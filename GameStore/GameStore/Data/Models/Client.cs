namespace GameStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        [Required]
        [Key]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(25)]
        public string Name { get; init; }

        [Required]
        public string UserId { get; init; }

        public IEnumerable<ClientGame> ClientGames { get; init; } = new List<ClientGame>();

        public IEnumerable<Review> Reviews { get; init; } = new List<Review>();
    }
}
