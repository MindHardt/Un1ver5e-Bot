using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Un1ver5e.Service
{
    /// <summary>
    /// Contains all the static objects which are used everywhere.
    /// </summary>
    public static class Generals
    {

        public readonly static JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = false
        };
        /// <summary>
        /// The main WebClient object.
        /// </summary>
        public readonly static WebClient WebClient = new WebClient();

        /// <summary>
        /// The main Random object.
        /// </summary>
        public static readonly Random Random = new Random();

        //public readonly static string BotFilesPath = "\\..\\..\\..\\..\\BotFiles";
        public readonly static string BotFilesPath = "..\\..\\..\\..\\BotFiles";
    }
}
