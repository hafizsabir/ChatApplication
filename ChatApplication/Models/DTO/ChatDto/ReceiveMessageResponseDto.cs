namespace ChatApplication.Models.DTO.ChatDto
{
    public class ReceiveMessageResponseDto
    {
        public string? MessageId { get; set; }
        public string? SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderProfilePicture { get; set; }

        public string? ReceiverId { get; set; }
        public string MessageText { get; set; }
        public string? ContentType { get; set; } = "text"; // e.g., text, image, video
        public string? FileUrl { get; set; }               // if any media or attachment
        public DateTime? Timestamp { get; set; }

        public bool? IsRead { get; set; }

        public bool? IsReply { get; set; }
        public string? ReplyToMessageId { get; set; }
        public string? ReplyToMessageText { get; set; }
    }
}
