using System.Collections.Generic;
using AuctionAppIep.Models.Database;

namespace AuctionAppIep.Models.View{
    public class AuctionDataModel{
        public Auction auction {get;set;}

        public string user {get;set;}

        public string error {get;set;}

        public ICollection<AuctionBid> auctionBids { get; set; }

        public List<string> lastusers {get;set;}
    }
}