namespace GameStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Publisher
    {
        [Required]
        [Key]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(25)]
        public string Name { get; init; }

        [Required]
        public string UserId { get; init; }

        public IEnumerable<Game> Games { get; init; } = new List<Game>();
    }
}
