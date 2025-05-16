
using ChatApplication.Data;
using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace ChatApplication.Repository.Implementation
{
    public class TcpChatRepository : ITcpChatRepository
    {
        private readonly ApplicationDbContext _context;

        public TcpChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task MessageIsRead(ChatMessage messageId)
        {
            // Find the existing message by ID
            var existingMessage = await _context.ChatMessages.FindAsync(messageId.SenderId);

            if (existingMessage != null)
            {
                existingMessage.IsRead = true; // ✅ Update the IsRead value

                _context.ChatMessages.Update(existingMessage); // Not strictly necessary, EF tracks changes
                await _context.SaveChangesAsync(); // Save to DB
            }
            else
            {
                // Optionally handle if message not found
                Console.WriteLine($"Message with ID {messageId} not found.");
            }
        }

        public async Task SaveMessageAsync(ChatMessage requestmesgData)
        {
            //var dto = JsonSerializer.Deserialize<ChatMessageDto>(jsonMessage);

            var chatMessage = new ChatMessage
            {
                SenderId = requestmesgData.SenderId,
                ReceiverId = requestmesgData.ReceiverId,
                MessageText = requestmesgData.MessageText,
                Timestamp = requestmesgData.Timestamp,
                IsRead = true
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
        }
    }

}
