using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    public class Student
    {
        [Key]
        public string StudentId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string AdmissionNumber { get; set; } = null!;  // used as username

        public string Department { get; set; } = "General";
       public string Program { get; set; } = "Undeclared";

        public int Year { get; set; } = 1;
        // Navigation properties
        [ForeignKey("UserId")]
        public AppUser User { get; set; } = null!;

       // public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

      //  public ICollection<Grade> Grades { get; set; } = new List<Grade>();

       // public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
