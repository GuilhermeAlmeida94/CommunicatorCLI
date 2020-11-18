using System;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CommunicatorCLI.Common.Models;
using CommunicatorCLI.Common.Enums;
using CommunicatorCLI.Common.Extensions;

namespace CommunicatorCLI.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly String _address = "localhost";
        private readonly String _port = "5000";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            String serverUriConnection = $"ws://{_address}:{_port}/ws";

            MessageModel registryMessage = new MessageModel()
            {
                MessageType = MessageTypeEnum.LogOn,
                Registry = RegistryFactory.CreateRegistry()
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var socket = new ClientWebSocket())
                {
                    try
                    {                            
                        await socket.ConnectAsync(new Uri(serverUriConnection), stoppingToken);
                        await Send(socket, JsonSerializer.Serialize(registryMessage), stoppingToken);
                        await ReceiveWebSocket(socket, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }

        private async Task Send(ClientWebSocket socket, string data, CancellationToken stoppingToken) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, stoppingToken);

        private String UserPath()
        {
            int majorCodeWindowsXP = 5;
            string path = Directory
                .GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                .FullName;
            if (Environment.OSVersion.Version.Major > majorCodeWindowsXP)
                path = Directory.GetParent(path).ToString();

            return path;
        }

        private async Task ReceiveWebSocket(ClientWebSocket socket,
            CancellationToken stoppingToken)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                runspace.SessionStateProxy.Path.SetLocation(UserPath());
                while (!stoppingToken.IsCancellationRequested)
                {
                    var messageText = await socket.ReturnText();
                    using (Pipeline pipeline = runspace.CreatePipeline())
                    {
                        string commandResult = GetCommandResult(pipeline, messageText);
                        await SendCommandResult(socket, commandResult);
                    }
                }
                runspace.Close();
            };
        }

        private static string GetCommandResult(Pipeline pipeline, string command)
        {
            pipeline.Commands.AddScript(command);

            var commandResult = string.Empty;
            var powerShellReturnCollection = pipeline.Invoke();
            foreach (var powerShellReturnItem in powerShellReturnCollection)
                commandResult += $"{powerShellReturnItem}\n";
            return commandResult;
        }

        private async Task SendCommandResult(ClientWebSocket socket, String commandResult)
        {
            MessageModel message = new MessageModel()
            {
                MessageType = MessageTypeEnum.CommandResult,
                CommandResult = commandResult
            };

            await Send(socket, JsonSerializer.Serialize(message), CancellationToken.None);
        }
    }
}