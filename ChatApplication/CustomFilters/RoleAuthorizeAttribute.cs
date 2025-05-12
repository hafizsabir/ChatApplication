using ChatApplication.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ChatApplication.CustomFilters
{
    public class RoleAuthorizeAttribute :Attribute, IAuthorizationFilter
    {
        private readonly string _requiredRole;

        public RoleAuthorizeAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(tokenHeader) || !tokenHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = tokenHeader.Substring("Bearer ".Length).Trim();

            // Resolve service manually (NO middleware)
            var tokenService = context.HttpContext.RequestServices.GetService<IEncryptedTokenService>();

            var result = tokenService?.DecryptJwtMethod(token).GetAwaiter().GetResult();  // Synchronously wait

            if (result == null || !result.Success || result.Data == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var claimsPrincipal = result.Data;

            var roleClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roleClaim == null || roleClaim.Value != _requiredRole)
            {
                context.Result = new ForbidResult();
            }

            // HttpContext.User fill karna optional hai
            context.HttpContext.User = claimsPrincipal;
        }
    }
}
