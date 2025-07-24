using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendApi.Data;
using BackendApi.Models;
using BackendApi.DTOs;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .ToListAsync();

            return Ok(teachers);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto)
        {
            var teacher = new Teacher
            {
                TeacherId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                Department = dto.Department
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return Ok(teacher);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTeacherDto dto)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            teacher.Department = dto.Department;
            await _context.SaveChangesAsync();
            return Ok(teacher);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        // Teacher: View their own profile
        [HttpGet("me")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null) return NotFound();

            return Ok(teacher);
        }
[HttpGet("debug")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DebugTeachers()
        {
            try
            {
                // Just get the raw teachers
                var teachers = await _context.Teachers.ToListAsync();

                return Ok(new {
                    Count = teachers.Count,
                    Teachers = teachers,
                    Message = teachers.Count == 0 ? "No teachers found" : $"Found {teachers.Count} teachers"
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }
    } }