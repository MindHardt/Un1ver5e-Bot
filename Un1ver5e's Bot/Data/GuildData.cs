using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Un1ver5e.Service;

namespace Un1ver5e.Bot
{
    public class GuildData
    {
        public ulong GuildId { get; set; }
        public ulong? BGChannelID { get; set; }

        public static GuildData GetByGuild(DiscordGuild guild) => _cache[guild.Id];
        public static GuildData GetByID(ulong id) => _cache[id];

        /// <summary>
        /// Used to save user's data into a JSON file.
        /// </summary>
        /// <param name="path">A path for a JSON file to save into.</param>
        public void SaveJson(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this, Generals.JsonOptions));
        /// <summary>
        /// Used to read user's data from a JSON file.
        /// </summary>
        /// <param name="path">A path to JSON file.</param>
        /// <returns>A read UserData object.</returns>
        public static UserData FromJson(string path) => JsonSerializer.Deserialize<UserData>(path);


        private static readonly Dictionary<ulong, GuildData> _cache = new Dictionary<ulong, GuildData>();
    }
}
