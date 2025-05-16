using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Models.Responses;
using ChatApplication.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ChatApplication.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/TcpChat")]
    public class TcpChatController : ControllerBase
    {
        private readonly ITcpChatService _tcpChatService;
        private readonly IChatService _chatService;
        public TcpChatController(ITcpChatService chatService, IChatService chatService2)
        {
            _tcpChatService = chatService;
            _chatService = chatService2;
        }

       

        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetMessages([FromBody] GetAllUserChatRequest request)
        {
            var response = await _chatService.GetAllMessages( request);
            return StatusCode((int)response.StatusCode, new InterlinkResponse<List<ChatMessage>>(
                response.Success,
                response.Data,
                response.Message,
                response.StatusCode
            ));
        }
    }
}
