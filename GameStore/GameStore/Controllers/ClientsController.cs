namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Clients;
    using GameStore.Models.Games;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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

        private bool IsUserPublisher()
            => this.data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => this.data.Clients.Any(p => p.UserId == this.User.GetId());
    }
}
