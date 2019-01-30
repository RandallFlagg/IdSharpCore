using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using IdSharp.Common.Utils;
using IdSharp.Tagging.ID3v2.Frames;


namespace IdSharp.Tagging.ID3v2.Frames
{
    // TODO: Unseal AttachedPicture for this extension to work.
    /*
    public class AttachedPictureWithImage : AttachedPicture, IAttachedPictureWithImage
    {
        private bool _loadingPicture;
        private bool _pictureCached;
        private Image _picture;

        public AttachedPictureWithImage()
        {
            this.PropertyChanged += (string name) =>
            {
                if (name = nameof(IAttachedPicture.PictureData))
                {
                    if (this.PictureData != null)
                    {
                        _picture = this.LoadPicture(ref _loadingPicture);
                        _pictureCached = true;
                    }
                }
            };
        }

        public Image Picture
        {
            get
            {
                if (_pictureCached == false)
                {
                    _picture = this.LoadPicture(ref _loadingPicture);
                    _pictureCached = true;
                }

                return (_picture == null ? null : (Image)_picture.Clone());
            }
            set
            {
                if (_picture != value)
                {
                    if (_picture != null)
                    {
                        _picture.Dispose();
                    }

                    _picture = value;

                    if (value == null)
                    {
                        this.PictureData = null;
                    }
                    else
                    {
                        this.SavePicture(value);
                    }
                }

                RaisePropertyChanged("Picture");
            }
        }

        public string PictureExtension
        {
            get
            {
                if (_picture == null)
                    return null;

                if (_picture.RawFormat.Equals(ImageFormat.Bmp))
                    return "bmp";
                else if (_picture.RawFormat.Equals(ImageFormat.Emf))
                    return "emf";
                else if (_picture.RawFormat.Equals(ImageFormat.Exif))
                    return null;  // TODO - Unsure of MIME type?
                else if (_picture.RawFormat.Equals(ImageFormat.Gif))
                    return "gif";
                else if (_picture.RawFormat.Equals(ImageFormat.Icon))
                    return "ico";
                else if (_picture.RawFormat.Equals(ImageFormat.Jpeg))
                    return "jpg";
                else if (_picture.RawFormat.Equals(ImageFormat.MemoryBmp))
                    return "bmp";
                else if (_picture.RawFormat.Equals(ImageFormat.Png))
                    return "png";
                else if (_picture.RawFormat.Equals(ImageFormat.Tiff))
                    return "tif";
                else if (_picture.RawFormat.Equals(ImageFormat.Wmf))
                    return "wmf";
                else
                    return "";
            }
        }
        */
    }
}
