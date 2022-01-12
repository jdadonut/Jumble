

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Data.Sqlite;

namespace Jumble.Extensions.RaidProtection
{
    public class RaidProtectionExtension : BaseExtension
    {
        // guildId: (channelId, List<userId>)
        private readonly Dictionary<ulong, (int, List<ulong>)> _guilds = new Dictionary<ulong, (int, List<ulong>)>();
        RaidProtectionExtensionConfig Config;
        SqliteConnection DbConnection;
        public RaidProtectionExtension(RaidProtectionExtensionConfig config)
        {
            this.Config = config;
            if (config.SqliteDatabasePath == null)
                DbConnection = new SqliteConnection("Data Source=raid_protection_serversettings.db");
            if (config.UseServerSettings)
            {
                // initialize database with server settings format
                this.DbConnection = new SqliteConnection("Data Source=" + config.SqliteDatabasePath);
                // create table if it doesn't exist
                this.DbConnection.Open();
                // Table should follow the following format
                // ulong guild_id PRIMARY KEY, int max_members, int members_time_seconds, int max_pings, int max_recurring_message
                string sql = "CREATE TABLE IF NOT EXISTS raid_protection_settings (guild_id BIGINT PRIMARY KEY, max_members INTEGER, members_time_seconds INTEGER, max_pings INTEGER, max_recurring_message INTEGER, logging_channel_id BIGINT)";


            }
        }
        
        protected override void Setup(DiscordClient client)
        {
            // 
        }

        private async Task OnGuildMemberAdd(GuildMemberAddEventArgs e)
        {
            (int, List<ulong>) settings = _guilds.GetValueOrDefault(e.Guild.Id);
            settings.Item1++;
            settings.Item2.Add(e.Member.Id);
            _guilds[e.Guild.Id] = settings;
            Utils.SetTimeout(delegate {
                lock (_guilds)
                {
                    var gs = _guilds[e.Guild.Id];
                    gs.Item1--;
                    gs.Item2.Remove(e.Member.Id);
                    _guilds[e.Guild.Id] = gs;
                }}, GetSetting(e.Guild.Id, Settings.MEMBER_TIME_SECONDS) * 1000);
        }
        private int GetSetting(ulong guildId, Settings setting)
        {
            if (Config.UseServerSettings)
            {
                // get settings from database
                SqliteCommand command = new SqliteCommand("SELECT * FROM raid_protection_settings WHERE guild_id = @guildId", DbConnection);
                command.Parameters.AddWithValue("@guildId", guildId);
                SqliteDataReader reader = command.ExecuteReader();
                // switch on setting
                switch (setting)
                {
                    case Settings.MAX_MEMBERS:
                        if (reader.Read())
                            return reader.GetInt32(1);
                        else
                            return 1;
                    case Settings.MEMBER_TIME_SECONDS:
                        if (reader.Read())
                            return reader.GetInt32(2);
                        else
                            return 1;
                    case Settings.MAX_PINGS:
                        if (reader.Read())
                            return reader.GetInt32(3);
                        else
                            return 1;
                    case Settings.MAX_RECURRING_MESSAGE:
                        if (reader.Read())
                            return reader.GetInt32(4);
                        else
                            return 1;
                    case Settings.LOGGING_CHANNEL_ID:
                        if (reader.Read())
                            return reader.GetInt32(5);
                        else
                            return 1;
                    default:
                        return 0;
                }
            }
            throw new NotImplementedException("Not using server settings is unsupported");
        }
    }

    enum Settings
    {
        MAX_MEMBERS = 1,
        MEMBER_TIME_SECONDS = 2,
        MAX_PINGS = 3,
        MAX_RECURRING_MESSAGE = 4,
        LOGGING_CHANNEL_ID = 5
    }
}