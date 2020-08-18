using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    public class ChangeDir : Command
    {
        private readonly FileSystem _fs;
        private string _location;
        
        public ChangeDir(ref FileSystem fs)
        {
            _aliases = new List<string>(new [] { "cd" });
            _helpMessage = "cd to change current directory";
            _fs = fs;
            _location = CommandProcessor.CurrentLocation;
        }

        public delegate void ChangeHandler(string location);

        public event ChangeHandler Navigated;
        public event ChangeHandler NavigatedToHome;

        public override void Execute(List<string> args)
        {
            if (args.Count == 1)
            {
                NavigatedToHome?.Invoke("");
                return;
            }

            var path = args[1];
            
            if (path.Contains(".") && !path.StartsWith("."))
            {
                CommandLine.Error("You cannot go to files.");
                return;
            }
            
            Navigate(FileSystem.ToAbsolutePath(path, CommandProcessor.CurrentLocation));

        }

        public void Navigate(string path)
        {
            var file = _fs.FindPath(path);
            if (file == null || !file.IsDirectory)
            {
                CommandLine.Error($"No directory found in {path}");
                return;
            }

            _location = path;
            Navigated?.Invoke(_location);
        } 

        
        

        
    }
}