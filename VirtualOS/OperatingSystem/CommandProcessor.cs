using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Commands;
using VirtualOS.OperatingSystem.Files;
using VirtualOS.OperatingSystem.StatusCodes;

namespace VirtualOS.OperatingSystem
{
    public class CommandProcessor
    {

        private SystemUser _currentUser;
        public  static string CurrentLocation;
        private readonly SystemInfo _info;
        private readonly List<Command> _commands;
        
        public CommandProcessor(SystemUser currentUser, SystemInfo info, ref FileSystem fs)
        {
            _currentUser = currentUser;
            CurrentLocation = currentUser.HomeDir;
            _info = info;
            
            /*
                COMMANDS HANDLERS INITIALIZATION 
            */
            
            var listFiles = new ListFiles(ref fs);
            
            var changeDir = new ChangeDirectory(ref fs);
            
            // Update location
            changeDir.Navigated += delegate(string location)
            {
                CurrentLocation = location;
            };
            // Set current location to current user home directory
            changeDir.NavigatedToHome += delegate(string location)
            {
                CurrentLocation = _currentUser.HomeDir;
            };
            _commands = new List<Command>
            {
                changeDir,
                listFiles,
                new CreateFile(ref fs),
                new CreateDirectory(ref fs),
                new RemoveFile(ref fs),
                new Echo(ref fs),
                new ReadFile(ref fs),
            };
        }
        public CommandProcessorCode ProcessCommands()
        {
            var location = CurrentLocation;
            if (_currentUser.HomeDir == CurrentLocation) location = "*home*";
            
            var userCommand = CommandLine.UserPrompt(_currentUser.Name, _info.SystemName, location);
            var args = GetArguments(userCommand.Trim()); // Split command to array of arguments

            
            // Commands that do not require arguments
            switch (args[0])
            {
                case "shutdown":
                    return CommandProcessorCode.ShutdownRequest;
                case "sayhi":
                    CommandLine.ColorLog("Oh, me? Hi-hi!", ConsoleColor.DarkCyan);
                    return CommandProcessorCode.Processed;
                case "reboot":
                    return CommandProcessorCode.RebootRequest;
                case "pwd": // Print Working Directory
                    CommandLine.ColorLog($"You're here: {CurrentLocation}");
                    return CommandProcessorCode.Processed;
                case "cls":
                case "clear":
                    CommandLine.ClearScreen();
                    return CommandProcessorCode.Processed;
            }
            
            // For other commands, check in available commands list
            foreach (var command in _commands)
            {
                if (command.IsMatch(args[0].Trim()))
                {
                    try
                    {
                        command.Execute(args);
                    }
                    catch (Exception e)
                    {
                        CommandLine.Error("Error while executing command:\n" + e.Message);
                        Console.WriteLine(e);
                        CommandLine.DefaultLog("You can send message to the developer about this bug.");
                        CommandLine.ColorLog("pythonisajoke@gmail.com", ConsoleColor.DarkCyan);
                    }
                    return CommandProcessorCode.Processed;
                }
            }
            
            CommandLine.Error($"Command Processor: Unknown command {userCommand}");
            return CommandProcessorCode.Processed;
        }
        
        private List<string> GetArguments(string command)
        {
            return new List<string>(command.Split(" "));
        }
    }

}