using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendApi.Data;
using BackendApi.Models;
using BackendApi.DTOs;
using Microsoft.AspNetCore.Identity;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Admin: Get all students
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAll()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .ToListAsync();

            return Ok(students);
        }

        // Admin: Create student
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
        {
            var student = new Student
            {
                StudentId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                Program = dto.Program,
                Year = dto.Year
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok(student);
        }

        // Admin: Update student
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStudentDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            student.Program = dto.Program;
            student.Year = dto.Year;

            await _context.SaveChangesAsync();
            return Ok(student);
        }

        // Admin: Delete student
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        // Student: View own profile
        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null) return NotFound();

            return Ok(student);
              // Student: View enrolled courses
      /*  [HttpGet("enrolled-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetEnrolledCourses()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null) return NotFound();

            var courses = await _context.Enrollments
                .Where(e => e.StudentId == student.StudentId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();

            return Ok(courses);
        }

        // Student: View available courses
       /* [HttpGet("available-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAvailableCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }*/

        // Student: View own grades
      /*  [HttpGet("grades")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyGrades()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null) return NotFound();

            var grades = await _context.Grades
                .Where(g => g.StudentId == student.StudentId)
                .Include(g => g.Course)
                .ToListAsync();

            return Ok(grades);  }

        // Student: View attendance
       // [HttpGet("attendance")]
       // [Authorize(Roles = "Student")]
       /* public async Task<IActionResult> GetMyAttendance()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null) return NotFound();

            var attendance = await _context.AttendanceRecords
                .Where(a => a.StudentId == student.StudentId)
                .Include(a => a.Course)
                .ToListAsync();

            return Ok(attendance);*/
        }
    }
}
