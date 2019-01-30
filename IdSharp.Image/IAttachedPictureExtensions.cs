using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using IdSharp.Common.Utils;
using IdSharp.Tagging.ID3v2.Frames;


namespace IdSharp.Tagging.ID3v2.Frames
{
    public static class AttachedPictureExtensions
    {
        public static void SavePicture(this IAttachedPicture attachedPicture, Image picture)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                picture.Save(memoryStream, picture.RawFormat);
                attachedPicture.PictureData = memoryStream.ToArray();
            }

            SetMimeType(attachedPicture, picture);
        }


        private static void SetMimeType(IAttachedPicture attachedPicture, Image picture)
        {
            if (picture != null)
            {
                if (picture.RawFormat.Equals(ImageFormat.Bmp))
                {
                    attachedPicture.MimeType = "image/bmp";
                }
                else if (picture.RawFormat.Equals(ImageFormat.Emf))
                {
                    attachedPicture.MimeType = "image/x-emf";
                }
                else if (picture.RawFormat.Equals(ImageFormat.Exif))
                {
                    // TODO - Unsure of MIME type?
                }
                else if (picture.RawFormat.Equals(ImageFormat.Gif))
                {
                    attachedPicture.MimeType = "image/gif";
                }
                else if (picture.RawFormat.Equals(ImageFormat.Icon))
                {
                    // TODO - How to handle this?
                }
                else if (picture.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    attachedPicture.MimeType = "image/jpeg";
                }
                else if (picture.RawFormat.Equals(ImageFormat.MemoryBmp))
                {
                    attachedPicture.MimeType = "image/bmp";
                }
                else if (picture.RawFormat.Equals(ImageFormat.Png))
                {
                    attachedPicture.MimeType = "image/png";
                }
                else if (picture.RawFormat.Equals(ImageFormat.Tiff))
                {
                    attachedPicture.MimeType = "image/tiff";
                }
                else if (picture.RawFormat.Equals(ImageFormat.Wmf))
                {
                    attachedPicture.MimeType = "image/x-wmf";
                }
                else
                {
                    // TODO
                    //attachedPicture.MimeType = "image/";
                }
            }
        }

        public static Image LoadPicture(this IAttachedPicture attachedPicture)
        {
            bool _ = false;
            return LoadPicture(attachedPicture, ref _);
        }

        public static Image LoadPicture(IAttachedPicture picture, ref bool loadingPicture)
        {
            loadingPicture = false;

            var _pictureData = picture.PictureData;
            if (_pictureData == null)
            {
                return null;
            }

            using (MemoryStream memoryStream = new MemoryStream(_pictureData))
            {
                bool isInvalidImage = false;
                Image image = null;
                try
                {
                    loadingPicture = true;
                    try
                    {
                        image = Image.FromStream(memoryStream);
                    }
                    finally
                    {
                        loadingPicture = false;
                    }
                }
                catch (OutOfMemoryException)
                {
                    string msg = string.Format("OutOfMemoryException caught in APIC's PictureData setter");
                    Trace.WriteLine(msg);

                    isInvalidImage = true;
                }
                catch (ArgumentException)
                {
                    string msg = string.Format("ArgumentException caught in APIC's PictureData setter");
                    Trace.WriteLine(msg);

                    isInvalidImage = true;
                }

                if (isInvalidImage)
                {
                    // Invalid image
                    if (image != null)
                    {
                        image.Dispose();
                    }

                    image = null;
                    try
                    {
                        string url = ByteUtils.ISO88591GetString(_pictureData);
                        if (url.Contains("://"))
                        {
                            picture.MimeType = "-->";
                        }
                    }
                    catch (Exception ex)
                    {
                        // don't throw an exception
                        Trace.WriteLine(ex);
                    }
                }

                return image;
            }
        }
    }



}
