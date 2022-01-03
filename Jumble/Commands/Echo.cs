using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Jumble.Commands
{
    public class Echo : BaseCommandModule
    {
        [Command("echo")]
        [Description("Echoes a message.")]
        public async Task Echo_c(CommandContext ctx, [RemainingText] string message)
        {
            await ctx.RespondAsync(message);
        }
    }
}