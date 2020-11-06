using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using CommunicatorCLI.Common.Models;

namespace CommunicatorCLI.Worker
{
    public static class RegistryFactory
    {
        public static RegistryModel CreateRegistry()
        {
            RegistryModel registry = new RegistryModel();
            registry.MachineName = Environment.MachineName;
            registry.WindowsVersion = Environment.OSVersion.VersionString;
            registry.DotNetVersion = Environment.Version.ToString();
            registry.IP = GetIpInformation();
            registry.Disk = GetDiskInformation();
            registry.Antivirus = GetAntivirusInformation();
            registry.FirewallStatus = GetFirewallStatusInformation();
            return registry;
        }

        private static string GetIpInformation()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .Where(address => address.AddressFamily == AddressFamily.InterNetwork)
                .FirstOrDefault().ToString();
        }

        private static List<DiskModel> GetDiskInformation()
        {
            List<DiskModel> listDisk = new List<DiskModel>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    DiskModel disk = new DiskModel();
                    disk.Letter = drive.Name;
                    disk.AvaliableFreeSpace = drive.AvailableFreeSpace;
                    disk.TotalSize = drive.TotalSize;
                    listDisk.Add(disk);
                }
            }
            return listDisk;
        }

        private static string GetAntivirusInformation()
        {
            int majorCodeWindowsXP = 5;
            string wmiNamespace = Environment.OSVersion.Version.Major > majorCodeWindowsXP
                ? "SecurityCenter2" : "SecurityCenter";
            string wmiPath = @"\\" + Environment.MachineName + @"\root\" + wmiNamespace;

            string antivirus = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiPath,
                    "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection antivirusProduct = searcher.Get();
                foreach (ManagementObject product in antivirusProduct)
                    antivirus = product["displayName"].ToString();
            }
            catch
            {
                antivirus = "Não detectado.";
            }
            return antivirus;
        }

        public static List<string> GetFirewallStatusInformation()
        {
            List<string> firewallStatusList = new List<string>();

            using (PowerShell powerShell = PowerShell.Create())
            {
                powerShell.AddScript("powershell { Get-NetFirewallProfile -Profile Domain, Public, Private | Select-Object Name, Enabled }");
                var psObjectList = powerShell.Invoke().ToList();
                foreach (var psObject in psObjectList)
                {
                    string firewallStatus = string.Empty;
                    var firewallArray = psObject.ToString().Split(";");
                    firewallStatus += $"{firewallArray[0].Substring(7)}: ";
                    firewallStatus += (firewallArray[1].Substring(9) == "True}" ? "Habilitado" : "Desabilitado");

                    firewallStatusList.Add(firewallStatus);
                }
            }
            return firewallStatusList;
        }
    }
}