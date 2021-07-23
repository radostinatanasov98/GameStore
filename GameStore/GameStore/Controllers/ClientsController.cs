namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Clients;
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

        private bool IsUserPublisher()
            => data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => data.Clients.Any(p => p.UserId == this.User.GetId());
    }
}
