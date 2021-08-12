namespace GameStore.Components
{
    using GameStore.Data;
    using GameStore.Infrastructure;
    using GameStore.Models.NavBar;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class NavBarViewComponent : ViewComponent
    {
        private GameStoreDbContext data;

        public NavBarViewComponent(GameStoreDbContext data)
        {
            this.data = data;
        }

        public IViewComponentResult Invoke()
        {
            bool isClient = false;
            bool isPublisher = false;
            int id = -1;

            if (this.User.Identity.IsAuthenticated)
            {
                isClient = this.data.Clients.Any(c => c.UserId == this.UserClaimsPrincipal.GetId());
                isPublisher = this.data.Publishers.Any(p => p.UserId == this.UserClaimsPrincipal.GetId());
            }

            if (this.data.Clients.Any(c => c.UserId == this.UserClaimsPrincipal.GetId()))
            {
                id = this.data.Clients.FirstOrDefault(c => c.UserId == this.UserClaimsPrincipal.GetId()).Id;
            }

            var model = new NavBarViewModel
            {
                IsClient = isClient,
                IsPublisher = isPublisher,
                Id = id
            };

            return View(model);
        }
    }
}
