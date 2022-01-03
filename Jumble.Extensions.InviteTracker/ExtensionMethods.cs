using System;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Jumble.Util;
namespace Jumble.Extensions
{
    public static class ExtensionMethods
    {
        public static void UseInviteTracker(this DiscordClient client)
        {
            new InviteTrackerExtension(client);
        }

    }
}