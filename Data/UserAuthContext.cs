using Microsoft.EntityFrameworkCore;
using UserAuthAPI.Models;

namespace UserAuthAPI.Data;

public class UserAuthContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<SecurityQandA> SecurityQandAs { get; set; }
    public DbSet<RecoveryPhrase> RecoveryPhrases { get; set; }
    public DbSet<OTP> OTPs { get; set; }
    public UserAuthContext(DbContextOptions<UserAuthContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable(nameof(User));
        modelBuilder.Entity<SecurityQandA>().ToTable(nameof(SecurityQandA));
        modelBuilder.Entity<RecoveryPhrase>().ToTable(nameof(RecoveryPhrase));
        modelBuilder.Entity<OTP>().ToTable(nameof(OTP));

        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired();
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<User>().Property(x => x.Password).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.FirstName).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.LastName).IsRequired();



        modelBuilder.Entity<SecurityQandA>().Property(x => x.Question).IsRequired();
        modelBuilder.Entity<SecurityQandA>().Property(x => x.Answer).IsRequired();
        modelBuilder.Entity<SecurityQandA>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<SecurityQandA>().HasKey(x => x.Id);


        modelBuilder.Entity<RecoveryPhrase>().HasKey(x => x.Id);
        modelBuilder.Entity<RecoveryPhrase>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<RecoveryPhrase>().HasIndex(x => x.Useremail).IsUnique();


        modelBuilder.Entity<OTP>().HasKey(x => x.Id);
        modelBuilder.Entity<OTP>().Property(x => x.Id).ValueGeneratedOnAdd();


        // Relationships

        modelBuilder.Entity<User>()
            .HasMany(x => x.SecurityQandA)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<User>()
            .HasMany(x => x.Phrases)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<User>()
            .HasMany(x => x.Otp)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}