using ChatApplication.Models.ChatMessageModel;
using ChatApplication.Models.DTO.ChatDto;
using ChatApplication.Repository.Interface;
using ChatApplication.Services.Interface;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Collections.Concurrent;

public class TcpChatService : ITcpChatService
{
    private readonly ITcpChatRepository _chatRepository;
    private static readonly ConcurrentDictionary<string, WebSocket> _userSockets = new(); // Thread-safe collection for user sockets

    public TcpChatService(ITcpChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task HandleConnectionAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        // Receive the user's ID when they connect (you can extract this from a query parameter or message)
        var userId = await ReceiveUserIdAsync(webSocket);  // This should be replaced with actual logic to extract userId

        Console.WriteLine($"User {userId} connected. WebSocket State: {webSocket.State}");

        // Add user connection if not already registered
        if (!_userSockets.ContainsKey(userId))
        {
            _userSockets[userId] = webSocket; // Store the WebSocket by userId in the dictionary
            Console.WriteLine($"User {userId} WebSocket registered. WebSocket State: {webSocket.State}");
        }

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                // Receiving the message
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    // Deserialize incoming message
                    var chatMessageReq = JsonSerializer.Deserialize<ChatMessageDto>(jsonMessage);

                    if (chatMessageReq != null)
                    {
                        chatMessageReq.timestamp = DateTime.UtcNow;

                        // Save message to the database
                        var chatEntity = new ChatMessage
                        {
                            SenderId = chatMessageReq.senderId,
                            ReceiverId = chatMessageReq.selectedReceiverId,
                            MessageText = chatMessageReq.messageTxt,
                            Timestamp = chatMessageReq.timestamp
                        };

                        await _chatRepository.SaveMessageAsync(chatEntity);

                        // Serialize again (in case Timestamp was updated)
                        string broadcastJson = JsonSerializer.Serialize(chatMessageReq);

                        // Check if the receiver is connected
                        if (_userSockets.TryGetValue(chatMessageReq.selectedReceiverId, out var receiverSocket))
                        {
                            Console.WriteLine($"Receiver found: {chatMessageReq.selectedReceiverId}, WebSocket State: {receiverSocket.State}");
                               
                            if (receiverSocket.State == WebSocketState.Open)
                            {
                                var isReadReq = new ChatMessage
                                {
                                    SenderId = chatMessageReq.senderId,
                                    ReceiverId = chatMessageReq.selectedReceiverId,
                                    MessageText = chatMessageReq.messageTxt,
                                    Timestamp = chatMessageReq.timestamp
                                };
                                await _chatRepository.MessageIsRead(isReadReq);
                                // Send the message to the receiver's WebSocket
                                Console.WriteLine($"Sending message to receiver {chatMessageReq.selectedReceiverId}.");
                                await receiverSocket.SendAsync(
                                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(broadcastJson)),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None
                                );
                            }
                            else
                            {
                                Console.WriteLine($"Receiver {chatMessageReq.selectedReceiverId}'s WebSocket is not open.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Receiver {chatMessageReq.selectedReceiverId} is not connected.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    // Handle message processing error
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during connection handling: {ex.Message}");
        }
        finally
        {
            // Remove WebSocket when the user disconnects
            _userSockets.TryRemove(userId, out _); // Safely remove the user's socket
            Console.WriteLine($"User {userId} disconnected.");
        }
    }


    private async Task<string> ReceiveUserIdAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var userId = Encoding.UTF8.GetString(buffer, 0, result.Count);

        // In a real-world scenario, you should authenticate this userId (e.g., verify JWT or session)
        return userId;  // Return the user ID, this could be retrieved from headers, query string, etc.
    }
}
