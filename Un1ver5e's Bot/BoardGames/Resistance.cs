using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Un1ver5e.Service;

namespace Un1ver5e.Bot.BoardGames.Resistance
{
    public class Game
    {
        public readonly DiscordChannel Channel;
        public Player[] Players;
        public Operation[] Operations;
        private int round = 0;
        private int captainIndex = 0;
        private Player Captain { get => Players[captainIndex]; }
        private Operation CurrentOperation { get => Operations[round]; }

        private int teamAssemblyFails = 0;

        public void Play()
        {
            do
            {
                PlayRound();
            }
            while (Operations.Count(o => o.IsSuccess.Value == true) < 3 && Operations.Count(o => o.IsSuccess.Value == false) < 3);

            if (Operations.Count(o => o.IsSuccess.Value == true) == 3) Channel.SendMessageAsync("Победили повстанцы!".FormatAs());
            else Channel.SendMessageAsync("Победили шпионы!".FormatAs());

            var sb = new StringBuilder();
            foreach (var player in Players.Where(p => p.IsSpy))
            {
                sb.Append(player.DiscordMember.Nickname + "\n");
            }
            Channel.SendMessageAsync($"Шпионами были:\n{sb}".FormatAs());

            var dmb = new DiscordMessageBuilder()
                .WithContent("Нажмите кнопку чтобы удалить канал.".FormatAs())
                .AddComponents(new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "DeleteChannel", "Удалить"));

            Channel.SendMessageAsync(dmb);
        }

        public void PlayRound()
        {
            Channel.SendMessageAsync($"Начинается {round + 1} раунд!".FormatAs());
            Team team;

            do
            {
                Channel.SendMessageAsync($"Капитаном назначен {Captain}.".FormatAs());

                team = Team.AskAssemble(Captain, CurrentOperation);

                if (team.AskConfirm(this, Captain).Result)
                {
                    Channel.SendMessageAsync("Команда собрана!".FormatAs());
                    break;
                }
                else
                {
                    teamAssemblyFails++;
                    Channel.SendMessageAsync("Не удалось собрать команду.".FormatAs());
                }

                captainIndex = captainIndex++ % Players.Length;
            }
            while (++teamAssemblyFails < 5);

            teamAssemblyFails = 0;

            CurrentOperation.IsSuccess = team.CollectResults(CurrentOperation).Result;
            round++;
        }

        public Game(DiscordGuild guild, DiscordMember[] players)
        {
            if (players.Length < 5 || players.Length > 10) throw new Exception("Неверное количество игроков!");

            Channel = BoardGamesBase.CreateGameChannel(guild, players, "ИГРА - Сопротивление.");
            Operations = Operation.GetOperations(players.Length);
            Players = new Player[players.Length];

            Stack<bool> roles = Player.GetRoleConfiguration(Players.Length);

            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = new Player(players[i], roles.Pop(), this);
            }

