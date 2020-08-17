using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using VirtualOS.OperatingSystem;
using VirtualOS.Encryption;

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
        // This variable represents file when all system is stored
        private ZipArchive _systemFile;
        /*
         DIRECTORIES IN THE SYSTEM ROOT
         sys: Folder for all system files, like config and executables
         home: Folder for users accounts, similar to Linux /home folder
        */
        private readonly string[] _rootDirectories = new string[] {"sys", "home"};
        
        public void Install()
        {
            CommandLine.ColorLog("Installing VirtualOS", ConsoleColor.Green);
            try
            {
                DefineSystemInfo();
                
                CommandLine.ColorLog("Installing VirtualOS System...", ConsoleColor.Green);
                
                CreateSystemFile();
                InstallSystemFiles();
                
                ConfigureSystem();
                
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
        #region Define System Information
        /*
            This section is used for defining all data needed while installing the system
        */
        private void DefineSystemInfo()
        {
            DefineSystemInstallDirectory();
            DefineSystemName();
        }
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
            var acceptableSymbols = new Regex(@"^[a-zA-Z]+$");
            while (true)
            {
                
                var systemName = CommandLine.GetInput("System name");
                if (File.Exists($"{_info.InstallDir}/{systemName}.vos"))
                {
                    CommandLine.Error($"System {systemName} already exists in given path.");
                    continue;
                }
                
                if (String.IsNullOrEmpty(systemName) || systemName.Length < 5 || !acceptableSymbols.IsMatch(systemName) )
                {
                    CommandLine.Error("System name shouldn't be empty or less than 5 symbols and contain only letters.");
                    continue;   
                }

                _info.SystemName = systemName;
                break;
            }
        }
        #endregion

        #region System Installaton
        /*
            All Operations needed to install the system
        */
        
        // Creates and Defines system file to install system in 
        private void CreateSystemFile()
        {
            CommandLine.DefaultLog($"Creating {_info.SystemName}.vos system in {_info.InstallDir}");
            
            var systemFilePath = $"{_info.InstallDir}/{_info.SystemName}.vos";
            
            FileStream systemFile = File.Open($"{systemFilePath}", FileMode.Create);
            _systemFile = new ZipArchive(systemFile, ZipArchiveMode.Update);
        }
        
        // Create All Basic Files and Directories for system
        private void InstallSystemFiles()
        {
            CommandLine.DefaultLog("Creating Directories...");
            
            foreach (var dir in _rootDirectories)
                _systemFile.CreateEntry($"{dir}/");
            
            CommandLine.ColorLog("System Directories Created.", ConsoleColor.Green);
            CreateSystemInfoFile();
            CreateUsersDirectory();
        }
        
        // sysinfo is needed to define information about the system, such as System Name, etc.
        // It uses SystemInfo class as the template to store the data
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
        
        // Users directory is a directory where all users' accounts data stored
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
        
        #region System Configuration Operations

        /*
         This Section is used to configure system after installing.
         For example, create very first user on system.
        */
        private void ConfigureSystem()
        {
            CommandLine.ColorLog("System already installed, few more things.", ConsoleColor.Green);
            
            CreateUser();
            
            CommandLine.ColorLog("System successfully configured.", ConsoleColor.Green);
        }
        
        private void CreateUser()
        {
                
            var usersDir = "sys/usr";
            var usersFile = _systemFile.CreateEntry($"{usersDir}/users.info");
            var passwordsFile = _systemFile.CreateEntry($"{usersDir}/passwd.info"); // TODO: Make this file encrypted
                
            GetRootUserInfo(out var userName, out var userPass);
            userPass = Encryptor.GenerateHash(userPass);
                
            using (StreamWriter writer = new StreamWriter(usersFile.Open()))
                writer.WriteLine($"{userName}:root, {userName}");
                
            using (StreamWriter writer = new StreamWriter(passwordsFile.Open()))
                writer.WriteLine($"{userName}:{userPass}");
                
            CommandLine.ColorLog("User Created.", ConsoleColor.Green);
        }
        
        private void GetRootUserInfo(out string userName, out string userPass)
        {
            CommandLine.ColorLog("Creating account.", ConsoleColor.Magenta);
            
            userName = CommandLine.GetInput("User Name");
            while (true)
            {
                userPass = CommandLine.GetInput("User Password");
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