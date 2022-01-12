using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Jumble
{
    public static class Util
    {
        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            return list.ElementAt(Random.Shared.Next(0, list.Count()));
        }
        public static bool IsMention(string mention) { return new Regex("(<@|<@!)[0-9]{1,30}>").IsMatch(mention); }
        public static async Task<string> GetMention(CommandContext ctx, string mention, string message = "Please send a valid mention.")
        {
            await ctx.RespondAsync($"{mention} isn't a valid tag! {message}");
            InteractivityResult<DiscordMessage> mes = await GetMessageWithMention(ctx);
            if (mes.Result.Content.ToLower() == "cancel" || mes.TimedOut)
            {
                await ctx.RespondAsync("Command Cancelled.");
                return ctx.Message.Author.Id.ToString();
            }
            else
            {
                mention = mes.Result.Content;
                if (!IsMention(mention))
                {
                    return await GetMention(ctx, mention, message);
                }
                else
                    return mention;
            }
        }
        private static async Task<InteractivityResult<DiscordMessage>> GetMessageWithMention(CommandContext ctx)
        {
            return await ctx.Client.GetInteractivity().WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
        }
    }
}