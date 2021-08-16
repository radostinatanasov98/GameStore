namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Publishers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    public class PublishersController : Controller
    {
        private readonly GameStoreDbContext data;

        public PublishersController(GameStoreDbContext data)
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
        public IActionResult Become(BecomePublisherFormModel publisher)
        {
            if (IsUserClient() || IsUserPublisher())
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(publisher);
            }

            var validPublisher = new Publisher
            {
                DisplayName = publisher.Name,
                UserId = this.User.GetId()
            };

            this.data.Publishers.Add(validPublisher);

            this.data.SaveChanges();

            return Redirect("/Games/Add");
        }

        private bool IsUserPublisher()
            => data.Publishers.Any(p => p.UserId == this.User.GetId());

        private bool IsUserClient()
            => data.Clients.Any(p => p.UserId == this.User.GetId());
    }
}
