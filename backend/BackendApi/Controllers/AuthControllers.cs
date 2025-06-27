using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using BackendApi.Models;
using BackendApi.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = normalizedEmail,
                EmailConfirmed = true,
                UserName = normalizedEmail // Temporary username; real one is assigned later
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Registration failed",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok("User registered successfully. An admin will assign a role and identifier.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var normalizedUsername = dto.Username.Trim().ToLower();

            var user = await _userManager.FindByNameAsync(normalizedUsername);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role))
                return StatusCode(403, "User has no assigned role. Contact admin.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? throw new Exception("Username is null")),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role)
            };
            

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not found in configuration.");
            var jwtIssuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not found in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                username = user.UserName,
                role = role
            });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleAndIdentifierDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Role assignment failed",
                    errors = roleResult.Errors.Select(e => e.Description)
                });
            }

            // Set identifier and username based on role
            switch (dto.Role)
            {
                case "Student":
                    user.AdmissionNumber = dto.Identifier;
                    user.UserName = dto.Identifier;
                    break;
                case "Teacher":
                    user.TeacherID = dto.Identifier;
                    user.UserName = dto.Identifier;
                    break;
                case "Parent":
                    user.ParentID = dto.Identifier;
                    user.UserName = dto.Identifier;
                    break;
                default:
                    return BadRequest("Invalid role. Only Student, Teacher, or Parent are supported.");
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Failed to update user identifier",
                    errors = updateResult.Errors.Select(e => e.Description)
                });
            }

            return Ok($"Role '{dto.Role}' and identifier '{dto.Identifier}' assigned to user.");
        }
    }
}
