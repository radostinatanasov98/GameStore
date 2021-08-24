namespace GameStore.Controllers
{
    using GameStore.Infrastructure;
    using GameStore.Models.Chats;
    using GameStore.Services.Chat;
    using GameStore.Services.Clients;
    using GameStore.Services.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ChatsController : Controller
    {
        private readonly IUserService userService;
        private readonly IClientService clientService;
        private readonly IChatService chatService;

        public ChatsController(IUserService userService,
            IClientService clientService,
            IChatService chatService)
        {
            this.userService = userService;
            this.clientService = clientService;
            this.chatService = chatService;
        }


        [Authorize]
        public IActionResult Main()
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            var id = this.clientService.GetClientId(this.User.GetId());
            var model = new ChatsViewModel
            {
                Id = id,
                Friends = this.chatService.GetFriendsForChat(id)
            };

            return View(model);
        }

        [Authorize]
        public IActionResult Chat(int clientId, int friendId)
        {
            if (!this.userService.IsUserClient(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View(this.chatService.GetChat(clientId, friendId));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Chat(ChatViewModel model)
        {
            this.chatService.SendMessage(model.ClientId, model.FriendId, model.Message);

            return RedirectToAction(nameof(ChatsController.Chat), "Chats", new { clientId = model.ClientId, friendId = model.FriendId });
        }
    }
}
