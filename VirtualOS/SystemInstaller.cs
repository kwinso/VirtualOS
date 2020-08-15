using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace VirtualOS
{
    public struct InstallingInfo
    {
        public string InstallingDir;
        public string SystemName;
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
        // Returning path to the config .vos file after installing
        public void Install()
        {
            CommandLine.ColorLog("Installing VirtualOS", ConsoleColor.Green);
            try
            {
                GetSystemInfo();
                CommandLine.ColorLog("Installing VirtualOS System...", ConsoleColor.Green);
                
                CreateSystemDirectory();
                CreateSystemConfig();
                CreateDefaultFolders();
                FillSystemDirectory();
                // return $"{_systemDir.FullName}/SysConf.vos";
            }
            catch (Exception e)
            {
                // Directory.Delete($"{_info.InstallingDir}/{_info.SystemName}");
                CommandLine.ColorLog("Error while installing: " + e);
                throw;
            }
        }
        // collect all data from user before start the installation
        private void GetSystemInfo()
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
        }
        
        private void CreateSystemDirectory()
        {
            var systemDirPath = $"{_info.InstallingDir}/{_info.SystemName}/system/";
            CommandLine.DefaultLog($"Creating {_info.SystemName} directory in {_info.InstallingDir}");
            Directory.CreateDirectory(systemDirPath);
            
            _systemDir = new DirectoryInfo(systemDirPath);
        }

        private void CreateDefaultFolders()
        {
            CommandLine.DefaultLog("Creating Directories...");
            foreach (var dir in _defaultDirectories)
            {
                _systemDir.CreateSubdirectory(dir);
            }
            CommandLine.ColorLog("System Directories Created.", ConsoleColor.Green);
        }

        private void FillSystemDirectory()
        {
            CommandLine.DefaultLog("Creating users directory...");
            string usersFilePath = $"{_systemDir.FullName}/sys/usr/";
            _systemDir.CreateSubdirectory("sys/usr");
            File.Create($"{usersFilePath}/users.info");
            // TODO: Make this file encrypted
            File.Create($"{usersFilePath}/passwd.info");
        }
        private void CreateSystemConfig()
        {
            CommandLine.DefaultLog("Creating .vos config file...");
            var config = new SystemInfo(_info);
            Stream stream = File.Open($"{_info.InstallingDir}/{_info.SystemName}/SysConf.vos", FileMode.Create);
            var bf = new BinaryFormatter();
            bf.Serialize(stream, config);
            stream.Close();
        }
    }
}