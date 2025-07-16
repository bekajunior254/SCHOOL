using BackendApi.Models;
public class Course
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public string? Description { get; set; }

    public string? TeacherId { get; set; }
    public AppUser? Teacher { get; set; }
}
