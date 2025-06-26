// Inside Models/Admin.cs or similar
namespace BackendApi.Models
{
    public class Admin
    {
        public string AdminId { get; set; } = Guid.NewGuid().ToString();
        public required string UserId { get; set; }
        public required string Department { get; set; }

        // Optional: navigation property
        public AppUser? User { get; set; }
    }
}
