using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    
    public class ListFiles : Command
    {

        private readonly FileSystem _fs;
        private readonly string _location;
        public ListFiles(ref FileSystem fs, ref string location)
        {
            _aliases = new List<string>(new [] { "ls", "dir", "ld" });
            _helpMessage = "ld/dir/ls to show all files in system";
            _fs = fs;
            _location = location;
        }

        public override void Execute(List<string> args)
        {
            if (IsHelpRequested(args))
            {
                CommandLine.DefaultLog(_helpMessage);
                return;
            }
            
            List<string> info;
            
            // IF parameters were provided
            if (args.Count > 1 && !String.IsNullOrEmpty(args[1].Trim()))
            {
                var path = args[1];
                
                // For shortcuts like "." and "./"
                if (path.StartsWith("./") || path == ".") path = _location;
                
                if (path.Contains("."))
                {
                    CommandLine.Error("You can only list directories.");
                    return;
                }
                
                // For relative paths
                if (!path.StartsWith("/")) path = _location + path;
                
                // For paths that do not end with slash, like "/home"
                if (!path.EndsWith("/")) path += "/";
                
                info = _fs.ShowFiles(path);
            }
            else info = _fs.ShowFiles(_location);
                
            if (info.Count == 0)
                CommandLine.DefaultLog("No files found.");
            else
            {
                foreach (var line in info)
                {
                    CommandLine.DefaultLog(line);
                }
            }
        }
    }
}