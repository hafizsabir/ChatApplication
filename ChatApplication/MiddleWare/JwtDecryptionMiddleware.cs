//using Jose;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//namespace ChatApplication.MiddleWare
//{
//    public class JwtDecryptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<JwtDecryptionMiddleware> _logger;
//        private readonly IConfiguration _configuration;

//        public JwtDecryptionMiddleware(RequestDelegate next, ILogger<JwtDecryptionMiddleware> logger, IConfiguration configuration)
//        {
//            _next = next;
//            _logger = logger;
//            _configuration = configuration;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var token = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

//            if (!string.IsNullOrWhiteSpace(token))
//            {
//                try
//                {
//                    var key = Convert.FromBase64String(_configuration["Jwt:Key"]);

//                    // Decrypt token
//                    var payloadJson = JWT.Decode(token, key, JweAlgorithm.DIR, JweEncryption.A256GCM);
//                    var payload = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);

//                    var claims = new List<Claim>
//                {
//                    new Claim(ClaimTypes.NameIdentifier, payload["sub"].ToString()),
//                    new Claim(ClaimTypes.Name, payload["name"].ToString()),
//                    new Claim(ClaimTypes.Email, payload["email"].ToString()),
//                };

//                    var identity = new ClaimsIdentity(claims, "jwt");
//                    context.User = new ClaimsPrincipal(identity);

//                    _logger.LogInformation("Token successfully decrypted and claims set.");
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogWarning("Failed to decrypt JWT token: " + ex.Message);
//                }
//            }

//            await _next(context);
//        }
//    }
//}
