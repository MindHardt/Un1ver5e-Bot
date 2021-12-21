using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Un1ver5e.Service;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using Service;

namespace Un1ver5e.Bot
{
    public class UserData
    {
        public ulong UserId { get; set; }
        public AccessLevel Access{ get; set; }


        public int BeerDrunk { get; set; }
        public int Smoked { get; set; }

        [JsonIgnore()]
        public string AccessLevelEmoji
        {
            get => Access switch
            {
                AccessLevel.Banned => "👺",
                AccessLevel.Default => "👶",
                AccessLevel.Intermediate => "👦",
                AccessLevel.Advanced => "👨",
                AccessLevel.Helper => "👷",
                AccessLevel.Moderator => "🎅",
                AccessLevel.Admin => "👼",
                _ => throw new NotImplementedException()
            };
        }
        [JsonIgnore()]
        public string AccessLevelName
        {
            get => Access switch
            {
                AccessLevel.Banned => "Бан",
                AccessLevel.Default => "Стандартный",
                AccessLevel.Intermediate => "Опытный",
                AccessLevel.Advanced => "Продвинутый",
                AccessLevel.Helper => "Помощник",
                AccessLevel.Moderator => "Модератор",
                AccessLevel.Admin => "Администратор",
                _ => throw new NotImplementedException()
            };
        }


        public static UserData GetByUser(DiscordUser user) => _cache[user.Id];
        public static UserData GetByID(ulong id) => _cache[id];

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

        /// <summary>
        /// Represents a user's access level. The higher the more commands are available.
        /// </summary>
        public enum AccessLevel
        {
            Banned = -1,
            Default = 0,
            Intermediate = 1,
            Advanced = 2,
            Helper = 3,
            Moderator = 4,
            Admin = 5,
        }

        public static void Initialize()
        {
            var task = Task.Run(() =>
            {
                UserData temp;
                foreach (var file in Directory.GetFiles($"{Generals.BotFilesPath}\\UserData", "*.json"))
                {
                    temp = FromJson(file);
                    _cache.Add(temp.UserId, temp);
                }
            });
            Logs.Log("Initiated UserData loading.");
            task.Wait();
            Logs.Success("Loaded UserData successfully.");
        }

        private static readonly Dictionary<ulong, UserData> _cache = new Dictionary<ulong, UserData>();
    }
}
