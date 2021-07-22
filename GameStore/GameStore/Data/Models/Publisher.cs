namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Publisher
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(25)]
        public string Name { get; init; }

        [Required]
        public string UserId { get; init; }

        public IEnumerable<Game> Games { get; init; } = new List<Game>();
    }
}
