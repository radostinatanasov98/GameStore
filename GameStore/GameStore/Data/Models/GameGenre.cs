namespace GameStore.Data.Models
{
    public class GameGenre
    {
        public int GameId { get; init; }

        public Game Game { get; init; }

        public int GenreId { get; init; }

        public Genre Genre { get; init; }
    }
}
