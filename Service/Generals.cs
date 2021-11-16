using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Un1ver5e.Service
{
    /// <summary>
    /// Contains all the static objects which are used everywhere.
    /// </summary>
    public static class Generals
    {
        /// <summary>
        /// The main WebClient object.
        /// </summary>
        public readonly static WebClient WebClient = new WebClient();

        /// <summary>
        /// The main Random object.
        /// </summary>
        public static readonly Random Random = new Random();

        public readonly static string BotFilesPath = "C:\\Users\\igorb\\Desktop\\BotFiles";
    }
}
