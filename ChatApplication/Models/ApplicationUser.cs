using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
         public string FullName { get; set; }
        

        public string Password { get; set; }
        public string ProfilePictureBase64 { get; set; } // ✅ New field


    }
}
