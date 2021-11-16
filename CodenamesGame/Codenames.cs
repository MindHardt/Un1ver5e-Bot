using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Un1ver5e.Bot;
using Un1ver5e.Service;
using static Un1ver5e.Bot.BoardGamesBase;

namespace CodenamesGame
{
    public class CodenamesGame : BoardGame
    {
        public static string[] Dictionary;
        
        public CodenamesWord[][] Words;

        public override void End()
        {
            
        }

        public override void Start()
        {
            
        }
        public CodenamesGame(DiscordGuild guild, int? seed)
        {
            if (!CheckGuild(guild)) throw new Exception("На сервере не установлен канал для настолок! Используйте \"mo bg_init\" для настройки.");
            RootChannel = GetChannel(guild);
            Words = CodenamesWord.GetField(seed);
        }

        public DiscordEmbedBuilder GetEmbed()
        {
            var deb = new DiscordEmbedBuilder();
            for (int y = 0; y < Words.Length; y++)
            {
                for (int x = 0; x < Words[y].Length; x++)
                {
                    deb.AddField(string.Empty, Words[y][x].Word, true);
                }
            }
            return deb;
        }

        public class CodenamesWord
        {
            public readonly string Word;
            public readonly WordColor Color;
            public bool IsRevealed = true;//false

            public enum WordColor
            {
                Black = -1,
                White = 0,
                Red = 1,
                Blue = 2
            }

            public static CodenamesWord[][] GetField(int? seed)
            {
                Random random = new Random(seed ?? DateTime.Now.Millisecond);

                CodenamesWord[][] raw = new CodenamesWord[5][];
                Stack<string> wordList = new Stack<string>(Dictionary.Shuffle().ToArray()[0..24]);
                Stack<WordColor> colorList = GetColorList();

                for (int y = 0; y < raw.Length; y++)
                {
                    raw[y] = new CodenamesWord[5];
                    for (int x = 0; x < raw[y].Length; x++)
                    {
                        raw[y][x] = new CodenamesWord(wordList.Pop(), colorList.Pop());
                    }
                }

                return raw;
            }

            public CodenamesWord(string word, WordColor color)
            {
                Word = word;
                Color = color;
            }

            private static Stack<WordColor> GetColorList() => new Stack<WordColor>(new WordColor[]
                {
                    WordColor.Red, WordColor.Red, WordColor.Red, WordColor.Red,
                    WordColor.Red, WordColor.Red, WordColor.Red, WordColor.Red,
                    WordColor.Blue, WordColor.Blue, WordColor.Blue, WordColor.Blue,
                    WordColor.Blue, WordColor.Blue, WordColor.Blue, WordColor.Blue,
                    WordColor.White, WordColor.White, WordColor.White, WordColor.White,
                    WordColor.White, WordColor.White, WordColor.White, WordColor.Black,
                    Generals.Random.Next(0, 2) == 0 ? WordColor.Red : WordColor.Blue
                }.Shuffle());
        }

        public partial class CodeNamesCommands : BaseCommandModule
        {
            [Command("cn_field"), RequireGuild()]
            public async Task CNField(CommandContext ctx)
            {
                CodenamesGame cng = new CodenamesGame(ctx.Guild, null);
                await cng.RootChannel.SendMessageAsync(cng.GetEmbed());
            }
        }
    }
}
