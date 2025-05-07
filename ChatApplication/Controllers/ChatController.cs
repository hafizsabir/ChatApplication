using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Models.Responses;
using ChatApplication.Models.Responses.Common;
using ChatApplication.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChatApplication.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("GetAllMessages")]
        public async Task<IActionResult> GetAllMessages([FromBody] GetAllUserChatRequest GetUsersRequest)
        {
            // request.SenderId and request.ReceiverId will be available here
            var messages = await _chatService.GetUserMessage(GetUsersRequest);
            return StatusCode((int)ErrorCodes.OK
                , new InterlinkResponse<List<ChatMessage>>(messages.Success, messages.Data, messages.Message,messages.StatusCode));
        }


        [HttpPost("sendchat")]

        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                        new ApiResponse<string>(false, null, ResponseMessages.TokenIsMissing.GetDescription(), ErrorCodes.Unauthorized));
                }

                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                        new ApiResponse<string>(false, null, ResponseMessages.TokenIsMissing.GetDescription(), ErrorCodes.Unauthorized));
                }

                var token = authHeader.Replace("Bearer ", "");

                var response = await _chatService.SendMessageAsync(request, token);

                return StatusCode((int)response.StatusCode,
                    new ApiResponse<ChatMessage>(response.Success, response.Data, response.Message, response.StatusCode));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, null, "Server error", ErrorCodes.InternalServerError));
            }
        }


    }
}
