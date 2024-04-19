using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.ViewModel
{
    public class Appointment
    {
        [Required(ErrorMessage = "UserName cannot be blank")]
        public string Username { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string DateofBirth { get; set; }
        public string City { get; set; }

    }

}
