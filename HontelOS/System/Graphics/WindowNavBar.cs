﻿/*
* PROJECT:          HontelOS
* CONTENT:          Window Navigation Bar
* PROGRAMMERS:      Jort van Dalen
*/

namespace HontelOS.System.Graphics
{
    public class WindowNavBar
    {
        private Style Style = StyleManager.Style;
        public Window Window;
        public DirectBitmap c;

        public int Width = 100;
        public int Height;

        private int itemSize = 24;

        public WindowNavBar(Window window)
        {
            Window = window;
            c = window.canvas;
            Height = window.Height;

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; });
        }

        public void Draw()
        {
            var Items = Window.Pages;
            if (Items.Count <= 1) return;

            c.DrawFilledRectangle(Style.WindowNavBar_BackgroundColor, 0, 0, Width, Height);

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                int posY = 5 + i * (itemSize + 5);
                int textY = posY + (itemSize / 2) - Style.SystemFont.Height / 2;

                if (Window.CurrentPage == i)
                {
                    c.DrawFilledRoundedRectangle(Style.WindowNavBar_HoverColor, 5, posY, Width - 10, itemSize, 5);
                    c.DrawString(item.Title, Style.SystemFont, Style.WindowNavBar_TextColor, 10, textY);
                }
                else if (Kernel.MouseInArea(Window.ContainerX - Width + 5, Window.ContainerY + posY, Window.ContainerX - 5, Window.ContainerY + posY + itemSize))
                {
                    c.DrawFilledRoundedRectangle(Style.WindowNavBar_SelectedColor, 5, posY, Width - 10, itemSize, 5);
                    c.DrawString(item.Title, Style.SystemFont, Style.WindowNavBar_SelectedTextColor, 10, textY);
                }
                else
                {
                    c.DrawString(item.Title, Style.SystemFont, Style.WindowNavBar_TextColor, 10, textY);
                }
            }
        }

        public void Update()
        {
            var Items = Window.Pages;
            if (Items.Count <= 1) return;

            for (int i = 0; i < Items.Count; i++)
            {
                int posY = 5 + i * (itemSize + 5);

                if (Kernel.MouseInArea(Window.ContainerX - Width + 5, Window.ContainerY + posY, Window.ContainerX - 5, Window.ContainerY + posY + itemSize))
                {
                    Window.IsDirty = true;
                    if (Kernel.MouseClick())
                        Window.CurrentPage = i;
                }
            
            }
        }

        public void UpdateWindow()
        {
            if (Window.Pages.Count > 1)
                Window.ContainerX += Width;
        }
    }
}