namespace UserAuthAPI.Models;
public class SecurityQandA
{
    public int Id { get; set; }
    public string Question { get; set; }
    public byte[] Answer { get; set; }
    public byte[] Answerkey { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; }
    public virtual User? User { get; set; }
}