using System.IO;
using IdSharp.Common.Utils;

namespace IdSharp.AudioInfo;

/// <summary>
/// Musepack
/// </summary>
public class Musepack : IAudioFile
{
    private const uint STREAM_VERSION_70_ID = 0x072B504D;
    private const uint STREAM_VERSION_71_ID = 0x172B504D;
    private static readonly int[] _sampleRates = new[] { 44100, 48000, 37800, 32000 };

    /// <summary>
    /// Initializes a new instance of the <see cref="Musepack"/> class.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    public Musepack(string path)
    {
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            // Skip ID3v2 tag
            int tmpID3v2TagSize = ID3v2.GetTagSize(stream);
            int tmpID3v1TagSize = ID3v1.GetTagSize(stream);
            int tmpAPEv2TagSize = APEv2.GetTagSize(stream);
            stream.Seek(tmpID3v2TagSize, SeekOrigin.Begin);

            StreamVersion = 0;

            byte[] byteArray = stream.Read(32);
            int[] integerArray = new int[8];
            for (int i = 0; i < 8; i++)
            {
                integerArray[i] = (byteArray[i * 4]) +
                                                      (byteArray[i * 4 + 1] << 8) +
                                                      (byteArray[i * 4 + 2] << 16) +
                                                      (byteArray[i * 4 + 3] << 24);
            }

            // Size TODO - ignore Lyrics3
            long audioDataLength = stream.Length - tmpID3v2TagSize - tmpID3v1TagSize - tmpAPEv2TagSize;

            // Stream version
            if (integerArray[0] == STREAM_VERSION_70_ID)
            {
                StreamVersion = 7;
            }
            else if (integerArray[0] == STREAM_VERSION_71_ID)
            {
                StreamVersion = 7.1m;
            }
            else
            {
                switch ((byteArray[1] % 32) / 2)
                {
                    case 3: 
                        StreamVersion = 4; 
                        break;
                    case 7: 
                        StreamVersion = 5; 
                        break;
                    case 11: 
                        StreamVersion = 6; 
                        break;
                }
            }

            if (StreamVersion == 0)
            {
                throw new InvalidDataException("Unrecognized MPC stream");
            }

            // Sample rate
            Frequency = _sampleRates[byteArray[10] & 0x03];

            // Channels
            if (StreamVersion == 7 || StreamVersion == 7.1m)
            {
                if ((byteArray[11] % 128) < 64)
                {
                    Mode = "Stereo";
                }
                else
                {
                    Mode = "Joint Stereo";
                }
            }
            else
            {
                if ((byteArray[2] % 128) == 0)
                {
                    Mode = "Stereo";
                }
                else
                {
                    Mode = "Joint Stereo";
                }
            }

            // Frames
            if (StreamVersion == 4)
            {
                Frames = integerArray[1] >> 16;
            }
            else
            {
                Frames = integerArray[1];
            }

            TotalSeconds = Frames * 1152 / (decimal)Frequency;
            Bitrate = (audioDataLength / TotalSeconds) / 125.0m;
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
    /// Gets the stream version.
    /// </summary>
    /// <value>The stream version.</value>
    public decimal StreamVersion { get; }

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
    /// Gets the mode.
    /// </summary>
    /// <value>The mode.</value>
    public string Mode { get; }

    /// <summary>
    /// Gets the number of channels.
    /// </summary>
    /// <value>The number of channels.</value>
    public int Channels { get; } = 2;

    /// <summary>
    /// Gets the type of the audio file.
    /// </summary>
    /// <value>The type of the audio file.</value>
    public AudioFileType FileType
    {
        get { return AudioFileType.Musepack; }
    }
}
