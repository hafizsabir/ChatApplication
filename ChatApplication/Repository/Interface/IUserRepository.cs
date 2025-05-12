using ChatApplication.Models;
using ChatApplication.Models._2FA_Models;
using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.ChatMessageModel.UserInfo;
using ChatApplication.Models.DTO;
using ChatApplication.Models.ResetPassword;
using ChatApplication.Models.Responses;
using ChatApplication.Models.UpdateRequestModels;
using ChatApplication.Models.UpdateRequestModels.UpdatedProfileData;
using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Repository.Interface
{
    public interface IUserRepository
    {
         Task<InterlinkResponse<UpdatedProfileDataDTO>> UpdateProfilePictureAsync(string base64Image,UpdateProfileRequest updateProfileRequest);    
        Task<InterlinkResponse<ProfilePictureDto>> GetProfilePictureAsync(string email);
        Task<InterlinkResponse<IList<UserListInfo>>> GetAllUsersAsync();
        Task<InterlinkResponse<ApplicationUser>> FindByEmail(string email);
        Task<InterlinkResponse<IdentityResult>> CreateUserAsync(RegisterRequestDto user);
        Task<InterlinkResponse<ApplicationUser>> LoginAsync(LoginRequestDto logininfo);
        Task<InterlinkResponse<bool>> SaveOtpAsync(string userId, string otp, DateTime expirationTime);
        Task<InterlinkResponse<UserOtp>> GetOtpAsync(string userId);
        Task<InterlinkResponse<string>> GeneratePasswordResetTokenAsync(string email);
        Task<InterlinkResponse<string>> ResetPasswordAsync(ResetPasswordModel model);
        Task<InterlinkResponse<string>> DeleteUserAsync(string email);
        Task<InterlinkResponse<UpdateUserRequest>> UpdateUserAsnyc(UpdateUserRequest request);


    }
}
