namespace ChatApplication.Models.DTO.ChatDto
{
    public class ChatMessageDto
    {
        public string senderId { get; set; }
        public string selectedReceiverId { get; set; }
        public string messageTxt { get; set; }
        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }
}
