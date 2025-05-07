using ChatApplication.Models.Responses;
using System.Security.Claims;

namespace ChatApplication.Services.Interface
{
    public interface IEncryptedTokenService
    {
       Task<InterlinkResponse<ClaimsPrincipal>>? DecryptJwtMethod(string encryptedToken);
       // Task<InterlinkResponse<ClaimsPrincipal>>? TrustedDeviceTokenDecryption(string encryptedToken);
    }
}
