using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    public class RemoveFile : Command
    {
        private readonly FileSystem _fs;
        public RemoveFile(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "rm", "delete", "del" });
            _helpMessage = "rm/delete/del to delete files.";
            _fs = fs;
        }
        public override void Execute(List<string> args)
        {
            // remove command name and proceed only with names of files
            args.RemoveAt(0);
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }

            // Delete all file in directory.
            var isReversive = false;
            for (var i = 0; i < args.Count; i++)
            {
                if (args[i] == "-r")
                {
                    isReversive = true;
                    args.RemoveAt(i);
                }
                
            }

            var paths = new List<string>(); // Paths to delete.
            
            for (var i = 0; i < args.Count; i++)
            {
                var path = Path.ToAbsolutePath(args[i], CommandProcessor.CurrentLocation);
                if (!Path.IsFile(path, CommandProcessor.CurrentLocation) && !isReversive)
                {
                    CommandLine.Error($"{path} is a directory. Specify the \"-r\" parameter to remove all contents in directory.");
                    return;
                }

                paths.Add(path);
            }

            foreach (var filepath in paths)
                _fs.RemoveFile(filepath, !Path.IsFile(filepath, CommandProcessor.CurrentLocation));
            
            CommandLine.ColorLog("Done", ConsoleColor.DarkGreen);
        }
    }
}