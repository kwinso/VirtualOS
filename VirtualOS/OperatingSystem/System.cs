using System;
using System.IO;
using System.Xml.Serialization;
using VirtualOS.Encryption;
using VirtualOS.OperatingSystem.Files;
using VirtualOS.OperatingSystem.StatusCodes;

namespace VirtualOS.OperatingSystem
{
    public class System
    {
        private static FileSystem _fileSystem;
        private SystemInfo _info;
        private SystemUser _user; // Current System user.
        private static CommandProcessor _commandProcessor;
        
        public System(string systemPath)
        {
            try
            {
                //  System will instantiate file system to operate with system files.
                _fileSystem = new FileSystem(systemPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CommandLine.Error("Could not run the system.");
            }
        }

        // This method is used for clear all data about system in memory before shutting the system down
        public static void ExitSystem()
        {
            _fileSystem.Close();
            _commandProcessor = null; // Stop the command processor.
        }
        public SystemExitCode Start()
        {
            CommandLine.ClearScreen();
            CommandLine.DefaultLog("Welcome to the system.");
            
            try
            {
                GetSystemInfo();
                LoginUser();
            }
            catch (Exception e)
            {
                CommandLine.Error("An error occured while starting the system;");
                return SystemExitCode.SystemBroken;
            }
            
            // After info about system will received, start the command processor
            _commandProcessor = new CommandProcessor(_user, _info, ref _fileSystem);

            var exitCode = StartCommandProcessor();
            
            ExitSystem();
            // System will ask the boot manager to reboot if there was request from Command Processor
            if (exitCode == CommandProcessorCode.RebootRequest) return SystemExitCode.Reboot;
            return SystemExitCode.Shutdown;
        }

        private CommandProcessorCode StartCommandProcessor()
        {
            while (true)
            {
                var processedCode = _commandProcessor.ProcessCommands();
                // If there's no default exit code, stop the processor with given exit code
                if (processedCode != CommandProcessorCode.Processed) return processedCode;
            }
        }

        // Load system info from sysinfo file
        private void GetSystemInfo()
        {
            try
            {
                var infoFile = _fileSystem.GetFile("sys/sysinfo.xml");
                using (Stream sr = infoFile.Open())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SystemInfo));
                    _info = (SystemInfo) serializer.Deserialize(sr);
                }
            }
            catch (NullReferenceException e)
            {
                CommandLine.Error("System's broken: No system information file.");
                throw;
            }
        }
        
        private void LoginUser()
        {
            try
            {
                while (true)
                {
                    var name = CommandLine.GetInput("System User");
                    
                    if (!UserExists(name))
                    {
                        CommandLine.Error("No user found with name " + name);
                        continue;
                    }
                    var password = CommandLine.GetInput($"{name}'s password");
                    if (!ValidatePassword(name, password))
                    {
                        CommandLine.Error("Invalid password for user: " + name);
                        continue;
                    }
                    // Find user's home directory. If not found - start at the root of the system 
                    var userHome = _fileSystem.FindPath($"/home/{name}/") != null ? $"/home/{name}/" : "/";
                    _user = new SystemUser(name, userHome);
                    break;
                }
            }
            catch
            {
                CommandLine.Error("Error while logging into the system");
                throw;
            }
        }
        
        // Find user in users file
        private bool UserExists(string name)
        {
            try
            {
                var usersFile = _fileSystem.GetFile("sys/usr/users.info");
                using (StreamReader reader = new StreamReader(usersFile.Open()))
                {
                    // Split info to the array of users
                    var users = reader.ReadToEnd().Split("\n"); 
                    foreach (var user in users)
                    {
                        var userName = user.Split(":")[0]; // Split user name and users groups
                        if (userName == name)
                            return true;
                    }
                    return false;
                }
            }
            catch (NullReferenceException e)
            {
                CommandLine.Error("System's broken: User files not found in /sys/usr/");
                throw;
            }
        }

        private bool ValidatePassword(string username, string userpass)
        {
            var passwordsFile = _fileSystem.GetFile("sys/usr/passwd.info");
            
            using (StreamReader reader = new StreamReader(passwordsFile.Open()))
            {
                // Get password for each user and find coincidental
                var passwords = reader.ReadToEnd().Split("\n");
                
                foreach (var password in passwords)
                {
                    // First value represents username, the second one is password
                    var passwordInfo = password.Split(":");
                    
                    if (passwordInfo[0] == username)
                        return Encryptor.CompareWithHash(userpass, passwordInfo[1]);
                }
            }
            return false;
        }
    }
}