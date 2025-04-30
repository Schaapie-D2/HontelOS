using Cosmos.System;
using Cosmos.System.Graphics;
using CosmosPNG.PNGLib.Decoders.PNG;
using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;
using System.IO;

namespace HontelOS.System.Applications.ImageViewer
{
    public class ImageViewerProgram : Window
    {
        Bitmap image;

        PictureBox pictureBox;

        float scale = 1f;

        public ImageViewerProgram(string path) : base("Image viewer", WindowStyle.Normal, 50, 50, 900, 500)
        {
            OnResize.Add(Resize);

            Page p = Pages[0];

            switch(Path.GetExtension(path))
            {
                case ".bmp":
                    image = new Bitmap(path, ColorOrder.RGB);
                    break;
                case ".png":
                    image = new PNGDecoder().GetBitmap(path);
                    break;
                default:
                    new MessageBox("Unsupported image format", $"The file format {Path.GetExtension(path)} is not supported!", null, MessageBoxButtons.Ok);
                    Close();
                    break;
            }

            int scaledWidth = (int)(image.Width / ((float)image.Height / Height));
            int x = (Width - scaledWidth) / 2;
            pictureBox = new PictureBox(image, x, 0, scaledWidth, Height, p);

            WindowManager.Register(this);
        }

        void Resize()
        {
            float aspectRatio = (float)image.Width / image.Height;
            int scaledHeight = (int)(Height * scale);
            int scaledWidth = (int)(scaledHeight * aspectRatio);

            int x = (Width - scaledWidth) / 2;
            int y = (Height - scaledHeight) / 2;

            pictureBox.X = x;
            pictureBox.Y = y;
            pictureBox.Width = scaledWidth;
            pictureBox.Height = scaledHeight;

            Pages[0].FullRedrawNeeded = true;
        }

        public override void CustomUpdate()
        {
            base.CustomUpdate();

            if(MouseManager.ScrollDelta != 0)
            {
                if (MouseManager.ScrollDelta > 0 && scale < 10f)
                    scale += 0.1f;
                else if (MouseManager.ScrollDelta < 0 && scale > 0.1f)
                    scale -= 0.1f;

                Resize();
            }
        }
    }
}
