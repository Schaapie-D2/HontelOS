﻿/*
* PROJECT:          HontelOS
* CONTENT:          Single-line text box control
* PROGRAMMERS:      Jort van Dalen
*/

using Cosmos.System;
using HontelOS.System.Input;
using System;
using System.Drawing;

namespace HontelOS.System.Graphics.Controls
{
    public class TextBox : Control
    {
        public string Text = "";
        public string Placeholder = "Enter text...";

        public Action<string> OnSubmit;

        public TextBox(string placeholder, Action<string> onSubmit, int x, int y, int width, int height, Window window) : base(window)
        {
            Placeholder = placeholder;
            OnSubmit = onSubmit;

            X = x;
            Y = y;
            Width = width;
            Height = height;
            Cursor = Cursor.Text;
        }

        public override void Draw()
        {
            if (IsDisabled)
                c.DrawFilledRoundedRectangle(Style.TextBox_DisabledColor, X, Y, Width, Height, 5);
            else if (IsHovering)
                c.DrawFilledRoundedRectangle(Style.TextBox_HoverColor, X, Y, Width, Height, 5);
            else
                c.DrawFilledRoundedRectangle(Style.TextBox_NormalColor, X, Y, Width, Height, 5);

            if (string.IsNullOrEmpty(Text))
                c.DrawString(Placeholder, Style.SystemFont, Color.Gray, X + 10, Y + Height / 2 - Style.SystemFont.Height / 2);
            else
                c.DrawString(Text, Style.SystemFont, Color.Black, X + 10, Y + Height / 2 - Style.SystemFont.Height / 2);
        }

        public override void Update()
        {
            base.Update();
            if (IsSelected && KeyboardManagerExt.KeyAvailable)
            {
                var key = KeyboardManagerExt.ReadKey().Key;

                if (key == ConsoleKeyEx.Backspace)
                {
                    if(Text.Length > 0)
                        Text = Text.Remove(Text.Length - 1);
                }
                else if (key == ConsoleKeyEx.Enter)
                    OnSubmit?.Invoke(Text);
                else
                    Text += KeyboardManagerExt.ReadKey().KeyChar;

                Window.IsDirty = true;
            }
        }
    }
}