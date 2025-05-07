using System.ComponentModel.DataAnnotations.Schema;
using ChatApplication.Models;

namespace ChatApplication.Models.ChatMessageModel
{
    public class ChatMessage
    {
        public int Id { get; set; }  //primary key 

        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        public string? MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; }
    }
}
