using System;
using System.Collections.Generic;
using System.IO;

namespace CommunicatorCLI.API.Helpers
{
    public static class FileHelper
    {
        static String pathLog = AppDomain.CurrentDomain.BaseDirectory + @"log\";

        public static void Write(List<String> fileNameList, String log)
        {
            foreach (var fileName in fileNameList)
                Write(fileName, log);
        }
        public static void Write(String fileName, String log)
        {
            Directory.CreateDirectory(pathLog);
            String pathLogFile = pathLog + fileName + ".txt";

            var stw = new StreamWriter(pathLogFile, true);
            stw.Write(log);
            stw.Close();
        }
    }
}