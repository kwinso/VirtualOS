using System;
using System.IO;

namespace VirtualOS
{
    enum BootMode {BootExisting, InstallNew}
    public class BootManager
    {
        public delegate void BootWatcher();

        public BootWatcher BootReload; 
        public void Boot()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            CommandLine.ColorLog("Welcome to the VirtualOS Boot Manager.", ConsoleColor.Cyan);
            Start:
            // TODO: Boot system from Systems Config / VirtualOS Folder
            var selected = SelectBootMode();
            if (selected == BootMode.InstallNew)
            {
                SystemInstaller systemInstaller = new SystemInstaller();
                try
                {
                    systemInstaller.StartInstall();

                }
                catch (Exception e)
                {
                    CommandLine.Error("System not installed.\nRestarting Boot Manager.");
                    goto Start;
                }
            }
            else
            {
                CommandLine.ColorLog("Select path to\nVirtualOS System folder\nor to\nSystems Config", ConsoleColor.Green);
                var systemPath = CommandLine.GetInput("Path");
                CommandLine.DefaultLog($"Searching in ${systemPath}");
            }
        }

        private BootMode SelectBootMode()
        {
            CommandLine.DefaultLog("System boot mode:\n1. Select existing VirtualOS.\n2. Install new VirtualOS");
            while (true)
            {
                string input = CommandLine.GetInput("Mode");
                if (input == "1") return BootMode.BootExisting;
                else if (input == "2") return BootMode.InstallNew;
                else
                {
                    CommandLine.Error("Invalid Option. Select 1 or 2.");
                    continue;
                }
            }
        }
    }
}