using System;
using System.Collections.Generic;

namespace CommunicatorCLI.Common.Models
{
    public class CommandInputModel
    {
        public List<String> MachineNames { get; set; }
        public String Command { get; set; }
    }
}