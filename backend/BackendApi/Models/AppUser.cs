using Microsoft.AspNetCore.Identity;

namespace BackendApi.Models
{
    public class AppUser : IdentityUser
    {
        public  string FirstName { get; set; } = string.Empty;
        public  string LastName { get; set; } = string.Empty;
        public string? AdmissionNumber { get; set; }
       public string? TeacherID { get; set; }
      public string? ParentID { get; set; }
 }
   
}
