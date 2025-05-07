using ChatApplication.Services;
using ChatApplication.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace ChatApplication.Pages.ChatPages
{

    public class MainChatModel : PageModel
    {
        private readonly IEncryptedTokenService _encryptedTokenService;

        public string CurrentUserEmail { get; set; }
        public string? CurrentUserName { get; set; }



        public MainChatModel(IEncryptedTokenService encryptedTokenService)
        {
            _encryptedTokenService = encryptedTokenService;
        }

        public void OnGet()
        {
           

        }
    }
}
