namespace ChatApplication.Models._2FA_Models
{
    public class LoginResponseDto
    {
        public bool Requires2FA { get; set; }
        public string Token { get; set; } // optional
    }
}
