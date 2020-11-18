using System;
using CommunicatorCLI.Common.Enums;

namespace CommunicatorCLI.Common.Models
{
    public class MessageModel
    {
        public MessageTypeEnum MessageType { get; set; }
        public RegistryModel Registry { get; set; }
        public CommandModel Command { get; set; }
        public String CommandResult { get; set; }
    }
}