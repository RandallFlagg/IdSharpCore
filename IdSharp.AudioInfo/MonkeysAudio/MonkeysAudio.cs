using System;
using System.IO;
using System.Text;
using IdSharp.Common.Utils;

namespace IdSharp.AudioInfo;

/// <summary>
/// Monkey's Audio
/// </summary>
public class MonkeysAudio : IAudioFile
{
    private const int COMPRESSION_LEVEL_EXTRA_HIGH = 4000;
    private static readonly byte[] MAC_IDENTIFIER = Encoding.ASCII.GetBytes("MAC ");

    /// <summary>
    /// Initializes a new instance of the <see cref="MonkeysAudio"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public MonkeysAudio(string path)
    {
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            // Skip ID3v2 tag
            int tmpID3v2TagSize = ID3v2.GetTagSize(stream);
            int tmpID3v1TagSize = ID3v1.GetTagSize(stream);
            int tmpAPEv2TagSize = APEv2.GetTagSize(stream);
            stream.Seek(tmpID3v2TagSize, SeekOrigin.Begin);

            byte[] identifier = stream.Read(4);
            if (ByteUtils.Compare(identifier, MAC_IDENTIFIER, 4) == false)
            {
                throw new InvalidDataException("Invalid Monkey's Audio file");
            }

            byte[] buf = stream.Read(4);

            Version = buf[0] + (buf[1] << 8);
            int blocksPerFrame;
            int finalBlocks;

            if (Version >= 3980 && Version <= 3990)
            {
                buf = stream.Read(4);
                int descriptorLength = buf[0] + (buf[1] << 8) + (buf[2] << 16) + (buf[3] << 24);
                stream.Seek(descriptorLength - 12, SeekOrigin.Current); // skip DESCRIPTOR

                buf = stream.Read(4);
                CompressionLevel = buf[0] + (buf[1] << 8);

                blocksPerFrame = stream.ReadInt32LittleEndian();
                finalBlocks = stream.ReadInt32LittleEndian();
                Frames = stream.ReadInt32LittleEndian();

                buf = stream.Read(4);
                // skip bits per sample
                Channels = buf[2] + (buf[3] << 8);

                Frequency = stream.ReadInt32LittleEndian();
            }
            else if (Version <= 3970)
            {
                // TODO: This section needs work

                CompressionLevel = buf[2] + (buf[3] << 8);

                buf = stream.Read(24);

                // skip format flags
                Channels = buf[2] + (buf[3] << 8);

                Frequency = buf[4] + (buf[5] << 8) + (buf[6] << 16) + (buf[7] << 32);

                if (Version >= 3950)
                {
                    blocksPerFrame = 73728 * 4;
                }
                else if (Version >= 3900 || (Version >= 3800 && CompressionLevel == COMPRESSION_LEVEL_EXTRA_HIGH))
                {
                    blocksPerFrame = 73728;
                }
                else
                {
                    blocksPerFrame = 9216;
                }

                // TODO: This is definitely fucked up
                finalBlocks = buf[0] + (buf[1] << 8) + (buf[2] << 16) + (buf[3] << 24);
                Frames = buf[0] + (buf[1] << 8) + (buf[2] << 16) + (buf[3] << 24);
            }
            else
            {
                throw new NotImplementedException($"MAC {Version / 1000.0:0.00} not supported");
            }

            long totalBlocks = ((Frames - 1) * blocksPerFrame) + finalBlocks;

            long totalSize = stream.Length - stream.Position - tmpAPEv2TagSize - tmpID3v1TagSize;

            TotalSeconds = totalBlocks / (decimal)Frequency;
            Bitrate = totalSize / (TotalSeconds * 125.0m);
        }
    }

    /// <summary>
    /// Gets the frequency.
    /// </summary>
    /// <value>The frequency.</value>
    public int Frequency { get; }

    /// <summary>
    /// Gets the frame count.
    /// </summary>
    /// <value>The frame count.</value>
    public int Frames { get; }

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
        get { return AudioFileType.MonkeysAudio; }
    }

    /// <summary>
    /// Gets the compression level.
    /// </summary>
    /// <value>The compression level.</value>
    public int CompressionLevel { get; }

    /// <summary>
    /// Gets the version.
    /// </summary>
    /// <value>The version.</value>
    public int Version { get; }
}
