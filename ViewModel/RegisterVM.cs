
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
namespace PBL3Hos.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "UserName cannot be blank")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password cannot be blank")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmpassword cannot be blank")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }

}
