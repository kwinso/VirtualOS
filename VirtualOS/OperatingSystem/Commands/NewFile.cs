using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;
namespace VirtualOS.OperatingSystem.Commands
{
    public class NewFile : Command
    {
        private readonly FileSystem _fs;
        public NewFile(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "touch", "new", "nf" });
            _helpMessage = "touch/new/nf to create new file.\nSpecify all files you want to create.\nTo create directories, use md (md -h for help)";
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
            _fs.CreateFiles(args, false);
        }
    }
}