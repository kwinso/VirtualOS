using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using VirtualOS.OperatingSystem;

namespace VirtualOS.Install
{
    public struct InstallingInfo
    {
        public string InstallDir;
        public string SystemName;
    }
    public class SystemInstaller
    {
        private InstallingInfo _info;
        private ZipArchive _systemFile;

        /*
         sys: Folder for all system files, like config and executables
         home: Folder for users accounts, similar to Linux /home folder
        */
        private readonly string[] _rootDirectories = new string[] {"sys", "home"};
        
        // Returning path to the config .vos file after installing
        public void Install()
        {
            CommandLine.ColorLog("Installing VirtualOS", ConsoleColor.Green);
            try
            {
                DefineSystemInfo();
                
                CommandLine.ColorLog("Installing VirtualOS System...", ConsoleColor.Green);
                
                CreateSystemFile();
                CreateSystemFolders();
                
                ProcessPostInstallOperations();
                _systemFile.Dispose();
            }
            catch (Exception e)
            {
                var systemFile = $"{_info.InstallDir}/${_info.SystemName}.vos";
                if (File.Exists(systemFile)) File.Delete(systemFile);
                CommandLine.ColorLog("Error while installing: " + e);
                throw;
            }
        }
        
        #region Pre-Installation Operations
        // collect all data from user before start the installation
        private void DefineSystemInfo()
        {
            DefineSystemInstallDirectory();
            DefineSystemName();
        }

        #region Define System Information
        private void DefineSystemInstallDirectory()
        {
            while (true)
            {
                var installDirectory = CommandLine.GetInput("VirtualOS install directory");
                if (!Directory.Exists(installDirectory))
                {
                    CommandLine.Error($"Directory \"{installDirectory}\" not found.");
                    continue;
                }
                _info.InstallDir = installDirectory;
                break;
            }
        }
        private void DefineSystemName()
        {
            while (true)
            {
                
                var systemName = CommandLine.GetInput("System name");
                if (File.Exists($"{_info.InstallDir}/{systemName}.vos"))
                {
                    CommandLine.Error($"System {systemName} already exists in given path.");
                    continue;
                }

                _info.SystemName = systemName;
                break;
            }
        }

        #endregion
        
        
        private void CreateSystemFile()
        {
            CommandLine.DefaultLog($"Creating {_info.SystemName}.vos system in {_info.InstallDir}");
            
            var systemFilePath = $"{_info.InstallDir}/{_info.SystemName}.vos";
            
            FileStream systemVos = File.Open($"{systemFilePath}", FileMode.Create);
            _systemFile = new ZipArchive(systemVos, ZipArchiveMode.Update);
        }
        private void CreateSystemFolders()
        {
            CommandLine.DefaultLog("Creating Directories...");
            
            foreach (var dir in _rootDirectories)
                _systemFile.CreateEntry($"{dir}/");
            
            CommandLine.ColorLog("System Directories Created.", ConsoleColor.Green);
            CreateSystemInfoFile();
            CreateUsersDirectory();
        }
        #endregion

        #region System Installaton
        private void CreateSystemInfoFile()
        {
            try
            {
                var sysInfo = new SystemInfo(_info.SystemName);
                var infoFile = _systemFile.CreateEntry("sys/sysinfo.xml");

                using var sr = new StreamWriter(infoFile.Open());
                var serializer = new XmlSerializer(typeof(SystemInfo));
                serializer.Serialize(sr, sysInfo);
            }
            catch
            {
                CommandLine.Error("Error While creating system info file.");
                throw;
            }
        }
        private void CreateUsersDirectory()
        {
            CommandLine.DefaultLog("Creating users directory...");
            try { _systemFile.CreateEntry("sys/usr/"); }
            catch
            {
                CommandLine.Error("Error while users directory.");
                throw;
            }
        }
        #endregion

        #region Post-install operations

        private void ProcessPostInstallOperations()
        {
            CreateRootUser();
            CommandLine.ColorLog("System successfully installed.", ConsoleColor.Green);
        }
        
        private void CreateRootUser()
        {
            CommandLine.ColorLog("System already installed, few more things.", ConsoleColor.Green);
                
            var usersDir = "sys/usr";
            var usersFile = _systemFile.CreateEntry($"{usersDir}/users.info");
            var passwordsFile = _systemFile.CreateEntry($"{usersDir}/passwd.info"); // TODO: Make this file encrypted
                
            GetRootUserInfo(out var userName, out var userPass);
                
            using (StreamWriter writer = new StreamWriter(usersFile.Open()))
                writer.WriteLine($"{userName}:root, {userName}");
                
            using (StreamWriter writer = new StreamWriter(passwordsFile.Open()))
                writer.WriteLine($"{userName}:{userPass}");
                
            CommandLine.ColorLog("First user Created.", ConsoleColor.Green);
        }
        
        private void GetRootUserInfo(out string userName, out string userPass)
        {
            CommandLine.ColorLog("Creating root account.", ConsoleColor.Magenta);
            userName = CommandLine.GetInput("Root User Name");
            while (true)
            {
                userPass = CommandLine.GetInput("Root User Password");
                var userPassRepeat = CommandLine.GetInput("Repeat Password");
                if (userPass != userPassRepeat)
                {
                    CommandLine.Error("Passwords did not match!");
                    continue;
                }
                break;
            }
        }
        #endregion

    }
}