using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuctionAppIep.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AuctionAppIep.Models.View {
    public class RegisterModel{
        public string[] gens={"male","female","other"};
        public List<SelectListItem> genders {get;set;}
     
        [Required]
        [Display(Name = "First name")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string gender {get;set;}      

        [Required]
        [Display ( Name = "Username" ) ]
        [Remote ( controller: "User", action: nameof ( UserController.usernameUnique ) ) ]
        public string username { get; set; }

        [Required]
        [Display ( Name = "Password" ) ]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [Display ( Name = "Confirm password" ) ]
        [Compare ( nameof ( password ), ErrorMessage = "Password and Confirm password fields must match!" ) ]
        [DataType(DataType.Password)]
        public string confirmPassword { get; set; }



     
    }
}