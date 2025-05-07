using ChatApplication.Models;
using ChatApplication.Repository.Interface;
using ChatApplication.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using ChatApplication.Models.ResetPassword;
using ChatApplication.Models.Responses.Common;
using ChatApplication.Models.Responses;
using ChatApplication.Models._2FA_Models;
using ChatApplication.Models.DTO;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.WebRequestMethods;
using System.Net;
using ChatApplication.Models.ChatMessageModel.UserInfo;
using ChatApplication.Models.UpdateRequestModels;
using ChatApplication.Models.UpdateRequestModels.UpdatedProfileData;


namespace ChatApplication.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService; // Inject your email service
        private readonly ITokenService _tokenService;
        private readonly OTPSetting _otpSetting;
        private readonly IEncryptedTokenService _encryptedTokenService;

        public UserServices(IOptions<OTPSetting> OtpSetting,IUserRepository userRepository, IEmailService emailService, ITokenService tokenService, IEncryptedTokenService encryptedTokenService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _tokenService = tokenService;
            _otpSetting = OtpSetting.Value;
            _encryptedTokenService = encryptedTokenService;
        }


        public async Task<InterlinkResponse<UpdatedProfileDataDTO>> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest)
        {
             try
            {
                       
                var base64Image = ConvertFileToBase64(updateProfileRequest.ProfilePicture);
                var userResponse = await _userRepository.UpdateProfilePictureAsync(base64Image,updateProfileRequest);
                if (!userResponse.Success)
                {
                    return InterlinkResponse<UpdatedProfileDataDTO>.FailResponse(
                        message: userResponse.Message,
                        statusCode: userResponse.StatusCode
                    );
                }
                 var updateduser= userResponse.Data;
                ApplicationUser applicationUser = new ApplicationUser() { 
                
                
                UserName = updateduser.UserName,
                Email = updateduser.Email,
                FullName=updateduser.FullName,
                
                
                };
                 
                var NewToken =  _tokenService.GenerateUpdatedInfoToken(applicationUser,updateProfileRequest.TimeRemaining);
                  if(string.IsNullOrEmpty(NewToken))
                {
                    return InterlinkResponse<UpdatedProfileDataDTO>.FailResponse(
              message: ResponseMessages.TokenFailedToGenerate.GetDescription(),
              statusCode: ErrorCodes.Token_GenerationFailed
          );
                }
                UpdatedProfileDataDTO updatedInfoWithhToken = new UpdatedProfileDataDTO()
                {
                    RegeneratedToken = NewToken,
                };



                return InterlinkResponse<UpdatedProfileDataDTO>.SuccessResponse(
                    updatedInfoWithhToken,
                    ResponseMessages.SuccessfullyUpdatedProfile.GetDescription(),
                    userResponse.StatusCode
                );
            }
             catch
            {
                return InterlinkResponse<UpdatedProfileDataDTO>.FailResponse(
                  message: ResponseMessages.InternalServerError.GetDescription(),
                  statusCode: ErrorCodes.InternalServerError);
            }  
        }
        private string ConvertFileToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public async Task<InterlinkResponse<ProfilePictureDto>> GetProfilePictureAsync(string token)
        {
            var result = await _encryptedTokenService.DecryptJwtMethod(token);

            if (!result.Success || result.Data == null)
            {
                return InterlinkResponse<ProfilePictureDto>.FailResponse(

                    message: result.Message,
                    statusCode: result.StatusCode
                );
            }

            var principal = result.Data;
            var expClaim = principal.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expClaim != null)
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expClaim.Value));
                if (expTime < DateTimeOffset.UtcNow)
                {
                    return InterlinkResponse<ProfilePictureDto>.FailResponse(

                      message: ResponseMessages.TokenExpired.GetDescription(),
                        statusCode: ErrorCodes.Unauthorized
                    );
                }

               // var name = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Unknown";
                var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown";
               // var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
              
                 var ProfilePictureResponse = await _userRepository.GetProfilePictureAsync(email);
              //  var userpic = userProfilePicture.Data;
               

                return InterlinkResponse<ProfilePictureDto>.SuccessResponse(
                     ProfilePictureResponse.Data,
                    ProfilePictureResponse.Message, ProfilePictureResponse.StatusCode
                );
            }

            return InterlinkResponse<ProfilePictureDto>.FailResponse(

                message: ResponseMessages.TokenExpirationCalimMissing.GetDescription(),
                statusCode: ErrorCodes.BadRequest
            );
         
        }
        public async Task<InterlinkResponse<IList<UserListInfo>>> GetAllUsers()
        {
            try
            {
               
                var users = await _userRepository.GetAllUsersAsync();

                if (users == null)
                {
                   
                    return InterlinkResponse<IList<UserListInfo>>.FailResponse(
                        message: ResponseMessages.UserNotFound.GetDescription(),
                        statusCode: ErrorCodes.NotFound
                    );
                }

            
                return InterlinkResponse<IList<UserListInfo>>.SuccessResponse(users.Data,
                     
                    users.Message,
                    users.StatusCode
                );
            }
            catch (Exception ex)
            {
                
                return InterlinkResponse<IList<UserListInfo>>.FailResponse(
                    message: ex.Message,
                    statusCode: ErrorCodes.InternalServerError 
                );
            }
        }





        public async Task<InterlinkResponse<UserInfoDto>> GetUserInfoFromRequest(string token)
        {
            var result = await _encryptedTokenService.DecryptJwtMethod(token);

            if (!result.Success || result.Data == null)
            {
                return InterlinkResponse<UserInfoDto>.FailResponse(
                   
                    message: result.Message,
                    statusCode: result.StatusCode
                );
            }

            var principal = result.Data;
            var expClaim = principal.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expClaim != null)
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expClaim.Value));
                if (expTime < DateTimeOffset.UtcNow)
                {
                    return InterlinkResponse<UserInfoDto>.FailResponse(
                       
                      message:  ResponseMessages.TokenExpired.GetDescription(),
                        statusCode: ErrorCodes.Unauthorized
                    );
                }

                var name = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Unknown";
                var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown";
                var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

                // var profilePicture = principal.Claims.FirstOrDefault(c => c.Type == "profilePicture")?.Value ?? "default-avatar.png";


                var remainingTime = expTime - DateTimeOffset.UtcNow;
               // var profileImage = _userRepository.GetProfilePictureAsync(userId);
                var userInfo = new UserInfoDto
                {
                    Name = name,
                    Email = email,
                    ExpirationTime = expTime.ToUnixTimeSeconds(),
                    RemainingTimeInSeconds = (int)remainingTime.TotalSeconds,
                   // ProfilePicture= profileImage
                };

                return InterlinkResponse<UserInfoDto>.SuccessResponse(
                     userInfo,
                    ResponseMessages.UserInforRetrieved.GetDescription(),ErrorCodes.OK
                );
            }

            return InterlinkResponse<UserInfoDto>.FailResponse(
                
                message: ResponseMessages.TokenExpirationCalimMissing.GetDescription(),
                statusCode: ErrorCodes.BadRequest
            );
        }

        public async Task<InterlinkResponse<ApplicationUser>> FindByEmail(string email)
        {
            var user = await _userRepository.FindByEmail(email);
            if (user == null)
            {
                return InterlinkResponse<ApplicationUser>.FailResponse(
                     
                     message: ResponseMessages.UserNotFound.GetDescription(),
                     statusCode: ErrorCodes.NotFound
                 );
            }
            return InterlinkResponse<ApplicationUser>.SuccessResponse(
                      user.Data, // Use the actual user data
                      ResponseMessages.UserFoundSuccessfully.GetDescription(),
                      ErrorCodes.User_Found
            );
        }

        public async Task<InterlinkResponse<string>> GenerateAndSendOtpAsync(string email)
        {
            var userResult = await _userRepository.FindByEmail(email);

            if (!userResult.Success || userResult.Data == null)
            {
                return InterlinkResponse<string>.FailResponse( ResponseMessages.UserNotFound.GetDescription(), statusCode: ErrorCodes.NotFound);
            }

            var user = userResult.Data;

            var otp = GenerateOtp();  // Generate OTP
            var expirationTime = DateTime.UtcNow.AddMinutes(3);  // OTP valid for 3 minutes app setting 

            // Store OTP and expiration time in the repository
            await _userRepository.SaveOtpAsync(user.Id, otp, expirationTime);

            // Send OTP via email (using your email service)
            await _emailService.SendOtpEmailAsync(user.Email, otp);

            return InterlinkResponse<string>.SuccessResponse("OTP sent successfully", ResponseMessages.OTPSent.GetDescription(), ErrorCodes.OTP_Sent);
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<InterlinkResponse<string>> VerifyOtpAsync(string email, string otp)
        {
            var userResult = await _userRepository.FindByEmail(email);

            if (!userResult.Success || userResult.Data == null)
            {
                return InterlinkResponse<string>.FailResponse( ResponseMessages.UserNotFound.GetDescription(), statusCode: ErrorCodes.NotFound);
            }

            var user = userResult.Data;

            var otpResult = await _userRepository.GetOtpAsync(user.Id);
            if (!otpResult.Success || otpResult.Data == null || otpResult.Data.ExpirationTime < DateTime.UtcNow)
            {
                return InterlinkResponse<string>.FailResponse(ResponseMessages.OTPExpired.GetDescription(), statusCode: ErrorCodes.OTP_Expired);
            }

            if (otpResult.Data.Otp != otp)
            {
                return InterlinkResponse<string>.FailResponse(ResponseMessages.InValidOTP.GetDescription(), statusCode: ErrorCodes.OTP_Invalid);
            }


            // Send login success email
          
            var token = _tokenService.GenerateToken(user);
            
            
              
            
            
            await _emailService.SendLoginSuccessEmailAsync(email);

            return InterlinkResponse<string>.SuccessResponse(token, ResponseMessages.OTPVerifiedAndTokenGenerated.GetDescription(), ErrorCodes.OTP_Verified);
        }

        // verifyUser 
        public async Task<InterlinkResponse<LoginRequestDto>> VerifyUserCredentials(LoginRequestDto logininfo)
        {
            var repoResponse = await _userRepository.LoginAsync(logininfo);
            if (!repoResponse.Success || repoResponse.Data == null)
            {
                return InterlinkResponse<LoginRequestDto>.FailResponse(
                    message: ResponseMessages.UserNotFound.GetDescription(),
                    statusCode: ErrorCodes.NotFound
                );
            }
         
            return InterlinkResponse<LoginRequestDto>.SuccessResponse(
              null,
               ResponseMessages.UserVerifiedSuccessFully.GetDescription(),
               ErrorCodes.OK
            );
        }
        // Login Method
        public async Task<InterlinkResponse<LoginResponseDto>> LoginAsync(LoginRequestDto logininfo)
        {
            var repoResponse = await _userRepository.LoginAsync(logininfo);

            if (!repoResponse.Success)
            {
                return InterlinkResponse<LoginResponseDto>.FailResponse(
                    
                   message: repoResponse.Message,
                    statusCode: repoResponse.StatusCode
                );
            }

            try
            {
               
                if(!repoResponse.Data.TwoFactorEnabled)
                {
                    // Generate token and return
                    var token = _tokenService.GenerateToken(repoResponse.Data);

                    var loginResp = new LoginResponseDto
                    {
                        Requires2FA = false,
                        Token = token
                    };
                    return InterlinkResponse<LoginResponseDto>.SuccessResponse(
                        loginResp,
                        ResponseMessages.LoginSuccessfull.GetDescription(),
                        ErrorCodes.OK
                    );
                }
                // Send OTP for 2FA
                var otpResponse = await GenerateAndSendOtpAsync(logininfo.Email);

                if (!otpResponse.Success)
                {
                    return InterlinkResponse<LoginResponseDto>.FailResponse(
                        
                        message: ResponseMessages.OTPFailedToSend.GetDescription(),
                        statusCode: ErrorCodes.OTP_Failed_ToSend
                    );
                }

                // Return 2FA flag so frontend can redirect
                var loginResponse = new LoginResponseDto
                {
                    Requires2FA = true,
                    Token = null // Or null intentionally till OTP verified
                };

                return InterlinkResponse<LoginResponseDto>.SuccessResponse(
                    loginResponse,
                    ResponseMessages.OTPSent.GetDescription(),
                    ErrorCodes.OK
                );
            }
            catch (Exception)
            {
                return InterlinkResponse<LoginResponseDto>.FailResponse(
                    null,
                    ResponseMessages.InternalServerError.GetDescription(),
                    ErrorCodes.InternalServerError
                );
            }
        }


        // Register Method
        public async Task<InterlinkResponse<string>> RegisterAsync(RegisterRequestDto user)
        {
            var createResponse = await _userRepository.CreateUserAsync(user);

            if (!createResponse.Success)
            {
                return InterlinkResponse<string>.FailResponse(
                  
                   message: ResponseMessages.UserAlreadyExists.GetDescription(),
                   statusCode: ErrorCodes.Registration_UserAlreadyExists
                );
            }

            return InterlinkResponse<string>.SuccessResponse(
                "UserIdOrInfoIfNeeded", // Replace with actual user ID or other relevant info
                ResponseMessages.SuccessfullyRegistered.GetDescription(),
                ErrorCodes.Created
            );
        }

        public async Task<InterlinkResponse<string>> ResendOtpAsync(string email)
        {
            return await GenerateAndSendOtpAsync(email);
        }

        // Password Reset Methods
        public async Task<InterlinkResponse<string>> RequestPasswordResetAsync(string email)
        {
            var userResult = await _userRepository.FindByEmail(email);

            if (!userResult.Success || userResult.Data == null)
            {
                return InterlinkResponse<string>.FailResponse(message: ResponseMessages.UserNotFound.GetDescription(), statusCode: ErrorCodes.NotFound);
            }

            await _userRepository.GeneratePasswordResetTokenAsync(email);

            return InterlinkResponse<string>.SuccessResponse("Reset link sent to your email.", ResponseMessages.ResetLinkSentSuccessfully.GetDescription(), ErrorCodes.ResetLinkSent);
        }

        public async Task<InterlinkResponse<string>> ResetPasswordAsync(ResetPasswordModel model)
        {
            var result = await _userRepository.ResetPasswordAsync(model);
            if (!result.Success)
                return InterlinkResponse<string>.FailResponse(message: ResponseMessages.UserNotFound.GetDescription(), statusCode: ErrorCodes.NotFound);

            return InterlinkResponse<string>.SuccessResponse(null, ResponseMessages.SuccessfullyUpdatedProfile.GetDescription(), ErrorCodes.PasswordReset_Success);
        }
    }
}
