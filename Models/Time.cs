namespace UserAuthAPI.Models;

    public abstract class Time
    {
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }

        public Time()
        {
            CreatedOn = DateTime.Now;
            LastModified = DateTime.UtcNow;
        }
    }
