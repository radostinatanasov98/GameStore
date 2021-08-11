namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Clients;
    using GameStore.Models.Games;
    using GameStore.Models.Reviews;
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

            return Redirect("/Games/All");
        }

        [Authorize]
        public IActionResult Library()
        {
            if (!IsUserClient()) return BadRequest();

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

        [Authorize]
        public IActionResult ShoppingCart()
        {
            if (!IsUserClient()) return BadRequest();

            var clientQuery = this.data
                .Clients
                .FirstOrDefault(c => c.UserId == this.User.GetId());

            var shoppingCartProductsQuery = this.data
                    .ShoppingCartProducts
                    .Where(scp => scp.ShoppingCartId == clientQuery.ShoppingCartId);

            var gamesQuery = shoppingCartProductsQuery
                .Select(p => new GameShoppingCartViewModel
                {
                    Id = p.Game.Id,
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

        public IActionResult ShoppingCartRemove(int GameId)
        {
            var product = this.data
                .ShoppingCartProducts
                .FirstOrDefault(scp => scp.GameId == GameId);

            if (product == null) return BadRequest();

            this.data
                .ShoppingCartProducts
                .Remove(product);

            this.data.SaveChanges();

            return Redirect("/Clients/ShoppingCart");
        }

        public IActionResult Profile(int clientId)
        {
            var client = this.data
                .Clients
                .FirstOrDefault(c => c.Id == clientId);

            if (client == null) return BadRequest();

            var model = new ClientProfileViewModel
            {
                Id = clientId,
                Username = client.Name,
                Games = this.data
                    .ClientGames
                    .Where(cg => cg.ClientId == clientId)
                    .Select(cg => new GameHoverViewModel
                    {
                        GameId = cg.GameId,
                        CoverImageUrl = this.data.Games.First(g => g.Id == cg.GameId).CoverImageUrl,
                        Name = this.data.Games.First(g => g.Id == cg.GameId).Name
                    })
                    .Take(6)
                    .ToList(),
                AreGamesPrivate = client.AreGamesPrivate,
                AreFriendsPrivate = client.AreFriendsPrivate,
                Description = client.Description,
                Friends = this.data.ClientRelationships.Where(r => r.ClientId == client.Id).ToList(),
                ProfilePictureUrl = client.ProfilePictureUrl,
                Reviews = this.data
                    .Reviews
                    .Where(r => r.ClientId == client.Id && r.Content != null && r.Caption != null)
                    .Select(r => new ReviewViewModel
                    {
                        Username = client.Name,
                        Caption = r.Caption,
                        Content = r.Content,
                        Rating = r.Rating
                    })

            };

            return View(model);
        }
        private bool IsUserPublisher()
            => this.data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => this.data.Clients.Any(p => p.UserId == this.User.GetId());
    }
}
