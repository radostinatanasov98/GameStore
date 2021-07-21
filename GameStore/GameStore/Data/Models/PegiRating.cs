namespace GameStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PegiRating
    {
        [Required]
        [Key]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        public string Name { get; init; }

        public IEnumerable<Game> Games { get; init; } = new List<Game>();
    }
}
