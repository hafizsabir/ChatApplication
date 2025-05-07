using Azure;
using ChatApplication.Data;
using ChatApplication.Models;
using ChatApplication.Models._2FA_Models;
using ChatApplication.Models.ChatMessageModel.UserInfo;
using ChatApplication.Models.DTO;
using ChatApplication.Models.ResetPassword;
using ChatApplication.Models.Responses;
using ChatApplication.Models.Responses.Common;
using ChatApplication.Models.UpdateRequestModels;
using ChatApplication.Models.UpdateRequestModels.UpdatedProfileData;
using ChatApplication.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ChatApplication.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<InterlinkResponse<UpdatedProfileDataDTO>> UpdateProfilePictureAsync(string base64Image, UpdateProfileRequest updateProfileRequest)
        {
            try
            {
               
               // var Response = await _userManager.FindByEmailAsync(email);
                var Response = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateProfileRequest.Email);

                if (Response == null)
                {
                    return InterlinkResponse<UpdatedProfileDataDTO>.FailResponse(
                        message: ResponseMessages.UserNotFound.GetDescription(),
                        statusCode: ErrorCodes.NotFound
                    );
                }

                Response.ProfilePictureBase64 = base64Image;
                Response.FullName = updateProfileRequest.FullName;
                Response.UserName = updateProfileRequest.UserName;

                
                  

                var updateResult = await _userManager.UpdateAsync(Response);
               // var nameUpdatee = await _userManager.UpdateAsync();

                if (updateResult.Succeeded)
                {
                    var UpdatedProfileData = new UpdatedProfileDataDTO()
                    {
                        FullName = Response.FullName,
                        Email = Response.Email,
                        UserName = Response.UserName,
                        ProfilePicture = updateProfileRequest.ProfilePicture

                    };
                    return InterlinkResponse<UpdatedProfileDataDTO>.SuccessResponse(
                        UpdatedProfileData,
                      message:  ResponseMessages.ProfileUpdated.GetDescription(),
                       statusCode: ErrorCodes.OK
                    );
                }
                else
                {
                    return InterlinkResponse<UpdatedProfileDataDTO>.FailResponse(
                        message: ResponseMessages.ProfileNotUpdated.GetDescription(),
                        statusCode: ErrorCodes.Operation_Failed
                    );
                }
            }
            catch (Exception ex)
            {
                // Log error if needed (for debugging)
                // You can log 'ex' to your logger for further inspection.

                return InterlinkResponse<UpdatedProfileDataDTO>.FailResponse(
                    message: ResponseMessages.InternalServerError.GetDescription(),
                    statusCode: ErrorCodes.InternalServerError
                );
            }
        }

        public async Task<InterlinkResponse<ProfilePictureDto>> GetProfilePictureAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || string.IsNullOrEmpty(user.ProfilePictureBase64))
            {
                return InterlinkResponse<ProfilePictureDto>.FailResponse(message:ResponseMessages.ProfilePictureNotFound.GetDescription(),statusCode: ErrorCodes.NotFound);

            }
            var userpicture = new ProfilePictureDto
            {
                ProfilePicture = user.ProfilePictureBase64
            };


           return InterlinkResponse<ProfilePictureDto>.SuccessResponse(userpicture, ResponseMessages.ProfilePictureFound.GetDescription(), ErrorCodes.OK);
        }
        public async Task<InterlinkResponse<IList<UserListInfo>>> GetAllUsersAsync()
        {
            try
            {
                // Fetch users from the database
                var users = await _context.Users.ToListAsync();

                // Map User entities to UserListInfo objects
                var userListInfo = users.Select(u => new UserListInfo
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email,
                   ProfilePicture = u.ProfilePictureBase64


                    // Add other properties as needed
                }).ToList();

               
                return InterlinkResponse<IList<UserListInfo>>.SuccessResponse(userListInfo, ResponseMessages.AllUsersRetrieved.GetDescription(), ErrorCodes.OK);
            }
            catch (Exception ex)
            {
                // Return fail response in case of error
                return InterlinkResponse<IList<UserListInfo>>.FailResponse(message: ResponseMessages.NotAnyUserFound.GetDescription(), statusCode: ErrorCodes.NotFound);
            }
        }




        public async Task<InterlinkResponse<string>> GeneratePasswordResetTokenAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return InterlinkResponse<string>.FailResponse(message: ResponseMessages.UserNotFound.GetDescription(),statusCode: ErrorCodes.NotFound);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                return InterlinkResponse<string>.SuccessResponse(token, ResponseMessages.TokenGeneratedSuccessFully.GetDescription(), ErrorCodes.Token_Generated);
            }
            catch (Exception ex)
            {
                // Log error if needed
                return InterlinkResponse<string>.FailResponse(message: ResponseMessages.InternalServerError.GetDescription(), statusCode: ErrorCodes.InternalServerError);
            }
        }

        public async Task<InterlinkResponse<string>> ResetPasswordAsync(ResetPasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return InterlinkResponse<string>.FailResponse(ResponseMessages.UserNotFound.GetDescription(),statusCode: ErrorCodes.NotFound);
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    return InterlinkResponse<string>.FailResponse( ResponseMessages.PasswordMismatch.GetDescription(), statusCode: ErrorCodes.PasswordReset_Mismatch);
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (!result.Succeeded)
                {
                   
                    return InterlinkResponse<string>.FailResponse(ResponseMessages.PasswordFailed_To_Reset.GetDescription(), statusCode: ErrorCodes.PasswordReset_Failed);
                }
                 

                return InterlinkResponse<string>.SuccessResponse("Password Reset Successfully", ResponseMessages.PAssword_Reset_Successfully.GetDescription(), ErrorCodes.PasswordReset_Success);
            }
            catch (Exception ex)
            {
                // Log error if needed
                return InterlinkResponse<string>.FailResponse(ResponseMessages.InternalServerError.GetDescription(), statusCode: ErrorCodes.InternalServerError);
            }
        }
        public async Task<InterlinkResponse<bool>> SaveOtpAsync(string userId, string otp, DateTime expirationTime)
        {
            try
            {
                var existingOtp = await _context.UserOtps.FirstOrDefaultAsync(u => u.UserId == userId);

                if (existingOtp != null)
                {
                    // Update existing OTP
                    existingOtp.Otp = otp;
                    existingOtp.ExpirationTime = expirationTime;
                    existingOtp.CreatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Insert new OTP
                    var otpRecord = new UserOtp
                    {
                        UserId = userId,
                        Otp = otp,
                        ExpirationTime = expirationTime
                    };
                    _context.UserOtps.Add(otpRecord);
                }

                await _context.SaveChangesAsync();
                return InterlinkResponse<bool>.SuccessResponse(true, ResponseMessages.OTPSent.GetDescription(), ErrorCodes.OTP_Sent);
            }
            catch (Exception ex)
            {
                return InterlinkResponse<bool>.FailResponse(false, ResponseMessages.InternalServerError.GetDescription(), ErrorCodes.InternalServerError);
            }
        }

        public async Task<InterlinkResponse<UserOtp>> GetOtpAsync(string userId)
        {
            try
            {
                var otp = await _context.UserOtps
                    .Where(u => u.UserId == userId)
                    .OrderByDescending(u => u.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    return InterlinkResponse<UserOtp>.FailResponse(
                        null,
                        ResponseMessages.OTPFailedToSend.GetDescription(),
                        ErrorCodes.OTP_Failed_ToSend
                    );
                }

                return InterlinkResponse<UserOtp>.SuccessResponse(
                    otp,
                    ResponseMessages.OTPSent.GetDescription(), ErrorCodes.OTP_Sent
                );
            }
            catch (Exception ex)
            {
                return InterlinkResponse<UserOtp>.FailResponse(
                   
                   message: ResponseMessages.OTPFailedToSend.GetDescription(),
                   statusCode: ErrorCodes.OTP_Failed_ToSend
                );
            }
        }
        // get user from database 

        public async Task<InterlinkResponse<ApplicationUser>> FindByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return InterlinkResponse<ApplicationUser>.FailResponse(
                        user,
                        ResponseMessages.UserNotFound.GetDescription(),
                        ErrorCodes.NotFound
                    );
                }
                else
                {
                    return InterlinkResponse<ApplicationUser>.SuccessResponse(
                      user,
                      ResponseMessages.UserFoundSuccessfully.GetDescription(), ErrorCodes.User_Found
                  );
                }
            }
            catch (Exception ex)
            {
                return InterlinkResponse<ApplicationUser>.FailResponse(
                    
                   message: ResponseMessages.InternalServerError.GetDescription(),
                   statusCode: ErrorCodes.InternalServerError
                );
            }
        }

        public async Task<InterlinkResponse<IdentityResult>> CreateUserAsync(RegisterRequestDto user)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(user.Email);
                if (userExists != null)
                {
                    return InterlinkResponse<IdentityResult>.FailResponse(
                        null,
                        ResponseMessages.UserAlreadyExists.GetDescription(),
                        ErrorCodes.Registration_UserAlreadyExists
                    );
                }
                var NewUSer = new ApplicationUser
                {
                    FullName = user.FullName,
                    UserName = user.UserName, // required by Identity
                    Email = user.Email,
                    Password = user.Password,
                    ProfilePictureBase64 = user.ProfilePicture
                };

                var result = await _userManager.CreateAsync(NewUSer, NewUSer.Password);

                if (result.Succeeded)
                {
                    return InterlinkResponse<IdentityResult>.SuccessResponse(
                        result,
                        ResponseMessages.UserCreatedSuccessfully.GetDescription(), ErrorCodes.Created
                    );
                }

                return InterlinkResponse<IdentityResult>.FailResponse(
                    result,
                    ResponseMessages.InternalServerError.GetDescription(),
                    ErrorCodes.BadRequest
                );
            }
            catch (Exception ex)
            {
                return InterlinkResponse<IdentityResult>.FailResponse(
                    
                   message: ResponseMessages.InternalServerError.GetDescription(),
                    statusCode: ErrorCodes.InternalServerError
                );
            }
        }
        public async Task<InterlinkResponse<ApplicationUser>> LoginAsync(LoginRequestDto logininfo)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(logininfo.Email);
                if (user == null)
                {
                    return InterlinkResponse<ApplicationUser>.FailResponse(
                        message: ResponseMessages.UserNotFound.GetDescription(),
                       statusCode: ErrorCodes.NotFound
                    );
                }

                // Check for account lockout
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return InterlinkResponse<ApplicationUser>.FailResponse(null,
                        ResponseMessages.AccountLockedMultipleAttempts.GetDescription(),
                        ErrorCodes.Account_Locked
                    );
                }

                var passwordHasher = new PasswordHasher<ApplicationUser>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, logininfo.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    await _userManager.AccessFailedAsync(user); // Increase failure count

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        return InterlinkResponse<ApplicationUser>.FailResponse(
                           message: ResponseMessages.AccountLockedMultipleAttempts.GetDescription(),
                           statusCode: ErrorCodes.Account_Locked
                        );
                    }

                    return InterlinkResponse<ApplicationUser>.FailResponse(
                        message: ResponseMessages.PasswordIncorrect.GetDescription(),
                       statusCode: ErrorCodes.PasswordReset_Mismatch
                    );
                }

                // Reset failed count if login is successful
                await _userManager.ResetAccessFailedCountAsync(user);
               

                return InterlinkResponse<ApplicationUser>.SuccessResponse(user,
                    ResponseMessages.LoginSuccessfull.GetDescription()
                );
            }
            catch (Exception)
            {
                return InterlinkResponse<ApplicationUser>.FailResponse(
                   message: ResponseMessages.InternalServerError.GetDescription(),
                    statusCode: ErrorCodes.InternalServerError
                );
            }
        }

        


    }
}
