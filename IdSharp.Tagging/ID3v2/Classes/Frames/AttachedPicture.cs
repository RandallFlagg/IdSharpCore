using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using IdSharp.Common.Utils;
using IdSharp.Tagging.ID3v2.Extensions;

namespace IdSharp.Tagging.ID3v2.Frames
{
    internal sealed class AttachedPicture : Frame, IAttachedPicture
    {
        private EncodingType _textEncoding;
        private string _mimeType;
        private PictureType _pictureType;
        private string _description;
        private byte[] _pictureData;

        public AttachedPicture()
        {
            _pictureType = PictureType.CoverFront;
        }

        public EncodingType TextEncoding
        {
            get { return _textEncoding; }
            set
            {
                if (_textEncoding != value)
                {
                    // TODO: From what I can tell, it looks like iTunes can't handle a picture with a Unicode encoded description. 
                    // You add an ID3PictureFrame with the TextEncoding set to Unicode and iTunes doesn't recognize the picture. 
                    // Interestingly, Windows Media Player does recognize it. If the ID3PictureFrame has a TextEncoding of ISO-8859-1, 
                    // iTunes will recognize it.

                    _textEncoding = value;
                    RaisePropertyChanged("TextEncoding");
                }
            }
        }

        public string MimeType
        {
            get { return _mimeType; }
            set
            {
                if (_mimeType != value)
                {
                    _mimeType = value;
                    RaisePropertyChanged("MimeType");
                }
            }
        }

        public PictureType PictureType
        {
            get { return _pictureType; }
            set
            {
                // TODO: Validate enum
                // TODO: Validate uniqueness for 0x01 and 0x02
                if (_pictureType != value)
                {
                    _pictureType = value;
                    RaisePropertyChanged("PictureType");
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public byte[] PictureData
        {
            get { return ByteUtils.Clone(_pictureData); }
            set
            {
                if (!ByteUtils.Compare(_pictureData, value))
                {
                    _pictureData = ByteUtils.Clone(value);

                    RaisePropertyChanged("PictureData");
                }
            }
        }



        public override string GetFrameID(ID3v2TagVersion tagVersion)
        {
            switch (tagVersion)
            {
                case ID3v2TagVersion.ID3v24:
                case ID3v2TagVersion.ID3v23:
                    return "APIC";
                case ID3v2TagVersion.ID3v22:
                    return "PIC";
                default:
                    throw new ArgumentException("Unknown tag version");
            }
        }

        public override void Read(TagReadingInfo tagReadingInfo, Stream stream)
        {
            // Read header
            _frameHeader.Read(tagReadingInfo, ref stream);

            // Read frame data
            int bytesLeft = _frameHeader.FrameSizeExcludingAdditions;
            if (bytesLeft >= 6) // note: 6 was chosen arbitrarily
            {
                // Read text encoding
                TextEncoding = (EncodingType)stream.Read1(ref bytesLeft);

                if (tagReadingInfo.TagVersion == ID3v2TagVersion.ID3v22)
                {
                    // TODO: Do something with this?
                    string imageFormat = ID3v2Utils.ReadString(EncodingType.ISO88591, stream, 3);
                    bytesLeft -= 3;
                }
                else
                {
                    // Read MIME type
                    MimeType = ID3v2Utils.ReadString(EncodingType.ISO88591, stream, ref bytesLeft);
                }

                // Read picture type
                PictureType = (PictureType)stream.Read1(ref bytesLeft);

                // Short description
                Description = ID3v2Utils.ReadString(TextEncoding, stream, ref bytesLeft);

                // Picture data
                if (bytesLeft > 0)
                {
                    byte[] pictureData = stream.Read(bytesLeft);
                    bytesLeft = 0;
                    try
                    {
                        PictureData = pictureData;
                    }
                    finally
                    {
                    }
                }
                else
                {
                    // Incomplete frame
                    PictureData = null;
                }
            }
            else
            {
                // Incomplete frame
                TextEncoding = EncodingType.ISO88591;
                Description = null;
                MimeType = null;
                PictureType = PictureType.CoverFront;
                PictureData = null;
            }

            // Seek to end of frame
            if (bytesLeft > 0)
            {
                stream.Seek(bytesLeft, SeekOrigin.Current);
            }
        }

        public override byte[] GetBytes(ID3v2TagVersion tagVersion)
        {
            if (_pictureData == null || _pictureData.Length == 0)
                return new byte[0];

            // iTunes doesn't like Unicode in APIC descriptions - fixed in iTunes 7.1.0.59
            //TextEncoding = EncodingType.ISO88591;

            using (MemoryStream stream = new MemoryStream())
            {
                byte[] descriptionData;

                do
                {
                    descriptionData = ID3v2Utils.GetStringBytes(tagVersion, _textEncoding, _description, true);
                } while (
                    this.RequiresFix(tagVersion, _description, descriptionData)
                );

                stream.WriteByte((byte)_textEncoding);
                if (tagVersion == ID3v2TagVersion.ID3v22)
                {
                    // http://id3.org/id3v2-00#line-1095 (section 4.15)
                    string format;
                    switch (_mimeType)
                    {
                        case "image/png":
                            format = "PNG";
                                break;
                        case "image/jpeg":
                            format = "JPG";
                            break;
                         default:
                             format = "   ";
                             break;
                    }

                    stream.Write(Encoding.ASCII.GetBytes(format));
                }
                else
                {
                    // iTunes needs this set properly
                    stream.Write(ByteUtils.ISO88591GetBytes(_mimeType));
                    stream.WriteByte(0); // terminator
                }
                stream.WriteByte((byte)_pictureType);
                stream.Write(descriptionData);
                stream.Write(_pictureData);
                return _frameHeader.GetBytes(stream, tagVersion, GetFrameID(tagVersion));
            }
        }


    }
}
