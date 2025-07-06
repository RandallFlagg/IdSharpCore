namespace IdSharp.AudioInfo
{
    /// <summary>
    /// MPEG-4
    /// </summary>
    public class Mpeg4 : IAudioFile
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Mpeg4"/> class.
        /// </summary>
        /// <param name="path">The full path of the file.</param>
        public Mpeg4(string path)
            : this(new Mpeg4Tag(path))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mpeg4"/> class.
        /// </summary>
        /// <param name="mpeg4Tag">The MPEG-4 tag object.  This constructor is more efficient if an Mpeg4Tag object is 
        /// available.  Changes made to the Mpeg4Tag object will not affect the properties in this object.</param>
        internal Mpeg4(Mpeg4Tag mpeg4Tag)
        {
            Frequency = mpeg4Tag.Frequency;
            Channels = mpeg4Tag.Channels;
            Samples = mpeg4Tag.Samples;

            if (Frequency != 0)
            {
                TotalSeconds = (decimal)Samples / Frequency;
                if (TotalSeconds != 0)
                {
                    Bitrate = mpeg4Tag.MdatAtomSize / TotalSeconds / 125.0m;
                }
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
            get { return AudioFileType.Mpeg4; }
        }

        /// <summary>
        /// Gets the number of samples.
        /// </summary>
        /// <value>The number of samples.</value>
        public long Samples { get; }
    }
}
