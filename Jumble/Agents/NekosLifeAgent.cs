using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Logging;
// using DSharpPlus.SlashCommands.Entities;
using Newtonsoft.Json;

namespace Jumble.Agents {
    public static class NekosLifeAgent {
        public static readonly string Endpoint = "https://nekos.life/api/v2/";
        public static readonly HttpClient EmbeddedClient = new HttpClient();
        public static async Task<string> GetNekosLifeEndpoint(CommandContext ctx, string s, bool img = true)
    {
        string url = Endpoint;
        if (img)
            url += "img/";
        url += s;
        IServiceProvider deps = ctx.Services;
        HttpClient client = deps.GetService<HttpClient>();
        HttpResponseMessage res =  await client.GetAsync(url);
        if (((int)res.StatusCode) != 200)
            return "";
        else
            return await DecodeNekosLifeApiResponse(res);
    }
        public static async Task<string> GetNekosLifeEndpoint(string s, bool img = true)
    {
        string url = Endpoint;
        if (img)
            url += "img/";
        url += s;
        HttpClient client = EmbeddedClient;
        HttpResponseMessage res =  await client.GetAsync(url);
        if (((int)res.StatusCode) != 200)
            return "";
        else
            return await DecodeNekosLifeApiResponse(res);
    }
    private static async Task<string> DecodeNekosLifeApiResponse(HttpResponseMessage res)
    {
        try
        {
            var a = ((Newtonsoft.Json.Linq.JObject)
            JsonConvert.DeserializeObject(await res.Content.ReadAsStringAsync()))
            .GetValue("url").ToString();
            return a;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Nekos.Life endpoint errored. Error: {e.Message}, Response: " + await res.Content.ReadAsStringAsync());
            return "";
        }
    }
    public static async Task DoNekosLifeCommand(CommandContext ctx, string endpoint)
    {
        await ctx.TriggerTypingAsync();
        DiscordEmbedBuilder emb = new DiscordEmbedBuilder();
        emb.WithImageUrl(await GetNekosLifeEndpoint(ctx, endpoint));
        await ctx.RespondAsync(embed: emb);
    }
    // public static async Task DoNekosLifeSlashCommand(InteractionContext ctx, string endpoint)
    // {
    //     DiscordEmbedBuilder emb = new DiscordEmbedBuilder();
    //     emb.WithImageUrl(await GetNekosLifeEndpoint(endpoint));
    //     await ctx.ReplyAsync(embeds: new DiscordEmbed[] {emb});
    // }
    public static async Task DoActionCommand(CommandContext ctx, string endpoint, string action, string append, string mention, string? ending = null)
    {
        await ctx.TriggerTypingAsync();
        if (!Util.IsMention(mention))
            mention = await Util.GetMention(ctx, mention, $"Please mention the user you want to {action}!");
        if (mention == ctx.Message.Author.Id.ToString())
            return;

        DiscordEmbedBuilder emb = new DiscordEmbedBuilder();
        emb.WithImageUrl(await GetNekosLifeEndpoint(ctx, endpoint))
            .WithDescription($"<@!{ctx.Message.Author.Id}> {action}{append} {mention}! {"" + (ending ?? "")}");

        await ctx.RespondAsync(embed: emb);

    }
    public static async Task DoSelfActionCommand(CommandContext ctx, string endpoint, string message)
    {
        await ctx.TriggerTypingAsync();
        DiscordEmbedBuilder emb = new DiscordEmbedBuilder();
        emb.WithImageUrl(await GetNekosLifeEndpoint(ctx, endpoint))
            .WithDescription($"<@!{ctx.Message.Author.Id}> {message}!");
        await ctx.RespondAsync(embed: emb);

    }
    }
}