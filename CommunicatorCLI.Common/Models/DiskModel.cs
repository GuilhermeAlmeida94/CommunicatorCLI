using System;

namespace CommunicatorCLI.Common.Models
{
    public class DiskModel
    {
        public String Letter { get; set; }
        public Int64 AvaliableFreeSpace { get; set; }
        public Int64 TotalSize { get; set; }
    }
}