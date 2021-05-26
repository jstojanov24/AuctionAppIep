using System.Collections.Generic;
using AuctionAppIep.Models.Database;

namespace AuctionAppIep.Models.View{
    
    public class AllAuctionsModel{
        public ICollection<Auction> auctions {get;set;}
    }

}