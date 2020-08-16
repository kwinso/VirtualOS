using System;
using VirtualOS.OperatingSystem.StatusCodes;

namespace VirtualOS.OperatingSystem
{
    public class CommandProcessor
    {
        public CommandProcessorCode Command(string command)
        {
            switch (command)
            {
                case "shutdown":
                    return CommandProcessorCode.ShutdownRequest;
                case "sayhi":
                    CommandLine.ColorLog("Oh, me? Hi-hi!", ConsoleColor.DarkCyan);
                    break;
                case "reboot":
                    return CommandProcessorCode.RebootRequest;
                default:
                    CommandLine.Error($"Command Processor: Unknown command {command}");
                    break;
            }
            return CommandProcessorCode.Processed;
        }
    }
}