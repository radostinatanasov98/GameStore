namespace GameStore.Components
{
    using GameStore.Data;
    using GameStore.Infrastructure;
    using GameStore.Models.Games;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    [ViewComponent(Name = "GameButtons")]
    public class GameButtonsViewComponent : ViewComponent
    {
        private GameStoreDbContext data;

        public GameButtonsViewComponent(GameStoreDbContext data)
        {
            this.data = data;
        }

        public IViewComponentResult Invoke(int GameId, decimal price)
        {
            bool isPublisher = false;

            if (this.User.Identity.IsAuthenticated)
            {
                isPublisher = this.data
                    .Publishers
                    .Where(p => p.UserId == this.UserClaimsPrincipal.GetId())
                    .Any(p => p.Games.Any(g => g.Id == GameId));
            }

            var model = new ButtonsViewModel
            {
                IsPublisher = isPublisher,
                IsOwned = false,
                GameId = GameId,
                Price = price
            };

            var client = this.data
                .Clients
                .FirstOrDefault(c => c.UserId == this.UserClaimsPrincipal.GetId());

            if (client != null)
            {
                if (this.data.ClientGames.Any(cg => cg.ClientId == client.Id && cg.GameId == GameId)) model.IsOwned = true;
            }

            return View(model);
        }
    }
}
