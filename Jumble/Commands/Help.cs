using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Starciad.DSharpPlus.Interactivity.Paginated;
namespace Jumble.Commands
{
    public class Help : DefaultHelpFormatter
    {
        protected Dictionary<string, SimplePage> pages = new Dictionary<string, SimplePage>();
        protected CommandContext ctx;

        public Help(CommandContext ctx) : base(ctx)
        {
            // make a custom help command using interactivity, by allowing the user to select from a paginated list of categories.
            // this is a bit more advanced than the default help command, but it's also a lot more powerful.
            // this is also a good example of how to use the paginated embeds.
            var c = ctx.Client;
            this.ctx = ctx;
            foreach (Command cmd in this.ctx.CommandsNext.RegisteredCommands.Values)
            {
                c.Logger.LogInformation($"Helping {cmd.Name}");
                this.WithCommand(cmd);
            }

            this.Build();

        }

        public override BaseHelpFormatter WithCommand(Command cmd)
        {
            CategoryAttribute attr = Attribute.GetCustomAttribute(cmd.GetType(), typeof(CategoryAttribute)) as CategoryAttribute;
            string category = attr?.Category ?? "General";
            if (!this.pages.ContainsKey(category))
            {
                this.pages.Add(category, new SimplePage()
                {
                    TitleContent = $"{category} Commands",
                    PageMainContent = ""
                });
            }
            this.pages[category].PageMainContent += $"**{cmd.Name}** - {cmd.Description}\n";
            
            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> commands)
        {
            // foreach (var cmd in commands)
            // {
            //     if (!pages.ContainsKey(cmd.Parent.Name))
            //     {
            //         pages[cmd.Parent.Name] = new SimplePage()
            //         {
            //             TitleContent = cmd.Parent.Name,
            //             PageMainContent = $"" + cmd.Parent.Description ?? "",
            //         };
            //     }
            //     pages[cmd.Parent.Name].PageMainContent += $"\n**{cmd.Name}** - {cmd.Description}";
            // }
            return this;
        }

        public override CommandHelpMessage Build()
        {
            PagesCollection pc = new PagesCollection(pages.Values.ToArray(), "Help", "", new PagesCollectionVisualModel
            {
                TitleContent = null,
                PageMainContent = null,
                FooterContent = null,
                BeforeMainContent = null,
                AfterMainContent = null,
                PageIconUrl = null,
                PageImageUrl = null,
                PageColor = default,
                PageNumber = 0,
                EnablePagesCountInFooter = false,
                EnableDateTimeInFooter = false,
                EnableInvokeCommandAuthorInfosInFooter = false
            });
            ctx.Channel.SendMessagePaginated(ctx.User, ctx.Client, pc).Wait();
            
            return new CommandHelpMessage("Help message sent.");
        }
    }
}