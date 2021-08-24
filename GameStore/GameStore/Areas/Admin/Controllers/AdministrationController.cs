namespace GameStore.Areas.Admin.Controllers
{
    using GameStore.Areas.Admin.Models.Administration;
    using GameStore.Services.Games;
    using GameStore.Services.Reviews;
    using Microsoft.AspNetCore.Mvc;

    public class AdministrationController : AdminController
    {
        private readonly IGamesService gamesService;
        private readonly IReviewService reviewService;

        public AdministrationController(IGamesService gamesService,
          IReviewService reviewService)
        {
            this.gamesService = gamesService;
            this.reviewService = reviewService;
        }

        public IActionResult Main()
            => View(new AdministrationPageViewModel 
            {
                Games = this.gamesService.GetGamesForAllView(),
                Reviews = this.reviewService.GetReviewsAdmin()
            });
    }
}
