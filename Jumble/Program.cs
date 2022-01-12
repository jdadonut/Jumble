
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Jumble.Commands;
using Jumble.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TenorSharp;

namespace Jumble
{
    public class Program
    {
        public static DiscordClient discord;
        public static CommandsNextExtension commandsNext;
        public static IConfigurationRoot config;
        public static TenorClient tenor;
        
        public static void Main(string[] args)
        {
            new Program(args).RunAsync().GetAwaiter().GetResult();
        }

        public Program(string[] args)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
            tenor = new TenorClient(config.GetSection("tenor").GetSection("key").Value);
        }

        private IServiceProvider GetServices()
        {
            ServiceCollection coll = new ServiceCollection();
            coll.AddSingleton<Program>(this); // means we can access the commandsNext instance from the commands, among other things.
            coll.AddSingleton<IConfigurationRoot>(config);
            coll.AddSingleton<DiscordClient>(discord);
            coll.AddSingleton<TenorClient>(tenor);
            coll.AddSingleton<HttpClient>(new HttpClient());
            return coll.BuildServiceProvider();
        }

        public async Task RunAsync()
        {
            discord = new DiscordClient(new DiscordConfiguration()
            {
                AlwaysCacheMembers = true,
                AutoReconnect = true,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                Intents = DiscordIntents.AllUnprivileged,
                LoggerFactory = new LFactory(),
                TokenType = TokenType.Bot,
                Token = config.GetSection("discord").GetSection("token").Value
            });
            commandsNext = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new List<string>() { config.GetSection("discord").GetSection("prefix").Value },
                EnableDms = false,
                EnableMentionPrefix = true,
                EnableDefaultHelp = true,
                CaseSensitive = false,
                IgnoreExtraArguments = true,
                Services = GetServices()
                // PrefixResolver = PrefixResolver TO DO: not made yet
            });
            discord.UseInteractivity();
            // find all commands in the running assembly and register them
            LoadCommands();
            commandsNext.SetHelpFormatter<Help>();
            // discord.UseInviteTracker();
            discord.Ready += async (e, a) =>
            {
                discord.Logger.LogInformation("Connected as {0}", discord.CurrentUser.Username + "#" + discord.CurrentUser.Discriminator);
            };
            await discord.ConnectAsync();
            while (true)
            {
                await Task.Delay(1000);
            }
        }
        private void LoadCommands()
        {
            var type = typeof(BaseCommandModule);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract && !p.Namespace.StartsWith("DSharpPlus") && !p.IsNested);
            var typeList = types as Type[] ?? types.ToArray();
            foreach (var t in typeList)
            {
                commandsNext.RegisterCommands(t);
                discord.Logger.LogInformation($"Module {t.Namespace}.{t.Name} Loaded...");

            }
            discord.Logger.LogInformation($"Loaded {typeList.Count()} modules.");
        }

        ~Program()
        {
            discord.Dispose();
            // write config to config.json
            commandsNext.Dispose();
        }
        public static PrefixResolverDelegate PrefixResolver = (DiscordMessage msg) =>
        {
            return Task.Run<int>(() => -1);
        };
    }
}


