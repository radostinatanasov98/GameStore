namespace GameStore.Models.Home
{
    using GameStore.Models.Games;
    using System.Collections.Generic;

    public class HomePageViewModel
    {
        public IEnumerable<GameHoverViewModel> TopRatedGames { get; init; } = new List<GameHoverViewModel>();

        public IEnumerable<GameHoverViewModel> LatestGames { get; init; } = new List<GameHoverViewModel>();
    }
}
