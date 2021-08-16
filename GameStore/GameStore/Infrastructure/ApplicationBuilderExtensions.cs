namespace GameStore.Infrastructure
{
    using GameStore.Data;
    using GameStore.Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var serviceProvider = scopedServices.ServiceProvider;

            var data = serviceProvider.GetRequiredService<GameStoreDbContext>();

            data.Database.Migrate();

            SeedGenres(data);
            SeedPegiRatings(data);
            SeedAdministrator(serviceProvider);

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

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
                    if (await roleManager.RoleExistsAsync("Administrator"))
                    {
                        return;
                    }

                    var role = new IdentityRole { Name = "Administrator" };

                    await roleManager.CreateAsync(role);

                    const string adminEmail = "admin@gs.com";
                    const string adminPassword = "654321";

                    var user = new User
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        Name = "Admin"
                    };

                    await userManager.CreateAsync(user, adminPassword);

                    await userManager.AddToRoleAsync(user, role.Name);
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}
