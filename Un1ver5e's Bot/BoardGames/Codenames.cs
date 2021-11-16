using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Un1ver5e.Service;

namespace Un1ver5e.Bot.BoardGames.Codenames
{
    public class Game //: BoardGamesBase.BoardGame
    {
        public readonly int Seed;
        public readonly string HexSeed;
        
        public DiscordThreadChannel Channel;
        public List<DiscordMember> Players;

        public List<DiscordMember> Blue;
        public List<DiscordMember> Red;

        public Word[] Words; //= Word.GetWords(RandomSeed);

        public Game(Dictionary dictionary, string seed)
        {
            Seed = ReadID(seed);
            Words = Word.GetWords(dictionary.GetWords(Seed), Seed % 2 == 0, Seed);
            HexSeed = Seed.ToString("X");
        }

        public Game(Dictionary dictionary, int seed)
        {
            Seed = seed;
            Words = Word.GetWords(dictionary.GetWords(Seed), Seed % 2 == 0, Seed);
            HexSeed = Seed.ToString("X");
        }


        private static int ReadID(string id)
        {
            int result = 0;
            byte order = 0;
            while (id.Length > 0)
            {
                byte last = (byte)id[^1];
                int power = order switch
                {
                    0 => 0,
                    1 => 256,
                    2 => 65536,
                    3 => 16777216,
                    _ => throw new ArgumentException()
                };
                result += (last * power);
                order = (byte)(order == 3 ? 0 : order++);
            }
            return result;
        }
    }

    public class Dictionary
    {
        public static Dictionary GetDummy()
        {
            var lines = File.ReadAllText("C:\\Users\\igorb\\Desktop\\BotFiles\\BGs\\Codenames\\Dictionaries\\GaGa (400 слов).txt").Split(", ");
            var test = new Dictionary("test", lines);
            return test;
        }
        
        public static readonly Dictionary<string, Dictionary> DictionariesList = new Dictionary<string, Dictionary>();
        public static readonly string PathToFolder = Generals.BotFilesPath + "\\BGs\\Codenames\\Dictionaries";
        public readonly string Name;

        /// <summary>
        /// Static list of all the words.
        /// </summary>
        private readonly string[] allWords;

        /// <summary>
        /// A list of unused words in random order, from which the words are taken. 
        /// </summary>
        private Stack<string> tempWords;

        /// <summary>
        /// Takes the random word from the list.
        /// </summary>
        /// <returns></returns>
        private string GetWord() => tempWords.Pop();

        /// <summary>
        /// Gets 25 random words from the Dictionary. Calling this method repeatedly will return unique words if possible.
        /// </summary>
        /// <returns>The stack with 25 words.</returns>
        public Stack<string> GetWords(int seed)
        {
            ShuffleWords(seed);
            return new Stack<string>(new string[25].FillWith(() => GetWord()));
        }

        /// <summary>
        /// Loads a dictionary from file name.
        /// </summary>
        /// <param name="name">A name of a dictionary.</param>
        /// <returns></returns>
        public static Dictionary LoadFromFile(string name)
        {
            if (DictionariesList.ContainsKey(name)) throw new Exception("Словарь с таким именем уже загружен!");
            return new Dictionary(name, File.ReadAllText($"{PathToFolder}\\{name}").Split(", "));
        }

        /// <summary>
        /// Builds new list of unused words in random order using specified random seed.
        /// </summary>
        public void ShuffleWords(int seed) => tempWords = new Stack<string>(allWords.Shuffle(new Random(seed)));


        /// <summary>
        /// Creates a new instance of Dictionary object and places it in list.
        /// </summary>
        /// <param name="name">Dictionary name.</param>
        /// <param name="words">The list of all words.</param>
        private Dictionary(string name, string[] words)
        {
            Name = name;
            allWords = words;
            DictionariesList.Add(Name, this);
        }
    }
    public class Word
    {
        public readonly string Content;
        public readonly WordColor Color;
        public bool IsGuessed;
        public enum WordColor
        {
            Black = -1,
            White = 0,
            Blue = 1,
            Red = 2
        }

        /// <summary>
        /// Returns a random collection of colors.
        /// </summary>
        /// <param name="redMore">True if there are more red words than blue, false otherwise.</param>
        /// <returns></returns>
        private static Stack<WordColor> GetColors(bool redMore, int seed) =>
            new Stack<WordColor>(
                new WordColor[]
                {
                    WordColor.Blue, WordColor.Blue, WordColor.Blue, WordColor.Blue,
                    WordColor.Blue, WordColor.Blue, WordColor.Blue, WordColor.Blue,
                    WordColor.Red, WordColor.Red, WordColor.Red, WordColor.Red,
                    WordColor.Red, WordColor.Red, WordColor.Red, WordColor.Red,
                    WordColor.White, WordColor.White, WordColor.White, WordColor.White,
                    WordColor.White, WordColor.White, WordColor.White, WordColor.Black,
                    redMore ? WordColor.Red : WordColor.Blue
                }.Shuffle(new Random(seed)));

        private Word(string word, WordColor color)
        {
            Content = word;
            Color = color;
        }

        public static Word[] GetWords(Stack<string> words, bool redMore, int seed)
        {
            var colors = GetColors(redMore, seed);
            Word[] result = new Word[25];
            for (int i = 0; i < 25; i++)
            {
                result[i] = new Word(words.Pop(), colors.Pop());
            }
            return result;
        }
    }







