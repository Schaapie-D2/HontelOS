/*
* PROJECT:          HontelOS
* CONTENT:          Context menu button control
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System;
using System;
using System.Drawing;

namespace HontelOS.System.Graphics.Controls
{
    public class ContextMenuButton : Control
    {
        public string Text;
        string[] items;
        Action<int>[] actions;

        public ContextMenuButton(string text, string[] items, Action<int>[] actionsForItems, int x, int y, int width, int height, IControlContainer container) : base(container)
        {
            Text = text;
            this.items = items;
            actions = actionsForItems;

            OnClick.Add(() => IsDirty = true);
            OnEndClick.Add(() => IsDirty = true);
            OnStartHover.Add(() => IsDirty = true);
            OnEndHover.Add(() => IsDirty = true);

            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        public override void Draw()
        {
            base.Draw();

            if (IsDisabled)
                c.DrawFilledRoundedRectangle(Style.Button_DisabledColor, X, Y, Width, Height, 5);
            else if (IsHovering && MouseManager.MouseState == MouseState.Left)
                c.DrawFilledRoundedRectangle(Style.Button_PressedColor, X, Y, Width, Height, 5);
            else if (IsHovering)
                c.DrawFilledRoundedRectangle(Style.Button_HoverColor, X, Y, Width, Height, 5);
            else
                c.DrawFilledRoundedRectangle(Style.Button_NormalColor, X, Y, Width, Height, 5);

            c.DrawString(Text, Style.SystemFont, Style.Button_TextColor, X + Width / 2 - Style.SystemFont.Width * Text.Length / 2, Y + Height / 2 - Style.SystemFont.Height / 2);

            DoneDrawing();
        }

        public override void Update()
        {
            base.Update();
            if (IsHovering && Kernel.MouseClick())
            {
                ContextMenu menu = new ContextMenu(items, actions, Container.ContainerX + X, Container.ContainerY + Y + Height, Width);
                menu.Show();
            }
        }
    }
}
