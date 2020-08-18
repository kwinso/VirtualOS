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
            
            List<string> info;
            string path;
            
            if (args.Count > 1) path = args[1];
            else path = CommandProcessor.CurrentLocation;

            if (path.Contains(".") && !path.StartsWith("."))
            {
                CommandLine.Error("You can only list directories.");
                return;
            }
            
            path = FileSystem.ToAbsolutePath(path, CommandProcessor.CurrentLocation);

            info = _fs.ShowFiles(path);

            if (info.Count == 0)
                CommandLine.DefaultLog("No files found.");
            else
            {
                foreach (var line in info)
                {
                    CommandLine.DefaultLog(line);
                }
            }
        }
    }
}