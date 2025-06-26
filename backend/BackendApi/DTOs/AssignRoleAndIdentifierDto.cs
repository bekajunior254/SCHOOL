namespace BackendApi.DTOs
{
    public class AssignRoleAndIdentifierDto
    {
        public string UserId { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Identifier { get; set; } = null!;
    }
}
