using PBL3Hos.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PBL3Hos.Identity;
namespace PBL3Hos.Models.DbModel
{
   

        public class UserPatient
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

            public string id { get; set; } // Khóa chính

            public string NamePatient { get; set; }
            public string Phone { get; set; }
            public string Fullname { get; set; }

            public string? ImgFile { get; set; }

            // Khóa ngoại trỏ đến khóa chính trong một bảng khác (AspNetUsers)

            [ForeignKey("AppUser")]
            public string AccountId { get; set; }
            public AppUser AppUser { get; set; }

            public AppointmentDB Appointment { get; set; }
            public ICollection<Rating> Ratings { get; set; }
        public ICollection<AppointmentCancel> AppointmentCancels { get; set; }




    }


}
