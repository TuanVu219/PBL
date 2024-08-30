using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.Models.DbModel
{
    public class AppointmentHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; } // Khóa chính của Appointment

        public DateTime Date { get; set; } // Thời gian cuộc hẹn

        public string Rate { get; set; } = "Unrate";

        [ForeignKey("Doctors")]
        public string DoctorId{ get; set; }
        public UserDoctor Doctor { get; set; }

        [ForeignKey("Patients")]
        public string PatientId { get; set; }
        public UserPatient Patient { get; set; }

    }
}
