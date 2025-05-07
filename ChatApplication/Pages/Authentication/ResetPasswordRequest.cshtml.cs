using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using ChatApplication.Models;
using ChatApplication.Services.Interface;
using System.Web;

namespace ChatApplication.Pages.Authentication
{
    public class ResetPasswordRequestModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ResetPasswordRequestModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Email))
            {
                StatusMessage = "Please enter a valid email.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByEmailAsync(Email);

            // Do not reveal user existence
            StatusMessage = "If an account exists with this email, a reset link has been sent.";

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);

                var resetLink = $"{Request.Scheme}://{Request.Host}/Authentication/ResetPassword?email={user.Email}&token={encodedToken}";

                Console.WriteLine("🔗 Reset Link: " + resetLink); // for debugging

                await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
            }

            return RedirectToPage();
        }
    }
}
