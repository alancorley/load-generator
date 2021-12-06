using System;

namespace LoadGenerator.Utils
{
    public static class ConsoleUtil
    {
        public static void DisplayMessage(ConsoleColor color, string message, object? arg = null)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message, arg);
            Console.ResetColor();
        }
        
    }
}
