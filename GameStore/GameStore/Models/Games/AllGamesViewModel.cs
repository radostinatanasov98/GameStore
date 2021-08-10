namespace GameStore.Models.Games
{
    using System.Collections.Generic;

    public class AllGamesViewModel
    {
        public string SearchQuery { get; init; }

        public IEnumerable<GameListingViewModel> Games { get; init; } = new List<GameListingViewModel>();
    }
}
