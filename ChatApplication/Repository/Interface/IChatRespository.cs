using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Models.Responses;

namespace ChatApplication.Repository.Interface
{
    public interface IChatRepository<T> where T : class
    {
        /// <summary>
        /// Sends a chat message asynchronously.
        /// </summary>
        /// <param name="message">The chat message to send.</param>
        /// <returns>An InterlinkResponse containing the result of the operation.</returns> 
    
        Task<InterlinkResponse<T>> SendMessageAsync(T message);
        Task<InterlinkResponse<List<T>>> GetMessagesAsync(GetAllUserChatRequest request);
    }
}