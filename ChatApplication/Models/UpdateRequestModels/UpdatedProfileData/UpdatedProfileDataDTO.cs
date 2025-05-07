namespace ChatApplication.Models.UpdateRequestModels.UpdatedProfileData
{
    public class UpdatedProfileDataDTO
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? RegeneratedToken { get; set; }
    }
}
