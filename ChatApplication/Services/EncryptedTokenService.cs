using ChatApplication.Models.Responses.Common;
using ChatApplication.Models.Responses;
using ChatApplication.Models;
using ChatApplication.Services.Interface;
using Jose;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;

namespace ChatApplication.Services
{
    public class EncryptedTokenService : IEncryptedTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EncryptedTokenService> _logger;

        public EncryptedTokenService(IConfiguration configuration, ILogger<EncryptedTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
      

        public Task<InterlinkResponse<ClaimsPrincipal>>? DecryptJwtMethod(string encryptedToken)
        {
            try
            {
                var key = Convert.FromBase64String(_configuration["Jwt:Key"]);
                var payloadJson = JWT.Decode(encryptedToken, key, JweAlgorithm.DIR, JweEncryption.A256GCM);
                var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadJson);

                if (payload == null)
                {
                    _logger.LogWarning("JWT payload deserialization returned null.");
                    return Task.FromResult(
                      InterlinkResponse<ClaimsPrincipal>.FailResponse(
                          message: ResponseMessages.JWTFailedToDecrypted.GetDescription(),
                          statusCode: ErrorCodes.NotFound
                      )
                  );
                   // return null;
                }

                var claims = new List<Claim>();

                if (payload.TryGetValue("sub", out var sub))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.ToString()));

                if (payload.TryGetValue("name", out var name))
                    claims.Add(new Claim(ClaimTypes.Name, name.ToString()));

                if (payload.TryGetValue("email", out var email))
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
                if (payload.TryGetValue(ClaimTypes.Role, out var rolesElement))
                {
                    if (rolesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var role in rolesElement.EnumerateArray())
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesElement.ToString()));
                    }
                }
                //if (payload.TryGetValue("ProfilePicture", out var profilePicture))
                //{
                //    claims.Add(new Claim("ProfilePicture", profilePicture.ToString()));
                //}

                if (payload.TryGetValue("exp", out var exp))
                {
                    try
                    {
                        // Get as Int64 safely
                        long expValue = exp.GetInt64();
                        claims.Add(new Claim("exp", expValue.ToString()));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to parse 'exp' claim: {ex.Message}");

                        return Task.FromResult(
       InterlinkResponse<ClaimsPrincipal>.FailResponse(
           message: ResponseMessages.FailedtoRetrievedExpiry.GetDescription(),
           statusCode: ErrorCodes.NotFound
       )
   );
                    }
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                _logger.LogInformation("Encrypted JWT token successfully decrypted and claims created.");
                // return new ClaimsPrincipal(identity);
                return Task.FromResult(InterlinkResponse<ClaimsPrincipal>.SuccessResponse(
                   new ClaimsPrincipal(identity),
                 ResponseMessages.jwtDataretrievedSuccessfully.GetDescription(), ErrorCodes.OK
));
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to decrypt JWT token: {ex.Message}");
                return Task.FromResult(
InterlinkResponse<ClaimsPrincipal>.FailResponse(
message: ResponseMessages.InternalServerErrorJWT.GetDescription(),
statusCode: ErrorCodes.InternalServerError
)
);
               // return null;
            }
        }
    }
}
