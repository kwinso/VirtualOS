using System;
using System.Collections.Generic;
using VirtualOS.OperatingSystem.Files;

namespace VirtualOS.OperatingSystem.Commands
{
    public class ChangeDir : Command
    {
        private readonly FileSystem _fs;
        private string _location;
        
        public ChangeDir(ref FileSystem fs, string location)
        {
            _aliases = new List<string>(new [] { "cd" });
            _helpMessage = "cd to change current directory";
            _fs = fs;
            _location = location;
        }

        public delegate void ChangeHandler(string location);

        public event ChangeHandler Executed; 

        public override void Execute(List<string> args)
        {
            // TODO: Directories navigation
            /* 
                1. ./, ./path
                2. ../, ../path
                3. Absolute /path
            */
        }
        
    }
}