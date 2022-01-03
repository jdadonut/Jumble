
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Jumble.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jumble
{
    public class Program
    {
        public DiscordClient discord;
        public CommandsNextExtension commandsNext;
        public IConfigurationRoot config;
        
        
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
        }

        private IServiceProvider GetServices()
        {
            ServiceCollection coll = new ServiceCollection();
            coll.AddSingleton<Program>(this); // means we can access the commandsNext instance from the commands, among other things.
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
            // find all commands in the running assembly and register them
            commandsNext.RegisterCommands(Assembly.GetExecutingAssembly());
            discord.UseInviteTracker();
            await discord.ConnectAsync();
            while (true)
            {
                await Task.Delay(1000);
            }
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