using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuctionAppIep.Models.View;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuctionAppIep.Models.Database {
    public class User : IdentityUser {
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }

        [Required]
        public string gender {get;set;}


        public int tokens {get;set;}

        public int isValidUser {get;set;}
        //Provera da l je userName max 6 karaktera 

        //Moras da dodas listu Aukcije
        public ICollection<Auction> auctionList { get; set; }

        //Lista Narudzbina
        public ICollection<TokensOrder> tokensList { get; set; }

        //Lista licitacija
        public ICollection<AuctionBid> auctionBidsList { get; set; }


    }
    public class UserProfile : Profile {
        public UserProfile ( ) {
            base.CreateMap<RegisterModel, User>( )
              .ReverseMap( );
        }
    }
}