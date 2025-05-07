using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.Responses;
using ChatApplication.Repository.Interface;
using ChatApplication.Data;
using ChatApplication.Models.Responses.Common;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatApplication.Models.DTO.ChatDto;

namespace ChatApplication.Repository
{
    public class ChatRepository<T> : IChatRepository<T> where T : ChatMessage
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // Update the return type to be generic List<T> instead of List<ChatMessage>
        public async Task<InterlinkResponse<List<T>>> GetMessagesAsync(GetAllUserChatRequest request)
        {
            try
            {
                var messages = await _context.ChatMessages
                    .Where(m =>
                        (m.SenderId == request.SenderId && m.ReceiverId == request.ReceiverId) ||
                        (m.SenderId == request.ReceiverId && m.ReceiverId == request.SenderId))
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();

                // Convert List<ChatMessage> to List<T> (assuming ChatMessage is T or a derived class)
                var responseMessages = messages.Cast<T>().ToList();

                if (responseMessages == null || responseMessages.Count == 0)
                {
                    return InterlinkResponse<List<T>>.FailResponse(
                        new List<T>(),
                        ResponseMessages.MessagesFailedToRetrieved.GetDescription(),
                        ErrorCodes.NotFound
                    );
                }

                return InterlinkResponse<List<T>>.SuccessResponse(
                    responseMessages,
                    ResponseMessages.AllUsersRetrieved.GetDescription(),
                    ErrorCodes.OK
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMessagesAsync: {ex.Message}");
                return InterlinkResponse<List<T>>.FailResponse(
                    message: ResponseMessages.InternalServerError.GetDescription(),
                    statusCode: ErrorCodes.InternalServerError
                );
            }
        }


        public async Task<InterlinkResponse<T>> SendMessageAsync(T message)
        {
            try
            {
                _context.Set<T>().Add(message);
                await _context.SaveChangesAsync();

                return InterlinkResponse<T>.SuccessResponse(
                    data: message,
                    message: ResponseMessages.MessageSentSuccessFully.GetDescription(), // Adjust this message
                    statusCode: ErrorCodes.OK
                );
            }
            catch (Exception ex)
            {
                return InterlinkResponse<T>.FailResponse(
                    data: default,
                    message: $"Error sending message: {ex.Message}",
                    statusCode: ErrorCodes.Operation_Failed
                );
            }
        }
    }
}
