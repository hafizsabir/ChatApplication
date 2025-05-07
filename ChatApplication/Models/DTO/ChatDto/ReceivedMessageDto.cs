namespace ChatApplication.Models.DTO.ChatDto
{
    public class ReceivedMessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecieverId { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
}
