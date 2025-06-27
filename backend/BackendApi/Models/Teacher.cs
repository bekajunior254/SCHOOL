using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    public class Teacher
    {
        [Key]
        public string TeacherId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = null!;

        public string Department { get; set; } = null!;

        // Navigation property
        [ForeignKey("UserId")]
        public AppUser User { get; set; } = null!;
    }
}
