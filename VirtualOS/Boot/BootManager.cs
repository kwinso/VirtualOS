using System;
using System.IO;
using VirtualOS.Install;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace VirtualOS
{
    enum BootMode {BootExisting, InstallNew}
    public class BootManager
    {
        private string _systemPath;
        private System _system;                

        public void Boot()
        {
            CommandLine.ColorLog("Welcome to the VirtualOS Boot Manager.", ConsoleColor.Cyan);
            Start:
            var selected = SelectBootMode();
            if (selected == BootMode.InstallNew)
            {
                SystemInstaller systemInstaller = new SystemInstaller();
                try
                {
                    systemInstaller.Install();
                    systemInstaller = null;
                    CommandLine.ColorLog("System installed.", ConsoleColor.Green);
                    CommandLine.GetInput("Press enter to reboot");
                    CommandLine.ClearScreen();
                    goto Start;
                }
                catch
                {
                    CommandLine.Error("System not installed.\nRestarting Boot Manager.");
                    goto Start;
                }
            }
            else
            {
                var systemPath = SelectSystem();
                StartSystem(systemPath);
            }
        }

        private void StartSystem(string systemInfo)
        {
            _system = new System(systemInfo);
            _system.Exited += SystemExit;
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
        private string SelectSystem()
        {
            CommandLine.ColorLog("Select path to\nVirtualOS System folder\nor to\nSystems Config", ConsoleColor.Green);
            while (true)
            {
                var systemPath = CommandLine.GetInput("Path to .vos file");
                try
                {
                    // If directory found, check for .vos files and run with the first one
                    if (!systemPath.EndsWith(".vos"))
                    {
                        CommandLine.Error("Invalid type of system file!");
                    }
                    else if (File.Exists(systemPath))
                    {
                        return systemPath;
                    }
                    else
                    {
                        CommandLine.Error("No System Config File found.");
                        continue;
                    }
                }
                catch
                {
                    CommandLine.Error($"Error while reading System Config File");
                    continue;
                }
            }
        }

        private void SystemExit()
        {
            CommandLine.DefaultLog("System work cancelled. Exiting...");
        }
    }
}