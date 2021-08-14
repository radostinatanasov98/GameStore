﻿namespace GameStore.Services.ShoppingCart
{
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Models.ShoppingCart;
    using System.Collections.Generic;
    using System.Linq;

    public interface IShoppingCartService
    { 
        public IQueryable<ShoppingCartProduct> GetShoppingCartProducts(int shoppingCartId);

        public ShoppingCartViewModel GetShoppingCartViewModel(List<GameShoppingCartViewModel> games);

        public List<int> GetProducts(int shoppingCartId);

        public void Purchase(List<int> products, int clientId);

        public ShoppingCartProduct GetProduct(int gameId, int shoppingCartId);

        public void RemoveProdutc(ShoppingCartProduct product);

        public ShoppingCart GetShoppingCart(string userId);

        public void AddShoppingCartProduct(int id, int gameId);
    }
}
