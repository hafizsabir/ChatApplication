namespace ChatApplication.Models.DTO
{
    public class UserInfoDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public long ExpirationTime { get; set; }  // Add ExpirationTime property
        public int RemainingTimeInSeconds { get; set; }
        public string otp_verification { get; set; } // Add Otp_verification property
        public string ProfilePicture { get; set; }

    }
}
