using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Un1ver5e.Service;

namespace Un1ver5e.Bot
{
    public static class Features
    {
        public static DiscordEmbedBuilder Forbidden { get => new DiscordEmbedBuilder().WithImageUrl("https://cdn.discordapp.com/attachments/751088503877795981/906989457775947786/i.png");  }
    }
    public class BasicCommands : BaseCommandModule
    {
        [Command("fuck"), Description("Уникальная возможность пойти нахуй.")]
        public async Task Fuck(CommandContext ctx)
        {
            var dmb = new DiscordMessageBuilder()
                .WithContent(ctx.User.Mention + ", ПОШЕЛ НАХУЙ");
            await ctx.RespondAsync(dmb);
        }

        [Command("fuck"), RequireGuild(), Description("Уникальная возможность послать человека нахуй.")]
        public async Task Fuck(CommandContext ctx, DiscordMember member)
        {
            if (member.IsCurrent) member = ctx.Member;
            var dmb = new DiscordMessageBuilder()
                .WithContent(member.Mention + ", ПОШЕЛ НАХУЙ");
            await ctx.RespondAsync(dmb);
        }

        [Command("rnd"), Aliases("random"), Description("Выдает случайное число между двумя данными числами включительно.")]
        public async Task Rnd(CommandContext ctx, int min, int max)
        {
            if (max < min) await ctx.RespondAsync(new DiscordEmbedBuilder().WithImageUrl("https://cdn.discordapp.com/attachments/751088503877795981/906989457775947786/i.png"));
            else await ctx.RespondAsync($"Ваше число - {Service.Generals.Random.Next(min, max + 1)}".FormatAs());
        }

        [Command("pivo"), Aliases("beer"), RequireGuild(), Description("Позволяет бахнуть пивка с кем-то. Не жизнь а сказка.")]
        public async Task Beer(CommandContext ctx, DiscordMember member)
        {
            var btnYes = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "beerYes", "КОНЕЧНО");
            var btnNo = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Danger, "beerNo", "НЕТ");

            var msg = await ctx.Channel.SendMessageAsync(
                new DiscordMessageBuilder()
                .WithContent(member.Mention + ", " + ctx.Member.Nickname + " предлагает вам БАХНУТЬ ПИВКА.\nСогласны ли вы?")
                .WithFile("beerQ.jpg", Drawing.GetBeerQuery(ctx.Member.AvatarUrl, member.AvatarUrl))
                .AddComponents(
                    btnYes,
                    btnNo
                    )
                );
            var result = await Program.Interactivity.WaitForButtonAsync(msg, member, new TimeSpan(0, 0, 45));
            if (result.TimedOut || result.Result.Id == "beerNo")
            {
                await ctx.Channel.SendMessageAsync(
                    new DiscordMessageBuilder()
                    .WithContent(member.Nickname + " ТОКСИК И НЕ ЗАХОТЕЛ БАХНУТЬ ПИВКА!")
                    .WithFile("beerQ.jpg", Drawing.GetBeerNo(ctx.Member.AvatarUrl, member.AvatarUrl))
                    );
            }
            else
            {
                await ctx.Channel.SendMessageAsync(
                    new DiscordMessageBuilder()
                    .WithContent(ctx.Member.Nickname + " и " + member.Nickname + " БАХНУЛИ ПИВКА!")
                    .WithFile("beerQ.jpg", Drawing.GetBeerYes(ctx.Member.AvatarUrl, member.AvatarUrl))
                    );
            }
        }

        [Command("smoke"), RequireGuild(), Description("Позволяет выйти покурить с кем-то. Не жизнь а сказка.")]
        public async Task Smoke(CommandContext ctx, DiscordMember member)
        {
            var btnYes = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, "smokeYes", "КОНЕЧНО");
            var btnNo = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Danger, "smokeNo", "НЕТ");

            var msg = await ctx.Channel.SendMessageAsync(
                new DiscordMessageBuilder()
                .WithContent(member.Mention + ", " + ctx.Member.Nickname + " предлагает вам ВЫЙТИ ПОКУРИТЬ.\nСогласны ли вы?")
                .WithFile("beerQ.jpg", Drawing.GetSmokeQuery(ctx.Member.AvatarUrl, member.AvatarUrl))
                .AddComponents(
                    btnYes,
                    btnNo
                    )
                );
            var result = await Program.Interactivity.WaitForButtonAsync(msg, member, new TimeSpan(0, 0, 45));
            if (result.TimedOut || result.Result.Id == "smokeNo")
            {
                await ctx.Channel.SendMessageAsync(
                    new DiscordMessageBuilder()
                    .WithContent(member.Nickname + " ТОКСИК И НЕ ЗАХОТЕЛ ВЫЙТИ ПОКУРИТЬ!")
                    .WithFile("beerQ.jpg", Drawing.GetSmokeNo(ctx.Member.AvatarUrl, member.AvatarUrl))
                    );
            }
            else
            {
                await ctx.Channel.SendMessageAsync(
                    new DiscordMessageBuilder()
                    .WithContent(ctx.Member.Nickname + " и " + member.Nickname + " СХОДИЛИ ПОКУРИТЬ!")
                    .WithFile("beerQ.jpg", Drawing.GetSmokeYes(ctx.Member.AvatarUrl, member.AvatarUrl))
                    );
            }
        }

        [Command("avatar"), Description("Выдает ваш аватар.")]
        public async Task Avatar(CommandContext ctx)
        {
            await ctx.Message.RespondAsync(new DiscordEmbedBuilder().WithImageUrl(ctx.User.AvatarUrl));
        }

        [Command("avatar"), RequireGuild(), Description("Выдает аватар указанного человека.")]
        public async Task Avatar(CommandContext ctx, DiscordMember member)
        {
            await ctx.Message.RespondAsync(new DiscordEmbedBuilder().WithImageUrl(member.AvatarUrl));
        }

        [Command("openfolder"), RequireOwner()]
        public async Task OpenFolder(CommandContext ctx)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(Program.Client, ":white_check_mark:"));
            Process.Start("explorer", Service.Generals.BotFilesPath);
        }
    }

    /// <summary>
    /// Defines that the command is only usable if called in a respond message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireReferencedMessage : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(ctx.Message.ReferencedMessage != null || help);
        }
    }

    /// <summary>
    /// Defines that the command is only usable if called in a thread channel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireThreadChannel : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(ctx.Channel is DiscordThreadChannel || help);
        }
    }

    /// <summary>
    /// Defines that the command is only usable if called in a board games channel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireBoardGamesChannel : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(ctx.Channel.IsBGChannel() || help);
        }
    }
}
