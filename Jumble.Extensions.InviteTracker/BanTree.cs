using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Jumble.Extensions.InviteTracker
{
    class BanTree_c : BaseCommandModule
    {
        [Command("bantree"), Aliases("bigban"), RequirePermissions(Permissions.BanMembers),
         Description("Ban a user, and everyone they've invited, from the server.")]
        public static async Task BanTree(CommandContext ctx, DiscordMember user)
        {
            ulong uid = user.Id;
            List<ulong> invitees = InviteTrackerExtension.GetUserInvites(uid);
            List<DiscordMember> invitees_m = new List<DiscordMember>();
            // get all users invited by the user, and confirm with user
            foreach (ulong invitee in invitees)
            {
                DiscordMember inviteeMember = await ctx.Guild.GetMemberAsync(invitee);
                invitees_m.Add(inviteeMember);
            }
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithTitle("BanTree with user " + user.DisplayName);
            embed.WithDescription("Are you sure you want to ban " + user.DisplayName + " and all of their invitees?");
            embed.WithColor(DiscordColor.Red);
            embed.AddField("Invitees", string.Join("\n", invitees_m.Select(x => $"<@{x.Id}>")));
            DiscordMessage msg = await ctx.RespondAsync(embed: embed.Build());
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"));
            await msg.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));
            DiscordMessage reactionMsg = await ctx.Channel.GetMessageAsync(msg.Id);
            MessageReactionAddEventArgs margs = (await reactionMsg.WaitForReactionAsync(ctx.User)).Result;
            while (margs.Emoji.Name != ":white_check_mark:" && margs.Emoji.Name != ":x:")
            {
                margs = (await reactionMsg.WaitForReactionAsync(ctx.User)).Result;
            }

            if (margs.Emoji.Name == ":white_check_mark:")
            {
                foreach (DiscordMember invitee in invitees_m)
                {
                    DiscordMessage _msg = ctx.Channel.SendMessageAsync("Banning user " + invitee.DisplayName).Result;
                    await invitee.BanAsync();
                    _msg.ModifyAsync("Banned User " + invitee.DisplayName);
                }
                await user.BanAsync();
                await ctx.RespondAsync("Banned user " + user.DisplayName);
            }
        }
    }
}