using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public static class Logs
    {
        private static void LogAs(string status, ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{status}] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }


        /// <summary>
        /// Used for most common logs.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message) => LogAs("LOG", ConsoleColor.Gray, message);

        /// <summary>
        /// Used when it's ok.
        /// </summary>
        /// <param name="message"></param>
        public static void Success(string message) => LogAs("SUCCESS", ConsoleColor.Green, message);

        /// <summary>
        /// Used for logs that might require attention.
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message) => LogAs("WARN", ConsoleColor.Yellow, message);

        /// <summary>
        /// Used when something goes entirely wrong.
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message) => LogAs("ERROR", ConsoleColor.Red, message);
    }
}
