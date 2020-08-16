using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using VirtualOS.OperatingSystem.StatusCodes;

namespace VirtualOS.OperatingSystem
{
    public class System
    {
        private ZipArchive _sys;
        private SystemInfo _info;
        private SystemUser _user;
        private string _currLocation = "/";
        private CommandProcessor _commandProcessor = new CommandProcessor();
        
        public System(string systemPath)
        {
            try
            {
                CommandLine.ClearScreen();
                _sys = ZipFile.Open(systemPath, ZipArchiveMode.Update);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CommandLine.Error("Could not run the system.");
            }
        }

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
            var exitCode = StartProcessingCommands();
            ClearSystem();
            if (exitCode == CommandProcessorCode.RebootRequest) return SystemExitCode.Reboot;
            return SystemExitCode.Shutdown;
        }

        private CommandProcessorCode StartProcessingCommands()
        {
            while (true)
            {
                var command = CommandLine.UserPrompt(_user.Name, _info.SystemName, _currLocation);
                var processedCode = _commandProcessor.Command(command);
                if (processedCode != CommandProcessorCode.Processed) return processedCode;
            }
        }

        private void GetSystemInfo()
        {
            var infoFile = _sys.GetEntry("sys/sysinfo.xml");
            using (Stream sr = infoFile.Open())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SystemInfo));
                _info = (SystemInfo) serializer.Deserialize(sr);
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
                    if (!ValidPassword(name, password))
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

        private bool ValidPassword(string username, string userpass)
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