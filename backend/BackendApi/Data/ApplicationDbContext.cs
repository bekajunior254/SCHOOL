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
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        // Optional: if you're using attendance or grades later
        // public DbSet<Attendance> Attendance { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Setup Course–Teacher relationship
            builder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany() // No navigation from AppUser to courses
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.SetNull); // if teacher is deleted, don't delete course
        }
    }
}
