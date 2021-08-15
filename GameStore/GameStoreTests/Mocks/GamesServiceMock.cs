namespace GameStore.Tests.Mocks
{
    using GameStore.Models.Games;
    using GameStore.Models.Home;
    using GameStore.Services.Games;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;

    public static class GamesServiceMock
    {
        public static IGamesService Instance
        {
            get
            {
                var gamesServiceMock = new Mock<IGamesService>();

                gamesServiceMock
                    .Setup(s => s.GetGamesForHoverModel())
                    .Returns(new List<GameHoverViewModel>(){
                        new GameHoverViewModel
                        {
                            CoverImageUrl = "banan",
                            GameId = 1,
                            Name = "banan",
                            Rating = 4
                        },
                        new GameHoverViewModel
                        {
                            CoverImageUrl = "troleibus",
                            GameId  = 2,
                            Name = "Kometa",
                            Rating = 2
                        }});

                var games = new List<GameHoverViewModel>(){
                        new GameHoverViewModel
                        {
                            CoverImageUrl = "banan",
                            GameId = 1,
                            Name = "banan",
                            Rating = 4
                        },
                        new GameHoverViewModel
                        {
                            CoverImageUrl = "troleibus",
                            GameId  = 2,
                            Name = "Kometa",
                            Rating = 2
                        }};

                gamesServiceMock
                    .Setup(s => s.GetGamesForHomePage())
                    .Returns(new HomePageViewModel
                    {
                        LatestGames = games.OrderByDescending(g => g.GameId),
                        TopRatedGames = games.OrderByDescending(g => g.Rating)
                    });

                return gamesServiceMock.Object;
            }
        }
    }
}
