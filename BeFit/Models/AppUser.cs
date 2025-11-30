using Microsoft.AspNetCore.Identity;

namespace BeFit.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Age { get; set; }
        public string? Gender { get; set; }
    }
}
