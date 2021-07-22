namespace GameStore.Infrastructure
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var data = scopedServices.ServiceProvider.GetService<GameStoreDbContext>();

            data.Database.Migrate();

            SeedGenres(data);
            SeedPegiRatings(data);

            return app;
        }

        private static void SeedGenres(GameStoreDbContext data)
        {
            if (data.Genres.Any()) return;

            data.Genres.AddRange(new[]
            {
                new Genre { Name = "RPG"},
                new Genre { Name = "Shooter"},
                new Genre { Name = "Action"},
                new Genre { Name = "Mystery"},
                new Genre { Name = "Strategy"},
                new Genre { Name = "Sci-Fi"},
                new Genre { Name = "Fantasy"},
                new Genre { Name = "Sports"},
                new Genre { Name = "Racing"},
                new Genre { Name = "Platformer"},
                new Genre { Name = "Third Person"},
                new Genre { Name = "MMORPG"},
                new Genre { Name = "MOBA"},
                new Genre { Name = "Sandbox"},
                new Genre { Name = "Simulator"},
            });

            data.SaveChanges();
        }

        private static void SeedPegiRatings(GameStoreDbContext data)
        {
            if (data.PegiRatings.Any()) return;

            data.PegiRatings.AddRange(new PegiRating[]
            {
                new PegiRating { Name = "PEGI 03"},
                new PegiRating { Name = "PEGI 07"},
                new PegiRating { Name = "PEGI 12"},
                new PegiRating { Name = "PEGI 16"},
                new PegiRating { Name = "PEGI 18"}
            });

            data.SaveChanges();
        }
    }
}
