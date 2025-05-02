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
        int oldSelectedIndex = -1;

        int hoverIndex = -1;
        int oldHoverIndex = -1;

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
                    if (SelectedIndex == i)
                    {
                        c.DrawFilledRoundedRectangle(Style.ItemsList_SelectedColor, X, Y + i * itemHeight, Width, itemHeight, 5);
                        c.DrawString(Items[itemIndex], PCScreenFont.Default, Style.ItemsList_SelectedTextColor, X + 2, Y + i * itemHeight);
                        continue;
                    }

                    if (hoverIndex == i)
                        c.DrawFilledRoundedRectangle(Style.ItemsList_HoverColor, X, Y + i * itemHeight, Width, itemHeight, 5);

                    c.DrawString(Items[itemIndex], PCScreenFont.Default, Style.ItemsList_TextColor, X + 2, Y + i * itemHeight);
                }
            }

            DoneDrawing();
        }

        public override void Update()
        {
            base.Update();
            if (IsSelected && KeyboardManagerExt.KeyAvailable && !IsDisabled)
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

                IsDirty = true;
            }
            if (IsHovering)
            {
                if(MouseManager.ScrollDelta != 0)
                {
                    if (MouseManager.ScrollDelta > 0 && scrollPosition > 0)
                        scrollPosition--;
                    else if (MouseManager.ScrollDelta < 0 && scrollPosition < Items.Count - Height / itemHeight)
                        scrollPosition++;

                    IsDirty = true;
                }

                for(int i = 0; i < Items.Count; i++)
                {
                    if (Kernel.MouseInArea(Container.ContainerX + X, Container.ContainerY + Y + i * itemHeight, Container.ContainerX + X + Width, Container.ContainerY + Y + i * itemHeight + itemHeight))
                    {
                        if (oldHoverIndex != i)
                        {
                            hoverIndex = i;
                            IsDirty = true;
                        }

                        if (Kernel.MouseClick())
                        {
                            SelectedIndex = i;
                            if (oldSelectedIndex != i)
                            {
                                IsDirty = true;
                                SelectedIndex = i;
                            }
                        }

                        oldHoverIndex = i;
                    }
                }
            }
            else
            {
                if (oldHoverIndex != -1)
                {
                    hoverIndex = -1;
                    IsDirty = true;
                    oldHoverIndex = -1;
                }
            }
        }
    }
}
