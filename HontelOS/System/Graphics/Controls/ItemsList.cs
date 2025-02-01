﻿/*
* PROJECT:          HontelOS
* CONTENT:          Items list control
* PROGRAMMERS:      Jort van Dalen
*/

using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using HontelOS.System.Input;
using System;
using System.Collections.Generic;

namespace HontelOS.System.Graphics.Controls
{
    public class ItemsList : Control
    {
        public List<string> Items;
        public int SelectedIndex = -1;

        int scrollPosition = 0;
        const int itemHeight = 18;

        public List<Action<int>> OnSubmit = new();

        public ItemsList(List<string> items, int x, int y, int width, int height, IControlContainer container) : base(container)
        {
            Items = items;

            X = x;
            Y = y;
            Width = width;
            Height = height;
            OnMouseMove.Add(() => Container.IsDirty = true);
        }

        public override void Draw()
        {
            base.Draw();

            c.DrawFilledRoundedRectangle(Style.ItemsList_BackgroundColor, X, Y, Width, Height, 5);

            int visibleItemCount = Height / itemHeight;

            if (scrollPosition > Items.Count - visibleItemCount)
                scrollPosition = Math.Max(0, Items.Count - visibleItemCount);

            for (int i = 0; i < visibleItemCount; i++)
            {
                int itemIndex = i + scrollPosition;

                if (itemIndex >= Items.Count)
                    break;
                if (!string.IsNullOrEmpty(Items[itemIndex]))
                {
                    if (Kernel.MouseInArea(Container.ContainerX + X, Container.ContainerY + Y + i * itemHeight, Container.ContainerX + X + Width, Container.ContainerY + Y + i * itemHeight + itemHeight) && SelectedIndex != i)
                        c.DrawFilledRoundedRectangle(Style.ItemsList_HoverColor, X, Y + i * itemHeight, Width, itemHeight, 5);
                    if (Kernel.MouseInArea(Container.ContainerX + X, Container.ContainerY + Y + i * itemHeight, Container.ContainerX + X + Width, Container.ContainerY + Y + i * itemHeight + itemHeight) && Kernel.MouseClick())
                        SelectedIndex = i;
                    if (SelectedIndex == i)
                    {
                        c.DrawFilledRoundedRectangle(Style.ItemsList_SelectedColor, X, Y + i * itemHeight, Width, itemHeight, 5);
                        c.DrawString(Items[itemIndex], PCScreenFont.Default, Style.ItemsList_SelectedTextColor, X + 2, Y + i * itemHeight);
                    }
                    else
                        c.DrawString(Items[itemIndex], PCScreenFont.Default, Style.ItemsList_TextColor, X + 2, Y + i * itemHeight);
                }
            }

            DoneDrawing();
        }

        public override void Update()
        {
            base.Update();
            if (IsSelected && KeyboardManagerExt.KeyAvailable)
            {
                var key = KeyboardManagerExt.ReadKey().Key;

                if (key == ConsoleKeyEx.UpArrow && SelectedIndex >= 1)
                    SelectedIndex--;
                else if (key == ConsoleKeyEx.DownArrow && SelectedIndex < Items.Count - 1)
                    SelectedIndex++;
                else if (key == ConsoleKeyEx.Enter)
                    foreach (var a in OnSubmit) a.Invoke(SelectedIndex);

                if((key == ConsoleKeyEx.UpArrow || key == ConsoleKeyEx.DownArrow) && SelectedIndex >= Items.Count)
                    SelectedIndex = Items.Count - 1;

                Container.IsDirty = true;
            }
            if (IsHovering)
            {
                if (MouseManager.ScrollDelta > 0 && scrollPosition > 0)
                    scrollPosition--;
                else if (MouseManager.ScrollDelta < 0 && scrollPosition < Items.Count - Height / itemHeight)
                    scrollPosition++;
            }
        }
    }
}
