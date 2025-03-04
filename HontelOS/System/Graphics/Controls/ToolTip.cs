using Cosmos.System.Graphics;

namespace HontelOS.System.Graphics.Controls
{
    public class ToolTip : SystemControl
    {
        Canvas c = Kernel.canvas;
        Style Style = StyleManager.Style;

        public string Text;
        public ToolTipOrginDirection OrginDirection;
        public int OrginX;
        public int OrginY;

        public ToolTip(string text, ToolTipOrginDirection orginDirection, int orginX, int orginY)
        {
            Text = text;
            OrginDirection = orginDirection;
            OrginX = orginX;
            OrginY = orginY;

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; });
        }

        public void Show() => Kernel.systemControls.Add(this);
        public void Hide() => Kernel.systemControls.Remove(this);

        public void Draw()
        {
            int x;
            int y;
            int textLength = Text.Length * 8;

            if(OrginDirection == ToolTipOrginDirection.Up)
            {
                x = OrginX - textLength / 2;
                y = OrginY + 20 + 5;

                c.DrawFilledRoundedRectangle(Style.ToolTip_Color, x, y, textLength + 8, 24, 5);
                c.DrawString(Text, Style.SystemFont, Style.ToolTip_TextColor, x + 4, y + 4);
            }
            else if (OrginDirection == ToolTipOrginDirection.Down)
            {
                x = OrginX - textLength / 2;
                y = OrginY - (20 + 5);

                c.DrawFilledRoundedRectangle(Style.ToolTip_Color, x, y, textLength + 8, 24, 5);
                c.DrawString(Text, Style.SystemFont, Style.ToolTip_TextColor, x + 4, y + 4);
            }
            else if (OrginDirection == ToolTipOrginDirection.Left)
            {

            }
            else if (OrginDirection == ToolTipOrginDirection.Right)
            {

            }
        }

        public void Update()
        {
            
        }

        public enum ToolTipOrginDirection
        {
            Up,
            Down,
            Left,
            Right,
        }
    }
}
