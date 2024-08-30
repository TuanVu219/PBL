using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.Models.DbModel
{
    public class AppointmentCancel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; } // Khóa chính của Appointment

        public DateTime Date { get; set; } // Thời gian cuộc hẹn
        public string FullnameDoctor { get; set; }
        public string FullnamePatient { get; set; }

        public string? Availabale { get; set; } = "Unseen";


        // Khóa ngoại trỏ đến Doctor
        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public virtual UserDoctor Doctor { get; set; }

        // Khóa ngoại trỏ đến Patient
        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public virtual UserPatient Patient { get; set; }
    }
}
