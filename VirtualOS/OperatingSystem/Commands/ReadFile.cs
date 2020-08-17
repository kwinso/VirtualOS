using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    public class ReadFile : Command
    {
        private readonly FileSystem _fs;
        public ReadFile(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "read", "cat" });
            _helpMessage = "read/cat <file> to write text to the file.";
            _fs = fs;
        }
        public override void Execute(List<string> args)
        {
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }
            // Second Params is a name of a file to read
            _fs.ReadFile(args[1]);
        }
    }
}