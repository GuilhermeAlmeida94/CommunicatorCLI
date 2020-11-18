using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicatorCLI.Common.Extensions
{
    public static class WebSocketExtension
    {

        public static async Task<string> ReturnText(this WebSocket webSocket){
            var byteArrayForBuffer = new byte[1024 * 4];
            WebSocketReceiveResult socketResult;
            var package = new List<byte>();
            do
            {
                socketResult = await webSocket
                    .ReceiveAsync(new ArraySegment<byte>(byteArrayForBuffer), CancellationToken.None);
                package.AddRange(new ArraySegment<byte>(byteArrayForBuffer, 0, socketResult.Count));
            } while (!socketResult.EndOfMessage);
            return Encoding.ASCII.GetString(package.ToArray());
        }
        
    }
}