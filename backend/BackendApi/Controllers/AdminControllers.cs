using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BackendApi.Models;
using BackendApi.DTOs;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("assign-role-identifier")]
        public async Task<IActionResult> AssignRoleAndIdentifier([FromBody] AssignRoleAndIdentifierDto dto)
        {
            // Validate user
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found.");

            // Assign role if not already assigned
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(dto.Role))
            {
                if (!await _roleManager.RoleExistsAsync(dto.Role))
                    return BadRequest("Invalid role.");

                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            // Assign identifier + set as username
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
    }
}
