using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionAppIep.Models.Database
{
    public class AuctionBid
    {

        [Key]
        public int id { get; set; }

        [Required]
        public DateTime bidDate { get; set; }

        //user
        [Required]
        public string userId { get; set; }

        public User user { get; set; }

        //auction
        [Required]
        public int auctionid { get; set; }

        public Auction auction { get; set; }
    }
    public class AuctionBidConfiguration : IEntityTypeConfiguration<AuctionBid>
    {
        public void Configure(EntityTypeBuilder<AuctionBid> builder)
        {
            builder.Property(auc => auc.id).ValueGeneratedOnAdd();

            builder.HasOne<User>(item => item.user)
                .WithMany(item => item.auctionBidsList)
                .HasForeignKey(item => item.userId);

            builder.HasOne<Auction>(item => item.auction)
               .WithMany(item => item.auctionBidsList)
               .HasForeignKey(item => item.auctionid);
        }
    }
}