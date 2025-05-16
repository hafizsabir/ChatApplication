using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace ChatApplication.TCPServver
{
    public class WebSocketServer : BackgroundService
    {
        private readonly string _ipAddress = "127.0.0.1";
        private readonly int _port = 7231;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://{_ipAddress}:{_port}/");
            listener.Start();
            Console.WriteLine($"WebSocket Server started on port {_port}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                var webSocket = await context.AcceptWebSocketAsync(null);
                WebSocketManager.AddConnection(webSocket.WebSocket);

                _ = Task.Run(() => HandleWebSocketCommunication(webSocket.WebSocket, stoppingToken));
            }
        }

        private async Task HandleWebSocketCommunication(WebSocket webSocket, CancellationToken stoppingToken)
        {
            using (var stream = webSocket)
            {
                byte[] buffer = new byte[1024];

                while (!stoppingToken.IsCancellationRequested)
                {
                    var message = await ReceiveMessageAsync(stream, buffer);
                    if (message != null)
                    {
                        Console.WriteLine($"Received: {message}");
                        await BroadcastMessageToAllClients(message);
                    }
                }
            }
        }

        private async Task<string> ReceiveMessageAsync(WebSocket stream, byte[] buffer)
        {
            WebSocketReceiveResult result;
            var message = new StringBuilder();

            do
            {
                result = await stream.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            } while (!result.EndOfMessage);

            return message.ToString();
        }

        private async Task BroadcastMessageToAllClients(string message)
        {
            var connections = WebSocketManager.GetConnections();
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var connection in connections)
            {
                try
                {
                    if (connection.State == WebSocketState.Open)
                    {
                        await connection.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch
                {
                    // Handle disconnection or failed send
                }
            }
        }
    }
}
