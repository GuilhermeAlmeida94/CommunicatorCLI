using System;
using System.Collections.Generic;
using System.IO;

namespace CommunicatorCLI.API.Storage
{
    public class FileStorage : IStorage
    {
        private String pathLog = AppDomain.CurrentDomain.BaseDirectory + @"log\";

        public void SaveLog(List<String> registryNameList, String log)
        {
            foreach (var fileName in registryNameList)
                SaveLog(fileName, log);
        }

        public void SaveLog(String registryName, String log)
        {
            Directory.CreateDirectory(pathLog);
            String pathLogFile = pathLog + registryName + ".txt";

            var stw = new StreamWriter(pathLogFile, true);
            stw.Write(log);
            stw.Close();
        }

        public string Retrieve(String registryName)
        {
            Directory.CreateDirectory(pathLog);
            String pathLogFile = pathLog + registryName + ".txt";

            var stw = new StreamReader(pathLogFile, true);
            return stw.ReadToEnd();
        }
    }
}