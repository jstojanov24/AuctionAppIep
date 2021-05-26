using System.Linq;
using AuctionAppIep.Models.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Models.Database {
    public class AuctionAppContext : IdentityDbContext<User> {
        public DbSet<Auction> auctions { get; set; }
        public DbSet<TokensOrder> orders { get; set; }

        public DbSet<AuctionBid> auctionBids {get;set;}
        public AuctionAppContext ( DbContextOptions options ) : base ( options ) { }

        protected override void OnModelCreating ( ModelBuilder builder ) {
            base.OnModelCreating ( builder );
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
                 relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

            builder.ApplyConfiguration ( new IdentityRoleConfiguration ( ) );
            builder.Entity<User>().Property(x => x.tokens).HasDefaultValue(0);
            builder.Entity<User>().Property(x => x.isValidUser).HasDefaultValue(1);
            builder.ApplyConfiguration(new AuctionConfiguration());
            builder.ApplyConfiguration(new TokensOrderConfiguration());
            builder.ApplyConfiguration(new AuctionBidConfiguration());


        }
    }
}