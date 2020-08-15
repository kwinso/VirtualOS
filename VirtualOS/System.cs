using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VirtualOS
{
    public class System
    {
        private SystemInfo _info;

        public delegate void SystemEventHandler();

        public event SystemEventHandler Exited;
        public System(SystemInfo info)
        {
            _info = info;
            Start();
        }

        private void Start()
        {
            CommandLine.DefaultLog($"System {_info.SystemName} started!");
            CommandLine.UserPrompt();
        }
    }
}