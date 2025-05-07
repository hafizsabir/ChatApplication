using ChatApplication.Models.Responses;

namespace ChatApplication.Services.Interface
{
    public interface IRefreshTokenService
    {
         Task<InterlinkResponse<string>> RefreshTokenAsync(string token);
    }
}
