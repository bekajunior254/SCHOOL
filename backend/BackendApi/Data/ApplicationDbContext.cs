using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BackendApi.Models;

namespace BackendApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
      public DbSet<Admin> Admins { get; set; } 
        // Add your DbSets here (Students, Teachers, etc.)
    }
}
