using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Un1ver5e.Service;
using DSharpPlus.CommandsNext;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using System.Text.Json;

namespace Un1ver5e.Bot
{
    class Tag
    {
        private static readonly Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
        private static readonly string PathToFolder = Generals.BotFilesPath + "\\Tags";

        public string[] Names { get; set; }
        public string Text { get; set; }
        public string[] Files { get; set; }

        /// <summary>
        /// Deletes this tag as well as its files.
        /// </summary>
        public void Delete()
        {
            File.Delete($"{PathToFolder}\\{Names[0]}.json");
            foreach (var name in Names)
            {
                tags.Remove(name);
            }
        }

        /// <summary>
        /// Reads all tags from hard drive and activates them.
        /// </summary>
        /// <returns></returns>
        public static async Task Initialize()
        {
            foreach (var path in Directory.GetFiles(PathToFolder))
            {
                await Task.Run(() =>
                {
                    string json = File.ReadAllText(path);
                    var tag = Read(json);
                    tag.Activate();
                });
            }
        }

        /// <summary>
        /// Adds a tag to active tags list.
        /// </summary>
        private void Activate()
        {
            foreach (var name in Names)
            {
                tags.Add(name, this);
            }
        }

        /// <summary>
        /// Gets a message builder for this tag.
        /// </summary>
        /// <returns>A DiscordMessageBuilder object which represents this Tag.</returns>
        public DiscordMessageBuilder GetMessage()
        {
            var dmb = new DiscordMessageBuilder()
                .WithContent(Text);

            foreach (var file in Files)
            {
                dmb = dmb.WithEmbed(new DiscordEmbedBuilder().WithImageUrl(file));
            }

            return dmb;
        }

        /// <summary>
        /// Saves a tag into a JSON file.
        /// </summary>
        private void Save() => File.WriteAllText($"{PathToFolder}\\{Names[0]}.json", JsonSerializer.Serialize<Tag>(this));

        /// <summary>
        /// Reads a tag from json format.
        /// </summary>
        /// <param name="json">JSON file content.</param>
        /// <returns>A tag build on this JSON.</returns>
        private static Tag Read(string json) => JsonSerializer.Deserialize<Tag>(json);

        /// <summary>
        /// Initializes a new Tag with given names and content from a specified Discord Message.
        /// </summary>
        /// <param name="names">All names of this tag.</param>
        /// <param name="msg">Reference message.</param>
        public Tag(string[] names, DiscordMessage msg)
        {
            Names = names;
            Text = msg.Content;
            Files = msg.Attachments.Select(a => a.Url).ToArray();
            Activate();
            Save();
        }

        /// <summary>
        /// An empty builder for JSON deserialization.
        /// </summary>
        public Tag() { }

        public class TagCommands : BaseCommandModule
        {
            [Command("tag"), Description("Вызывает тег, воспроизводя его сообщение.")]
            public async Task GetTag(CommandContext ctx, string name)
            {
                if (!tags.ContainsKey(name)) 
                    await ctx.RespondAsync("Нет такого тега!".FormatAs());
                else await ctx.RespondAsync(tags.GetValueOrDefault(name).GetMessage());
            }

            [Command("tag_add"), RequireReferencedMessage(), Description("Добавляет тег. Можно использовать только как ответ на сообщение.")]
            public async Task AddTag(CommandContext ctx, params string[] names)
            {
                if (!names.All((s) => !tags.ContainsKey(s))) await ctx.RespondAsync("Тег с таким именем уже существует!".FormatAs(34));
                else
                {
                    new Tag(names, ctx.Message.ReferencedMessage);
                    await ctx.RespondAsync($"Добавлен тег {names[0]}.".FormatAs());
                }
            }

            [Command("tag_delete"), Description("Удаляет тег с данным именем.")]
            public async Task DeleteTag(CommandContext ctx, string name)
            {
                if (!tags.ContainsKey(name)) await ctx.RespondAsync("Нет такого тега!".FormatAs());
                else
                {
                    tags.GetValueOrDefault(name).Delete();
                    await ctx.RespondAsync($"Тег {name} удален!".FormatAs());
                }
            }

            [Command("tag_overwrite"), RequireReferencedMessage(), Description("Перезаписывает данный тег. Можно использовать только как ответ на сообщение.")]
            public async Task OverwriteTag(CommandContext ctx, string name)
            {
                if (!tags.ContainsKey(name)) await ctx.RespondAsync("Нет такого тега!".FormatAs());
                else
                {
                    var deleted = tags.GetValueOrDefault(name);
                    var names = deleted.Names;
                    DeleteTag(ctx, name).Wait();
                    AddTag(ctx, names).Wait();
                }
            }

            [Command("tag_list"), Description("Выводит листаемый список тэгов.")]
            public async Task ListTag(CommandContext ctx)
            {
                int num = 1;
                StringBuilder sb = new StringBuilder();
                Tag current = null;
                
                foreach (var tag in tags)
                {
                    if (current != null && current != tag.Value)
                    {
                        num++;
                        sb.Append("\n");
                    }
                    sb.Append($"{num}. {tag.Key}\n");
                    current = tag.Value;
                }
                await Program.Interactivity.SendPaginatedMessageAsync(
                    ctx.Channel,
                    ctx.User,
                    Program.Interactivity.GeneratePagesInContent(sb.ToString().FormatAs())
                    );
            }
        }
    }
}
