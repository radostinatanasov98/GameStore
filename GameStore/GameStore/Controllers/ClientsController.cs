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

        [Authorize]
        public IActionResult Profile(int profileId)
        {
            if (!IsUserClient()) return BadRequest();

            var profile = this.data
                .Clients
                .FirstOrDefault(c => c.Id == profileId);

            if (profile == null) return BadRequest();


            var model = new ClientProfileViewModel
            {
                ClientId = GetClientId(),
                ProfileId = profileId,
                Username = profile.Name,
                Games = this.data
                    .ClientGames
                    .Where(cg => cg.ClientId == profileId)
                    .Select(cg => new GameHoverViewModel
                    {
                        GameId = cg.GameId,
                        CoverImageUrl = this.data.Games.First(g => g.Id == cg.GameId).CoverImageUrl,
                        Name = this.data.Games.First(g => g.Id == cg.GameId).Name
                    })
                    .Take(6)
                    .ToList(),
                AreGamesPrivate = profile.AreGamesPrivate,
                AreFriendsPrivate = profile.AreFriendsPrivate,
                Description = profile.Description,
                Friends = this.data
                    .ClientRelationships
                    .Where(cr => cr.ClientId == profileId && (cr.HasFriendRequest || cr.AreFriends))
                    .Select(cr => new FriendsViewModel
                    {
                        Id = cr.Id,
                        FriendId = cr.FriendId,
                        ClientId = cr.ClientId,
                        OwnerId = GetClientId(),
                        HasRequest = cr.HasFriendRequest,
                        ProfilePictureUrl = this.data.Clients.First(c => c.Id == cr.FriendId).ProfilePictureUrl,
                        Username = this.data.Clients.First(c => c.Id == cr.FriendId).Name
                    })
                    .ToList(),
                ProfilePictureUrl = profile.ProfilePictureUrl,
                Reviews = this.data
                    .Reviews
                    .Where(r => r.ClientId == profile.Id && r.Content != null && r.Caption != null)
                    .Select(r => new ReviewViewModel
                    {
                        Username = profile.Name,
                        Caption = r.Caption,
                        Content = r.Content,
                        Rating = r.Rating
                    }),
                ReviewsCount = this.data.Reviews.Where(r => r.ClientId == profileId).Count(),
                AvarageRating = this.data.Reviews.Where(r => r.ClientId == profileId).Average(r => r.Rating)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Profile")]
        public IActionResult ProfilePost(int profileId)
        {
            if (!IsUserClient()) return BadRequest();

            var clinet = this.data
                .Clients
                .First(c => c.UserId == this.User.GetId());

            if (clinet.Id == profileId || this.data.ClientRelationships.Any(cr => cr.ClientId == clinet.Id && cr.FriendId == profileId)) return BadRequest();

            var clientRelationship = new ClientRelationship
            {
                ClientId = GetClientId(),
                FriendId = profileId,
                AreFriends = false,
                HasFriendRequest = false,
            };

            var friendRelationship = new ClientRelationship
            {
                ClientId = profileId,
                FriendId = GetClientId(),
                AreFriends = false,
                HasFriendRequest = true
            };

            this.data.ClientRelationships.AddRange(clientRelationship, friendRelationship);

            this.data.SaveChanges();

            return Redirect("~/");
        }

        [Authorize]
        public IActionResult Accept(int requestId)
        {
            var clientRelationship = this.data.ClientRelationships.First(cr => cr.Id == requestId);

            if (GetClientId() != clientRelationship.ClientId) return BadRequest();

            clientRelationship.AreFriends = true;
            clientRelationship.HasFriendRequest = false;

            this.data.ClientRelationships.First(cr => cr.ClientId == clientRelationship.FriendId).AreFriends = true;

            this.data.SaveChanges();

            return Redirect("/Games/All");
        }

        [Authorize]
        public IActionResult Decline(int requestId)
        {
            var clientRelationship = this.data.ClientRelationships.First(cr => cr.Id == requestId);

            if (GetClientId() != clientRelationship.ClientId) return BadRequest();

            clientRelationship.AreFriends = false;
            clientRelationship.HasFriendRequest = false;

            var friendRelationship = this.data.ClientRelationships.First(cr => cr.ClientId == clientRelationship.FriendId);

            this.data.Remove(clientRelationship);
            this.data.Remove(friendRelationship);
            this.data.SaveChanges();

            return Redirect("/Games/All");
        }

        private bool IsUserPublisher()
            => this.data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => this.data.Clients.Any(p => p.UserId == this.User.GetId());

        private int GetClientId()
            => this.data.Clients.First(c => c.UserId == this.User.GetId()).Id;
    }
}
