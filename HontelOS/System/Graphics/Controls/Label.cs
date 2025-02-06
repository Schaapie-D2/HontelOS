/*
* PROJECT:          HontelOS
* CONTENT:          Label control
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System.Graphics.Fonts;
using HontelOS.Resources;
using System.Drawing;

namespace HontelOS.System.Graphics.Controls
{
    public class Label : Control
    {
        public string Text;
        public Color Color;
        public Font Font;

        public Label(string text, Font font, Color color, int x, int y, IControlContainer container) : base(container)
        {
            Text = text;
            if(color != Color.Empty)
                Color = color;
            else
                Color = Style.Label_TextColor;
            if(font != null)
                Font = font;
            else
                Font = Style.SystemFont;

            X = x;
            Y = y;

            SystemEvents.OnStyleChanged.Add(() => {
                if (Color == StyleManager.PreviousStyle.Label_TextColor)
                    Color = StyleManager.Style.Label_TextColor;
                if (Font == StyleManager.PreviousStyle.SystemFont)
                    Font = StyleManager.Style.SystemFont;
            });
        }

        public override void Draw()
        {
            base.Draw();
            c.DrawString(Text, Font, Color, X, Y);
            DoneDrawing();
        }
    }
}
