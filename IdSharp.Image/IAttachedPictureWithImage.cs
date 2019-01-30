using System.Drawing;

namespace IdSharp.Tagging.ID3v2.Frames
{
    public interface IAttachedPictureWithImage : IAttachedPicture
    {
        /// <summary>
        /// Gets or sets the picture.
        /// </summary>
        /// <value>The picture.</value>
        Image Picture { get; set; }

        /// <summary>
        /// Gets the picture extension.
        /// </summary>
        /// <value>The picture extension.</value>
        string PictureExtension { get; }
    }
}