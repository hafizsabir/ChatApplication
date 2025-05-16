using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Models.Responses;
using System.Net.WebSockets;

namespace ChatApplication.Services.Interface
{
    public interface ITcpChatService
    {
        Task HandleConnectionAsync(WebSocket webSocket);
    }
}
