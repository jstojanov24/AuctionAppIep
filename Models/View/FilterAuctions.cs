using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuctionAppIep.Models.Database;

namespace AuctionAppIep.Models.View
{
    public class FilterAuctionsModel
    {

        //filtriranje
         [Display(Name = "Keyword")]
        public string word {get;set;}
        public string status {get;set;}

        [Display(Name = "Min")]
        public int min {get;set;}
        
        [Display(Name = "Max")]
        public int max {get;set;}

        public ICollection<Auction> auctions { get; set; }


        //paging
        public int currentPage { get; set; } = 1;
        public int count { get; set; }

        public int pageSize { get; set; } = 12;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(count, pageSize));

    }
}