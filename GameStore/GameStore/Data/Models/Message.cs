namespace GameStore.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Message
    {
        [Required]
        [Key]
        public int Id { get; init; }

        public int SenderId { get; init; }

        public int RecipientId { get; init; }

        public string Content { get; init; }

        public DateTime TimeStamp { get; init; }
    }
}
