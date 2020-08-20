using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using VirtualOS.OperatingSystem.Files;
using Path = VirtualOS.OperatingSystem.Files.Path;

namespace VirtualOS.OperatingSystem.Commands
{
    public class Echo : Command
    {
        private readonly FileSystem _fs;
        public Echo(ref FileSystem fs)
        {
            _aliases = new List<string>() { "echo" };
            _helpMessage = "echo to echo text / write to file.\nEcho <text> -> Print text to the console.\nEcho <your text> > <file name> -> Rewrite the file\n Echo <your text> >> <file name> -> Append text to file";
            _fs = fs;
        }
        
        // TODO: Writing text with double quotes.
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

            string text = "";
            
            if (!args.Contains(">>") && !args.Contains(">"))
            {
                foreach (var word in args)
                {
                    text += word;
                }
                CommandLine.DefaultLog(text);
                return;
            }

            string filename = ""; // Name of a file to write to
            bool isAppending = true; // Text will be append to existing text by default
            for(int i = 0; i < args.Count; i++)
            {
                if (args[i] == ">>" || args[i] == ">") // End loop if write modifier is detected
                {
                    isAppending = args[i].Trim() == ">>"; // Set type of writing
                    filename = args[i + 1]; // File name must be next word(path) after the write modifier
                    break;
                }
                text += args[i] + " "; // else, add word to the text
            }

            if (String.IsNullOrEmpty(filename))
            {
                CommandLine.Error("No file specified.");
                return;
            }

            var path = Path.ToAbsolutePath(filename.Trim(), CommandProcessor.CurrentLocation);
            path = Path.ToZipFormat(path);
            
            if (!Path.IsFile(path, CommandProcessor.CurrentLocation))
            {
                CommandLine.Error("You cannot write to directories.");
                return;
            }

            _fs.WriteToFile(text, path, isAppending);
        }
    }
}