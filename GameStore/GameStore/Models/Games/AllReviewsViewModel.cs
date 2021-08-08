﻿namespace GameStore.Models.Games
{
    using GameStore.Models.Reviews;
    using System.Collections.Generic;

    public class AllReviewsViewModel
    {
        public string Name { get; init; }

        public int GameId { get; init; }

        public IEnumerable<ReviewViewModel> Reviews { get; init; } = new List<ReviewViewModel>();
    }
}