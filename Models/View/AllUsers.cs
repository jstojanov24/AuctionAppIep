using System.Collections.Generic;
using AuctionAppIep.Models.Database;

namespace AuctionAppIep.Models.View{
    
    public class AllUsersModel{
        public ICollection<User> users {get;set;}
    }

}