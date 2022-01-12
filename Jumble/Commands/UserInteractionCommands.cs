using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Jumble.Agents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TenorSharp;
using TenorSharp.Enums;

namespace Jumble.Commands
{
    public class UserInteractionCommands : BaseCommandModule
    {
        private List<string> hugMessages = new List<string>()
        {
            "Cute!",
            "<3333",
            ":3",
            ":heart:",
            ":heart_eyes:",
            ":kissing_heart:",
            ":kissing_closed_eyes:",
            ":kissing:",
            "Lucky...",
            "Now kiss!"
        };
        [Command("hug"), 
        DSharpPlus.CommandsNext.Attributes.Description("Hug someone!"), 
        Category("Action"),]
        // Usage("{prefix}hug {Mention}")]                                                       // [USES] nekos.life => hug
        public async Task Hug(CommandContext ctx, string mention) { await NekosLifeAgent.DoActionCommand(ctx, "hug", "hug", "ged", mention, hugMessages.GetRandom()); }
        [Command("kiss"), 
        DSharpPlus.CommandsNext.Attributes.Description("Kiss someone!"), 
        Category("Action"),]
        // Usage("{prefix}kiss {mention}")]                                                      // [USES] nekos.life => kiss
        public async Task Kiss(CommandContext ctx, string mention) { await NekosLifeAgent.DoActionCommand(ctx, "kiss", "kiss", "ed", mention); }
        [Command("cuddle"), 
        DSharpPlus.CommandsNext.Attributes.Description("Cuddle someone!"), 
        Category("Action"),]
        // Usage("{prefix}cuddle {Mention}")]                                                    // [USES] nekos.life => cuddle
        public async Task Cuddle(CommandContext ctx, string mention) { await NekosLifeAgent.DoActionCommand(ctx, "cuddle", "cuddle", "d", mention); }
        [Command("poke"), 
        DSharpPlus.CommandsNext.Attributes.Description("Poke someone!"), 
        Category("Action"),]
        // Usage("{prefix}poke {Mention}")]                                                      // [USES] nekos.life => poke
        public async Task Poke(CommandContext ctx, string mention) { await NekosLifeAgent.DoActionCommand(ctx, "poke", "poke", "ed", mention); }
        [Command("tickle"), 
        DSharpPlus.CommandsNext.Attributes.Description("Tickle someone!"), 
        Category("Action"),]
        // Usage("{prefix}tickle {Mention}")]                                                    // [USES] nekos.life => tickle
        public async Task Tickle(CommandContext ctx, string mention) { await NekosLifeAgent.DoActionCommand(ctx, "tickle", "tickl", "ed", mention); }
        [Command("feed"), 
        DSharpPlus.CommandsNext.Attributes.Description("Feed someone!"), 
        Category("Action"),]
        // Usage("{prefix}feed {Mention}")]                                                      // [USES] nekos.life => feed
        public async Task Feed(CommandContext ctx, string mention) { await NekosLifeAgent.DoActionCommand(ctx, "feed", "fed", "", mention); }
        [Command("poke"), 
        DSharpPlus.CommandsNext.Attributes.Description("poke!"), 
        Category("Action"),]
        // Usage("{prefix}baka")]                                                                // [USES] nekos.life => poke
        public async Task Baka(CommandContext ctx) { await NekosLifeAgent.DoSelfActionCommand(ctx, "baka", ": \"B-Baka...\""); }
    }
}