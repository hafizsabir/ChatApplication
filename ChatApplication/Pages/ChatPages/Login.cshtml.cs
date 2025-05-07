using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text;
using ChatApplication.Services.Interface;
using ChatApplication.Models._2FA_Models;
using ChatApplication.Models.Responses;
using ChatApplication.Models;

namespace ChatApplication.Pages.ChatPages
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public LoginRequestDto Input { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginStatus"] = "Invalid input.";
                TempData["LoginIcon"] = "warning";
                TempData["LoginTitle"] = "Validation Error";
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrls:Login"];
            Console.WriteLine($"API URL: {apiUrl}"); // Debugging line

            var requestContent = new StringContent(
                JsonSerializer.Serialize(Input),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                // Make the HTTP POST request
                var response = await client.PostAsync(apiUrl, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Raw Response: " + responseContent); // Log the raw response for debugging

                if (!response.IsSuccessStatusCode)
                {
                    TempData["LoginStatus"] = $"Error: {response.StatusCode}";
                    TempData["LoginIcon"] = "error";
                    TempData["LoginTitle"] = "API Request Failed";
                    return Page();
                }

                // Deserialize the response content
                var result = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result == null || !result.Success)
                {
                    TempData["LoginStatus"] = result?.Message ?? "Login failed.";
                    TempData["LoginIcon"] = "error";
                    TempData["LoginTitle"] = "Login Failed";
                    return Page();
                }

                // Handle 2FA requirement
                if (result.Data.Requires2FA)
                {
                    TempData["LoginStatus"] = "OTP has been sent to your device.";
                    TempData["LoginIcon"] = "success";
                    TempData["LoginTitle"] = "OTP Sent";
                    TempData["RedirectTo"] = "/ChatPages/LoginWith2fa";
                }
                else
                {
                    TempData["jwtToken"] = result.Data.Token;
                    TempData["TwoFactorIsDisabled"] = result.Data.Requires2FA;
                    TempData["LoginStatus"] = "Login successful!";
                    TempData["LoginIcon"] = "success";
                    TempData["LoginTitle"] = "Welcome";
                    TempData["RedirectTo"] = "/ChatPages/MainChat";
                }

                return Page();
            }
            catch (HttpRequestException httpEx)
            {
                // Handle HTTP request specific exceptions
                Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
                TempData["LoginStatus"] = "Failed to connect to the API.";
                TempData["LoginIcon"] = "error";
                TempData["LoginTitle"] = "API Request Error";
                return Page();
            }
            catch (JsonException jsonEx)
            {
                // Handle JSON deserialization errors
                Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
                TempData["LoginStatus"] = "Error processing the server response.";
                TempData["LoginIcon"] = "error";
                TempData["LoginTitle"] = "Response Error";
                return Page();
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                TempData["LoginStatus"] = "An unexpected error occurred.";
                TempData["LoginIcon"] = "error";
                TempData["LoginTitle"] = "Login Error";
                return Page();
            }
        }
    }
}
