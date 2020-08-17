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
            _fs.RemoveFiles(args);
        }
    }
}