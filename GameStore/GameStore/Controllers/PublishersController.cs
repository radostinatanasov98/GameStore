namespace GameStore.Controllers
{
    using GameStore.Infrastructure;
    using GameStore.Models.Publishers;
    using GameStore.Services.Publishers;
    using GameStore.Services.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class PublishersController : Controller
    {
        private readonly IPublisherService publisherService;
        private readonly IUserService userService;

        public PublishersController(IPublisherService publisherService,
            IUserService userService)
        {
            this.publisherService = publisherService;
            this.userService = userService;
        }

        [Authorize]
        public IActionResult Become()
        {
            if (this.userService.IsUserClient(this.User.GetId()) || this.userService.IsUserPublisher(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Become(BecomePublisherFormModel model)
        {
            if (this.userService.IsUserClient(this.User.GetId()) || this.userService.IsUserPublisher(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            this.publisherService.CreatePublisher(model, this.User.GetId());

            return RedirectToAction(nameof(GamesController.Add), "Games");
        }

        public IActionResult All()
            => View(this.publisherService.GetPublishers());

        [Authorize]
        public IActionResult EditLogo()
        {
            if (!this.userService.IsUserPublisher(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditLogo(EditLogoFormModel model)
        {
            if (!this.userService.IsUserPublisher(this.User.GetId()))
            {
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            model.Id = this.publisherService.GetPublisherId(this.User.GetId());

            if (!ModelState.IsValid) return View(model);

            this.publisherService.EditLogo(model);

            return RedirectToAction(nameof(PublishersController.All), "Publishers");
        }
    }
}
