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
            _aliases = new List<string>() { "ls", "dir", "ld" };
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
            
            List<string> info; // Info about each file by lines.
            string path; // Path to list.
            
            // If command called without arguments - list current directory
            if (args.Count > 1) path = args[1];
            else path = CommandProcessor.CurrentLocation;

            if (Path.IsFile(path, CommandProcessor.CurrentLocation))
            {
                CommandLine.Error("You can only list directories.");
                return;
            }
            
            path = Path.ToAbsolutePath(path, CommandProcessor.CurrentLocation);

            info = _fs.ShowFiles(path);

            foreach (var line in info)
                CommandLine.DefaultLog(line);
        }
    }
}