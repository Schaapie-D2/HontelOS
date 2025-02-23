/*
* PROJECT:          HontelOS
* CONTENT:          Multi-line text box control
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
using HontelOS.System.Input;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HontelOS.System.Graphics.Controls
{
    public class TextAreaBox : Control
    {
        public List<string> Text = new List<string>() { "" };
        public string Placeholder;

        private int currentLine = 0;
        private int currentPosition = 0;

        public TextAreaBox(string placeholder, int x, int y, int width, int height, IControlContainer container) : base(container)
        {
            Placeholder = placeholder;

            OnClick.Add(() => IsDirty = true);
            OnEndClick.Add(() => IsDirty = true);
            OnStartHover.Add(() => IsDirty = true);
            OnEndHover.Add(() => IsDirty = true);

            X = x;
            Y = y;
            Width = width;
            Height = height;
            Cursor = Cursor.Text;
        }

        public override void Draw()
        {
            base.Draw();

            if (IsDisabled)
                c.DrawFilledRoundedRectangle(Style.TextBox_DisabledColor, X, Y, Width, Height, 5);
            else if (IsHovering)
                c.DrawFilledRoundedRectangle(Style.TextBox_HoverColor, X, Y, Width, Height, 5);
            else
                c.DrawFilledRoundedRectangle(Style.TextBox_NormalColor, X, Y, Width, Height, 5);

            if (Text.All(line => string.IsNullOrEmpty(line)))
            {
                c.DrawString(Placeholder, Style.SystemFont, Color.Gray, X, Y);
            }
            else
            {
                for (int i = 0; i < Text.Count; i++)
                {
                    c.DrawString(Text[i], Style.SystemFont, Color.Black, X, Y + Style.SystemFont.Height * i);
                }
            }

            int linX = X + Style.SystemFont.Width * currentPosition;
            int linY = Y + Style.SystemFont.Height * currentLine;
            c.DrawLine(Color.Black, linX, linY, linX, linY + Style.SystemFont.Height);

            DoneDrawing();
        }

        public override void Update()
        {
            base.Update();

            if (IsSelected && KeyboardManagerExt.KeyAvailable && !IsDisabled)
            {
                var key = KeyboardManagerExt.ReadKey();

                if (key.Key == ConsoleKeyEx.DownArrow && currentLine < Text.Count - 1)
                {
                    currentLine++;
                    currentPosition = Math.Min(currentPosition, Text[currentLine].Length);
                }
                else if (key.Key == ConsoleKeyEx.UpArrow && currentLine > 0)
                {
                    currentLine--;
                    currentPosition = Math.Min(currentPosition, Text[currentLine].Length);
                }
                else if (key.Key == ConsoleKeyEx.RightArrow && currentPosition < Text[currentLine].Length)
                {
                    currentPosition++;
                }
                else if (key.Key == ConsoleKeyEx.LeftArrow && currentPosition > 0)
                {
                    currentPosition--;
                }
                else if (key.Key == ConsoleKeyEx.Backspace)
                {
                    if (currentPosition > 0)
                    {
                        Text[currentLine] = Text[currentLine].Remove(currentPosition - 1, 1);
                        currentPosition--;
                    }
                    else if (currentLine > 0)
                    {
                        currentPosition = Text[currentLine - 1].Length;
                        Text[currentLine - 1] += Text[currentLine];
                        Text.RemoveAt(currentLine);
                        currentLine--;
                    }
                }
                else if (key.Key == ConsoleKeyEx.Enter)
                {
                    string newLine = (currentPosition < Text[currentLine].Length) ? Text[currentLine].Substring(currentPosition) : "";
                    Text[currentLine] = Text[currentLine].Substring(0, currentPosition);
                    Text.Insert(currentLine + 1, newLine);
                    currentLine++;
                    currentPosition = 0;
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    Text[currentLine] = Text[currentLine].Insert(currentPosition, key.KeyChar.ToString());
                    currentPosition++;
                }

                IsDirty = true;
            }
        }
    }
}
