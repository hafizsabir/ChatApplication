using ChatApplication.Services.Interface;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITcpChatService tcpChatService)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await tcpChatService.HandleConnectionAsync(webSocket);
        }
        else
        {
            await _next(context);
        }
    }
}
