using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Models.Responses;

namespace ChatApplication.Repository.Interface
{
    public interface ITcpChatRepository
    {
        Task SaveMessageAsync(ChatMessage Request);
        Task MessageIsRead(ChatMessage isMesgRead);

    }
}
