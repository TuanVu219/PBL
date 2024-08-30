using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3Hos.Models.DbModel
{
    public class DoctorAvailability
    {

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public string Id { get; set; }
            public string AvailableDate { get; set; } 
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string? Shift { get; set; }
        [ForeignKey("Doctor")]
            public string DoctorId { get; set; }

            public virtual UserDoctor Doctor { get; set; }
        }
    
}
