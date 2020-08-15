using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VirtualOS
{
    public struct InstallingInfo
    {
        public string InstallingDir;
        public string SystemName;
        public string UserName;
    }
    
    public class SystemInstaller
    {
        private InstallingInfo _info = new InstallingInfo();
        private DirectoryInfo _systemDir;

        /*
         sys: Folder for all system files, like config and executables
         home: Folder for users accounts, similar to Linux /home folder
        */
        private string[] _defaultDirectories = new string[2] {"sys", "home"};
        public void StartInstall()
        {
            CommandLine.ColorLog("Installing VirtualOS", ConsoleColor.Green);
            try
            {
                // Directory to install system
                while (true)
                {
                    var installDirectory = CommandLine.GetInput("VirtualOS install directory");
                    if (!Directory.Exists(installDirectory))
                    {
                        CommandLine.Error($"Directory \"{installDirectory}\" not found.");
                        continue;
                    }
                    _info.InstallingDir = installDirectory;
                    break;

                }
                
                // Name for the system
                while (true)
                {
                    var systemName = CommandLine.GetInput("System name");
                    if (Directory.Exists($"{_info.InstallingDir}/{systemName}"))
                    {
                        CommandLine.Error($"System {systemName} in directory already exists.");
                        continue;
                    }

                    _info.SystemName = systemName;
                    break;
                }
                
                _info.UserName = CommandLine.GetInput("User name");
                CommandLine.ColorLog("Installing VirtualOS System.");
                CreateSystemDirectory();
                CreateDefaultFolders();
                
            }
            catch (Exception e)
            {
                CommandLine.ColorLog("Error while installing: " + e);
                throw;
            }
            
        }

        private void CreateSystemDirectory()
        {
            var systemDirPath = $"{_info.InstallingDir}/{_info.SystemName}";
            CommandLine.DefaultLog($"Creating {_info.SystemName} directory in {_info.InstallingDir}");
            Directory.CreateDirectory(systemDirPath);
            
            _systemDir = new DirectoryInfo(systemDirPath);
        }

        private void CreateDefaultFolders()
        {
            CommandLine.DefaultLog("Creating Directories.");
            foreach (var dir in _defaultDirectories)
            {
                _systemDir.CreateSubdirectory(dir);
            }
            CommandLine.ColorLog("System Folders Created.", ConsoleColor.Green);
        }

        public void CreateSystemConfig()
        {
            
        }
    }
}