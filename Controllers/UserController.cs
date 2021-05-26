using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionAppIep.Models.Database;
using AuctionAppIep.Models.View;
using AutoMapper;
using MessagingApp.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuctionAppIep.Controllers{
    public class UserController : Controller {
        private AuctionAppContext context;
        private UserManager<User> userManager;
        private IMapper mapper;
        private SignInManager<User> signInManager;

        public UserController(AuctionAppContext context,UserManager<User> userManager,IMapper mapper,SignInManager<User> signInManager){
            this.context=context;
            this.userManager=userManager;
            this.mapper=mapper;
            this.signInManager=signInManager;
        }
        public IActionResult Register( ) {
            RegisterModel registerModel=new RegisterModel();
            registerModel.genders = new List<SelectListItem>
                            {
                    new SelectListItem {Text = "Female", Value = "female"},
                    new SelectListItem {Text = "Male", Value = "male"},
                    new SelectListItem {Text = "Prefer not to answer", Value = "other"} };
            return View (registerModel);
        }
        public IActionResult usernameUnique ( string username ) {
            bool exists = this.context.Users.Where ( user => user.UserName == username ).Any( );

            if ( exists ) {
                return Json ( "Username already taken!" );
            } else {
                return Json ( true );
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register ( RegisterModel model ) {
            model.genders = new List<SelectListItem>
                            {
                    new SelectListItem {Text = "Female", Value = "female"},
                    new SelectListItem {Text = "Male", Value = "male"},
                    new SelectListItem {Text = "Prefer not to answer", Value = "other"} };
            if ( !ModelState.IsValid ) {
                
                return View ( model );
            }

            //User user = this.mapper.Map<User> ( model );
            User user=this.mapper.Map<User> ( model );
            
            IdentityResult result = await this.userManager.CreateAsync ( user, model.password );

            if ( !result.Succeeded ) {
                foreach ( IdentityError error in result.Errors ) {
                    ModelState.AddModelError ( "", error.Description );
                }

                return View ( model );
            }

            result = await this.userManager.AddToRoleAsync ( user, Roles.user.Name );

            if ( !result.Succeeded ) {
                foreach ( IdentityError error in result.Errors ) {
                    ModelState.AddModelError ( "", error.Description );
                }

                return View ( model );
            }

            return RedirectToAction ( nameof ( HomeController.Index ), "Home" );
        }
        public IActionResult Login (  ) {
           return View ();
        }

        public async Task<IActionResult> UserStartPage(  ) {
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

            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            if(loggedInUser.firstName=="Admin") 
               return RedirectToAction("AdminStartPage","User");
            
            UserDataModel model=await this.getUserData();
            return View (model);
            
        }
        public async Task<UserDataModel> getUserData(){
            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            UserDataModel model=new UserDataModel(){
                firstName=loggedInUser.firstName,
                lastName=loggedInUser.lastName,
                userName=loggedInUser.UserName,
                email=loggedInUser.Email,
                gender=loggedInUser.gender,
                tokens=loggedInUser.tokens
            };
            return model;
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login ( LoginModel model ) {
            //da pitam da l je korisnik validan 
            if ( !ModelState.IsValid ) {
                return View ( model );
            }
            User user= await this.context.Users.Where(u =>u.UserName==model.username).FirstOrDefaultAsync();
            if(user.isValidUser!=1){
                ModelState.AddModelError ( "", "Can't do login with username/password" );
                return View ( model );
            }

            var result = await this.signInManager.PasswordSignInAsync ( model.username, model.password, false, false );

            if ( !result.Succeeded ) {
                ModelState.AddModelError ( "", "Username or password not valid!" );
                return View ( model );
            }

            //User loggedInUser=await this.userManager.GetUserAsync(base.User);
            //bool flag= await this.userManager.IsInRoleAsync(loggedInUser,"User");
            if(model.username!="admin")
                return RedirectToAction ( "UserStartPage", "User" );
            else return  RedirectToAction ( "AdminStartPage", "User" );
            //return Redirect("UserStartPage");
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout ( ) {
            await this.signInManager.SignOutAsync ( );
            return RedirectToAction ("Index", "Home" );
        }
        //Editovanje
        public  async Task<IActionResult> Edit(){
            EditModel model= await this.getUserDataForEditing();
            return View(model);
        }
        public async Task<EditModel> getUserDataForEditing(){
            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            EditModel model=new EditModel(){
                firstName=loggedInUser.firstName,
                lastName=loggedInUser.lastName,
                email=loggedInUser.Email,
                gender=loggedInUser.gender,
                username=loggedInUser.UserName
            };
             model.genders = new List<SelectListItem>
                            {
                    new SelectListItem {Text = "Female", Value = "female"},
                    new SelectListItem {Text = "Male", Value = "male"},
                    new SelectListItem {Text = "Prefer not to answer", Value = "other"} };
            return model;
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit2(EditModel model){
             User loggedInUser=await this.userManager.GetUserAsync(base.User);
             model.genders = new List<SelectListItem>
                            {
                    new SelectListItem {Text = "Female", Value = "female"},
                    new SelectListItem {Text = "Male", Value = "male"},
                    new SelectListItem {Text = "Prefer not to answer", Value = "other"} };
            if ( !ModelState.IsValid ) {
                return RedirectToAction("Edit","User");
            }
            //kako da promenim sifru
            // User user=new User(){
            //     firstName=model.firstName,
            //     lastName=model.lastName,
            //     Email=model.email,
            //     gender=model.gender
            // };
            //IdentityResult result= await this.userManager.UpdateAsync(user);
            loggedInUser.firstName=model.firstName;
            loggedInUser.lastName=model.lastName;
            loggedInUser.Email=model.email;
            loggedInUser.gender=model.gender;
            loggedInUser.UserName=model.username;
            loggedInUser.NormalizedEmail=model.email.ToUpper();
            loggedInUser.NormalizedUserName=model.username.ToUpper();
            try{
                    this.context.Update(loggedInUser);
                    await this.context.SaveChangesAsync();
            }catch (DbUpdateConcurrencyException){
                //nije uspelo
                return RedirectToAction("Edit","User");
            }
            await this.signInManager.SignOutAsync ( );
            return RedirectToAction ("Login", "User" );



        }
        public IActionResult AdminStartPage(  ) {
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
           return View ();
        }

        public async Task<IActionResult> AllUsers(){
            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            if(loggedInUser.UserName!="admin"){
                
                return RedirectToAction("UserStartPage","User");
            }
            AllUsersModel model=new AllUsersModel(){
                    users=await this.context.Users.Where(u =>u.isValidUser==1 && u.firstName!="Admin").ToListAsync()
            };
            return View(model);
        }
        public async Task<IActionResult> AllAuctions(){
            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            if(loggedInUser.UserName!="admin"){
                return RedirectToAction("UserStartPage","User");
            }
            AllAuctionsModel model=new AllAuctionsModel(){
                auctions=await this.context.auctions.Where(u =>u.status=="DRAFT").ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> DeleteUser(string username){
            User user =await this.context.Users.Where(u =>u.UserName==username).FirstOrDefaultAsync();
            user.isValidUser=0;
            this.context.Users.Update(user);
            await this.context.SaveChangesAsync();
            return RedirectToAction("AllUsers","User");

        }
        public async Task<IActionResult> DeleteAuction(int id){
             Auction auction = await this.context.auctions.Where(item => item.id == id).FirstOrDefaultAsync();
             auction.status = "DELETED";

            this.context.auctions.Update(auction);
            await this.context.SaveChangesAsync();
            return RedirectToAction("AllAuctions","User");
        }
        public async Task<IActionResult> AcceptAuction(int id){
             Auction auction = await this.context.auctions.Where(item => item.id == id).FirstOrDefaultAsync();
             
             if(auction.dateStart<=DateTime.Now){
                 auction.status="EXPIRED";
             }else auction.status = "READY";

            this.context.auctions.Update(auction);
            await this.context.SaveChangesAsync();
            return RedirectToAction("AllAuctions","User");
        }

        public IActionResult ShowTokens(){
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyTokens(int type){
            int t=type;
            int tokens=5;
            int price=30;
            if(type==2) {
                tokens=10;
                price=50;
            }
            if(type==3) {
                tokens=20;
                price=80;
            }
            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            //treba da dodam
            TokensOrder order=new TokensOrder(){
                userId=loggedInUser.Id,
                dateOrdered=DateTime.Now,
                price=price,
                tokens=tokens
            };
            await this.context.orders.AddAsync(order);
            await this.context.SaveChangesAsync();

            loggedInUser.tokens=loggedInUser.tokens+tokens;
            this.context.Users.Update(loggedInUser);
            await this.context.SaveChangesAsync();
            //return RedirectToAction("ShowTokens","User");
            return Json(true);
        }

        public async Task<IActionResult> ShowOrders(){
            User loggedInUser=await this.userManager.GetUserAsync(base.User);
            AllOrdersModel model=new AllOrdersModel(){

                orders= await this.context.orders.Where(o =>o.userId==loggedInUser.Id).ToListAsync()
            };
            return View(model);

        }

        



    }
}