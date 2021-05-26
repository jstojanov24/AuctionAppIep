using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuctionAppIep.Models.Database;
using AuctionAppIep.Models.View;
using AutoMapper;
using MessagingApp.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionAppIep.Controllers
{

    public class AuctionController : Controller
    {
        private AuctionAppContext context;
        private UserManager<User> userManager;
        private IMapper mapper;
        private SignInManager<User> signInManager;

        public AuctionController(AuctionAppContext context, UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> ShowAuction()
        {
            User loggedInUser = await this.userManager.GetUserAsync(base.User);
            ShowAuctionModel model = new ShowAuctionModel()
            {
                auctions = await this.context.auctions.Where(a => a.userId == loggedInUser.Id && a.status != "DELETED").OrderBy
                        (a => a.dateCreated).ToListAsync()

            };
            return View(model);
        }

       public async Task<IActionResult> ShowWonAuctions()
        {
            User loggedInUser = await this.userManager.GetUserAsync(base.User);
            ShowAuctionModel model = new ShowAuctionModel()
            {
                auctions = await this.context.auctions.Where(a => a.lastUser == loggedInUser.UserName && a.status == "SOLD").OrderBy
                        (a => a.dateCreated).ToListAsync()

            };
            return View(model);
        }
        public async Task<IActionResult> WonData(int id){
             Auction auction=await this.context.auctions.Where(a =>a.id==id).FirstOrDefaultAsync();
             User user=await this.context.Users.Where(d =>d.Id==auction.userId).FirstOrDefaultAsync();
             AuctionDataModel model=new AuctionDataModel(){
                 auction=auction,
                 user=user.UserName
             };
             return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if(model.dateEnd<model.dateStart) return View(model);    
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            User loggedInUser = await this.userManager.GetUserAsync(base.User);
            using (BinaryReader reader = new BinaryReader(model.file.OpenReadStream()))
            {
                Auction auction = new Auction()
                {
                    name = model.name,
                    userId = loggedInUser.Id,
                    description = model.description,
                    startingPrice = model.startingPrice,
                    bidPrice = model.startingPrice,
                    dateCreated = DateTime.Now,
                    dateEnd = model.dateEnd,
                    dateStart = model.dateStart,
                    status = "DRAFT",
                    imageData = reader.ReadBytes(Convert.ToInt32(reader.BaseStream.Length))
                };
                await this.context.auctions.AddAsync(auction);
                await this.context.SaveChangesAsync();

            }

            return RedirectToAction("UserStartPage", "User");
        }
        public async Task<IActionResult> EditAuction(int id)
        {
            Auction auction = await this.context.auctions.Where(item => item.id == id).FirstOrDefaultAsync();

            EditAuctionModel model = new EditAuctionModel()
            {
                name = auction.name,
                description = auction.description,
                startingPrice = auction.startingPrice,
                dateStart = auction.dateStart,
                dateEnd = auction.dateEnd,
                id = auction.id

            };
            return View(model);
        }
        public async Task<IActionResult> DeleteAuction(int id)
        {
            Auction auction = await this.context.auctions.Where(item => item.id == id).FirstOrDefaultAsync();
            auction.status = "DELETED";

            this.context.auctions.Update(auction);
            await this.context.SaveChangesAsync();

            return RedirectToAction("ShowAuction", "Auction");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAuction(EditAuctionModel model)
        {
            Auction auction = await this.context.auctions.Where(item => item.id == model.id).FirstOrDefaultAsync();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditAuction", "Auction", new { id = model.id });
            }
            using (BinaryReader reader = new BinaryReader(model.file.OpenReadStream()))
            {
                auction.name = model.name;
                auction.description = model.description;
                auction.startingPrice = model.startingPrice;
                auction.bidPrice = model.startingPrice;
                auction.dateStart = model.dateStart;
                auction.dateEnd = model.dateEnd;
                auction.imageData = reader.ReadBytes(Convert.ToInt32(reader.BaseStream.Length));
            }
            try
            {
                this.context.auctions.Update(auction);
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //nije uspelo
                return RedirectToAction("EditAuction", "Auction", new { id = model.id });
            }
            return RedirectToAction("ShowAuction", "Auction");


        }


    }
}