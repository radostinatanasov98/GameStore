namespace GameStore.Models.Games
{
    using System.Collections.Generic;

    public class AllGamesViewModel
    {
        public string SortQuery { get; init; }

        public string SearchQuery { get; init; }

        public string SearchByQuery { get; init; }

        public IEnumerable<GenreViewModel> Genres { get; init; } = new List<GenreViewModel>();

        public IEnumerable<GameListingViewModel> Games { get; init; } = new List<GameListingViewModel>();
    }
}
