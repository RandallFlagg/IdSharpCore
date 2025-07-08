using System;
using System.IO;

namespace IdSharp.AudioInfo;

/// <summary>
/// OptimFROG
/// </summary>
public class OptimFrog : IAudioFile
{

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimFrog"/> class.
    /// </summary>
    /// <param name="path">The full path of the file.</param>
    public OptimFrog(string path)
    {
        using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            fs.Position = 31;
            var wav = new RiffWave(fs);

            Channels = wav.Channels;
            Frequency = wav.Frequency;
            TotalSeconds = wav.TotalSeconds;
            Bitrate = (fs.Length / 125.0m) / TotalSeconds;
        }
    }

    /// <summary>
    /// Gets the bitrate.
    /// </summary>
    /// <value>The bitrate.</value>
    public decimal Bitrate { get; }

    /// <summary>
    /// Gets the total seconds.
    /// </summary>
    /// <value>The total seconds.</value>
    public decimal TotalSeconds { get; }

    /// <summary>
    /// Gets the number of channels.
    /// </summary>
    /// <value>The number of channels.</value>
    public int Channels { get; }

    /// <summary>
    /// Gets the frequency.
    /// </summary>
    /// <value>The frequency.</value>
    public int Frequency { get; }

    /// <summary>
    /// Gets the type of the audio file.
    /// </summary>
    /// <value>The type of the audio file.</value>
    public AudioFileType FileType
    {
        get { return AudioFileType.OptimFrog; }
    }
}
