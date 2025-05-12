using Microsoft.AspNetCore.Mvc;
using ChatApplication.Models;
using ChatApplication.Models.Responses;
using ChatApplication.Services.Interface;
using System.Threading.Tasks;
using ChatApplication.Models._2FA_Models;
using ChatApplication.Models.ResetPassword;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Azure;
using System.Net;
using ChatApplication.Services;
using ChatApplication.Models.DTO;
using ChatApplication.Models.Responses.Common;
using ChatApplication.Models.ChatMessageModel.UserInfo;
using Microsoft.AspNetCore.Identity;
using ChatApplication.Models.UpdateRequestModels;
using ChatApplication.Models.UpdateRequestModels.UpdatedProfileData;
using Microsoft.AspNetCore.Identity.Data;
using ChatApplication.Models.ChatMessageModel;
using ChatApplication.CustomFilters;

namespace ChatApplication.Controllers
{
    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IEncryptedTokenService _tokenEncrypte;

        public AuthController(IEncryptedTokenService tokenDecrypt,IUserServices userServices, IRefreshTokenService refreshTokenService)
        {
            _userServices = userServices;
            _tokenEncrypte = tokenDecrypt;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("VerifyForEditProfile")]
        public async Task<IActionResult> VerifyForEditProfile([FromBody] LoginRequestDto LoginRequest)
        {
            var response = await _userServices.VerifyUserCredentials(LoginRequest);

            return StatusCode((int)response.StatusCode, new ApiResponse<LoginResponseDto>(
                success: response.Success,
        null,
        message: response.Message,
        statusCode: response.StatusCode
            ));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest model)
        {
                var response = await _refreshTokenService.RefreshTokenAsync(model.Token);

                return StatusCode((int)response.StatusCode, new ApiResponse<string>(
                                 success: response.Success,
                                data: response.Data,
                                  message: response.Message,
                                  statusCode: response.StatusCode ));
        }

        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfileRequest updateProfileRequest)
        {
              var updateResponse = await _userServices.UpdateProfileAsync(updateProfileRequest);
            if (updateResponse == null || updateProfileRequest.ProfilePicture.Length == 0)
            {
                return StatusCode((int)ErrorCodes.BadRequest,
                        new ApiResponse<UpdatedProfileDataDTO>(false, message: ResponseMessages.ProfileNotUpdated.GetDescription(), statusCode:ErrorCodes.BadRequest));
            }

            return StatusCode((int)updateResponse.StatusCode,
                         new ApiResponse<UpdatedProfileDataDTO>(updateResponse.Success, updateResponse.Data, updateResponse.Message, updateResponse.StatusCode));
        }

        [HttpGet("GetProfilePicture")]
        public async Task<IActionResult> GetProfilePictureAsync()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Replace("Bearer ", "");
                    var result = await _userServices.GetProfilePictureAsync(token);

                    return StatusCode((int)result.StatusCode,
                        new ApiResponse<ProfilePictureDto>(result.Success, result.Data, result.Message, result.StatusCode));
                }
                return StatusCode((int)HttpStatusCode.Unauthorized,
                   new ApiResponse<string>(false, null, ResponseMessages.AuthorizationHeaderMissing.GetDescription(), ErrorCodes.Unauthorized));
            }
            catch (Exception ex)
            {
                return StatusCode((int)ErrorCodes.InternalServerError,
                    new ApiResponse<string>(false, null, ResponseMessages.InternalServerErrorJWT.GetDescription(), ErrorCodes.InternalServerError));
            }


        }
        [HttpGet("getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userServices.GetAllUsers(); // Await the task

            return StatusCode((int)response.StatusCode, new ApiResponse<IEnumerable<UserListInfo>>(
                success: response.Success,
                data: response.Data,
                message: response.Message,
                statusCode: response.StatusCode
            ));
        }
        // [Authorize]

        [HttpGet("getuserinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Replace("Bearer ", "");
                    var result = await _userServices.GetUserInfoFromRequest(token); 

                    return StatusCode((int)result.StatusCode,
                        new ApiResponse<UserInfoDto>(result.Success, result.Data, result.Message, result.StatusCode));
                }

                return StatusCode((int)HttpStatusCode.Unauthorized,
                    new ApiResponse<string>(false, null, ResponseMessages.AuthorizationHeaderMissing.GetDescription(), ErrorCodes.Unauthorized));
            }
            catch (Exception ex)
            {
                return StatusCode((int)ErrorCodes.InternalServerError,
                    new ApiResponse<string>(false, null, ResponseMessages.InternalServerErrorJWT.GetDescription(), ErrorCodes.InternalServerError));
            }
        }
        
        
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var response = await _userServices.ResetPasswordAsync(model);
            return StatusCode((int)response.StatusCode, new ApiResponse<string>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }

        [HttpPost("resend-otp")]
        public async Task<ActionResult> ResendOtp([FromBody] SendOtpRequest request)
        {
            var response = await _userServices.ResendOtpAsync(request.Email); // Make sure this method exists and returns InterlinkResponse<string>
            return StatusCode((int)response.StatusCode, new ApiResponse<string>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }

        [HttpPost("send-otp")]
        public async Task<ActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            var response = await _userServices.GenerateAndSendOtpAsync(request.Email);
            return StatusCode((int)response.StatusCode, new ApiResponse<string>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var response = await _userServices.VerifyOtpAsync(request.Email, request.Otp);
          
            return StatusCode((int)response.StatusCode, new ApiResponse<string>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            var response = await _userServices.RegisterAsync(registerRequest);
            return StatusCode((int)response.StatusCode, new ApiResponse<string>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var response = await _userServices.LoginAsync(loginRequest);

            return StatusCode((int)response.StatusCode, new ApiResponse<LoginResponseDto>(
                success: response.Success,
        data: response.Data,
        message: response.Message,
        statusCode: response.StatusCode
            ));
        }
        // [Authorize(Roles = "Admin")]
        [RoleAuthorize("Admin")]
        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            var response = await _userServices.RemoveUser(request.email);
            return StatusCode((int)response.StatusCode, new ApiResponse<string>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }
        //[Authorize(Roles = "Admin")]
        [RoleAuthorize("Admin")]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var response = await _userServices.UpdateUser(request);
            return StatusCode((int)response.StatusCode, new ApiResponse<UpdateUserRequest>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }
    }
}
