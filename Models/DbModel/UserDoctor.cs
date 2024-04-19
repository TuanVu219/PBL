using PBL3Hos.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PBL3Hos.Identity;
namespace PBL3Hos.Models.DbModel
{
  

    
        public class UserDoctor
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public string id { get; set; } // Khóa chính

            public string NameDoctor { get; set; }
            public string Phone { get; set; }

            // Khóa ngoại trỏ đến khóa chính trong một bảng khác (AspNetUsers)

            [ForeignKey("AppUser")]
            public string AccountId { get; set; }
            public AppUser AppUser { get; set; }

            public ICollection<AppointmentDB> Appointments { get; set; }
        }
    
}
