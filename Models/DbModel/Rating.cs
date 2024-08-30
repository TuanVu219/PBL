using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.Models.DbModel
{
    public class Rating
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; } // Khóa chính của Appointment

        public double Star { get; set; }

        public string? Detail {  get; set; }
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
