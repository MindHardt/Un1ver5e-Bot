using System;
using System.Collections.Generic;
using System.Text;

namespace Un1ver5e.Service
{
    public interface IUn1ver5eLogger
    {
        public void Log(string message);
        public void Warn(string message);
        public void Fatal(string message);
        public void Success(string message);
        public void AwaitAction(string message);
        public void TryAndLog(Action action, string description)
        {
            Log("Trying to " + description);
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Warn("Failed: " + ex.Message);
            }
            finally
            {
                Success("Ok!");
            }
        }
        public static string GetLoggerTimeStamp()
        {
            return ("[" + DateTimeOffset.Now + "] ");
        }
    }
    public class ConsoleLogger : IUn1ver5eLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(IUn1ver5eLogger.GetLoggerTimeStamp() + message);
        }
        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(IUn1ver5eLogger.GetLoggerTimeStamp() + message);
            Console.ResetColor();
        }
        public void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(IUn1ver5eLogger.GetLoggerTimeStamp() + message);
            Console.ResetColor();
        }
        public void Fatal(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(IUn1ver5eLogger.GetLoggerTimeStamp() + message);
            Console.ResetColor();
        }
        public void AwaitAction(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Press any key to continue...");
            Console.ResetColor();
            Console.ReadKey(false);
        }
    }
}
