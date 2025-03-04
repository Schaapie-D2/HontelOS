using Cosmos.System.Graphics.Fonts;
using HontelOS.Resources;
using System.Drawing;

namespace HontelOS.System.Graphics.Controls
{
    public class Label : Control
    {
        public string Text;
        public Color Color;
        public Font Font;

        public Label(string text, Font font, Color color, int x, int y, IControlContainer container) : base(container)
        {
            Text = text;
            if(color != Color.Empty)
                Color = color;
            else
                Color = Style.Label_TextColor;
            if(font != null)
                Font = font;
            else
                Font = Style.SystemFont;

            X = x;
            Y = y;

            SystemEvents.OnStyleChanged.Add(() => {
                if (Color == StyleManager.PreviousStyle.Label_TextColor)
                    Color = StyleManager.Style.Label_TextColor;
                if (Font == StyleManager.PreviousStyle.SystemFont)
                    Font = StyleManager.Style.SystemFont;
            });
        }

        public override void Draw()
        {
            base.Draw();
            c.DrawString(Text, Font, Color, X, Y);
            DoneDrawing();
        }
    }
}
