namespace ChatApplication.Models
{
    public class ApplicationUserDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfilePictureBase64 { get; set; } // 🟢 new

    }
}
