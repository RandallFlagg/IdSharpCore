using System;
using System.IO;
using System.Text;
using IdSharp.Common.Utils;

namespace IdSharp.AudioInfo;

/// <summary>
/// Ogg-Vorbis
/// </summary>
public class OggVorbis : IAudioFile
{
    // References:
    // http://xiph.org/vorbis/doc/oggstream.html
    // http://xiph.org/vorbis/doc/Vorbis_I_spec.html (see section 4.2)

    private static readonly byte[] OGG_MARKER = Encoding.ASCII.GetBytes("OggS");
    private static readonly byte[] VORBIS_MARKER = Encoding.ASCII.GetBytes("vorbis");

    /// <summary>
    /// Initializes a new instance of the <see cref="OggVorbis"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public OggVorbis(string path)
    {
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            try
            {
                var tmpID3v2Size = ID3v2.GetTagSize(stream);
                stream.Seek(tmpID3v2Size, SeekOrigin.Begin);

                var oggMarker = stream.Read(4);
                if (ByteUtils.Compare(oggMarker, OGG_MARKER) == false)
                {
                    throw new InvalidDataException("OggS marker not found");
                }

                // Skip through Ogg page header to page_segments position
                stream.Seek(22, SeekOrigin.Current);

                // Skip segment_table
                int pageSegments = stream.Read1();
                stream.Seek(pageSegments, SeekOrigin.Current);

                // Read vorbis header
                int packetType = stream.Read1();
                if (packetType != 0x01)
                {
                    throw new InvalidDataException("Vorbis identification header not found");
                }

                var vorbisMarker = stream.Read(6);
                if (ByteUtils.Compare(vorbisMarker, VORBIS_MARKER) == false)
                {
                    throw new InvalidDataException("Vorbis marker not found");
                }

                // Skip vorbis_version
                stream.Seek(4, SeekOrigin.Current);

                Channels = stream.Read1();
                Frequency = stream.ReadInt32LittleEndian();

                var buf = new byte[251];
                var size = stream.Length;

                // Get total number of samples
                Samples = 0;
                for (var index = 1; index <= 50 && Samples == 0; index++)
                {
                    var dataIndex = size - ((251 - 10) * index) - 10;
                    stream.Seek(dataIndex, SeekOrigin.Begin);
                    stream.Read(buf, 0, 251);

                    // Get number of PCM samples from last Ogg packet header
                    for (var i = 251 - 10; i >= 0; i--)
                    {
                        var headerFound = true;
                        for (var j = 0; j < 4; j++)
                        {
                            if (buf[i + j] != OGG_MARKER[j])
                            {
                                headerFound = false;
                                break;
                            }
                        }

                        if (headerFound)
                        {
                            stream.Seek(dataIndex + i + 6, SeekOrigin.Begin);
                            stream.Read(buf, 0, 8);
                            for (i = 0; i < 8; i++)
                            {
                                Samples += buf[i] << (8 * i);
                            }

                            break;
                        }
                    }
                }

                if (Samples == 0)
                {
                    throw new InvalidDataException("Could not position to last frame");
                }

                TotalSeconds = Samples / (decimal)Frequency;
                Bitrate = (size - tmpID3v2Size) / TotalSeconds / 125.0m;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid Ogg-Vorbis file; stream may be corrupt", ex);
            }
        }
    }

    internal static bool IsOggVorbis(Stream stream)
    {
        // Read Ogg marker
        var oggMarker = new byte[4];
        stream.Read(oggMarker, 0, 4);
        stream.Seek(-4, SeekOrigin.Current);
        return ByteUtils.Compare(oggMarker, OGG_MARKER);
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
        get { return AudioFileType.OggVorbis; }
    }

    /// <summary>
    /// Gets the number of samples.
    /// </summary>
    /// <value>The number of samples.</value>
    public long Samples { get; }
}
