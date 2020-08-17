using System;

namespace VirtualOS.OperatingSystem
{
    public static class CommandLine
    {
        public static void ColorLog(string message, ConsoleColor color=ConsoleColor.White, bool newLine=true)
        {
            message = message + (newLine ? "\n" : "");
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        public static void DefaultLog(string message, bool newLine=true)
        {
            message = message + (newLine ? "\n" : "");
            Console.ResetColor();
            Console.Write(message);
        }

        public static string UserPrompt(string user, string system)
        {
            ColorLog($"{user}@{system} % ", ConsoleColor.DarkYellow, false);
            string input = Console.ReadLine();
            return input;
        }

        public static string GetInput(string prefix="Type here")
        {
            ColorLog($"{prefix}: ", ConsoleColor.DarkGreen, false);
            string input = Console.ReadLine();
            return input?.Trim();
        }
        
        public static void Error(string message)
        {
            ColorLog(message, ConsoleColor.Red);
        }
        public static void ClearScreen() => Console.Clear();
    }
}