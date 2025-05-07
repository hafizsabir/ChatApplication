using System.Text.Json.Serialization;

namespace ChatApplication.Models.DTO.ChatDto
{
    public class SendMessageRequest
    {
        public string ReceiverId { get; set; }
        public string MessageText { get; set; }
        public string SenderId { get; set; }
    }
}
