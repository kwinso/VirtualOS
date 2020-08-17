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
        private readonly SystemInfo _info;
        private readonly List<Command> _commands;
        
        public CommandProcessor(SystemUser currentUser, SystemInfo info, ref FileSystem fs)
        {
            _currentUser = currentUser;
            _info = info;

            _commands = new List<Command>
            {
                new ListFiles(ref fs), 
                new NewFile(ref fs),
                new NewDir(ref fs),
                new RemoveFile(ref fs),
                new WriteToFile(ref fs),
                new ReadFile(ref fs),
            };
        }
        public CommandProcessorCode ProcessCommands()
        {
            var userCommand = CommandLine.UserPrompt(_currentUser.Name, _info.SystemName);
            var args = GetArguments(userCommand);

            
            // Using switch for very little commands
            switch (args[0])
            {
                case "shutdown":
                    return CommandProcessorCode.ShutdownRequest;
                case "sayhi":
                    CommandLine.ColorLog("Oh, me? Hi-hi!", ConsoleColor.DarkCyan);
                    return CommandProcessorCode.Processed;
                case "reboot":
                    return CommandProcessorCode.RebootRequest;
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