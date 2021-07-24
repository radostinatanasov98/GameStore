namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Genre;

    public class Genre
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; init; }

        public IEnumerable<GameGenre> GameGenres { get; init; } = new List<GameGenre>();
    }
}
