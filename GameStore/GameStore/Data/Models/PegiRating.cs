namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PegiRating
    {
        [Required]
        [Key]
        public int Id { get; init; }

        public string Name { get; init; }

        public IEnumerable<Game> Games { get; init; } = new List<Game>();
    }
}
