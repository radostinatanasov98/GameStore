﻿namespace GameStore.Services.ShoppingCart
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Models.ShoppingCart;
    using System.Collections.Generic;
    using System.Linq;

    public class ShoppingCartService : IShoppingCartService
    {
        private readonly GameStoreDbContext data;

        public ShoppingCartService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public ShoppingCart CreateShoppingCart(Client client)
            => new ShoppingCart
            {
                Client = client
            };

        public ShoppingCartProduct GetProduct(int gameId, int shoppingCartId)
            => this.data.ShoppingCartProducts.FirstOrDefault(scp => scp.GameId == gameId && scp.ShoppingCartId == shoppingCartId);

        public List<int> GetProducts(int shoppingCartId)
            => this.data
                .ShoppingCartProducts
                .Where(scp => scp.ShoppingCartId == shoppingCartId)
                .Select(scp => scp.GameId)
                .ToList();

        public IQueryable<ShoppingCartProduct> GetShoppingCartProducts(int shoppingCartId)
            => this.data.ShoppingCartProducts.Where(scp => scp.ShoppingCartId == shoppingCartId);

        public ShoppingCartViewModel GetShoppingCartViewModel(List<GameShoppingCartViewModel> gamesQuery)
            => new ShoppingCartViewModel
            {
                Games = gamesQuery,
                TotalPrice = gamesQuery.Sum(g => g.Price)
            };

        public void Purchase(List<int> products, int clientId)
        {
            foreach (var gameId in products)
            {
                this.data.ClientGames.Add(new ClientGame { ClientId = clientId, GameId = gameId });
                var product = this.data.ShoppingCartProducts.First(scp => scp.GameId == gameId);
                this.data.ShoppingCartProducts.Remove(product);
            }

            this.data.SaveChanges();
        }

        public void RemoveProdutc(ShoppingCartProduct product)
        {
            this.data
                .ShoppingCartProducts
                .Remove(product);

            this.data.SaveChanges();
        }
    }
}
