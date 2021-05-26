using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionAppIep.Models.View{
    public class EditAuctionModel{

        [HiddenInput]
        public int id {get;set;}

         [Required]
         [Display(Name = "Auction name")]
        public string name {get;set;}

        [Required]
        [Display(Name = "Description")]
        public string description {get;set;} 
        
        [Required]
        [Display(Name="Offer price")]
        public int startingPrice {get;set;}

         [Required]
         [Display(Name="Auction opens")]
         public DateTime dateStart {get;set;}

         [Required]
        [Display(Name="Auction closes")]
         public DateTime dateEnd {get;set;}

        [Display ( Name = "Picture")]
        [Required]
         public IFormFile file {get;set;}
    }
}