using System.ComponentModel.DataAnnotations;

namespace ChatApplication.Models.UpdateRequestModels
{
    public class UpdateProfileRequest
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public double TimeRemaining { get; set; }
    }
}
