using ChatApplication.Models;
using ChatApplication.Models._2FA_Models;
using ChatApplication.Models.ChatMessageModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<UserOtp> UserOtps { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between ApplicationUser and ChatMessage (Sender)
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Sender) // Sender property in ChatMessage
                .WithMany() // One sender can send many messages
                .HasForeignKey(m => m.SenderId) // Foreign Key to ApplicationUser
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete for sender

            // Configure the relationship between ApplicationUser and ChatMessage (Receiver)
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver) // Receiver property in ChatMessage
                .WithMany() // One receiver can receive many messages
                .HasForeignKey(m => m.ReceiverId) // Foreign Key to ApplicationUser
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete for receiver
        }
    }
}
