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

        public DbSet<PegiRating> PegiRatings { get; init; }

        public DbSet<ShoppingCart> ShoppingCarts { get; init; }

        public DbSet<ShoppingCartProduct> ShoppingCartProducts { get; init; }

        public DbSet<ClientRelationship> ClientRelationships { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Client>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Client>()
                .HasOne<ShoppingCart>(c => c.ShoppingCart)
                .WithOne(sc => sc.Client)
                .HasForeignKey<Client>(c => c.ShoppingCartId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Client>()
                .HasMany<ClientRelationship>(c => c.Friends)
                .WithOne(cr => cr.Client)
                .HasForeignKey(cr => cr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Client>()
                .HasMany<ClientRelationship>(c => c.Friends)
                .WithOne(cr => cr.Client)
                .HasForeignKey(cr => cr.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Publisher>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<Publisher>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Publisher>()
                .HasMany(p => p.Games)
                .WithOne(g => g.Publisher)
                .HasForeignKey(g => g.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<PegiRating>()
                .HasMany(p => p.Games)
                .WithOne(g => g.PegiRating)
                .HasForeignKey(g => g.PegiRatingId)
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
                .Entity<ClientGame>()
                .HasKey(x => new { x.ClientId, x.GameId });

            builder
                .Entity<GameGenre>()
                .HasKey(x => new { x.GameId, x.GenreId });

            builder
                .Entity<ShoppingCartProduct>()
                .HasOne<ShoppingCart>(scp => scp.ShoppingCart)
                .WithMany(sc => sc.ShoppingCartProducts)
                .HasForeignKey(scp => scp.ShoppingCartId);

            builder
                .Entity<ShoppingCartProduct>()
                .HasOne<Game>(scp => scp.Game)
                .WithMany(g => g.ShoppingCartProducts)
                .HasForeignKey(scp => scp.GameId);

            base.OnModelCreating(builder);
        }
    }
}
