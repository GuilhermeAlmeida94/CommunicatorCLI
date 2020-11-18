using System.Net.WebSockets;
using CommunicatorCLI.API.Services;
using Microsoft.AspNetCore.Builder;

namespace CommunicatorCLI.API.Builder
{
    public static class BusinessMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketCommunication(this IApplicationBuilder app, BusinessOptions options)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == options.Path)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var deviceService = (WebSocketCommunicationService)app.ApplicationServices
                            .GetService(typeof(WebSocketCommunicationService));
                        await deviceService.ReceiveWebSocket(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });

            return app;
        }
    }
}