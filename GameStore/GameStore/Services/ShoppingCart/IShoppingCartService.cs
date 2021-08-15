namespace GameStore.Services.ShoppingCart
{
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Models.ShoppingCart;
    using System.Collections.Generic;
    using System.Linq;

    public interface IShoppingCartService
    {
        public void AddShoppingCartProduct(int id, int gameId);

        public ShoppingCartProduct GetProduct(int gameId, int shoppingCartId);

        public List<int> GetProducts(int shoppingCartId);

        public ShoppingCart GetShoppingCart(string userId);

        public IQueryable<ShoppingCartProduct> GetShoppingCartProducts(int shoppingCartId);

        public ShoppingCartViewModel GetShoppingCartViewModel(List<GameShoppingCartViewModel> games);

        public void Purchase(List<int> products, int clientId);

        public void RemoveProduct(ShoppingCartProduct product);
    }
}
