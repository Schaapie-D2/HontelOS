using Cosmos.System.Graphics;
using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;
using System.IO;

namespace HontelOS.System.Applications.ImageViewer
{
    public class ImageViewerProgram : Window
    {
        Bitmap image;

        public ImageViewerProgram(string path) : base("Image viewer", WindowStyle.Normal, 50, 50, 700, 500)
        {
            Page p = Pages[0];

            if (!path.EndsWith(".bmp"))
            {
                new MessageBox("Unsupported image format", $"The file format {Path.GetExtension(path)} is not supported!", null, MessageBoxButtons.Ok);
                Close();
            }

            image = new Bitmap(path, ColorOrder.RGB);

            new PictureBox(image, (int)(Width / 2 - image.Width / 2), (int)(Height / 2 - image.Height / 2), (int)image.Width, (int)image.Height, p);

            WindowManager.Register(this);
        }
    }
}
