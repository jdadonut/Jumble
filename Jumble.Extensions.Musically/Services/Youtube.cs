using System;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace Jumble.Extensions.Musically.Services
{
    public class Youtube
    {
        private static YoutubeClient _client = new();
        public static async Task<YoutubeVideo?> GetVideoFromId(string id)
        {
            var video = await _client.Videos.GetAsync(id);
            var streams = await _client.Videos.Streams.GetManifestAsync(id);

            return new YoutubeVideo(
                video.Title,
                video.Url,
                video.Author.Title,
                GetThumbFromVideo(video),
                video.Duration ?? TimeSpan.Zero,
                id,
                new AudioStream(
                    await _client.Videos.Streams.GetAsync(streams.GetAudioStreams().MaxBy(x => x.Bitrate)))
            );
        }
        private static YoutubeImage GetThumbFromVideo(Video video)
        {
            // sort the thumbnails by resolution
            var thumbnails = video.Thumbnails.OrderByDescending(x => x.Resolution);
            // if there are less then three thumbnails, omit medium. if there's only one, set high to it. 
            var high = thumbnails.Count() > 2 ? thumbnails.ElementAt(2) : thumbnails.ElementAt(0);
            var medium = thumbnails.Count() > 1 ? thumbnails.ElementAt(1) : thumbnails.ElementAt(0);
            var low = thumbnails.ElementAt(0);
            // put into a YoutubeImage object
            return new YoutubeImage(low.Url, medium.Url, high.Url);
        }
    }

    public record YoutubeVideo(
        string Title,
        string Url,
        string Author,
        YoutubeImage Thumb,
        TimeSpan Duration,
        string Id,
        AudioStream Audio
    ) : IVideo(Title, Url, Audio);
    public record YoutubeImage(
        string? Small,
        string? Medium,
        string? Large
    );
}