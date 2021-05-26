using System.Collections.Generic;
using AuctionAppIep.Models.Database;

namespace AuctionAppIep.Models.View{
    public class AllOrdersModel{

        public ICollection<TokensOrder> orders {get;set;}
    }
}