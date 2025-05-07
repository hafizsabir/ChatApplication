using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Models.Responses;

namespace ChatApplication.Services.Interface
{
    public interface IChatService
    {
        Task<InterlinkResponse<ChatMessage>> SendMessageAsync(SendMessageRequest dto,string token);
        Task<InterlinkResponse<List<ChatMessage>>> GetUserMessage(GetAllUserChatRequest request);
    }
}
