using Cosmos.System;
using System;

namespace HontelOS.System.Graphics.Controls
{
    public class DropdownButton : Control
    {
        string[] items;
        Action<int>[] actions;
        public int selectedIndex;

        public DropdownButton(string[] items, int selectedIndex, Action<int>[] onItemSelected, int x, int y, int width, int height, IControlContainer container) : base(container)
        {
            this.selectedIndex = selectedIndex;
            this.items = items;
            actions = onItemSelected;

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

            c.DrawString(items[selectedIndex], Style.SystemFont, Style.Button_TextColor, X + Width / 2 - Style.SystemFont.Width * items[selectedIndex].Length / 2, Y + Height / 2 - Style.SystemFont.Height / 2);

            DoneDrawing();
        }

        public override void Update()
        {
            base.Update();
            if (IsHovering && Kernel.MouseClick())
            {
                Action<int>[] act = new Action<int>[actions.Length];
                for (int i = 0; i < actions.Length; i++)
                {
                    int index = i;
                    act[i] = (_) => { actions[index].Invoke(_); selectedIndex = index; };
                }

                ContextMenu menu = new ContextMenu(items, act, Container.ContainerX + X, Container.ContainerY + Y + Height, Width);
                menu.Show();
            }
        }
    }
}
