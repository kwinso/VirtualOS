using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    
    public class ListFiles : Command
    {

        private readonly FileSystem _fs;
        public ListFiles(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "ls", "dir", "ld" });
            _helpMessage = "ld/dir/ls to show all files in system";
            _fs = fs;
        }

        public override void Execute(List<string> args)
        {
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }
            CommandLine.ColorLog("Files in system: ", ConsoleColor.DarkBlue);
            foreach (var file in _fs.Files)
            {
                CommandLine.DefaultLog(file);
            }
        }
    }
}