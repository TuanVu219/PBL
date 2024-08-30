using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PBL3Hos.Models.DbModel
{
    public class Specialist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Khóa chính

        public string SpecialistName { get; set; }
    }
}
