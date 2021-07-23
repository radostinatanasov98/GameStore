namespace GameStore.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Review;

    public class Review
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [MaxLength(CaptionLength)]
        public string Caption { get; init; }

        [MaxLength(ContentMaxLength)]
        public string Content { get; init; }

        public int Rating { get; init; }

        [Required]
        public int GameId { get; init; }

        public Game Game { get; init; }

        [Required]
        public int ClientId { get; set; }

        public Client Client { get; init; }
    }
}
