using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VirtualOS.OperatingSystem.Files;
namespace VirtualOS.OperatingSystem.Commands
{
    public class CreateFile : Command
    {
        private readonly FileSystem _fs;
        public CreateFile(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "touch", "new", "nf" });
            _helpMessage = "touch/new/nf to create new file.\nSpecify all files you want to create.\nTo create directories, use md (md -h for help)";
            _fs = fs;
        }
        public override void Execute(List<string> args)
        {
            // Remove command name and proceed only with names of files
            args.RemoveAt(0);
            
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }

            foreach (var filename in args)
            {
                var acceptableSymbols = new Regex(@"^[a-zA-Z0-9./]+$");

                var path = Path.ToAbsolutePath(filename, CommandProcessor.CurrentLocation);
                if (!Path.IsFile(path, CommandProcessor.CurrentLocation))
                {
                    CommandLine.Error($"You can create only files: {filename}. File's skipped.");
                    continue;
                }

                if (!acceptableSymbols.IsMatch(path))
                {   
                    CommandLine.Error($"Invalid name of file: {filename}.\nFile is skipped..");
                    continue;
                }
                
                _fs.CreateSystemUnit(path);
            }
        }
    }
}