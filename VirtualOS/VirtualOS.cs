using System;
using VirtualOS.Boot;
using VirtualOS.OperatingSystem;

namespace VirtualOS
{
    static class OS
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += SaveSystemState;
            Console.CancelKeyPress += SaveSystemState;
            
            // Run Boot Manager on system start
            BootManager bootManager = new BootManager();
            bootManager.Boot();

        }
        
        
        static void SaveSystemState (object sender, EventArgs e)
        {
            try
            {
                OperatingSystem.System.ExitSystem();
            }
            catch
            {
                // ignored
            }
        }
    }
}
