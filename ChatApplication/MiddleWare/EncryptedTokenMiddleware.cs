using ChatApplication.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.MiddleWare
{
    public class EncryptedTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EncryptedTokenMiddleware> _logger;

        public EncryptedTokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory, ILogger<EncryptedTokenMiddleware> logger)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Create a scope to resolve the scoped service
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var tokenService = scope.ServiceProvider.GetRequiredService<IEncryptedTokenService>();

                    var result = await tokenService.DecryptJwtMethod(token);

                    if (result != null && result.Success && result.Data != null)
                    {
                        context.User = result.Data;
                    }
                    else
                    {
                        context.User = new ClaimsPrincipal(new ClaimsIdentity()); // Clear the user if invalid
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"Error occurred while decrypting JWT token: {ex.Message}");
                context.User = new ClaimsPrincipal(new ClaimsIdentity()); // Clear the user if an exception occurs
            }

            await _next(context);
        }
    }
}
