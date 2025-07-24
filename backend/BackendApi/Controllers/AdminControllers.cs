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
    [Authorize(Roles = "Admin")]
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

        // ✅ Assign Role and Identifier + Auto Insert to Role Table
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

            // Set the identifier and username
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
                case "Admin":
                    // Admins don't have special ID field
                    break;
                default:
                    return BadRequest("Unsupported role.");
            }

            user.UserName = dto.Identifier;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 🔁 Auto-create corresponding entity
            switch (dto.Role)
            {
                case "Student":
                    if (!await _context.Students.AnyAsync(s => s.UserId == user.Id))
                    {
                        _context.Students.Add(new Student
                        {
                            StudentId = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                           // GradeLevel = "Unassigned"
                        });
                    }
                    break;

                case "Teacher":
                    if (!await _context.Teachers.AnyAsync(t => t.UserId == user.Id))
                    {
                        _context.Teachers.Add(new Teacher
                        {
                            TeacherId = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            Department = "Unassigned"
                        });
                    }
                    break;

               case "Parent":
                   /* if (!await _context.Parents.AnyAsync(p => p.UserId == user.Id))
                    {
                        _context.Parents.Add(new Parent
                        {
                            ParentId = Guid.NewGuid().ToString(),
                            UserId = user.Id
                        });
                    } */
                    break;

                case "Admin":
                    if (!await _context.Admins.AnyAsync(a => a.UserId == user.Id))
                    {
                        _context.Admins.Add(new Admin
                        {
                            AdminId = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            Department = "System"
                        });
                    }
                    break;
            }

            await _context.SaveChangesAsync();
            return Ok("Role and identifier assigned, and role entity added.");
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

        // ✅ Admin dashboard stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var studentCount = await _context.Students.CountAsync();
            var teacherCount = await _context.Teachers.CountAsync();
           // var parentCount = await _context.Parents.CountAsync();
            var adminCount = await _context.Admins.CountAsync();

            return Ok(new
            {
                totalStudents = studentCount,
                totalTeachers = teacherCount,
               // totalParents = parentCount,
                totalAdmins = adminCount
            });
        }
    }
}
