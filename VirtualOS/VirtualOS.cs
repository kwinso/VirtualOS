using System;
using VirtualOS.Boot;

namespace VirtualOS
{
    static class OS
    {
        static void Main(string[] args)
        {
            BootManager bootManager = new BootManager();
            bootManager.Boot();
        }

        private static void RestartOs()
        {
            
        }
    }
}
