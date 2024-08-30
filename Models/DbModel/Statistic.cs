using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.Models.DbModel
{
    public class Statistic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; } // Khóa chính của Appointment

        public int AmountPatient {  get; set; }

        public double AmountHours { get; set; }
       
        // Khóa ngoại trỏ đến Doctor
        [ForeignKey("Doctors")]
        public string DoctorId { get; set; }
        public virtual UserDoctor Doctor { get; set; }

      
    }
}