            foreach (var player in Players)
            {
                player.Notify();
            }
        }
    }

    public class Operation
    {
        public Team Team;
        public readonly int TeamCount;
        public bool? IsSuccess = null;

        private Operation(int teamCount)
        {
            TeamCount = teamCount;
        }

        public static Operation[] GetOperations(int players) => players switch
        {
            5 => new Operation[] { new Operation(2), new Operation(3), new Operation(2), new Operation(3), new Operation(3) },
            6 => new Operation[] { new Operation(2), new Operation(3), new Operation(4), new Operation(3), new Operation(4) },
            7 => new Operation[] { new Operation(2), new Operation(3), new Operation(3), new Operation(4), new Operation(4) },
            8 => new Operation[] { new Operation(3), new Operation(4), new Operation(4), new Operation(5), new Operation(5) },
            9 => new Operation[] { new Operation(3), new Operation(4), new Operation(4), new Operation(5), new Operation(5) },
            10 => new Operation[] { new Operation(3), new Operation(4), new Operation(4), new Operation(5), new Operation(5) },
            _ => throw new ArgumentException()
        };
    }

    /// <summary>
    /// Represents a blank team awaiting confirmation by vote.
    /// </summary>
    public class Team
    {
        public readonly Player[] Players;
        public int Count { get => Players.Length; }

        /// <summary>
        /// Starts a vote defining if a team will go to the operation or not.
        /// </summary>
        /// <param name="game">A game in which this team will do their operation.</param>
        /// <param name="leader">A leader who has built this team, he doesn't take part in competition.</param>
        /// <returns>True if team is chosen to perform the operation, otherwise false.</returns>
        public async Task<bool> AskConfirm(Game game, Player captain)
        {
            var sb = new StringBuilder();
            foreach (var player in Players)
            {
                sb.Append(player.DiscordMember.Nickname + "\n");
            }

            var dmb = new DiscordMessageBuilder()
                .WithContent($"@here, согласны ли вы послать на задание эту команду:\n{sb}".FormatAs())
                .AddComponents(
                new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "true", "Да"),
                new DiscordButtonComponent(DSharpPlus.ButtonStyle.Danger, "false", "Нет"));

            var respondents = game.Players.Where((p) => p != captain);

            var responses = new Dictionary<Player, bool?>(respondents.Select(p => new KeyValuePair<Player, bool?>(p, null)));

            var msg = await game.Channel.SendMessageAsync(dmb);

            foreach (Player player in respondents)
            {
                await Task.Run(
                    () =>
                    {
                        responses[player] = Program.Interactivity.WaitForButtonAsync(msg, player.DiscordMember, null).Result.Result.Id == "true";
                    });
            }

            return responses.Values.Where(a => a == true).Count() > responses.Values.Where(a => a == false).Count();


        }

        public async Task<bool> CollectResults(Operation operation)
        {
            var responses = new Dictionary<Player, bool?>(operation.Team.Players.Select(p => new KeyValuePair<Player, bool?>(p, null)));
            foreach (Player player in operation.Team.Players)
            {
                await Task.Run(
                    async () =>
                    {
                        var dmb = new DiscordMessageBuilder()
                        .WithContent("Каков ваш вклад в операцию?".FormatAs())
                        .AddComponents(
                            new DiscordButtonComponent(DSharpPlus.ButtonStyle.Secondary, "success", "Успех"),
                            new DiscordButtonComponent(DSharpPlus.ButtonStyle.Danger, "failure", "Провал", !player.IsSpy)
                            );
                        var msg = await player.DiscordMember.SendMessageAsync(dmb);
                        responses[player] = Program.Interactivity.WaitForButtonAsync(msg, player.DiscordMember, null).Result.Result.Id == "true";
                    });
            }
            return responses.Values.All(a => a.Value);
        }

        private Team(Player[] players)
        {
            Players = players;
        }



        public static Team AskAssemble(Player player, Operation operation)
        {
            int num = 0;
            var players = player.Game.Players.Select((p) => new DiscordSelectComponentOption(p.DiscordMember.Nickname, (++num).ToString(), p.DiscordMember.Mention));

            var dmb = new DiscordMessageBuilder()
                .WithContent($"{player.DiscordMember.Mention}, соберите команду.".FormatAs())
                .AddComponents(
                new DiscordSelectComponent(
                    "TeamAssembler", "Выберите команду для операции.", players, false, operation.TeamCount, operation.TeamCount));
            
            var msg = player.Game.Channel.SendMessageAsync(dmb).Result;

            var res = Program.Interactivity.WaitForSelectAsync(msg, player.DiscordMember, "TeamAssembler", new TimeSpan(-1));

            return new Team(res.Result.Result.Values.Select((p) => player.Game.Players[int.Parse(p)]).ToArray());
        }
    }

    public class Player
    {
        public readonly Game Game;
        public DiscordMember DiscordMember;
        public bool IsSpy;
        public async void Notify()
        {
            if (IsSpy)
            {
                var deb = new DiscordEmbedBuilder()
                .AddField("Ваша роль", $"{DiscordMember.Nickname}, вы - шпион! Ваша задача - не выдать себя и сорвать планы повстанцев.");
                deb.ImageUrl = "https://i.yapx.ru/PAEHT.png";

                var sb = new StringBuilder();
                foreach (var member in Game.Players)
                {
                    sb.Append(member.DiscordMember.Nickname + "\n");
                }
                deb.AddField("Ваши союзники:", sb.ToString());

                await DiscordMember.SendMessageAsync(deb);
            }
            else
            {
                var deb = new DiscordEmbedBuilder()
               .WithImageUrl("https://i.yapx.ru/PAEHQ.png")
               .AddField("Ваша роль", $"{DiscordMember.Nickname}, вы - повстанец! Ваша задача - вычислить шпионов и успешно завершить миссии.");

                await DiscordMember.SendMessageAsync(deb);
            }
        }
        public static Stack<bool> GetRoleConfiguration(int players) => new Stack<bool>(
            players switch
            {
                5  => new bool[] { true, true, false, false, false }.Shuffle(),
                6  => new bool[] { true, true, false, false, false, false }.Shuffle(),
                7  => new bool[] { true, true, true, false, false, false, false }.Shuffle(),
                8  => new bool[] { true, true, true, false, false, false, false, false }.Shuffle(),
                9  => new bool[] { true, true, true, false, false, false, false, false, false }.Shuffle(),
                10 => new bool[] { true, true, true, true, false, false, false, false, false, false }.Shuffle(),
                _ => throw new NotImplementedException()
                //5 - 2
                //6 - 2
                //7 - 3
                //8 - 3
                //9 - 3
                //10 - 4
            });
        public Player(DiscordMember member, bool isSpy, Game game)
        {
            Game = game;
            DiscordMember = member;
            IsSpy = isSpy;
        }
    }

    public class ResistanceCommands : BaseCommandModule
    {
        [Command("resistance"), RequireGuild()]
        public async Task Resistance(CommandContext ctx)
        {
            var buttons = new DiscordButtonComponent[]
            {
                new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "Me", "Участвую"),
                new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "Start", "Начать")
            };
            
            var dmb = new DiscordMessageBuilder()
                .WithContent($"{ctx.Member.Nickname} предлагает поиграть в Сопротивление.".FormatAs())
                .AddComponents(buttons);

            var players = new Dictionary<DiscordMember, string>
            {
                { ctx.Member, ctx.Member.Nickname }
            };

            var msg = await ctx.Channel.SendMessageAsync(dmb
                            .AddEmbed(new DiscordEmbedBuilder().AddField("Участвуют:", players.Values.Aggregate((s1, s2) => s1 + "\n" + s2)))
                            );

            bool stopRequested = false;

            do
            {
                var respond = Program.Interactivity.WaitForButtonAsync(msg, new TimeSpan(0, 10, 0));
                respond.Wait();
                var member = (DiscordMember)respond.Result.Result.User;
                switch (respond.Result.Result.Id)
                {
                    case "Me":
                        if (players.TryAdd(member, member.Nickname))
                        {
                            dmb.Embed = new DiscordEmbedBuilder().AddField("Участвуют:", players.Values.Aggregate((s1, s2) => s1 + "\n" + s2));
                            await respond.Result.Result.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.UpdateMessage);
                            await msg.ModifyAsync(dmb);
                        }
                        break;
                    case "Start":
                        if (member == ctx.Member) stopRequested = true;
                        break;
                }
            }
            while (!stopRequested);

            try
            {
                Game game = new Game(
                ctx.Guild,
                players.Keys.ToArray()
                );
                game.Play();
            }
            catch (Exception ex)
            {
                await msg.RespondAsync(ex.Message.FormatAs());
            }
        }
    }

}
