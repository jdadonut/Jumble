using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using TenorSharp.Enums;

namespace Jumble.Agents
{
    public class TenorAgent
    {
        public static async Task DoActionCommand(CommandContext ctx, string key, string before_mentioned, string? after_message, string? mentioned)
        {
            ctx.Client.Logger.LogInformation("Entering DoActionCommand scope");
            after_message = after_message ?? "";
            string mentionstring;
            if (!Util.IsMention(mentioned))
                mentionstring = await Util.GetMention(ctx, $"<@{ctx.User.Id}>");
            else mentionstring = mentioned;
            ctx.Client.Logger.LogInformation("Mention confirmed");
            var res = await Program.tenor.SearchAsync("hug", 20, 0);
            var gif = res.GifResults.GetRandom();
            ctx.Client.Logger.LogInformation("Gif found");
            if (!gif.Media.Where(x => x.ContainsKey(GifFormat.gif)).First().TryGetValue(GifFormat.gif, out var uri))
            {
                ctx.RespondAsync("Gif lookup failed. Sorry!");
                return;
            }
            await ctx.RespondAsync(embed: new DiscordEmbedBuilder
            {
                Title = $"<@{ctx.Member.Id}> {before_mentioned} <@{mentionstring}>! " + after_message,
                ImageUrl = uri.Url.ToString()
            });
        }
    }
}