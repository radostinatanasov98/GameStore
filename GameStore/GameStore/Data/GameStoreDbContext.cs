namespace GameStore.Data
{
    using GameStore.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class GameStoreDbContext : IdentityDbContext
    {
        public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; init; }

        public DbSet<Game> Games { get; init; }

        public DbSet<Genre> Genres { get; init; }

        public DbSet<Publisher> Publishers { get; init; }

        public DbSet<Requirements> Requirements { get; init; }

        public DbSet<Review> Reviews { get; init; }

        public DbSet<ClientGame> ClientGames { get; init; }

        public DbSet<GameGenre> GameGenres { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Client>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Publisher>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<Publisher>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Review>()
                .HasOne<Game>()
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Review>()
                .HasOne<Client>()
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Game>()
                .HasOne(g => g.MinimumRequirements)
                .WithOne()
                .HasForeignKey<Game>(g => g.MinimumRequirementsId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Game>()
                .HasOne(g => g.RecommendedRequirements)
                .WithOne()
                .HasForeignKey<Game>(g => g.RecommendedRequirementsId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Game>()
                .HasOne<Publisher>()
                .WithMany(p => p.Games)
                .HasForeignKey(g => g.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<ClientGame>()
                .HasKey(x => new { x.ClientId, x.GameId });

            builder
                .Entity<ClientGame>()
                .HasOne<Game>()
                .WithMany(g => g.ClientGames)
                .HasForeignKey(cg => cg.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<ClientGame>()
                .HasOne<Client>()
                .WithMany(g => g.ClientGames)
                .HasForeignKey(cg => cg.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<GameGenre>()
                .HasKey(x => new { x.GameId, x.GenreId });

            builder
                .Entity<GameGenre>()
                .HasOne<Game>()
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<GameGenre>()
                .HasOne<Genre>()
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
