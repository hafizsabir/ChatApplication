using System.ComponentModel.DataAnnotations;

namespace ChatApplication.Models._2FA_Models
{
    public class UserOtp
    {
        [Key]
        public string UserId { get; set; } 
        public string Otp { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsTwoFactorEnabled { get; set; } = false;
    }
}
