using Cosmos.System.Graphics;
using System;
using System.IO;
using CosmosPNG.PNGLib.Decoders.PNG;

namespace HontelOS.System.Helpers.Graphics
{
    public static class ImageFormatHelper
    {
        public static Bitmap GetBitmap(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp":
                    return new Bitmap(path, ColorOrder.RGB);
                case ".png":
                    return new PNGDecoder().GetBitmap(path);
                default:
                    throw new NotSupportedException($"The file format {Path.GetExtension(path)} is not supported!");
            }
        }

        public static Bitmap GetBitmap(byte[] data, string extension)
        {
            switch (extension)
            {
                case ".bmp":
                    return new Bitmap(data, ColorOrder.RGB);
                case ".png":
                    return new PNGDecoder().GetBitmap(data);
                default:
                    throw new NotSupportedException($"The file format {extension} is not supported!");
            }
        }
    }
}
