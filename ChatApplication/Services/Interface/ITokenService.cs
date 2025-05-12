using ChatApplication.Models;

namespace ChatApplication.Services.Interface
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
       Task< string> GenerateUpdatedInfoToken(ApplicationUser user,double timeRemaining);


        string TrustedDeviceToken(ApplicationUser user);
        //string GetPrincipalFromEncryptedToken(string token);
    }
}