    public class CodenamesCommands : BaseCommandModule
    {
        [Command("cn_load_dictionaries"), Aliases("cn_ld")]
        public async Task LoadDictionaries(CommandContext ctx)
        {
            var options = new List<DiscordSelectComponentOption>();
            foreach (var file in new DirectoryInfo(Dictionary.PathToFolder).GetFiles())
            {
                options.Add(new DiscordSelectComponentOption(file.Name, file.Name));
            }
            var dmb = new DiscordMessageBuilder()
                .WithContent("Выберите словари для загрузки.".FormatAs())
                .AddComponents(
                new DiscordSelectComponent(
                    "cd_dictionaryload_select",
                    "Выберите словари.", options,
                    false, 1, options.Count
                    ));
            var msg = await ctx.RespondAsync(dmb);
            var respond = await Program.Interactivity.WaitForSelectAsync(msg, ctx.User, "cd_dictionaryload_select", new TimeSpan(0, 3, 0));
            if (respond.TimedOut) await ctx.RespondAsync("Время вышло!".FormatAs());
            else
            {
                foreach (var option in respond.Result.Values)
                {
                    await Task.Run(() => Dictionary.LoadFromFile(option));
                }
            }
            await ctx.RespondAsync($"Успешно загружено!".FormatAs());
        }

        [Command("cn_add_dictionary"), Aliases("cn_ad")]
        public async Task AddDictionary(CommandContext ctx)
        {
            await ctx.RespondAsync("Автоматическое добавление словарей не предусмотрено, обратитесь к создателю.".FormatAs());
        }

        [Command("cn_engage"), Aliases("cn_en"), RequireThreadChannel()]
        public async Task Engage(CommandContext ctx)
        {
            
            await ctx.RespondAsync("Автоматическое добавление словарей не предусмотрено, обратитесь к создателю.".FormatAs());
        }

        [Command("cn_test"), Aliases("cn_t")]
        public async Task Test(CommandContext ctx, string seed)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.Client, ":ok_hand:"));

            var game = new Game(Dictionary.GetDummy(), seed);

            var words = game.Words;

            var btnWords = new Queue<string>(words.Select((w) => w.Content));

            FileStream stream = File.Create("C:\\Users\\igorb\\Desktop\\BotFiles\\BGs\\Codenames\\Gallery\\test.jpg");
            CodenamesDrawer.Display(
                game)
                .Save(stream, ImageFormat.Jpeg);
            stream.Seek(0, SeekOrigin.Begin);

            var buttons = new DiscordActionRowComponent[5];

            int maxlen = btnWords.Max((w) => w.Length);
            for (int i = 0; i < 5; i++)
            {
                buttons[i] = new DiscordActionRowComponent(
                    new DiscordComponent[]
                    {
                        new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, btnWords.Peek(), btnWords.Peek() + new string(' ', maxlen - btnWords.Dequeue().Length)),
                        new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, btnWords.Peek(), btnWords.Peek() + new string(' ', maxlen - btnWords.Dequeue().Length)),
                        new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, btnWords.Peek(), btnWords.Peek() + new string(' ', maxlen - btnWords.Dequeue().Length)),
                        new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, btnWords.Peek(), btnWords.Peek() + new string(' ', maxlen - btnWords.Dequeue().Length)),
                        new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, btnWords.Peek(), btnWords.Peek() + new string(' ', maxlen - btnWords.Dequeue().Length))
                    });
            }


            var dmb = new DiscordMessageBuilder()
                .WithFile(stream)
                .WithContent("Ваша хуета".FormatAs())
                .AddComponents((IEnumerable<DiscordActionRowComponent>)buttons);


            await ctx.RespondAsync(dmb);
        }
    }

    public static class CodenamesDrawer
    {
        public readonly static string GalleryPath = Generals.BotFilesPath + "\\BGs\\Codenames\\Gallery";
        public static Image White
        { 
            get => Image.FromFile(GalleryPath + "\\White.jpg"); 
        }
        public static Image Red
        {
            get => Image.FromFile(GalleryPath + "\\Red.jpg");
        }
        public static Image Blue
        {
            get => Image.FromFile(GalleryPath + "\\Blue.jpg");
        }
        public static Image Black
        {
            get => Image.FromFile(GalleryPath + "\\Black.jpg");
        }
        public static Image Table
        {
            get => Image.FromFile(Generals.BotFilesPath + "\\BGs\\.Tables\\1366x720.png");
        }

        public readonly static Brush BrushBlack = new SolidBrush(Color.Black);
        public readonly static Brush BrushGray   = new SolidBrush(Color.Gray);

        private static IEnumerable<Rectangle> GetRectangles()
        {
            List<Rectangle> rectangles = new List<Rectangle>();
            for (int x = 13; x <= 1073; x += 265)
            {
                for (int y = 60; y <= 600; y += 120)
                {
                    rectangles.Add(new Rectangle(x, y, 250, 100));
                }
            }
            return rectangles;
        }

        public static Image Display(Game game)
        {
            Image table = Table;
            Graphics gtable = Graphics.FromImage(table);
            Font font = new Font(FontFamily.GenericMonospace, 30);

            gtable.DrawString("ID: " + game.HexSeed, font, BrushBlack, 0, 0);

            Rectangle[] rectangles = GetRectangles().ToArray();
            for (int i = 0; i < 25; i++)
            {
                var word = game.Words[i];
                Bitmap iword = new Bitmap(word.Color switch
                {
                    Word.WordColor.Black => Black,
                    Word.WordColor.White => White,
                    Word.WordColor.Blue => Blue,
                    Word.WordColor.Red => Red,
                    _ => throw new ArgumentException()
                });
                Graphics gword = Graphics.FromImage(iword);

                Brush brush = word.IsGuessed ? BrushBlack : BrushGray;

                gword.DrawString(word.Content.Length <= 8 ? word.Content : word.Content[0..8] + "-\n" + word.Content[8..], font, brush, 0, 0);
                gtable.DrawImage(iword, rectangles[i]);
            }

            return table;
        }
    }
}
