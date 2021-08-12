namespace GameStore.Services.Games
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using System.Collections.Generic;
    using System.Linq;

    public class GamesService : IGamesService
    {
        private readonly GameStoreDbContext data;

        public GamesService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public List<GameHoverViewModel> GetGamesForHoverModel(int profileId)
            => this.data
                    .ClientGames
                    .Where(cg => cg.ClientId == profileId)
                    .Select(cg => new GameHoverViewModel
                    {
                        GameId = cg.GameId,
                        CoverImageUrl = this.data.Games.First(g => g.Id == cg.GameId).CoverImageUrl,
                        Name = this.data.Games.First(g => g.Id == cg.GameId).Name
                    })
                    .Take(6)
                    .ToList();

        public List<GameListingViewModel> GetGamesForLibraryView(string userId)
        {
            return this.data
                .ClientGames
                .Where(cg => cg.Client.UserId == userId)
                .Select(g => new GameListingViewModel
                {
                    Id = g.Game.Id,
                    Name = g.Game.Name,
                    CoverImageUrl = g.Game.CoverImageUrl,
                    PegiRating = g.Game.PegiRating.Name,
                    Genres = g.Game
                            .GameGenres
                            .Where(gg => gg.GameId == g.Game.Id)
                            .Select(gg => gg.Genre.Name)
                            .ToList()
                })
                .ToList();
        }

        public List<GameShoppingCartViewModel> GetGamesForShoppingCartView(IQueryable<ShoppingCartProduct> shoppingCartProductsQuery)
            => shoppingCartProductsQuery
                .Select(p => new GameShoppingCartViewModel
                {
                    Id = p.Game.Id,
                    Name = p.Game.Name,
                    Publisher = p.Game.Publisher.Name,
                    PegiRating = p.Game.PegiRating.Name,
                    Price = p.Game.Price,
                    ImageUrl = p.Game.CoverImageUrl
                })
                .ToList();
    }
}
