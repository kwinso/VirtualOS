using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    
    public class NewDir : Command
    {

        private readonly FileSystem _fs;
        public NewDir(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "md", "mkdir" });
            _helpMessage = "md/mkdir to create new directory.\nUse new to create files (new --h for help)";
            _fs = fs;
        }

        public override void Execute(List<string> args)
        {
            args.RemoveAt(0);
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }
            _fs.CreateFiles(args, true);
        }
    }
}