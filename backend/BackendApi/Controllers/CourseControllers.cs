// Controllers/CourseController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/courses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _context.Courses
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description
            })
            .ToListAsync();

        return Ok(courses);
    }

    // POST: api/courses
    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseDto dto)
    {
        var course = new Course
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Course created successfully" });
    }

    // PUT: api/courses/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        course.Name = dto.Name;
        course.Code = dto.Code;
        course.Description = dto.Description;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Course updated successfully" });
    }

    // DELETE: api/courses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Course deleted successfully" });
    }
}