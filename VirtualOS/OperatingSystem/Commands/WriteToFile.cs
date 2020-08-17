using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    public class WriteToFile : Command
    {
        private readonly FileSystem _fs;
        public WriteToFile(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "write" });
            _helpMessage = "write <file> <text> to write text to the file.";
            _fs = fs;
        }
        public override void Execute(List<string> args)
        {
            // remove command name and proceed only with params
            args.RemoveAt(0);
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }
            _fs.WriteFile(args);
        }
    }
}