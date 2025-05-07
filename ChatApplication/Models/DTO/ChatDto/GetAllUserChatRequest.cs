namespace ChatApplication.Models.DTO.ChatDto
{
    public class GetAllUserChatRequest
    {
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
    }
}
