using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.Areas.Admin.Models
{
    public class RegisterVMDT
    {

        [Required(ErrorMessage = "UserName cannot be blank")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Fullname cannot be blank")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Password cannot be blank")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmpassword cannot be blank")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must be a 10-digit number")]
        public string Mobile { get; set; }

        public DateTime? DateofBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Career {  get; set; }
       public string Specialist { get; set; }

        [Required(ErrorMessage = "Qualification cannot be blank")]
        public string qualification { get; set; }

        [Required(ErrorMessage = "Year of Experience cannot be blank")]
        public string yearofexperience { get; set; }

        [Required(ErrorMessage = "Study Process cannot be blank")]
        public string studyprocess { get; set; }
    }
}
