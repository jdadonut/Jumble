using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;
using Microsoft.Data.Sqlite;

namespace Jumble.Extensions.Musically
{
    
    public class AudioQueue
    {
        Dictionary<ulong, List<IVideo>> Queues = new();
        Dictionary<ulong, AudioController> Controllers = new();
        public SqliteConnection ConfigDb;
        
        public AudioQueue()
        {
            ConfigDb = new("Data Source=musically.db");
            ConfigDb.Open();
            new SqliteCommand("CREATE TABLE IF NOT EXISTS Queues (GuildId TEXT, Volume INTEGER)", ConfigDb).ExecuteNonQuery();
        }
        
        public void AddToQueue(ulong guildId, ulong? channelId, DiscordClient? client, IVideo video)
        {
            // if guild not exists in db, create it with a volume of 1.00 (100%)
            if (!Queues.ContainsKey(guildId))
            {
                if (channelId == null || client == null)
                {
                    
                }
                Queues.Add(guildId, new List<IVideo>());
                Controllers.Add(guildId, new AudioController());
                new SqliteCommand($"INSERT INTO Queues (GuildId, Volume) VALUES ({guildId}, 1.00)", ConfigDb).Parameters.AddWithValue("@guildId", guildId);
            }
            
            if (!Queues.ContainsKey(guildId))
            {
                Queues.Add(guildId, new List<IVideo>());
            }
            Queues[guildId].Add(video);
        }
        public void RemoveFromQueueAt(ulong guildId, int index)
        {
            if (!Queues.ContainsKey(guildId))
            {
                return;
            }
            Queues[guildId].RemoveAt(index);
        }
        public void ClearQueue(ulong guildId)
        {
            if (!Queues.ContainsKey(guildId))
            {
                return;
            }
            Queues[guildId].Clear();
        }

        public class AudioController
        {
            private ulong guildId, channelId;
            private AudioQueue Parent;
            private DiscordClient Client;
            private VoiceTransmitSink Sink;
            private VoiceNextConnection Connection;
            
            public AudioController(ulong guildId, ulong channelId, AudioQueue parent, DiscordClient client)
            {
                this.channelId = channelId;
                this.guildId = guildId;
                this.Parent = parent;
                this.Client = client;
            }

            public async Task<bool> TryStartPlaying()
            {
                if (Parent.Queues[this.guildId].Count == 0)
                {
                    return false;
                }

                if (Sink == null || Connection == null)
                {
                    // Connect to the voice channel
                    var vnc = Client.GetVoiceNext();
                    if ((Connection = await vnc.ConnectAsync(await Client.GetChannelAsync(channelId))) != null)
                    {
                        // connection successful, get sink
                        Sink = Connection.GetTransmitSink();
                    }
                }

                PlayNext();
                return true;
            }

            public void PlayNext()
            {                    
                Sink.ResumeAsync();

                var video = Parent.Queues[this.guildId][0];
                Parent.Queues[this.guildId].RemoveAt(0);
                int totalRead = 0;
                var buffer = new byte[4096];
                while (video.Audio.Length - totalRead > 0)
                {
                    // read from the stream then push it to the sink
                    
                    var read = video.Audio.Read(buffer, 0, 4096);
                    Sink.WriteAsync(buffer, 0, read);
                    totalRead += read;
                }
                // when the stream is done, run TryPlay again, as it will check all needed variables
                
                Connection.WaitForPlaybackFinishAsync().GetAwaiter().OnCompleted(() => Task.Run(async () => await TryStartPlaying()));
                
            }

            public void Pause()
            {
                Sink.Pause();
            }
            
            
        }
    }
}