
namespace ChatApplication.Services.Interface
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task SendAccountLockoutEmailAsync(string email);
        Task SendLoginSuccessEmailAsync(string email);
        Task SendOtpEmailAsync(string toEmail, string otp);
       // Task SendAccountLockedEmail(string toEmail);

    }
}
