namespace ChatApplication.Models
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool isOnline { get; set; }
    }
}
