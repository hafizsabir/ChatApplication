using System.Net.WebSockets;

public static class WebSocketManager
{
    private static readonly Dictionary<string, WebSocket> _connections = new();

    // Add connection
    public static string AddConnection(WebSocket webSocket)
    {
        string connectionId = Guid.NewGuid().ToString();
        _connections[connectionId] = webSocket;
        return connectionId;
    }

    // Remove connection
    public static void RemoveConnection(WebSocket webSocket)
    {
        var connection = _connections.FirstOrDefault(x => x.Value == webSocket);
        if (connection.Key != null)
        {
            _connections.Remove(connection.Key);
        }
    }

    // Get all connections
    public static List<WebSocket> GetConnections()
    {
        return _connections.Values.ToList();
    }

    // Get connection by ID
    public static WebSocket GetConnectionById(string connectionId)
    {
        _connections.TryGetValue(connectionId, out var socket);
        return socket;
    }
}
