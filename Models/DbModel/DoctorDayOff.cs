using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.Models.DbModel
{
    public class DoctorDayOff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public DateOnly DateOff { get; set; }
        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }

        public virtual UserDoctor Doctor { get; set; }
    }
}
