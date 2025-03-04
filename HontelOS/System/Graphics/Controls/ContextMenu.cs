using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;

namespace HontelOS.System.Graphics.Controls
{
    public class ContextMenu : SystemControl
    {
        Canvas c = Kernel.canvas;
        Style Style = StyleManager.Style;

        public string[] items;
        public Action<int>[] actions;
        public int x;
        public int y;
        public int width = 150;

        public ContextMenu(string[] items, Action<int>[] actionsForItems, int x, int y, int width)
        {
            this.items = items;
            actions = actionsForItems;
            this.width = width;
            this.x = x;
            this.y = y;

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; });
        }

        public void Show() => Kernel.systemControls.Add(this);

        public void Draw()
        {
            c.DrawRoundedRectangle(Color.Black, x - 1, y - 1, width + 2, items.Length * 18 + 2, 5);
            c.DrawFilledRoundedRectangle(Style.ContextMenu_BackgroundColor, x, y, width, items.Length * 18, 5);
            for (int i = 0; i < items.Length; i++)
            {
                if (Kernel.MouseInArea(x, y + i * 18, x + width, y + 18 + i * 18))
                    c.DrawFilledRectangle(Style.ContextMenu_HoverColor, x, y + i * 18, width, 16);

                c.DrawString(items[i], PCScreenFont.Default, Style.ContextMenu_TextColor, x + 2, y + i * 18);
            }
        }

        public void Update()
        {
            if(Kernel.MouseInArea(x, y, x + width, y + items.Length * 18))
            {
                if (Kernel.MouseClick())
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (Kernel.MouseInArea(x, y + i * 16, x + width, y + 16 + i * 18))
                        {
                            actions[i]?.Invoke(i);
                            Kernel.systemControls.Remove(this);
                            break;
                        }
                    }
                }
            }
            else if(Kernel.MouseClick())
            {
                Kernel.systemControls.Remove(this);
            }
        }
    }
}
