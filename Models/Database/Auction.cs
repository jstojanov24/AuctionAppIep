using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionAppIep.Models.Database{

    public class Auction {
        //user relationship
        [Required]
        public string userId {get;set;}

        public User user {get;set;}
        ////other

        [Key]
        public int id {get;set;}

        
        [Required]
        public string name {get;set;}

        [Required]
        public string description {get;set;}

        [Required]
        public byte[] imageData { get; set; }

        
         [Required]
        public int startingPrice {get;set;}

        public int bidPrice {get;set;}

         [Required]
         public DateTime dateCreated {get;set;}

         [Required]
         public DateTime dateStart {get;set;}

         [Required]
         public DateTime dateEnd {get;set;}

         public string status {get;set;}

         public string lastUser {get;set;}

         //Lista licitacija
         public ICollection<AuctionBid> auctionBidsList { get; set; }

    }
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction> {
        public void Configure(EntityTypeBuilder<Auction> builder) {
            builder.Property (auc =>auc.id ).ValueGeneratedOnAdd ( );

            builder.HasOne<User> ( item => item.user )
                .WithMany ( item => item.auctionList )
                .HasForeignKey ( item => item.userId );
        }
    }
}