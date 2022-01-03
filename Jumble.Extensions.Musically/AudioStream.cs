using System;
using System.ComponentModel;
using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;

namespace Jumble.Extensions.Musically
{
    public class AudioStream : Stream
    {
        private Stream _stream; // stream containing non-opus audio data.
        private Stream _opusStream; // stream containing opus audio data.
        private BackgroundWorker Converter = new();
        public AudioStream(Stream stream)
        {
            _stream = stream;
            Converter.DoWork += Converter_DoWork;
        }

        public void Converter_DoWork(object?  arg, DoWorkEventArgs? dwea)
        {
            // ignore arg and dwea.
            
            _opusStream = new MemoryStream();
            Codec opus;
            // opus = FFMpeg.GetCodec("libopus");
            opus = FFMpeg.GetCodec("pcm_s32le");
            if (opus == null)
            {
                throw new Exception("pcm not found");
            }

            FFMpegArguments.FromPipeInput(new StreamPipeSource(_stream))
                .OutputToPipe(new StreamPipeSink(_opusStream), options =>
                {
                    options.WithAudioCodec(opus);
                    options.UsingMultithreading(true);
                    options.DisableChannel(Channel.Video);
                    options.ForceFormat("s32le"); // 
                })
                .ProcessAsynchronously(false);

        }

        // allow reading from the opus stream.
        public Stream GetOpusStream()
        {
            return _opusStream;
        }

        public override void Flush()
        {
            throw new NotSupportedException("Opus Streams do not support Flushing");
        }

        public override int Read(byte[] buffer, int offset, int count) => _opusStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Opus Streams do not support Seeking.");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Opus Streams do not support setting length.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Opus Streams do not support being written to, as it is only a container for the conversion process.");
        }

        public override bool CanRead { get => _opusStream.CanRead; }
        public override bool CanSeek { get => false; }
        public override bool CanWrite { get => false; }
        public override long Length { get => _opusStream.Length; }
        public override long Position
        {
            get => _opusStream.Position;
            set { throw new NotSupportedException("Opus Streams do not support setting the position"); }
        }
    }
}