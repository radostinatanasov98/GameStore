﻿namespace GameStore.Data.Models
{
    public class GameGenre
    {
        public string GameId { get; init; }

        public Game Game { get; init; }

        public string GenreId { get; init; }

        public Genre Genre { get; init; }
    }
}
