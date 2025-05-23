﻿namespace ChatApplication.Models
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string isRole { get; set; }
        public string ProfilePicture { get; set; } // 🟢 new
    }
}
