namespace GameStore.Controllers
{
    using GameStore.Models.Games;
    using Microsoft.AspNetCore.Mvc;

    public class GamesController : Controller
    {
        public GamesController()
        {
        }

        public IActionResult All => View();

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddGameFormModel game)
        {
            return View();
        }
    }
}
