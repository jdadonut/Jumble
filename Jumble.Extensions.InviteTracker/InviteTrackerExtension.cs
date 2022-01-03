



using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Jumble.Extensions.InviteTracker;
using Jumble.Util;

namespace Jumble.Extensions
{

    public class InviteTrackerExtension : BaseExtension
    {
        /*
         * db should be in following format:
         * invite_code, creator_id, guild_id, uses, users (comma separated user ids)
         */
        public static SqliteConnection db = new SqliteConnection("Data Source=invite.db");
        private SqliteTransaction transaction;

        public static List<ulong> GetUserInvites(ulong creator_id)
        {
            SqliteDataReader reader = new SqliteCommand($"SELECT users FROM invites WHERE creator_id = {creator_id}", db).ExecuteReader();
            List<ulong> users = new List<ulong>();
            while (reader.Read())
            {
                string[] user_ids = reader.GetString(0).Split(',');
                foreach (string user_id in user_ids)
                {
                    users.Add(Convert.ToUInt64(user_id));
                }
            }
            return users;
        }

        public static async Task InviteUsed(DiscordClient _, GuildMemberAddEventArgs e)
        {
            // find the invite code used by finding which one has more uses then what is in the database, then fix the count
            // that's in the database and add the new user to the list of users
            SqliteDataReader reader = new SqliteCommand($"SELECT invite_code, uses, users FROM invites WHERE guild_id = {e.Guild.Id}", db).ExecuteReader();
            string invite_code = "";
            int uses = 0;
            string users = "";
            while (reader.Read())
            {
                string code = reader.GetString(0);
                int count = reader.GetInt32(1);
                string user_ids = reader.GetString(2);
                if (count < e.Guild.GetInvite(code).Uses)
                {
                    invite_code = code;
                    uses = e.Guild.GetInvite(code).Uses;
                    users = user_ids;
                }
            }
            if (invite_code != "")
            {
                // update the database
                var _transaction = db.BeginTransaction();
                new SqliteCommand($"UPDATE invites SET uses = {uses}, users = '{users},{e.Member.Id}' WHERE invite_code = '{invite_code}'", db, _transaction).ExecuteNonQuery();
                _transaction.Commit();
            }
        }

        public static async Task GuildAvailable(DiscordClient _, GuildCreateEventArgs a)
        {
            // cache all invites in the server
            foreach (DiscordInvite invite in await a.Guild.GetInvitesAsync())
            {
                await AddInvite(invite);
            }
        }

        public static async Task AddInvite(DiscordInvite invite)
        {
            // add the invite to the database
            var _transaction = db.BeginTransaction();
            new SqliteCommand($"INSERT INTO invites (invite_code, creator_id, guild_id, uses, users) VALUES ('{invite.Code}', {invite.Inviter.Id}, {invite.Guild.Id}, {invite.Uses}, '{invite.Inviter.Id}')", db, _transaction).ExecuteNonQuery();
            _transaction.Commit();
        }

        private void InitializeDatabase()
        {
            // make db follow the schema
            
            var _transaction = db.BeginTransaction();
            new SqliteCommand("CREATE TABLE IF NOT EXISTS invites (invite_code TEXT, creator_id INTEGER, guild_id INTEGER, uses INTEGER, users TEXT)", db, _transaction).ExecuteNonQuery();
            _transaction.Commit();
        }

        public InviteTrackerExtension(DiscordClient client) : base()
        {
            
            if (db.State != ConnectionState.Open)
            {
                foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                {
                    if (file.Contains("invite.db"))
                    {
                        File.Delete(file);
                        break;
                    }
                }
                db.Open();
                InitializeDatabase();
            }
            transaction = db.BeginTransaction();
            // get initial invite counts
            client.GuildAvailable += GuildAvailable;
            client.AddExtension(this);

        }
        protected override void Setup(DiscordClient client)
        {
            client.GuildMemberAdded += InviteUsed;
            client.GetCommandsNext().RegisterCommands(typeof(BanTree_c));
        }

        public void Backup()
        {
            db.BackupDatabase(new SqliteConnection(GetBackupConnectionString()));
            transaction.Commit();
            db.BackupDatabase(new SqliteConnection(GetBackupConnectionString(true)));
            transaction = db.BeginTransaction();
        }
        public string GetBackupConnectionString(bool isCommitted = false)
        {
            // used for backups, includes time and whether or not it was a committed backup
            return $"Data Source=invite.db.{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.{(isCommitted ? "committed" : "uncommitted")}.invites.db";
        }
        
        
    }
}