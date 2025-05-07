using ChatApplication.Models._2FA_Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using ChatApplication.Services.Interface;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration, IOptions<EmailSettings> emailSettings)
    {
        _configuration = configuration;
        var settings = emailSettings.Value;
        _smtpClient = new SmtpClient(settings.SmtpServer, settings.SmtpPort)
        {
            Credentials = new NetworkCredential(settings.SmtpUsername, settings.SmtpPassword),
            EnableSsl = true
        };
        _fromEmail = settings.FromEmail;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        try
        {
            using var message = new MailMessage(_fromEmail, toEmail)
            {
                Subject = "Password Reset Request",
                Body = $"Click the link to reset your password: {resetLink}",
                IsBodyHtml = false
            };

            await _smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Log the error message and stack trace for debugging
            Console.WriteLine($"Error sending email: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw new InvalidOperationException("Failed to send password reset email.", ex);
        }
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            using var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Your OTP Code",
                Body = $"Your OTP is: {otp}",
                IsBodyHtml = false
            };

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, smtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to send OTP email.", ex);
        }
    }

    public async Task SendAccountLockoutEmailAsync(string toEmail)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            using var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Account Locked",
                Body = "Your account has been locked due to multiple failed login attempts. Please try again after 5 minutes.",
                IsBodyHtml = false
            };

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, smtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to send account lockout email.", ex);
        }
    }

    public async Task SendLoginSuccessEmailAsync(string toEmail)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            using var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Login Successful",
                Body = "You have successfully logged into your account. If this wasn't you, please reset your password immediately.",
                IsBodyHtml = false
            };

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, smtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to send login success email.", ex);
        }
    }
}
