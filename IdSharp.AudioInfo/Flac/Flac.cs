using System;
using System.IO;
using System.Text;
using IdSharp.Common.Utils;

namespace IdSharp.AudioInfo;

/// <summary>
/// FLAC
/// </summary>
public class Flac : IAudioFile
{
    // References:
    // http://flac.sourceforge.net/format.html

    internal static readonly byte[] FLAC_MARKER = Encoding.ASCII.GetBytes("fLaC");

    /// <summary>
    /// Initializes a new instance of the <see cref="Flac"/> class.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    public Flac(string path)
    {
        try
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Skip ID3v2 tag
                int tmpID3v2TagSize = ID3v2.GetTagSize(stream);
                int tmpID3v1TagSize = ID3v1.GetTagSize(stream);
                int tmpAPEv2TagSize = APEv2.GetTagSize(stream);
                stream.Seek(tmpID3v2TagSize, SeekOrigin.Begin);

                // Read flac marker
                byte[] flacMarker = new Byte[4];
                stream.Read(flacMarker, 0, 4);
                if (ByteUtils.Compare(flacMarker, FLAC_MARKER) == false)
                {
                    throw new InvalidDataException("No header found");
                }

                // skip frame header and stuff we're not interested in
                stream.Seek(14, SeekOrigin.Current);

                byte[] buf = stream.Read(8);

                Frequency = (buf[0] << 12) + (buf[1] << 4) + (buf[2] >> 4);
                Channels = ((buf[2] >> 1) & 0x03) + 1;
                Samples = ((buf[3] & 0x0F) << 32) + (buf[4] << 24) +
                            (buf[5] << 16) + (buf[6] << 8) + buf[7];

                TotalSeconds = Samples / (decimal)Frequency;

                // Find first sync
                // TODO: There's probably a better way to do this.. also an embedded PICTURE might
                // cause a false sync. Not high priority since it will only cause a slight error
                // in bitrate calculation if a false sync is found.
                int c = stream.ReadByte(); // keep as ReadByte
                while (c != -1)
                {
                    if (c == 0xFF)
                    {
                        c = stream.ReadByte(); // keep as ReadByte
                        if (c >= 0xF8 && c <= 0xFB)
                        {
                            break;
                        }
                    }
                    else
                    {
                        c = stream.ReadByte(); // keep as ReadByte
                    }
                }

                if (c == -1)
                {
                    throw new InvalidDataException("No sync found");
                }

                long startaudio = stream.Position;
                long totalsize = stream.Length - startaudio - tmpAPEv2TagSize - tmpID3v1TagSize;
                Bitrate = totalsize / (TotalSeconds * 125);
            }
        }
        catch (InvalidDataException ex)
        {
            throw new InvalidDataException($"Cannot read FLAC file '{path}'", ex);
        }
    }

    /// <summary>
    /// Gets the frequency.
    /// </summary>
    /// <value>The frequency.</value>
    public int Frequency { get; }

    /// <summary>
    /// Gets the total seconds.
    /// </summary>
    /// <value>The total seconds.</value>
    public decimal TotalSeconds { get; }

    /// <summary>
    /// Gets the bitrate.
    /// </summary>
    /// <value>The bitrate.</value>
    public decimal Bitrate { get; }

    /// <summary>
    /// Gets the number of channels.
    /// </summary>
    /// <value>The number of channels.</value>
    public int Channels { get; }

    /// <summary>
    /// Gets the type of the audio file.
    /// </summary>
    /// <value>The type of the audio file.</value>
    public AudioFileType FileType
    {
        get { return AudioFileType.Flac; }
    }

    /// <summary>
    /// Gets the number of samples.
    /// </summary>
    /// <value>The number of samples.</value>
    public long Samples { get; }
}
