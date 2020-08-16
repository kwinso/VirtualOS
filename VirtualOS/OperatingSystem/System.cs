using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using VirtualOS.OperatingSystem.StatusCodes;

namespace VirtualOS.OperatingSystem
{
    public class System
    {
        private readonly ZipArchive _sys;
        private SystemInfo _info;
        private SystemUser _user;
        private string _currLocation = "/";
        private CommandProcessor _commandProcessor = new CommandProcessor();
        
        public System(string systemPath)
        {
            try
            {
                // On create, system will load system file into memory
                CommandLine.ClearScreen();
                _sys = ZipFile.Open(systemPath, ZipArchiveMode.Update);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CommandLine.Error("Could not run the system.");
            }
        }

        // This method is used for clear all data about system in memory before shutting the system down
        private void ClearSystem()
        {
            _sys.Dispose();
            _commandProcessor = null;
        }
        public SystemExitCode Start()
        {
            CommandLine.DefaultLog("Welcome to the system.");
            GetSystemInfo();
            LoginUser();
            
            // When processing commands is done, system will receive exit code of Command Processor
            var exitCode = StartProcessingCommands();
            
            ClearSystem();
            // System will ask the boot manager to reboot if there was request from Command Processor
            if (exitCode == CommandProcessorCode.RebootRequest) return SystemExitCode.Reboot;
            return SystemExitCode.Shutdown;
        }
        
        // Read the user input and give the command to the Command Processor
        private CommandProcessorCode StartProcessingCommands()
        {
            while (true)
            {
                var command = CommandLine.UserPrompt(_user.Name, _info.SystemName, _currLocation);
                var processedCode = _commandProcessor.Command(command);
                // If there's no default exit code, stop the processor with given exit code
                if (processedCode != CommandProcessorCode.Processed) return processedCode;
            }
        }

        // Load system info from sysinfo file
        private void GetSystemInfo()
        {
            var infoFile = _sys.GetEntry("sys/sysinfo.xml");
            using (Stream sr = infoFile.Open())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SystemInfo));
                _info = (SystemInfo) serializer.Deserialize(sr);
            }
        }
        
        // Ask the user for the login and password, if user is found, set user to the _user variable
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

                    _user = new SystemUser(name, $"/home/{name}");
                    AddUserGroups();
                    break;
                }
            }
            catch
            {
                CommandLine.Error("Error while logging into the system");
                throw;
            }
            
        }

        // Try to find the user in users config file
        private bool UserExists(string name)
        {
            var usersFile = _sys.GetEntry("sys/usr/users.info");
            using (StreamReader reader = new StreamReader(usersFile.Open()))
            {
                var users = reader.ReadToEnd().Split("\n");
                foreach (var user in users)
                {
                    var userName = user.Split(":")[0];
                    if (userName == name)
                        return true;
                }
                return false;
            }
        }

        // Try to find valid password for user in passwords file
        private bool ValidatePassword(string username, string userpass)
        {
            var passwordsFile = _sys.GetEntry("sys/usr/passwd.info");
            using (StreamReader reader = new StreamReader(passwordsFile.Open()))
            {
                var passwords = reader.ReadToEnd().Split("\n");
                foreach (var password in passwords)
                {
                    // First value represents username, the second one is password
                    var passwordLine = password.Split(":");
                    if (passwordLine[0] == username)
                        return userpass == passwordLine[1];
                }
            }
            return false;
        }

        // Load user's groups to the _user variable
        private void AddUserGroups()
        {
            var usersFile = _sys.GetEntry("sys/usr/users.info");
            using (StreamReader reader = new StreamReader(usersFile.Open()))
            {
                // Split file by lines and loop through each user 
                var users = reader.ReadToEnd().Split("\n");
                foreach (var user in users)
                {
                    var userName = user.Split(":")[0];
                    if (userName == _user.Name)
                    {
                        var userGroups = user.Split(":")[1].Split(", ");
                        foreach (var group in userGroups)
                            _user.AddInGroup(group);
                    }
                }
            }
        }
    }
}