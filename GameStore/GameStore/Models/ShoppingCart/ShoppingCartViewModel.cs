namespace GameStore.Models.ShoppingCart
{
    using GameStore.Models.Games;
    using System.Collections.Generic;

    public class ShoppingCartViewModel
    {
        public decimal TotalPrice { get; init; }

        public IEnumerable<GameShoppingCartViewModel> Games { get; init; } = new List<GameShoppingCartViewModel>();
    }
}
