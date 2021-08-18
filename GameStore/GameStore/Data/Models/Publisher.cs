namespace GameStore.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Publisher;

    public class Publisher
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string DisplayName { get; init; }

        [Required]
        public string UserId { get; init; }

        public string PictureUrl { get; set; }

        public IEnumerable<Game> Games { get; init; } = new List<Game>();
    }
}
