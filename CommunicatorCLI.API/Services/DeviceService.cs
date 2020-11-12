using CommunicatorCLI.API.Store;
using CommunicatorCLI.API.Helpers;
using CommunicatorCLI.Common.Enums;
using CommunicatorCLI.Common.Models;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicatorCLI.API.Services
{
    public class DeviceService
    {
        private ConcurrentDictionary<string, WebSocket>
            _leaders = new ConcurrentDictionary<string, WebSocket>();
        private ConcurrentDictionary<RegistryModel, WebSocket>
            _followers = new ConcurrentDictionary<RegistryModel, WebSocket>();

        public async Task FilterMessageByWebSocket(WebSocket websocket)
        {
            RegistryModel follower = null;
            try
            {
                while (websocket.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024 * 4];
                    WebSocketReceiveResult socketResult;
                    var package = new List<byte>();
                    do
                    {
                        socketResult = await websocket
                            .ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        package.AddRange(new ArraySegment<byte>(buffer, 0, socketResult.Count));
                    } while (!socketResult.EndOfMessage);
                    var bufferAsString = Encoding.ASCII.GetString(package.ToArray());
                    if (!string.IsNullOrEmpty(bufferAsString))
                    {
                        MessageModel message = JsonSerializer.Deserialize<MessageModel>(bufferAsString);
                        if (message.MessageType == MessageTypeEnum.Leader)
                            storeLeader(websocket);
                        else if (message.MessageType == MessageTypeEnum.LogOn)
                            follower = await sendLogOn(websocket, message.Registry);
                        else if (message.MessageType == MessageTypeEnum.CommandInput)
                            await sendCommandInput(message.CommandInput);
                        else if (message.MessageType == MessageTypeEnum.CommandOutput)
                            await sendCommandOutput(websocket, message.CommandOutput);
                    }
                }
                await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            catch 
            {
                await sendLogOff(follower);
            }
        }

        private async void storeLeader(WebSocket websocket)
        {
            _leaders.AddOrUpdate("leader", websocket, (key, oldSocket) => websocket);
            foreach(var follower in _followers)
                await sendLogOn(follower.Key, websocket);
        }

        private async Task<RegistryModel> sendLogOn(WebSocket webSocketLogOn, RegistryModel registry)
        {
            _followers.AddOrUpdate(registry, webSocketLogOn, (key, oldSocket) => webSocketLogOn);
            if (_leaders.Count > 0)
                await sendLogOn(registry, _leaders.First().Value);
            return registry;
        }

        private async Task sendCommandInput(CommandInputModel commandInput)
        {
            FileHelper.Write(commandInput.MachineNames, $"[INPUT]>> {commandInput.Command}\n");
            var followerToSendArray = _followers
                                            .Where(x => commandInput.MachineNames.Contains(x.Key.MachineName) &&
                                                x.Value.State == WebSocketState.Open)
                                            .Select(x => x.Value).ToArray();
            await send(commandInput.Command, followerToSendArray);
        }

        private async Task sendCommandOutput(WebSocket webSocketCommandOutput, string commandOutput)
        {
            String machineName = _followers.Where(x => x.Value == webSocketCommandOutput)
                .FirstOrDefault().Key.MachineName;
            FileHelper.Write(machineName, $"[OUTPUT]>> {commandOutput}");

            MessageModel message = new MessageModel()
            {
                MessageType = MessageTypeEnum.CommandOutput,
                CommandOutput = $"{machineName} >> \n{commandOutput}"
            };

            await send(JsonSerializer.Serialize(message), _leaders.First().Value);
        }

        private async Task sendLogOff(RegistryModel registry)
        {
            MessageModel message = new MessageModel()
            {
                Registry = registry,
                MessageType = MessageTypeEnum.LogOff
            };

            if (_leaders.Count > 0 )
                await send(JsonSerializer.Serialize(message), _leaders.First().Value);
        }

        private async Task sendLogOn(RegistryModel registry, WebSocket websocket)
        {
            MessageModel message = new MessageModel()
            {
                MessageType = MessageTypeEnum.LogOn,
                Registry = registry
            };

            await send(JsonSerializer.Serialize(message), websocket);
        }

        private async Task send(string message, params WebSocket[] websocketArray)
        {
            foreach (var websocket in websocketArray)
            {
                var stringAsBytes = Encoding.ASCII.GetBytes(message);
                var byteArraySegment = new ArraySegment<byte>(stringAsBytes, 0, stringAsBytes.Length);
                await websocket.SendAsync(byteArraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}