namespace UserAuthAPI.Models;

public class RecoveryPhrase
{
    public int Id { get; set; }
    public byte[] Phrase { get; set; }
    public byte[] PhraseKey { get; set; }
    public byte[] PhraseIV { get; set; }
    public int UserId { get; set; }
    public string Useremail { get; set; }
    public virtual User User { get; set; }
}
