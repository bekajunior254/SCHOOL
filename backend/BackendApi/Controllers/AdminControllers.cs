using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendApi.Models;
using BackendApi.DTOs;
using BackendApi.Data;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Uncomment this in production
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // ✅ Assign Role and Identifier
        [HttpPost("assign-role-identifier")]
        public async Task<IActionResult> AssignRoleAndIdentifier([FromBody] AssignRoleAndIdentifierDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(dto.Role))
            {
                if (!await _roleManager.RoleExistsAsync(dto.Role))
                    return BadRequest("Invalid role.");

                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            // Set the identifier + UserName
            switch (dto.Role)
            {
                case "Student":
                    user.AdmissionNumber = dto.Identifier;
                    break;
                case "Teacher":
                    user.TeacherID = dto.Identifier;
                    break;
                case "Parent":
                    user.ParentID = dto.Identifier;
                    break;
                default:
                    return BadRequest("Unsupported role.");
            }

            user.UserName = dto.Identifier;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Role and identifier assigned successfully.");
        }

        // ✅ Get users with no role
        [HttpGet("unassigned-users")]
        public async Task<IActionResult> GetUnassignedUsers()
        {
            var allUsers = _userManager.Users.ToList();
            var unassignedUsers = new List<object>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles == null || !roles.Any())
                {
                    unassignedUsers.Add(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email
                    });
                }
            }

            return Ok(unassignedUsers);
        }

        // ✅ New: Get Admin Dashboard Stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var studentCount = await _context.Students.CountAsync();
            var teacherCount = await _context.Teachers.CountAsync();
          //  var courseCount = await _context.Courses.CountAsync();

            return Ok(new
            {
                totalStudents = studentCount,
                totalTeachers = teacherCount,
               // totalCourses = courseCount
            });
        }
    }
}