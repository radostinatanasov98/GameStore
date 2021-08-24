namespace GameStore.Models.Games
{
    using System.Collections.Generic;

    public class AllGamesViewModel
    {
        public int GamesPerPage = 6;

        public int CurrentPage { get; set; } = 1;

        public string SortQuery { get; init; }

        public string SearchQuery { get; init; }

        public string SearchByQuery { get; init; }

        public IEnumerable<GenreViewModel> Genres { get; set; } = new List<GenreViewModel>();

        public IEnumerable<GameListingViewModel> Games { get; set; } = new List<GameListingViewModel>();
    }
}
