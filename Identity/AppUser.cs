using Microsoft.AspNetCore.Identity;

namespace PBL3Hos.Identity
{
 
        public class AppUser : IdentityUser
        {
            public DateTime? Birthday { get; set; }
            public string? Address { get; set; }
            public string? City { get; set; }


        }

    
}
