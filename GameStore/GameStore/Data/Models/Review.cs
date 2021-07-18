namespace GameStore.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Review
    {
        [Required]
        [Key]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [MaxLength(25)]
        public string Caption { get; init; }

        [MaxLength(500)]
        public string Content { get; init; }

        public int Rating { get; init; }

        [Required]
        public string GameId { get; init; }

        public Game Game { get; init; }

        [Required]
        public string ClientId { get; set; }

        public Client Client { get; init; }
    }
}
