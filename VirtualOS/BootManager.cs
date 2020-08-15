using System;
using System.IO;
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
            string systemPath;
            CommandLine.ColorLog("Welcome to the VirtualOS Boot Manager.", ConsoleColor.Cyan);
            Start:
            // TODO: Boot system from Systems Config / VirtualOS Folder
            var selected = SelectBootMode();
            if (selected == BootMode.InstallNew)
            {
                SystemInstaller systemInstaller = new SystemInstaller();
                try
                {
                    systemInstaller.Install();
                    CommandLine.ColorLog("System installed.", ConsoleColor.Green);
                    CommandLine.GetInput("Press enter to reboot");
                    CommandLine.ClearScreen();
                    goto Start;
                }
                catch (Exception e)
                {
                    CommandLine.Error("System not installed.\nRestarting Boot Manager.");
                    goto Start;
                }
            }
            else
            {
                var systemInfo = SelectSystem();
                StartSystem(systemInfo);
            }
        }

        private void StartSystem(SystemInfo systemInfo)
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

        private SystemInfo SelectSystem()
        {
            CommandLine.ColorLog("Select path to\nVirtualOS System folder\nor to\nSystems Config", ConsoleColor.Green);
            BinaryFormatter bf = new BinaryFormatter();
            Stream stream;
            SystemInfo info;
            while (true)
            {
                var systemPath = CommandLine.GetInput("Path");
                try
                {
                    // If directory found, check for .vos files and run with the first one
                    if (Directory.Exists(systemPath))
                    {
                        var configFiles = Directory.GetFiles(systemPath, "*.vos");
                        if (configFiles.Length == 0)
                        {
                            CommandLine.Error($"No .vos files found in {systemPath}");
                            continue;
                        }
                        CommandLine.ColorLog($"Reading {configFiles[0]} config file.", ConsoleColor.Green);
                        stream = File.Open(configFiles[0], FileMode.Open);
                        info = (SystemInfo) bf.Deserialize(stream);
                    } else if (systemPath.EndsWith(".vos")) // If config file specified
                    {
                        stream = File.Open(systemPath, FileMode.Open);
                        info = (SystemInfo) bf.Deserialize(stream);
                    }
                    else
                    {
                        CommandLine.Error("No System Config File found.");
                        continue;
                    }
                }
                catch (Exception e)
                {
                    CommandLine.Error($"Error while reading System Config File");
                    continue;
                }
                
                return info;
            }
        }

        private void SystemExit()
        {
            CommandLine.DefaultLog("System work cancelled. Exiting...");
        }
    }
}