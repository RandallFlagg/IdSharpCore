using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SharpImage = SixLabors.ImageSharp.Image;
using DrawingColor = System.Drawing.Color;

public static class ImageSharpInterop
{
    public static Bitmap GetBitmapFromBytes(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty.", nameof(imageBytes));
        }

        using var image = SharpImage.Load<Rgba32>(imageBytes);

        var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < image.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < image.Width; x++)
                {
                    var pixel = row[x];
                    bitmap.SetPixel(x, y, DrawingColor.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                }
            }
        });

        return bitmap;
    }


}