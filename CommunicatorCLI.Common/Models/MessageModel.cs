using System;
using CommunicatorCLI.Common.Enums;

namespace CommunicatorCLI.Common.Models
{
    public class MessageModel
    {
        public MessageTypeEnum MessageType { get; set; }
        public RegistryModel Registry { get; set; }
        public CommandInputModel CommandInput { get; set; }
        public String CommandOutput { get; set; }
    }
}