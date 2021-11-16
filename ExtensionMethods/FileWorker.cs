using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Un1ver5e.Service
{
    public static class FileWorker
    {
        public readonly static string BotFilesPath = "\\.botFiles\\";
        //public interface IСsvStorable
        //{
        //    public string CsvStore();
        //    public object[] CsvGet();
        //}
        public static void InitializeFolders()
        {

        }
        public static string ReadToken()
        {
            if (!File.Exists(BotFilesPath + ".token.txt")) CreateTokenFile();
            //Bot.Program.Logger.AwaitAction("Place your token in file and press any key.");
            return File.ReadAllText(BotFilesPath + ".token.txt");
        }
        public static void CreateTokenFile()
        {
            File.WriteAllText(BotFilesPath + ".token.txt", "Replace this line with your token.");
            System.Diagnostics.Process.Start("explorer", BotFilesPath + ".token.txt"); 
        }
        public static void DisposeToken()
        {

        }

    }
}
