namespace Jumble.Extensions.Musically
{
    public record IVideo(
        string Title,
        string Url,
        AudioStream Audio
        );
}