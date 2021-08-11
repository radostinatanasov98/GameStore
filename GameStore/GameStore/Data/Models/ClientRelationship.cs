namespace GameStore.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ClientRelationship
    {
        [Key]
        public int Id { get; init; }

        public int ClientId { get; init; }

        public Client Client { get; init; }

        public int FirendId { get; init; }

        public Client Friend { get; init; }
    }
}
