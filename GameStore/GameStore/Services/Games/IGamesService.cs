namespace GameStore.Services.Games
{
    using GameStore.Models.Games;
    using GameStore.Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public interface IGamesService
    {
        public List<GameListingViewModel> GetGamesForLibraryView(string userId);

        public List<GameShoppingCartViewModel> GetGamesForShoppingCartView(IQueryable<ShoppingCartProduct> shoppingCartProductsQuery);

        public List<GameHoverViewModel> GetGamesForHoverModel(int profileId);
    }
}
