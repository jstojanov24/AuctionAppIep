using System.Collections.Generic;
using AuctionAppIep.Models.Database;

namespace AuctionAppIep.Models.View{
    public class ShowAuctionModel {
        public ICollection<Auction> auctions { get; set; }
    }
}