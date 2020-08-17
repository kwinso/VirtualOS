using System.Collections.Generic;

namespace VirtualOS.OperatingSystem.Commands
{
    /*
        This class is used to define any system command.
        
        Aliases field is used for define ways command can be called.
        
        Help Message will be shown when user requested to show help for the command.
        
        IsMatch used for check if current command called by one of the aliases.
        
        IsHelpRequested used for check is user requested help for the command
    */
    public abstract class Command
    {
        protected List<string> _aliases;
        protected string _helpMessage;
        
        public virtual bool IsMatch(string command)
        {
            return _aliases.Contains(command);
        }

        public abstract void Execute(List<string> args);

        protected bool IsHelpRequested(List<string> args)
        {
            return args.Contains("--help") || args.Contains("-h");
        }  
    }
}