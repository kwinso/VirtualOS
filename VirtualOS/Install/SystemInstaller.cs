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
        public string InstallationDirectory; // Path where system will be installed.
        public string SystemName; // Name of a system.
    }
    public class SystemInstaller
    {
        private InstallingInfo _info;
        // This variable represents system file
        private ZipArchive _systemFile;
        /*
         DEFAULT DIRECTORIES FOR SYSTEM.
         sys: Folder for all system files are stored, like config and executables
         home: Folder for users accounts, similar to Linux /home folder
        */
        private readonly string[] _defaultDirectories = new string[] {"sys", "home"};
        
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
                
                _systemFile.Dispose(); // Cancel editing of system file.
            }
            catch (Exception e)
            {
                // Delete system file if installing was not successful.
                var systemFile = $"{_info.InstallationDirectory}/${_info.SystemName}.vos";
                
                if (File.Exists(systemFile)) File.Delete(systemFile);
                
                CommandLine.ColorLog("Error while installing: " + e);
                throw;
            }
        }
        #region Define System Information
        /*
            This section is used for defining all data needed to install the system.
        */
        private void DefineSystemInfo()
        {
            DefineSystemInstallationDirectory();
            DefineSystemName();
        }
        private void DefineSystemInstallationDirectory()
        {
            while (true)
            {
                var installationDirectory = CommandLine.GetInput("VirtualOS installation directory");
                if (!Directory.Exists(installationDirectory))
                {
                    CommandLine.Error($"Directory \"{installationDirectory}\" not found.");
                    continue;
                }
                _info.InstallationDirectory = installationDirectory;
                break;
            }
        }
        private void DefineSystemName()
        {
            var acceptableSymbols = new Regex(@"^[a-zA-Z]+$");
            while (true)
            {
                var systemName = CommandLine.GetInput("System name");
                
                if (File.Exists($"{_info.InstallationDirectory}/{systemName}.vos"))
                {
                    CommandLine.Error($"System {systemName} already exists in given path.");
                    continue;
                }
                
                if (String.IsNullOrEmpty(systemName) || systemName.Length < 5 || !acceptableSymbols.IsMatch(systemName))
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

        private void CreateSystemFile()
        {
            CommandLine.DefaultLog($"Creating {_info.SystemName}.vos system in {_info.InstallationDirectory}");
            
            var systemFilePath = $"{_info.InstallationDirectory}/{_info.SystemName}.vos";
            
            // Create file where system will be installed.
            FileStream systemFile = File.Open($"{systemFilePath}", FileMode.Create);
            
            // Opening Zip archive in this file to install files.
            _systemFile = new ZipArchive(systemFile, ZipArchiveMode.Update);
        }
        
        private void InstallSystemFiles()
        {
            CommandLine.DefaultLog("Creating Directories...");
            
            foreach (var dir in _defaultDirectories)
                _systemFile.CreateEntry($"{dir}/");
            
            CommandLine.ColorLog("System Directories Created.", ConsoleColor.Green);
            CreateSystemInfoFile();
            CreateUsersDirectory();
        }
        
        private void CreateSystemInfoFile()
        {
            try
            {
                var sysInfo = new SystemInfo(_info.SystemName);
                // sysinfo - file for storing information about system. E.g name of the system.
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
            
            // Directory for storing info about system users. (names, groups, passwords)
            try
            {
                _systemFile.CreateEntry("sys/usr/");
                
                // users.info - information about users and their groups
                _systemFile.CreateEntry($"sys/usr/users.info"); 
                // Users' passwords stored here
                _systemFile.CreateEntry($"sys/usr/passwd.info"); 
            }
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
                
            var usersDir = "sys/usr"; // Path to the users' info folder
            
            var usersFile = _systemFile.GetEntry($"{usersDir}/users.info"); // Users file (name, groups)
            var passwordsFile = _systemFile.GetEntry($"{usersDir}/passwd.info"); // Users' passwords
                
            GetUserInfo(out var userName, out var userPass);
            userPass = Encryptor.GenerateHash(userPass);
                
            // Format of saving user: "name:group1, group2"
            using (StreamWriter writer = new StreamWriter(usersFile.Open()))
                writer.WriteLine($"{userName}:root, {userName}");
                
            // Format of saving user's password: "name: password"
            using (StreamWriter writer = new StreamWriter(passwordsFile.Open()))
                writer.WriteLine($"{userName}:{userPass}");

            // Creating user's home directory.
            _systemFile.CreateEntry($"home/{userName}/");
            CommandLine.ColorLog("User Created.", ConsoleColor.Green);
        }
        
        private void GetUserInfo(out string userName, out string userPass)
        {
            var acceptableNameSymbols = new Regex(@"^[a-z]+$");
            
            while (true)
            {
                userName = CommandLine.GetInput("User Name");

                if (!acceptableNameSymbols.IsMatch(userName))
                {
                    CommandLine.Error("Username can only contain lowercase alphabetic characters.");
                    continue;
                }
                
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