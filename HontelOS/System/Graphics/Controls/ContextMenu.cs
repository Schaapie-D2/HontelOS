﻿/*
* PROJECT:          HontelOS
* CONTENT:          Context menu control
* PROGRAMMERS:      Jort van Dalen
*/

using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using HontelOS.System.Graphics;
using System;
using System.Drawing;

namespace HontelOS.System.Graphics.Controls
{
    public class ContextMenu : SystemControl
    {
        Canvas c = Kernel.canvas;
        Style Style = Kernel.style;

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
        }

        public void Show() => Kernel.systemControls.Add(this);

        public void Draw()
        {
            c.DrawRoundedRectangle(Color.Black, x - 1, y - 1, width + 2, items.Length * 18 + 2, 5);
            c.DrawFilledRoundedRectangle(Style.ContextMenu_BackgroundColor, x, y, width, items.Length * 18, 5);
            for (int i = 0; i < items.Length; i++)
            {
                c.DrawString(items[i], PCScreenFont.Default, Color.Black, x + 2, y + i * 18);
            }
        }

        public void Update()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (Kernel.MouseInArea(x, y + i * 16, x + width, y + 16 + i * 18))
                {
                    c.DrawFilledRectangle(Style.ContextMenu_HoverColor, x, y + i * 16, 150, 16);
                }
                if (Kernel.MouseInArea(x, y + i * 16, x + width, y + 16 + i * 18) && Kernel.MouseClick())
                {
                    actions[i]?.Invoke(i);
                    Kernel.systemControls.Remove(this);
                    break;
                }
                if (!Kernel.MouseInArea(x, y, x + width, y + items.Length * 18) && Kernel.MouseClick())
                {
                    Kernel.systemControls.Remove(this);
                    break;
                }
            }
        }
    }
}