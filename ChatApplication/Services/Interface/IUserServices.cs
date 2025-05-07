using ChatApplication.Models;
using ChatApplication.Models._2FA_Models;
using ChatApplication.Models.ChatMessageModel.UserInfo;
using ChatApplication.Models.DTO;
using ChatApplication.Models.ResetPassword;
using ChatApplication.Models.Responses;
using ChatApplication.Models.UpdateRequestModels;
using ChatApplication.Models.UpdateRequestModels.UpdatedProfileData;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ChatApplication.Services.Interface
{
    public interface IUserServices
    {

        //  Task<InterlinkResponse<string>> LoginAsync(string email, string password);
        // Task<InterlinkResponse<string>> RegisterAsync(string FullName, string Email, string Password);


        Task<InterlinkResponse<UpdatedProfileDataDTO>> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest);
        Task<InterlinkResponse<ProfilePictureDto>> GetProfilePictureAsync(string token);
        Task<InterlinkResponse<IList<UserListInfo>>> GetAllUsers();
        Task<InterlinkResponse<UserInfoDto>> GetUserInfoFromRequest(string token);
        Task<InterlinkResponse<ApplicationUser>> FindByEmail(string email);
        Task<InterlinkResponse<string>> RegisterAsync(RegisterRequestDto user);
        Task<InterlinkResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest);
        Task<InterlinkResponse<LoginRequestDto>> VerifyUserCredentials(LoginRequestDto loginRequest);

        Task<InterlinkResponse<string>> GenerateAndSendOtpAsync(string email);
        Task<InterlinkResponse<string>> VerifyOtpAsync(string email, string otp);
        Task<InterlinkResponse<string>> ResendOtpAsync(string email);
        Task<InterlinkResponse<string>> RequestPasswordResetAsync(string email);
        Task<InterlinkResponse<string>> ResetPasswordAsync(ResetPasswordModel model);

    }
}
