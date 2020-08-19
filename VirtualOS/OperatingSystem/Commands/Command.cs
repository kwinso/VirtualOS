using System.Collections.Generic;

namespace VirtualOS.OperatingSystem.Commands
{
    public abstract class Command
    {
        protected List<string> _aliases; // All ways command can be called
        protected string _helpMessage; // Message to show when -h/--h parameter specified.
        
        public virtual bool IsMatch(string command) // To check if any alias is matching to the command called
        {
            return _aliases.Contains(command);
        }

        public abstract void Execute(List<string> args); // All logic of command execution

        protected bool IsHelpRequested(List<string> args)
        {
            return args.Contains("--help") || args.Contains("-h");
        }  
    }
}