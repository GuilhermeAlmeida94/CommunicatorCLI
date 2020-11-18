using System;
using System.Collections.Generic;

namespace CommunicatorCLI.API.Storage
{
    public interface IStorage
    {
        void SaveLog(List<String> registryNameList, String log);
        void SaveLog(String registryName, String log);
        string Retrieve(String registryName);
    }
}