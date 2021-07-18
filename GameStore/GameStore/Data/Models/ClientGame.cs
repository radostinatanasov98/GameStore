namespace GameStore.Data.Models
{
    public class ClientGame
    {
        public string ClientId { get; init; }

        public Client Client { get; init; }

        public string GameId { get; init; }

        public Game Game { get; init; }
    }
}