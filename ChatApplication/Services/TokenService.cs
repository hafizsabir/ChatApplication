using ChatApplication.Models;
using ChatApplication.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Jose;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ChatApplication.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<string> GenerateUpdatedInfoToken(ApplicationUser user, double timeRemaining)
        {
            try
            {
                var keyString = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];

                if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    _logger.LogError("JWT configuration missing: Key, Issuer, or Audience not found.");
                    throw new InvalidOperationException("Missing JWT configuration.");
                }

                var key = Convert.FromBase64String(keyString); // Decode base64 to 256-bit key
               // var expiryMinutes = _configuration.GetValue<int>("OTP:ExpiryTimeOpt");
                // Create JWT payload
                var payload = new Dictionary<string, object>
                {
                    { "sub", user.Id },
                    { "username", user.UserName },
                    { "email", user.Email },
                    { "fullname", user.FullName },

                  //  {"profilePicture",user.ProfilePictureBase64 },
                    { "iss", issuer },
                    { "aud", audience },


                    { "exp", DateTimeOffset.UtcNow.AddSeconds(timeRemaining).ToUnixTimeSeconds() } // Token expiration (1 day)
                };

                // Encrypt the payload using JOSE (JWE - encryption)
                var token = JWT.Encode(payload, key, JweAlgorithm.DIR, JweEncryption.A256GCM);  // Use JWE encryption

                _logger.LogInformation("Encrypted JWT token generated successfully.");
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating encrypted JWT token: {ex.Message}");
                throw new InvalidOperationException("Error generating encrypted JWT token.", ex);
            }
        }
        public string TrustedDeviceToken(ApplicationUser user)
        {
            try
            {
                var keyString = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];

                if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    _logger.LogError("JWT configuration missing: Key, Issuer, or Audience not found.");
                    throw new InvalidOperationException("Missing JWT configuration.");
                }

                var key = Convert.FromBase64String(keyString); // Decode base64 to 256-bit key
                var expiryMinutes = _configuration.GetValue<int>("OTP:ExpiryTimeOpt");
                // Create JWT payload
                var payload = new Dictionary<string, object>
                {
                    { "sub", user.Id },
                    { "name", user.UserName },
                    { "email", user.Email },
                    { "iss", issuer },
                    { "aud", audience },
                  
                   
                    { "Factor2AuthenticationExpiry", DateTimeOffset.UtcNow.AddMinutes(expiryMinutes).ToUnixTimeSeconds() }, // Token expiration (1 day),
                   
                };

                // Encrypt the payload using JOSE (JWE - encryption)
                var token = JWT.Encode(payload, key, JweAlgorithm.DIR, JweEncryption.A256GCM);  // Use JWE encryption

                _logger.LogInformation("Encrypted JWT token generated successfully.");
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating encrypted JWT token: {ex.Message}");
                throw new InvalidOperationException("Error generating encrypted JWT token.", ex);
            }
        }
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            try
            {
                var keyString = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];

                if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    _logger.LogError("JWT configuration missing: Key, Issuer, or Audience not found.");
                    throw new InvalidOperationException("Missing JWT configuration.");
                }

                var key = Convert.FromBase64String(keyString);
                var expiryMinutes = _configuration.GetValue<int>("OTP:ExpiryTimeOpt");

                var roles = await _userManager.GetRolesAsync(user);

                // If only one role, store it as a string for ASP.NET Role claim matching
                var payload = new Dictionary<string, object>
        {
            { "sub", user.Id },
            { "username", user.UserName },
            { "email", user.Email },
            { "fullname", user.FullName },
            { ClaimTypes.Role, roles.Count == 1 ? roles[0] : roles }, // 👈 for [Authorize(Roles = "Admin")]
            { "iss", issuer },
            { "aud", audience },
            { "exp", DateTimeOffset.UtcNow.AddMinutes(expiryMinutes).ToUnixTimeSeconds() }
        };

                var token = JWT.Encode(payload, key, JweAlgorithm.DIR, JweEncryption.A256GCM);
                _logger.LogInformation("Encrypted JWT token generated successfully.");
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating encrypted JWT token: {ex.Message}");
                throw new InvalidOperationException("Error generating encrypted JWT token.", ex);
            }
        }



    }
}
