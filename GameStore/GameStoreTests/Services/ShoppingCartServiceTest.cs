namespace GameStore.Tests.Services
{
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Services.ShoppingCart;
    using GameStore.Tests.Mocks;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ShoppingCartServiceTest
    {
        private const int cartId = 1;
        private const int gameId = 1;

        [Fact]
        public void AddShoppingCartProductShouldAddCorrectShoppingCartProductInDatabase()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var shoppingCartService = new ShoppingCartService(data);

            // Act
            shoppingCartService.AddShoppingCartProduct(cartId, gameId);

            // Assert
            Assert.True(data.ShoppingCartProducts.Any(scp => scp.ShoppingCartId == cartId && scp.GameId == gameId));
        }

        [Fact]
        public void GetProductShouldReturnCorrectShoppingCartProduct()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.ShoppingCartProducts
                .Add(new ShoppingCartProduct
                {
                    GameId = gameId,
                    ShoppingCartId = cartId
                });

            data.SaveChanges();

            var shoppingCartService = new ShoppingCartService(data);

            // Act
            var result = shoppingCartService.GetProduct(gameId, cartId);

            // Assert
            Assert.Equal(gameId, result.GameId);
            Assert.Equal(cartId, result.ShoppingCartId);
        }

        [Fact]
        public void GetProductsShouldReturnCorrectIds()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            for (int i = 0; i < 6; i++)
            {
                data.ShoppingCartProducts
                    .Add(new ShoppingCartProduct
                    {
                        GameId = i,
                        ShoppingCartId = i < 4 ? cartId : 2
                    });
            }

            data.SaveChanges();

            var shoppingCartService = new ShoppingCartService(data);

            // Act
            var result = shoppingCartService.GetProducts(cartId);

            // Assert
            Assert.True(result.Count() == 4);
            Assert.Equal(0, result[0]);
            Assert.Equal(1, result[1]);
            Assert.Equal(2, result[2]);
            Assert.Equal(3, result[3]);
        }

        [Fact]
        public void GetShoppingCartShouldReturnTheCorrectShoppingCart()
        {
            // Arrange
            const string userId = "testId";
            using var data = DatabaseMock.Instance;

            for (int i = 0; i < 3; i++)
            {
                data.ShoppingCarts
                    .Add(new ShoppingCart
                    {
                        Client = new Client
                        {
                            UserId = userId + i
                        }
                    });
            }

            data.SaveChanges();

            var shoppingCartService = new ShoppingCartService(data);

            // Act
            var result = shoppingCartService.GetShoppingCart("testId2");

            // Assert
            Assert.Equal(3, result.Id);
            Assert.Equal("testId2", result.Client.UserId);
        }

        [Fact]
        public void GetShoppingCartProductsShouldReturnCorrectProducts()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            for (int i = 0; i < 6; i++)
            {
                data.ShoppingCartProducts
                    .Add(new ShoppingCartProduct
                    {
                        GameId = i,
                        ShoppingCartId = i < 4 ? cartId : 2
                    });
            }

            data.SaveChanges();

            var shoppingCartService = new ShoppingCartService(data);

            // Act
            var result = shoppingCartService.GetShoppingCartProducts(cartId);

            // Assert
            Assert.True(result.Count() == 4);
            int counter = 0;
            foreach (var product in result)
            {
                Assert.Equal(counter, product.GameId);

                counter++;
            }
        }

        [Fact]
        public void GetShoppingCartViewModelShouldReturnCorrectModel()
        {
            // Arrange
            var games = new List<GameShoppingCartViewModel>();

            for (int i = 0; i < 6; i++)
            {
                games.Add(new GameShoppingCartViewModel
                {
                    Price = 10
                });
            }

            var shoppingCartService = new ShoppingCartService(null);

            // Act
            var result = shoppingCartService.GetShoppingCartViewModel(games);

            // Assert
            Assert.True(result.Games.Count() == 6);
            Assert.Equal(60, result.TotalPrice);
        }

        [Fact]
        public void RemoveProdutcShouldWorkCorrectly()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var product = new ShoppingCartProduct
            {
                ShoppingCartId = cartId,
            };

            data.Add(product);

            data.SaveChanges();

            var shoppingCartService = new ShoppingCartService(data);

            // Act
            shoppingCartService.RemoveProduct(product);

            // Assert
            Assert.False(data.ShoppingCartProducts.Contains(product));
        }
    }
}
