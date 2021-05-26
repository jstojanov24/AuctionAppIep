using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionAppIep.Models.Database{

    public class TokensOrder{
        [Required]
        public string userId {get;set;}

        public User user {get;set;}
        ////other
        [Key]
        public int id {get;set;}

        [Required]
        public DateTime dateOrdered {get;set;}

        [Required]
        public int tokens {get;set;}

        [Required]
        public int price {get;set;}
    }
    
    public class TokensOrderConfiguration : IEntityTypeConfiguration<TokensOrder> {
        public void Configure(EntityTypeBuilder<TokensOrder> builder) {
            builder.Property (tok =>tok.id ).ValueGeneratedOnAdd ( );

            builder.HasOne<User> ( item => item.user )
                .WithMany ( item => item.tokensList )
                .HasForeignKey ( item => item.userId );
        }
    }
}