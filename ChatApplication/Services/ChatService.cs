using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Models.Responses;
using ChatApplication.Models.Responses.Common;
using ChatApplication.Repository.Interface;
using ChatApplication.Services.Interface;

namespace ChatApplication.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository<ChatMessage> _chatRepository;
        private readonly IEncryptedTokenService _DecryptTokenService ;

        public ChatService(IChatRepository<ChatMessage> chatRepository,IEncryptedTokenService decryptToken)
        {
            _chatRepository = chatRepository;
            _DecryptTokenService = decryptToken;
        }
        public async Task<InterlinkResponse<List<ChatMessage>>> GetUserMessage(GetAllUserChatRequest request)
        {
            if (string.IsNullOrEmpty(request.SenderId) || string.IsNullOrEmpty(request.ReceiverId))
            {
                return InterlinkResponse<List<ChatMessage>>.FailResponse(
                    data: null,
                    ResponseMessages.IdsAreMissing.GetDescription(),
                     ErrorCodes.Forbidden
                );
            }

            var messages = await _chatRepository.GetMessagesAsync(request);
            if (messages.Success)
            {
                return InterlinkResponse<List<ChatMessage>>.SuccessResponse(
                      data: messages.Data,
                      message: ResponseMessages.MessageSentSuccessFully.GetDescription(),
                      statusCode: ErrorCodes.OK
                  );
            }
            return InterlinkResponse<List<ChatMessage>>.FailResponse(
                data: null,
                message: ResponseMessages.MessagesFailedToRetrieved.GetDescription(),
                statusCode: ErrorCodes.NotFound
            );

        }
        public async Task<InterlinkResponse<ChatMessage>> SendMessageAsync(SendMessageRequest dto,string token)
        {
            try
            {
             //  var userclaim= _DecryptTokenService.DecryptJwtMethod(token);

                var message = new ChatMessage
                {
                    SenderId = dto.SenderId,
                    ReceiverId = dto.ReceiverId,
                    MessageText = dto.MessageText,
                    Timestamp = DateTime.UtcNow,
                    IsRead = false
                };

                var result = await _chatRepository.SendMessageAsync(message);

                if (result.Success)
                {
                    return InterlinkResponse<ChatMessage>.SuccessResponse(
                        data: message,
                        message: ResponseMessages.MessageSentSuccessFully.GetDescription(),
                        statusCode: ErrorCodes.OK
                    );
                }

                return InterlinkResponse<ChatMessage>.FailResponse(
                    data: null,
                    message: result.Message,
                    statusCode: result.StatusCode
                );
            }
            catch (Exception ex)
            {
                return InterlinkResponse<ChatMessage>.FailResponse(
                    data: null,
                    message: $"Failed to send message: {ex.Message}",
                    statusCode: ErrorCodes.Operation_Failed
                );
            }
        }
    }
}
