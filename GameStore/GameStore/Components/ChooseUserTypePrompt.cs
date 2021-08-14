using GameStore.Data;
using GameStore.Infrastructure;
using GameStore.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Components
{
    public class ChooseUserTypePrompt : ViewComponent
    {
        private readonly IUserService userService;

        public ChooseUserTypePrompt(GameStoreDbContext data)
        {
            this.userService = new UserService(data);
        }

        public IViewComponentResult Invoke()
        {
            var render = false;

            if(this.User.Identity.IsAuthenticated)
            {
                render = !this.userService.IsUserClient(this.UserClaimsPrincipal.GetId()) &&
                !this.userService.IsUserPublisher(this.UserClaimsPrincipal.GetId());
            }

            return View(render);
        }
    }
}
