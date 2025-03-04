using Cosmos.System.Graphics;

namespace HontelOS.System.Graphics.Controls
{
    public class PictureBox : Control
    {
        public Image Image;

        public PictureBox(Image image, int x, int y, int width, int height, IControlContainer container) : base(container)
        {
            Image = image;

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override void Draw()
        {
            base.Draw();
            c.DrawImage(Image, X, Y, Width, Height);
            DoneDrawing();
        }
    }
}
