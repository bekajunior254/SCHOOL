namespace BackendApi.DTOs
{
    public class LoginDto
    {
        public required string Username { get; set; }  // Could be AdmissionNumber, TeacherID, or ParentID

      
        public required string Password { get; set; }
    }
}