using ChatApplication.Models.Responses;
using ChatApplication.Models.Responses.Common;
using ChatApplication.Repository.Interface;
using ChatApplication.Services.Interface;
using System.Security.Claims;

namespace ChatApplication.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IEncryptedTokenService _encryptedTokenService;

        public RefreshTokenService(
            ITokenService tokenService,
            IUserRepository userRepository,
            IEncryptedTokenService encryptedTokenService)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _encryptedTokenService = encryptedTokenService;
        }

        public async Task<InterlinkResponse<string>> RefreshTokenAsync(string token)
        {
            try
            {
                // 1. Decrypt and validate token
                var userClaims = _encryptedTokenService.DecryptJwtMethod (token);

                if (userClaims == null || !userClaims.IsCompleted)
                {
                    return InterlinkResponse<string>.FailResponse(
                        message: ResponseMessages.TokenInvalid.GetDescription(),
                        statusCode: ErrorCodes.Token_Invalid
                    );
                }

                // 2. Extract email from claims
                var email = userClaims.Result.Data?.FindFirst (ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return InterlinkResponse<string>.FailResponse(
                        message: ResponseMessages.EmailNotFoundInToken.GetDescription(),
                        statusCode: ErrorCodes.NotFound
                    );
                }

                // 3. Lookup user by email
                var userResult = await _userRepository.FindByEmail(email);
                if (userResult == null || userResult.Data == null)
                {
                    return InterlinkResponse<string>.FailResponse(
                        message: ResponseMessages.UserNotFound.GetDescription(),
                        statusCode: ErrorCodes.User_NotFound
                    );
                }

                var user = userResult.Data;

                // 4. Generate new encrypted token
                var newToken = _tokenService.GenerateToken(user);
                if (string.IsNullOrEmpty(newToken))
                {
                    return InterlinkResponse<string>.FailResponse(
                        message: ResponseMessages.TokenFailedToRefreshed.GetDescription(),
                        statusCode: ErrorCodes.Token_GenerationFailed
                    );
                }

                // 5. Return success
                return InterlinkResponse<string>.SuccessResponse(
                    newToken,
                    message: ResponseMessages.TokenRefreshedSuccessfully.GetDescription(),
                    statusCode: ErrorCodes.OK
                );
            }
            catch (Exception)
            {
                return InterlinkResponse<string>.FailResponse(
                    message: ResponseMessages.InternalServerError.GetDescription(),
                    statusCode: ErrorCodes.InternalServerError
                );
            }
        }
    }
}
