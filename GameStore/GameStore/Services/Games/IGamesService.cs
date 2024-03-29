﻿namespace GameStore.Services.Games
{
    using GameStore.Models.Games;
    using GameStore.Data.Models;
    using System.Collections.Generic;
    using System.Linq;
    using GameStore.Models.Home;
    using GameStore.Models.PegiRatings;

    public interface IGamesService
    {
        public bool GameExists(int gameId);

        public List<GameListingViewModel> GetGamesForLibraryView(int clientId);

        public List<GameShoppingCartViewModel> GetGamesForShoppingCartView(IQueryable<ShoppingCartProduct> shoppingCartProductsQuery);

        public List<GameHoverViewModel> GetGamesForHoverModel();

        public List<GameHoverViewModel> GetHoverModelForProfile(List<int> ids);

        public List<GameListingViewModel> GetGamesForAllView();

        public List<GameListingViewModel> GetGamesListingSortedByName(List<string> tokens);

        public List<GameListingViewModel> GetGamesListingByGenre(List<string> tokens);

        public List<GameListingViewModel> HandleSearchQueries(string searchQuery, string searchByQuery);

        public List<GameListingViewModel> HandleSortQuery(string sortQuery, List<GameListingViewModel> games);

        public AllGamesViewModel CreateAllGamesViewModel(string sortQuery,
            string searchByQuery,
            string searchQuery,
            int currentPage,
            int gamesPerPage = 6);

        public List<PegiRatingViewModel> GetPegiRatings();

        public List<GenreViewModel> GetGenres();

        public AddGameFormModel CreateAddGameFormModel();

        public void CreateGame(AddGameFormModel model, Requirements minimumRequirements, Requirements recommendedRequirements, int publisherId);

        public GameDetailsViewModel GetGameDetailsViewModel(int gameId, int clientId);

        public Game GetGameById(int gameId);

        public void RemoveGame(Game game, Requirements minRequirements, Requirements recRequirements);

        public HomePageViewModel GetGamesForHomePage();

        public List<GameListingViewModel> WishedGames(int clientId);

    }
}
