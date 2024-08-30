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
             public string Fullname { get; set; }
             public string? Career {  get; set; }
            public string Specialist {  get; set; }

            public string? qualification { get; set; }
            public string? yearofexperience { get; set; }

            public string? studyprocess {  get; set; }

             public string? ImgFile { get; set; }

        public double DoctorRate { get; set; } = 0;

        // Khóa ngoại trỏ đến khóa chính trong một bảng khác (AspNetUsers)

        [ForeignKey("AppUser")]
            public string AccountId { get; set; }
            public AppUser AppUser { get; set; }

            public ICollection<AppointmentDB> Appointments { get; set; }
             public ICollection<Rating> Ratings { get; set; }

        public ICollection<AppointmentCancel> AppointmentCancels { get; set; }


    }

}
