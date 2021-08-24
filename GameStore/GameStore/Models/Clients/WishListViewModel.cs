namespace GameStore.Models.Clients
{
    using GameStore.Models.Games;
    using System.Collections.Generic;

    public class WishListViewModel
    {
        public int OwnerId { get; init; }

        public IEnumerable<GameListingViewModel> Games { get; init; } = new List<GameListingViewModel>();
    }
}
