namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Clients;
    using GameStore.Models.Games;
    using GameStore.Models.ShoppingCart;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class ClientsController : Controller
    {
        private readonly GameStoreDbContext data;

        public ClientsController(GameStoreDbContext data)
            => this.data = data;

        [Authorize]
        public IActionResult Become()
        {
            if (IsUserClient() || IsUserPublisher())
            {
                return BadRequest();
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Become(BecomeClientFormModel client)
        {
            if (IsUserClient() || IsUserPublisher())
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(client);
            }

            var validClient = new Client
            {
                Name = client.Name,
                UserId = this.User.GetId()
            };

            this.data.Clients.Add(validClient);

            var shoppingCart = new ShoppingCart
            {
                Client = validClient
            };

            this.data.ShoppingCarts.Add(shoppingCart);

            this.data.SaveChanges();

            return Redirect("/Games/ClientTest");
        }

        [Authorize]
        public IActionResult Library()
        {
            var gamesQuery = this.data
                .ClientGames
                .Where(cg => cg.Client.UserId == this.User.GetId())
                .Select(g => new GameListingViewModel
                {
                    Id = g.Game.Id,
                    Name = g.Game.Name,
                    CoverImageUrl = g.Game.CoverImageUrl,
                    PegiRating = g.Game.PegiRating.Name,
                    Genres = g.Game
                            .GameGenres
                            .Where(gg => gg.GameId == g.Game.Id)
                            .Select(gg => gg.Genre.Name)
                            .ToList()
                })
                .ToList();

            return View(gamesQuery);
        }

        public IActionResult ShoppingCart()
        {
            var clientQuery = this.data
                .Clients
                .FirstOrDefault(c => c.UserId == this.User.GetId());

            var shoppingCartProductsQuery = this.data
                    .ShoppingCartProducts
                    .Where(scp => scp.ShoppingCartId == clientQuery.ShoppingCartId);

            var gamesQuery = shoppingCartProductsQuery
                .Select(p => new GameShoppingCartViewModel
                {
                    Name = p.Game.Name,
                    Publisher = p.Game.Publisher.Name,
                    PegiRating = p.Game.PegiRating.Name,
                    Price = p.Game.Price,
                    ImageUrl = p.Game.CoverImageUrl
                })
                .ToList();

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                Games = gamesQuery,
                TotalPrice = gamesQuery.Sum(g => g.Price)
            };

            return View(shoppingCartViewModel);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ShoppingCart")]
        public IActionResult ShoppingCartPost()
        {
            var clientQuery = this.data
                .Clients
                .FirstOrDefault(c => c.UserId == this.User.GetId());

            var shoppingCartProductsQuery = this.data
                .ShoppingCartProducts
                .Where(scp => scp.ShoppingCartId == clientQuery.ShoppingCartId)
                .Select(scp => new 
                {
                    GameId = scp.GameId
                })
                .ToList();

            foreach (var game in shoppingCartProductsQuery)
            {
                this.data.ClientGames.Add(new ClientGame { ClientId = clientQuery.Id, GameId = game.GameId });
                var product = this.data.ShoppingCartProducts.First(scp => scp.GameId == game.GameId);
                this.data.ShoppingCartProducts.Remove(product);
            }

            this.data.SaveChanges();

            return Redirect("/Clients/Library");
        }
        private bool IsUserPublisher()
            => this.data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => this.data.Clients.Any(p => p.UserId == this.User.GetId());
    }
}
