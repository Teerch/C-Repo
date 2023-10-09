namespace UserAuthAPI.Models
{
    public class OTP
    {
        public OTP()
        {
            CreatedOn = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public int Code { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; }
        public string Useremail { get; set; }
        public virtual User? User { get; set; }


    }
}