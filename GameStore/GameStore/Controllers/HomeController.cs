namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Services.Games;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IGamesService gamesService;

        public HomeController(GameStoreDbContext data)
            => this.gamesService = new GamesService(data);

        public IActionResult Index()
            => View(this.gamesService.GetGamesForHomePage());

        public IActionResult Error() => View();
    }
}
