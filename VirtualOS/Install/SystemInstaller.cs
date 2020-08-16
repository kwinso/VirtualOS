using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace VirtualOS.Install
{
    public struct InstallingInfo
    {
        public string InstallingDir;
        public string SystemName;
    }
    public class SystemInstaller
    {
        private InstallingInfo _info = new InstallingInfo();
        private ZipArchive _systemFile;

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
                
                CreateAndCompressSystemDirectory();
                CreateDefaultFolders();
                FillSystemDirectory();

                _systemFile.Dispose();
            }
            catch (Exception e)
            {
                var systemFile = $"{_info.InstallingDir}/${_info.SystemName}.vos";
                if (File.Exists(systemFile)) File.Delete(systemFile);
                CommandLine.ColorLog("Error while installing: " + e);
                throw;
            }
        }

        #region Pre-Installation Operations
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
                if (File.Exists($"{_info.InstallingDir}/{systemName}.vos"))
                {
                    CommandLine.Error($"System {systemName} already exists in given path.");
                    continue;
                }

                _info.SystemName = systemName;
                break;
            }
        }
        
        private void CreateAndCompressSystemDirectory()
        {
            CommandLine.DefaultLog($"Creating {_info.SystemName}.vos system in {_info.InstallingDir}");
            
            // Create System Directory, then zip it and delete source dir
            var systemDirPath = $"{_info.InstallingDir}";

            Directory.CreateDirectory($"{systemDirPath}/{_info.SystemName}");
            Directory.SetCurrentDirectory($"{systemDirPath}");
            
            // Create system file and then delete directory it was created from
            ZipFile.CreateFromDirectory($"./{_info.SystemName}", $"./{_info.SystemName}.vos");
            Directory.Delete($"./{_info.SystemName}");
            
            // Set system to the created file
            FileStream systemVos = File.Open($"{_info.InstallingDir}/{_info.SystemName}.vos", FileMode.Open);
            _systemFile = new ZipArchive(systemVos, ZipArchiveMode.Update);
        }
        
        // This function creates all Template Folders
        private void CreateDefaultFolders()
        {
            CommandLine.DefaultLog("Creating Directories...");
            foreach (var dir in _defaultDirectories)
            {
                _systemFile.CreateEntry($"{dir}/");
            }
            CommandLine.ColorLog("System Directories Created.", ConsoleColor.Green);
        }
        #endregion

        // Fills sys directory with all needed system files
        private void FillSystemDirectory()
        { 
            CreateSystemInfoDir();
            CreateUsersDir();
        }

        #region System Installaton
        private void CreateSystemInfoDir()
        {
            try
            {
                var sysInfo = new SystemInfo(_info.SystemName);
                
                var infoDirPath = "sys/sysinfo.xml";
                var infoFile = _systemFile.CreateEntry($"{infoDirPath}");
                using (var tw = new StreamWriter(infoFile.Open()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SystemInfo));
                    serializer.Serialize(tw, sysInfo);
                }
            }
            catch (Exception e)
            {
                CommandLine.Error("Error While creating system info file.");
                throw;
            }
        }
        // Create first user and Directory for managing Users
        private void CreateUsersDir()
        {
            CommandLine.DefaultLog("Creating users directory..."); 
            CommandLine.ColorLog("Creating root account.", ConsoleColor.Magenta);
            var userName = CommandLine.GetInput("Root User Name");
            string userPass;
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
            try
            {
                var usersDir = "sys/usr";
                _systemFile.CreateEntry(usersDir + "/");
                
                var usersFile = _systemFile.CreateEntry($"{usersDir}/users.info");
                // TODO: Make this file encrypted
                var passwordsFile = _systemFile.CreateEntry($"{usersDir}/passwd.info");

                // Creating First user with ROOT and own group
                using (StreamWriter writer = new StreamWriter(usersFile.Open()))
                    writer.WriteLine($"{userName}:root, {userName}");
                
                using (StreamWriter writer = new StreamWriter(passwordsFile.Open()))
                    writer.WriteLine($"{userName}:{userPass}");
                
                CommandLine.ColorLog("First user Created.", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                CommandLine.Error("Error while creating root user");
                throw;
            }
        }

        #endregion
    }
}