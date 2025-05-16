using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatApplication.TCPServer
{
    public class TcpChatServer : BackgroundService
    {
        private readonly string _ipAddress = "127.0.0.1";
        private readonly int _port = 5000;

        // Optional: Keep track of connected clients
        private readonly List<TcpClient> _clients = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            listener.Start();
            Console.WriteLine($"TCP Chat Server started on port {_port}");

            while (!stoppingToken.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                lock (_clients) _clients.Add(client);

                _ = Task.Run(() => HandleClientAsync(client, stoppingToken));
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken stoppingToken)
        {
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (!stoppingToken.IsCancellationRequested)
            {
                int bytes = await stream.ReadAsync(buffer, 0, buffer.Length, stoppingToken);
                if (bytes == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytes);
                Console.WriteLine($"Received: {message}");

                // Parse and respond (example: forward to other clients)
                if (message.StartsWith("MESSAGE:"))
                {
                    // Broadcast to other clients
                    await BroadcastMessageToAllClients(message, client);
                }
            }

            lock (_clients) _clients.Remove(client);
            client.Close();
        }

        private async Task BroadcastMessageToAllClients(string message, TcpClient senderClient)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    if (client != senderClient && client.Connected)
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length);
                        }
                        catch
                        {
                            // Ignore disconnected client
                        }
                    }
                }
            }
        }
    }
}
