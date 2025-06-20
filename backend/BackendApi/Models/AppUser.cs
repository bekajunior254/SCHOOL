using Microsoft.AspNetCore.Identity;

namespace BackendApi.Models
{
    public class AppUser : IdentityUser
    {
        public required string FullName { get; set; }
        public required string Role { get; set; }  // Admin, Student, Teacher, Parent
    }
}
