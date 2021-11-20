using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Un1ver5e.Service;
using System.IO;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Un1ver5e.Bot.BoardGames;
using System.Diagnostics;

namespace Un1ver5e.Bot
{
    public static class Program
    {
        /// <summary>
        /// The main DiscordClient object, all the code is recommended to use this one
        /// </summary>
        public static DiscordClient Client { get; private set; }

        /// <summary>
        /// The main InteractivityExtension object, all the code is recommended to use this one
        /// </summary>
        public static InteractivityExtension Interactivity { get; private set; }

        /// <summary>
        /// The main CommandsNextExtension object, all the code is recommended to use this one
        /// </summary>
        public static CommandsNextExtension CommandsNext { get; private set; }

        //The underlying line is required for the bot to run asyncronously
        private static void Main(string[] args) => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        //The underlying block is the Main code
        private static async Task MainTask(string[] args)
        {
            _ = Task.Run(() => AlwaysUpdateStatus());
            
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllText(Generals.BotFilesPath + "\\token.txt"),
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug
            }
            );
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            }
            );
            CommandsNext = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { "mo ", "мо" }
            }
            );

            await Tag.Initialize();

            CommandsNext.RegisterCommands<BasicCommands>();
            CommandsNext.RegisterCommands<Tag.TagCommands>();
            CommandsNext.RegisterCommands<BoardGamesBase.BoardGamesCommands>();
            CommandsNext.RegisterCommands<BoardGames.Codenames.CodenamesCommands>();
            CommandsNext.RegisterCommands<BoardGames.Resistance.ResistanceCommands>();

            Client.ComponentInteractionCreated += async (s, e) =>
            {
                if (e.Id == "DeleteChannel") await e.Channel.DeleteAsync();
                else
                {
                    await Task.Delay(1000);
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
            };

            await Client.ConnectAsync(
                new DiscordActivity(
                    "Call me via \"MO\"",
                    ActivityType.Playing
                    )
                );

            await Task.Delay(-1);
        }

        private static void AlwaysUpdateStatus()
        {
            var splashes = File.ReadAllLines(Generals.BotFilesPath + "\\Splashes.txt");
            var splash = splashes[Generals.Random.Next(0, splashes.Length)];

            var timer = new Stopwatch();
            timer.Start();

            string mem;
            string time;
            while (true)
            {
                mem = $"Currentry using {GC.GetTotalMemory(false) / 1048576} MBs.";
                time = $"Online for {timer.Elapsed.ToString()[0..8]}";

                Console.Title = $"{mem} || {time} || {splash}";

                Task.Delay(500).Wait();
            }
        }
    }
}
