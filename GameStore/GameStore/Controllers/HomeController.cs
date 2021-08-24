namespace GameStore.Controllers
{
    using GameStore.Services.Games;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IGamesService gamesService;

        public HomeController(IGamesService gamesService)
            => this.gamesService = gamesService;

        public IActionResult Index()
            => View(this.gamesService.GetGamesForHomePage());

        public IActionResult Error() => View();
    }
}
