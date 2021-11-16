using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Un1ver5e.Bot
{
    public static class BoardGamesBase
    {
        private static readonly Dictionary<DiscordGuild, DiscordChannel> BGChannels = new Dictionary<DiscordGuild, DiscordChannel>();

        public static bool IsBGChannel(this DiscordChannel channel) => BGChannels.ContainsValue(channel);

        public static DiscordChannel GetChannel(DiscordGuild guild) => BGChannels.GetValueOrDefault(guild);

        public static DiscordChannel CreateGameChannel(DiscordGuild guild, DiscordMember[] players, string name = "lorem ispum")
        {
            if (!CheckGuild(guild)) throw new Exception("Нет канала для настолок!");
            var channel = GetChannel(guild).CloneAsync("Игра").Result;

            var dobs = new List<DiscordOverwriteBuilder>
            {
                new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(DSharpPlus.Permissions.AccessChannels)
            };
            foreach (var player in players)
            {
                dobs.Add(new DiscordOverwriteBuilder(player).Allow(DSharpPlus.Permissions.AccessChannels));
            }

            channel.ModifyAsync((c) =>
            {
                c.PermissionOverwrites = dobs;
                c.Name = name;
            });

            return channel;
        }

        /// <summary>
        /// Reads BG channels from file into memory.
        /// </summary>
        public static void InitializeChannels()
        {
            var channels = File.ReadAllLines(Service.Generals.BotFilesPath + "\\BGs\\BGChannels.txt");
            foreach (var channel in channels)
            {
                var chs = channel.Split(' ');
                BGChannels.Add(
                    Program.Client.GetGuildAsync(ulong.Parse(chs[0])).Result,
                    Program.Client.GetChannelAsync(ulong.Parse(chs[1])).Result
                    );
            }
        }

        /// <summary>
        /// Saves the channels from memory to file.
        /// </summary>
        /// <returns></returns>
        public static async Task SaveChannels()
        {
            var channels = new List<string>();
            foreach (var channel in BGChannels)
            {
                channels.Add(channel.Key.Id.ToString() + " " + channel.Value.Id.ToString());
            }
            await File.WriteAllLinesAsync(Service.Generals.BotFilesPath + "\\BGs\\BGChannels.txt", channels);
        }

        /// <summary>
        /// Checks whether guild has a board game channel.
        /// </summary>
        /// <param name="guild">A guild to check.</param>
        /// <returns>True if a guild has a board games channel, otherwise false.</returns>
        public static bool CheckGuild(DiscordGuild guild) => BGChannels.ContainsKey(guild);

        /// <summary>
        /// Represents a board game class. This is an abstract class.
        /// </summary>
        public abstract class BoardGame
        {
            public DiscordChannel RootChannel { get; protected set; }
            public DiscordThreadChannel ThreadChannel { get; protected set; }
            public DiscordMember Players { get; protected set; }
            public abstract void Start();
            public abstract void End();
        }
        public partial class BoardGamesCommands : BaseCommandModule
        {
            [Command("bg_init"), RequireGuild(), RequirePermissions(DSharpPlus.Permissions.Administrator)]
            public async Task BGInit(CommandContext ctx)
            {
                if (BGChannels.ContainsKey(ctx.Guild)) BGChannels.Remove(ctx.Guild);

                var msg = await ctx.Channel.SendMessageAsync(
                        new DiscordMessageBuilder()
                        .WithContent("Для игры в настолки необходим канал. Выберите опцию:")
                        .AddComponents(
                            new DiscordSelectComponent("BGChannelSelect", "Выберите опцию",
                            new DiscordSelectComponentOption[]
                            {
                                new DiscordSelectComponentOption("Использовать этот канал", "this"),
                                new DiscordSelectComponentOption("Создать новый канал", "new"),
                                new DiscordSelectComponentOption("Отмена", "abort")
                            }
                            )
                        )
                        );

                var option = Program.Interactivity.WaitForSelectAsync(
                        msg,
                        ctx.User,
                        "BGChannelSelect",
                        new TimeSpan(0, 1, 0)
                        );

                if (option.Result.TimedOut || option.Result.Result.Values[0] == "abort")
                {
                    await msg.RespondAsync("Вы можете настроить модуль настолок позже.");
                }
                else if (option.Result.Result.Values[0] == "this")
                {
                    await msg.RespondAsync("Канал настолок установлен - " + msg.Channel.Mention);
                    BGChannels.Add(ctx.Guild, msg.Channel);
                    await SaveChannels();
                }
                else
                {
                    var newChannel = await ctx.Channel.CloneAsync();
                    await newChannel.ModifyAsync(
                        (c) =>
                        {
                            c.Name = "Настолки";
                            c.Topic = "Канал для настольных игр.";
                        }
                    );
                    await msg.RespondAsync("Канал настолок установлен - " + newChannel.Mention);
                    BGChannels.Add(ctx.Guild, newChannel);
                    await SaveChannels();
                }

            }
        }

        
    }
}
