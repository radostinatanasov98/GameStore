namespace GameStore.Controllers
{
    using GameStore.Data;
    using GameStore.Models.Games;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class GamesController : Controller
    {
        private readonly GameStoreDbContext data;

        public GamesController(GameStoreDbContext data)
            => this.data = data;

        public IActionResult All => View();

        public IActionResult Add() => View(new AddGameFormModel
        {
            PegiRatings = this.GetPegiRatings()
        });

        [HttpPost]
        public IActionResult Add(AddGameFormModel game)
        {
            return View();
        }

        private IEnumerable<PegiRatingViewModel> GetPegiRatings()
            => this.data
            .PegiRatings
            .Select(pr => new PegiRatingViewModel
            {
                Id = pr.Id,
                Name = pr.Name
            })
            .ToList();
    }
}
