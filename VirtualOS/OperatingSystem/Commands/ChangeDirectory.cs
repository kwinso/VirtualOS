using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    public class ChangeDirectory : Command
    {
        private readonly FileSystem _fs; 
        
        public ChangeDirectory(ref FileSystem fs)
        {
            _aliases = new List<string>() { "cd" };
            _helpMessage = "cd to change current directory";
            _fs = fs;
        }

        public delegate void NavigateHandler(string location);

        public event NavigateHandler Navigated; // Will be called when user navigated
        public event NavigateHandler NavigatedToHome; 

        public override void Execute(List<string> args)
        {
            // Back to the current user's home
            if (args.Count == 1)
            {
                NavigatedToHome?.Invoke("");
                return;
            }

            // Required path to go
            var path = args[1];
            
            // If path contains "." and it's not relative path, e.g ./here or ../top
            if (Path.IsFile(path, CommandProcessor.CurrentLocation))
            {
                CommandLine.Error("You cannot go to files.");
                return;
            }
            // Convert to the absolute path and navigate.
            Navigate(path);

        }

        private void Navigate(string path)
        {
            path = Path.ToAbsolutePath(path, CommandProcessor.CurrentLocation);
            var file = _fs.FindPath(path);
            if (file == null || !file.IsDirectory)
            {
                CommandLine.Error($"No directory found in {path}");
                return;
            }
            
            Navigated?.Invoke(path);
        } 

        
        

        
    }
}