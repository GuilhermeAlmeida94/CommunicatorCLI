using System;
using System.Collections.Generic;

namespace CommunicatorCLI.Common.Models
{
    public class RegistryModel
    {
        public String MachineName { get; set; }
        public String IP { get; set; }
        public String Antivirus { get; set; }
        public String WindowsVersion { get; set; }
        public String DotNetVersion { get; set; }
        public List<DiskModel> Disk { get; set; }
        public List<String> FirewallStatus { get; set; }
    }
}