namespace GameStore.Services.Games
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using GameStore.Models.Games;
    using GameStore.Models.Home;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GamesService : IGamesService
    {
        private readonly GameStoreDbContext data;

        public GamesService(GameStoreDbContext data)
        {
            this.data = data;
        }

        public List<GameListingViewModel> GetGamesForAllView()
            => this.data
                .Games
                .Select(g => new GameListingViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                    CoverImageUrl = g.CoverImageUrl,
                    PegiRating = g.PegiRating.Name,
                    Genres = GetGameGenreNames(g, data),
                    DateAdded = g.DateAdded.ToString(),
                    Rating = GetGameRating(g.Id, this.data)
                })
                .ToList();

        public List<GameHoverViewModel> GetGamesForHoverModel()
            => this.data
                    .Games
                    .Select(g => new GameHoverViewModel
                    {
                        GameId = g.Id,
                        CoverImageUrl = g.CoverImageUrl,
                        Name = g.Name,
                        Rating = GetGameRating(g.Id, this.data)
                    })
                    .ToList();

        public List<GameHoverViewModel> GetHoverModelForProfile(List<int> ids)
            => this.GetGamesForHoverModel()
                .Where(g => ids.Contains(g.GameId))
                .ToList();

        public List<GameListingViewModel> GetGamesForLibraryView(int clientId)
            => this.data
                .ClientGames
                .Where(cg => cg.ClientId == clientId)
                .Select(g => new GameListingViewModel
                {
                    Id = g.Game.Id,
                    Name = g.Game.Name,
                    CoverImageUrl = g.Game.CoverImageUrl,
                    PegiRating = g.Game.PegiRating.Name,
                    Genres = GetGameGenreNames(g.Game, data)
                })
                .ToList();


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

        private static IEnumerable<string> GetGameGenreNames(Game game, GameStoreDbContext data)
        {
            var genreIds = data
                .GameGenres
                .Where(gg => gg.GameId == game.Id)
                .Select(gg => gg.GenreId)
                .ToList();

            return data
                .Genres
                .Where(g => genreIds.Contains(g.Id))
                .Select(g => g.Name);
        }

        public List<GameListingViewModel> GetGamesListingSortedByName(List<string> tokens)
        {
            var games = new List<GameListingViewModel>();
            tokens.ForEach(t => games.AddRange(GetGamesForAllView().Where(g => g.Name.ToLower().Contains(t.ToLower()) && !games.Any(ga => ga.Name == g.Name))));
            return games;
        }

        public List<GameListingViewModel> GetGamesListingByGenre(List<string> tokens)
        {
            var games = new List<GameListingViewModel>();

            tokens.ForEach(t => games.AddRange(GetGamesForAllView().Where(g => g.Genres.ToList().ConvertAll(g => g.ToLower()).Contains(t.ToLower()) && !games.Any(ga => ga.Name == g.Name))));

            return games;
        }

        public List<GameListingViewModel> HandleSearchQueries(string searchQuery, string searchByQuery)
        {
            if (searchQuery != null)
            {
                var tokens = searchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                if (searchByQuery == null || searchByQuery == "Name") return this.GetGamesListingSortedByName(tokens);
                else if (searchByQuery == "Genre") return this.GetGamesListingByGenre(tokens);
            }

            return this.GetGamesForAllView();
        }

        public List<GameListingViewModel> HandleSortQuery(string sortQuery, List<GameListingViewModel> games)
            => sortQuery switch
            {
                "Name" => games.OrderBy(gq => gq.Name).ToList(),
                "Rating" => games.OrderByDescending(gq => gq.Rating).ToList(),
                "Newest" => games.OrderByDescending(gq => gq.DateAdded).ToList(),
                "Oldest" => games.OrderBy(gq => gq.DateAdded).ToList(),
                _ => games.OrderBy(gq => gq.Id).ToList(),
            };

        public AllGamesViewModel CreateAllGamesViewModel(List<GameListingViewModel> games, List<GenreViewModel> genres)
            => new AllGamesViewModel
            {
                Games = games,
                Genres = genres
            };

        public List<PegiRatingViewModel> GetPegiRatings()
            => this.data
            .PegiRatings
            .Select(pr => new PegiRatingViewModel
            {
                Id = pr.Id,
                Name = pr.Name
            })
            .ToList();

        public List<GenreViewModel> GetGenres()
            => this.data
            .Genres
            .Select(g => new GenreViewModel
            {
                 Id = g.Id,
                 Name = g.Name
            })
            .ToList();

        public AddGameFormModel CreateAddGameFormModel()
            => new AddGameFormModel
            {
                PegiRatings = this.GetPegiRatings(),
                Genres = this.GetGenres()
            };

        private string GetEmbedUrl(string url)
        {
            var trailerUrlTokens = url.Split("watch?v=");

            return trailerUrlTokens[0] + "embed/" + trailerUrlTokens[1];
        }

        public void CreateGame(AddGameFormModel model, Requirements minimumRequirements, Requirements recommendedRequirements, int publisherId)
        {
            var embedUrl = this.GetEmbedUrl(model.TrailerUrl);

            var game = new Game
            {
                Name = model.Name,
                Description = model.Description,
                CoverImageUrl = model.CoverImageUrl,
                TrailerUrl = embedUrl,
                Price = model.Price,
                PegiRatingId = model.PegiRatingId,
                DateAdded = DateTime.UtcNow,
                MinimumRequirements = minimumRequirements,
                RecommendedRequirements = recommendedRequirements,
                PublisherId = publisherId
            };

            this.data.Games.Add(game);

            CreateGameGenreRelations(model.GenreIds, game);

            this.data.SaveChanges();
        }

        private void CreateGameGenreRelations(IEnumerable<int> genreIds, Game game)
        {
            foreach (var genreId in genreIds)
            {
                var genre = this.data
                    .Genres
                    .Where(g => g.Id == genreId)
                    .FirstOrDefault();

                this.data.GameGenres.Add(new GameGenre
                {
                    Game = game,
                    Genre = genre
                });
            }
        }

        public GameDetailsViewModel GetGameDetailsViewModel(int gameId)
            => this.data
                    .Games
                    .Where(g => g.Id == gameId)
                    .Select(g => new GameDetailsViewModel
                    {
                        Id = gameId,
                        Name = g.Name,
                        PublisherName = this.data.Publishers.First(p => p.Id == g.PublisherId).Name,
                        Price = g.Price,
                        Description = g.Description,
                        CoverImageUrl = g.CoverImageUrl,
                        TrailerUrl = g.TrailerUrl,
                        PegiRating = this.data.PegiRatings.First(pr => pr.Id == g.PegiRatingId).Name,
                        MinimumRequirementsId = g.MinimumRequirementsId,
                        RecommendedRequirementsId = g.RecommendedRequirementsId,
                        Rating = this.data.Reviews.Any(r => r.GameId == gameId) ? this.data.Reviews.Where(r => r.GameId == gameId).Average(r => r.Rating) : 0,
                        ReviewsCount = this.data.Reviews.Count(r => r.GameId == gameId && r.Content != null && r.Caption != null),
                        RatingsCount = this.data.Reviews.Count(r => r.GameId == gameId),
                        Genres = GetGameGenreNames(g, this.data)
                    })
                    .First();

        public Game GetGameById(int gameId)
            => this.data
            .Games
            .FirstOrDefault(g => g.Id == gameId);

        public void RemoveGame(Game game, Requirements minRequirements, Requirements recRequirements)
        {
            this.data.Games.Remove(game);
            this.data.Requirements.RemoveRange(minRequirements, recRequirements);
            this.data.SaveChanges();
        }

        private static double GetGameRating(int gameId, GameStoreDbContext data)
            => data
            .Reviews
            .Any(r => r.GameId == gameId) ?
            data.Reviews.Where(r => r.GameId == gameId)
            .Average(r => r.Rating) : 0;

        public HomePageViewModel GetGamesForHomePage()
            => new HomePageViewModel
            {
                TopRatedGames = GetGamesForHoverModel().OrderByDescending(g => g.Rating),
                LatestGames = GetGamesForHoverModel().OrderByDescending(g => g.GameId)
            };
    }
}
