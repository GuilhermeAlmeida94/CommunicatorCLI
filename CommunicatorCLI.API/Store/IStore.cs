using System;
using System.Collections.Generic;

namespace CommunicatorCLI.API.Store
{
    public interface IStore
    {
        void Write(List<String> fileNameList, String log);

        void Write(String fileName, String log);
    }
}