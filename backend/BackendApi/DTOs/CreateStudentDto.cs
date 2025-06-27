namespace BackendApi.DTOs
{
    public class CreateStudentDto
    {
        public string UserId { get; set; } = null!;
        public string Program { get; set; } = null!;
        public int Year { get; set; }
    }
}