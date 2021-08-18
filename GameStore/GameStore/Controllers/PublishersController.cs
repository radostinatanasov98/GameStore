namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Infrastructure;
    using GameStore.Models.Publishers;
    using GameStore.Services.Publishers;
    using GameStore.Services.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    public class PublishersController : Controller
    {
        private readonly IPublisherService publisherService;
        private readonly IUserService userService;

        public PublishersController(GameStoreDbContext data)
        {
            this.publisherService = new PublisherService(data);
            this.userService = new UserService(data);
        }

        [Authorize]
        public IActionResult Become()
        {
            if (this.userService.IsUserClient(this.User.GetId()) || this.userService.IsUserPublisher(this.User.GetId())) return Redirect("/Home/Error");

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Become(BecomePublisherFormModel model)
        {
            if (this.userService.IsUserClient(this.User.GetId()) || this.userService.IsUserPublisher(this.User.GetId())) return Redirect("/Home/Error");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            this.publisherService.CreatePublisher(model, this.User.GetId());

            return Redirect("/Games/Add");
        }

        public IActionResult All()
            => View(this.publisherService.GetPublishers());

        [Authorize]
        public IActionResult EditLogo()
        {
            if (!this.userService.IsUserPublisher(this.User.GetId())) return Redirect("/Home/Error");

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditLogo(EditLogoFormModel model)
        {
            if (!this.userService.IsUserPublisher(this.User.GetId())) Redirect("/Home/Error");

            model.Id = this.publisherService.GetPublisherId(this.User.GetId());

            if (!ModelState.IsValid) return View(model);

            this.publisherService.EditLogo(model);

            return Redirect("/Publishers/All");
        }
    }
}
