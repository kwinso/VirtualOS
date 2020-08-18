using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    public class WriteToFile : Command
    {
        private readonly FileSystem _fs;
        public WriteToFile(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "write" });
            _helpMessage = "write to write to file.\nWrite <your text> > <file name> -> Rewrite the file\nWrite <your text> >> <file name> -> Append text to file";
            _fs = fs;
        }
        
        // TODO: File does not update if specified more than one word in name.
        public override void Execute(List<string> args)
        {
            // remove command name and proceed only with params
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }

            // removing command name
            args.RemoveAt(0);
            
            if (args.Count < 3 || !args.Contains(">>") && !args.Contains(">") )
            {
                CommandLine.Error("Invalid Syntax: write <text> >> <file>.\nSee \"write -h\" for more info.");
                return;
            }

            string text = "";
            string filename = null;
            bool isAppending = false;
            for(int i = 0; i <= args.Count; i++)
            {
                if (args[i] != ">>" && args[i] != ">")
                {
                    text += args[i] + " ";
                    args.RemoveAt(i);                    
                }

                isAppending = args[i].Trim() == ">>";
                filename = args[i + 1];
                break;

            }

            if (String.IsNullOrEmpty(filename))
            {
                CommandLine.Error("No file specified.");
                return;
            }

            var path = FileSystem.ToAbsolutePath(filename.Trim(), CommandProcessor.CurrentLocation);
            path = FileSystem.ToZipFormat(path);

            _fs.WriteFile(text, path, isAppending);
        }
    }
}