using CommunicatorCLI.API.Store;
using CommunicatorCLI.API.Helpers;
using CommunicatorCLI.Common.Enums;
using CommunicatorCLI.Common.Models;
using CommunicatorCLI.Common.Extensions;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicatorCLI.API.Services
{
    public class WebSocketCommunicationService
    {
        private ConcurrentDictionary<string, WebSocket>
            _leaders = new ConcurrentDictionary<string, WebSocket>();
        private ConcurrentDictionary<RegistryModel, WebSocket>
            _followers = new ConcurrentDictionary<RegistryModel, WebSocket>();

        public async Task ReceiveWebSocket(WebSocket websocket)
        {
            RegistryModel follower = null;
            try
            {
                while (websocket.State == WebSocketState.Open)
                {
                    string messageText = await websocket.ReturnText();

                    if (!string.IsNullOrEmpty(messageText))
                    {
                        MessageModel message = JsonSerializer.Deserialize<MessageModel>(messageText);
                        if (message.MessageType == MessageTypeEnum.Leader)
                            StoreLeader(websocket);
                        else if (message.MessageType == MessageTypeEnum.LogOn)
                            follower = await SendLogOn(websocket, message.Registry);
                        else if (message.MessageType == MessageTypeEnum.Command)
                            await SendCommand(message.Command);
                        else if (message.MessageType == MessageTypeEnum.CommandResult)
                            await SendCommandResult(websocket, message.CommandResult);
                    }
                }
                await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            catch 
            {
                await SendLogOff(follower);
            }
        }

        public async void StoreLeader(WebSocket websocket)
        {
            _leaders.AddOrUpdate("leader", websocket, (key, oldSocket) => websocket);
            foreach(var follower in _followers)
                await SendLogOn(follower.Key, websocket);
        }

        public async Task<RegistryModel> SendLogOn(WebSocket webSocketLogOn, RegistryModel registry)
        {
            _followers.AddOrUpdate(registry, webSocketLogOn, (key, oldSocket) => webSocketLogOn);
            if (_leaders.Count > 0)
                await SendLogOn(registry, _leaders.First().Value);
            return registry;
        }

        public async Task SendCommand(CommandModel commandInput)
        {
            FileHelper.Write(commandInput.MachineNames, $"[COMMAND]>> {commandInput.Command}\n");
            var followerToSendArray = _followers
                    .Where(x => commandInput.MachineNames.Contains(x.Key.MachineName) &&
                        x.Value.State == WebSocketState.Open)
                    .Select(x => x.Value).ToArray();
            await Send(commandInput.Command, followerToSendArray);
        }

        public async Task SendCommandResult(WebSocket webSocketCommandResult, string commandResult)
        {
            String machineName = _followers.Where(x => x.Value == webSocketCommandResult)
                .FirstOrDefault().Key.MachineName;
            FileHelper.Write(machineName, $"[RESULT]>> {commandResult}");

            MessageModel message = new MessageModel()
            {
                MessageType = MessageTypeEnum.CommandResult,
                CommandResult = $"{machineName} >> \n{commandResult}"
            };

            await Send(JsonSerializer.Serialize(message), _leaders.First().Value);
        }

        public async Task SendLogOff(RegistryModel registry)
        {
            MessageModel message = new MessageModel()
            {
                Registry = registry,
                MessageType = MessageTypeEnum.LogOff
            };

            if (_leaders.Count > 0 )
                await Send(JsonSerializer.Serialize(message), _leaders.First().Value);
        }

        public async Task SendLogOn(RegistryModel registry, WebSocket websocket)
        {
            MessageModel message = new MessageModel()
            {
                MessageType = MessageTypeEnum.LogOn,
                Registry = registry
            };

            await Send(JsonSerializer.Serialize(message), websocket);
        }

        public async Task Send(string message, params WebSocket[] websocketArray)
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