namespace GameStore.Data.Models
{
    public class ClientGame
    {
        public int ClientId { get; init; }

        public Client Client { get; init; }

        public int GameId { get; init; }

        public Game Game { get; init; }
    }
}