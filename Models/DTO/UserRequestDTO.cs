namespace UserAuthAPI.Models.DTO;

public class UserRequestDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? OtherEmail { get; set; } = null;
    public string Password { get; set; }
}