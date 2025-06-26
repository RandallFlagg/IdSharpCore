using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class ImageSharpInterop
{
    public static Bitmap GetBitmapFromBytes(byte[] imageBytes)
    {
        using var image = Image.Load<Rgba32>(imageBytes, out IImageFormat format);

        // Convert ImageSharp image to System.Drawing.Bitmap
        var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

        for (int y = 0; y < image.Height; y++)
        {
            var row = image.GetPixelRowSpan(y);
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = row[x];
                bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
            }
        }

        return bitmap;
    }
}