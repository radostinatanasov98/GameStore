namespace GameStore.Areas.Admin.Models.Administration
{
    using GameStore.Areas.Admin.Models.Reviews;
    using GameStore.Models.Games;
    using System.Collections.Generic;

    public class AdministrationPageViewModel
    {
        public IEnumerable<GameListingViewModel> Games { get; init; } = new List<GameListingViewModel>();

        public IEnumerable<AllReviewsForAdminViewModel> Reviews { get; init; } = new List<AllReviewsForAdminViewModel>();
    }
}
