namespace GameStore.Components
{
    using GameStore.Models.Reviews;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class DisplayReviews : ViewComponent
    {
        public IViewComponentResult Invoke(List<ReviewViewModel> reviews)
        {
            return View(reviews);
        }
    }
}
