using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BackendApi.Models;
using BackendApi.DTOs;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

        //  Fetch users who don't have any role assigned
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
    }
}
