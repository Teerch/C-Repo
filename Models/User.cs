namespace UserAuthAPI.Models;

public class User : Time
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public byte[] Password { get; set; }
    public byte[] Passwordkey { get; set; }
    public string? OtherEmail { get; set; }
    public virtual ICollection<SecurityQandA>? SecurityQandA { get; set; }
    public virtual ICollection<RecoveryPhrase>? Phrases { get; set; }
    public virtual ICollection<OTP>? Otp { get; set; }
}