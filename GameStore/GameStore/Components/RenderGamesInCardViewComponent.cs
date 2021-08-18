namespace GameStore.Components
{
    using GameStore.Models.Games;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class RenderGamesInCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<GameHoverViewModel> games)
            => View(games);
    }
}
