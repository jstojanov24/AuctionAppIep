using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuctionAppIep.Models;
using Microsoft.AspNetCore.Authorization;
using AuctionAppIep.Models.View;
using MessagingApp.Models.Database;
using AuctionAppIep.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AuctionAppIep.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        AuctionAppContext context;
        UserManager<User> userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,AuctionAppContext context,UserManager<User> userManager)
        {
            _logger = logger;
            this.context=context;
            this.userManager=userManager;
        }

        [AllowAnonymous]
        public IActionResult Index(){
            /*int curr;
            ICollection<Auction> auctions2=this.context.auctions.ToList();
            FilterAuctionsModel model2=new FilterAuctionsModel(){
                auctions=this.context.auctions.OrderBy(d =>d.id)
                .Take(12).ToList(),
                pageSize=12,
                currentPage=1,
                count=auctions2.Count
            };
            return View(model2);*/
            ICollection<Auction> auctions=this.context.auctions
                .Where(d => d.status=="READY" && d.dateStart<=DateTime.Now).ToList();
            foreach (Auction item in auctions)
            {
                item.status="OPEN";
                this.context.auctions.Update(item);
                 this.context.SaveChanges();   
            }    
            auctions=this.context.auctions
                .Where(d =>d.status=="OPEN" && d.dateEnd<=DateTime.Now).ToList();
            foreach (Auction item in auctions)
            {
                item.status="SOLD";
                this.context.auctions.Update(item);
                 this.context.SaveChanges();   
            } 
            auctions=this.context.auctions
                .Where(d =>d.status=="DRAFT" && d.dateStart<=DateTime.Now).ToList();
            foreach (Auction item in auctions)
            {
                item.status="EXPIRED";
                this.context.auctions.Update(item);
                 this.context.SaveChanges();   
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAuctions(int? currentpage, int max,int min,string word)
        {
            //sa ovim parametrimaa da sredi
            int curr;
             if (max==0) max=int.MaxValue;
              string keyword=string.IsNullOrEmpty(word)?"":word;
            ICollection<Auction> auctions2= await this.context.auctions.
                Where(d =>   (d.status=="READY" || d.status=="OPEN") && d.bidPrice>=min && d.bidPrice<=max && d.name.Contains(keyword)).ToListAsync();
            if(currentpage!=null){
                    curr=currentpage.Value;
            }else{
                curr=1;
            }
           
            FilterAuctionsModel model2=new FilterAuctionsModel(){
                auctions=await this.context.auctions.Where( d =>   (d.status=="READY" || d.status=="OPEN") && 
                d.bidPrice>=min && d.bidPrice<=max
                && d.name.Contains(keyword)).
                OrderBy(d =>d.id)
                .Skip((curr-1)*12).Take(12).ToListAsync(),
                pageSize=12,
                currentPage=curr,
                count=auctions2.Count,
                max=max,
                min=min,
                word=keyword

            };
            //return  PartialView("IndexAuctions",model2);
           // return RedirectToAction("Index");
           return View(model2);
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAuctionsWithout()
        {
            //sa ovim parametrimaa da sredi
           // this.updateAuctions();
            ICollection<Auction> auctions2= await this.context.auctions.
                    Where(d=>d.status=="OPEN" ||
                        d.status=="READY").ToListAsync();
            FilterAuctionsModel model2=new FilterAuctionsModel(){
                auctions=await this.context.auctions.Where(d=>d.status=="OPEN" ||
                d.status=="READY").OrderBy(d =>d.id)
                .Take(12).ToListAsync(),
                pageSize=12,
                currentPage=1,
                count=auctions2.Count
            };
            //return  PartialView("IndexAuctions",model2);
           // return RedirectToAction("Index");
           return View("IndexAuctions",model2);
        }

        [AllowAnonymous]
        public IActionResult Filter(int min, int max,string word){
            if(max==0) max=int.MaxValue;
            string keyword=string.IsNullOrEmpty(word)?"":word;
            ICollection<Auction> auctions2=this.context.auctions.Where(d =>  (d.status=="READY" || d.status=="OPEN") &&  d.bidPrice>=min && d.bidPrice<=max && d.name.Contains(keyword)).ToList();
            FilterAuctionsModel model2=new FilterAuctionsModel(){
                auctions=this.context.auctions.Where(d =>  (d.status=="READY" || d.status=="OPEN")
                && d.bidPrice>=min && d.bidPrice<=max && d.name.Contains(keyword)).OrderBy(d =>d.id)
                .Take(12).ToList(),
                pageSize=12,
                currentPage=1,
                count=auctions2.Count,
                min=min,
                max=max,
                word=keyword
            };
            return View("IndexAuctions",model2);
        }
        public async Task<IActionResult> AuctionData(int id){
            Auction auction=await this.context.auctions.Where(a =>a.id==id).FirstOrDefaultAsync();
            string userid =auction.userId;
            User user=await this.context.Users.Where(u =>u.Id==userid).FirstOrDefaultAsync();
            //dodaj licitacije 
            //string [] lastUsers=new string[10];
            ICollection<AuctionBid> auctionBids=await this.context.auctionBids.Where(a =>a.auctionid==auction.id)
                    .OrderByDescending(d =>d.bidDate).Take(10).ToListAsync();
            List<string> list=new List<string>();
            // int i=0;
            foreach(AuctionBid ab in auctionBids){
                        User user1 =await  this.context.Users.Where(u =>u.Id==ab.userId).FirstOrDefaultAsync();
                        list.Add(user1.UserName);
                        
             }
            AuctionDataModel model=new AuctionDataModel(){
                auction=auction,
                user=user.UserName,
                error="",
                auctionBids=auctionBids,
                lastusers=list      
            };
            return View(model);
        }

        public async Task<IActionResult>  BidAuction(int id){
            //ne smes na svoju da licitiras
            //aukcija mora biti open
            //moras da imas tokene na stanju

            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            Auction auction= await this.context.auctions.Where(a => a.id==id).FirstOrDefaultAsync();
            ICollection<AuctionBid> auctionBids=await this.context.auctionBids.Where(a =>a.auctionid==auction.id)
                        .OrderByDescending(d =>d.bidDate).Take(10).ToListAsync();
            //tvoja licitacija
            List<string> list=new List<string>();
            // int i=0;
            foreach(AuctionBid ab in auctionBids){
                        User user1 =await  this.context.Users.Where(u =>u.Id==ab.userId).FirstOrDefaultAsync();
                        list.Add(user1.UserName);
                        
             }
            if(auction.userId==loggedInUser.Id){
                return  View("AuctionData",new AuctionDataModel(){
                        auction=auction,
                        user=loggedInUser.UserName,
                        error="Can't bid on your own auction!",
                        auctionBids=auctionBids,
                        lastusers=list
                });
            }
            User owner= await this.context.Users.Where(u =>u.Id==auction.userId).FirstOrDefaultAsync();
            //nije otvorena licitacija
            if(auction.status!="OPEN"){
                return  View("AuctionData",new AuctionDataModel(){
                        auction=auction,
                        user=owner.UserName,
                        error="Auction is not open! Come back later!",
                        auctionBids=auctionBids,
                        lastusers=list
                });
            }
            //nema dovoljno tokena
            if(loggedInUser.tokens<=0){
                return  View("AuctionData",new AuctionDataModel(){
                        auction=auction,
                        user=owner.UserName,
                        error="Not enough tokens!",
                        auctionBids=auctionBids,
                        lastusers=list
                });
            }
            //kreiraj licitaciju 
            AuctionBid bid=new AuctionBid(){
                bidDate=DateTime.Now,
                userId=loggedInUser.Id,
                auctionid=id
            };
            await this.context.auctionBids.AddAsync(bid);
            await this.context.SaveChangesAsync();
            //Update za korisnika
            loggedInUser.tokens=loggedInUser.tokens-1;
            this.context.Users.Update(loggedInUser);
            await this.context.SaveChangesAsync();
            //povecaj cenu  i oznaci poslednjeg koji je licitirao
            auction.bidPrice=auction.bidPrice+10;
            auction.lastUser=loggedInUser.UserName;
            this.context.auctions.Update(auction);
             await this.context.SaveChangesAsync();
             auctionBids=await this.context.auctionBids.Where(a =>a.auctionid==auction.id)
                        .OrderByDescending(d =>d.bidDate).Take(10).ToListAsync();
            list=new List<string>();
            // int i=0;
            foreach(AuctionBid ab in auctionBids){
                        User user1 =await  this.context.Users.Where(u =>u.Id==ab.userId).FirstOrDefaultAsync();
                        list.Add(user1.UserName);
                        
             }            
            AuctionDataModel model=new AuctionDataModel(){
                auction=auction,
                error="",
                user=owner.UserName,
                auctionBids=auctionBids,
                lastusers=list
            };
            return View("AuctionData",model);
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public  void updateAuctions(){
            ICollection<Auction> auctions=this.context.auctions
                .Where(d => d.status=="READY" && d.dateStart<=DateTime.Now).ToList();
            foreach (Auction item in auctions)
            {
                item.status="OPEN";
                this.context.auctions.Update(item);
                 this.context.SaveChanges();   
            }    
            auctions=this.context.auctions
                .Where(d =>d.status=="OPEN" && d.dateEnd<=DateTime.Now).ToList();
            foreach (Auction item in auctions)
            {
                item.status="SOLD";
                this.context.auctions.Update(item);
                 this.context.SaveChanges();   
            }      

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
