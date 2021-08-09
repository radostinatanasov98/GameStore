namespace GameStore.Models.Home
{
    using GameStore.Models.Games;
    using System.Collections.Generic;

    public class HomePageViewModel
    {
        public BecomePublisherOrClientHomeViewModel BecomeUserType { get; init; }

        public IEnumerable<GameHomePageViewModel> TopRatedGames { get; init; } = new List<GameHomePageViewModel>();

        public IEnumerable<GameHomePageViewModel> LatestGames { get; init; } = new List<GameHomePageViewModel>();

        public IEnumerable<GameHomePageViewModel> ReccommendedGames { get; init; } = new List<GameHomePageViewModel>();
    }
}
