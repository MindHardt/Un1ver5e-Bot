using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace Un1ver5e.Bot
{
    public static class Program
    {
        //<Global parameters>

        //The main Random object, all the code is recommended to use this one
        public readonly static Random Randomizer = new Random();

        //The main DiscordClient object, all the code is recommended to use this one
        public static DiscordClient Client { get; private set; }

        //</Global parameters>

        //The underlying line is required for the bot to run asyncronously
        private static void Main(string[] args) => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        //The underlying block is the Main code
        private static async Task MainTask(string[] args)
        {
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = "ss",
                TokenType = TokenType.Bot
            }
            );
        }
    }
}
