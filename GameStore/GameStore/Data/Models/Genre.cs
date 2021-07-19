namespace GameStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Genre
    {
        [Required]
        [Key]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        public string Name { get; init; }

        public IEnumerable<GameGenre> GameGenres { get; init; } = new List<GameGenre>();
    }
}
