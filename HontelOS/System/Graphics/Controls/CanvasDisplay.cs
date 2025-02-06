/*
* PROJECT:          HontelOS
* CONTENT:          Canvas control
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System;

namespace HontelOS.System.Graphics.Controls
{
    public class CanvasDisplay : Control
    {
        public DirectBitmap Canvas;

        public CanvasDisplay(string text, Action onClick, int x, int y, int width, int height, IControlContainer container) : base(container)
        {
            Canvas = new DirectBitmap(width, height);

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override void Draw()
        {
            base.Draw();
            c.DrawImage(Canvas.Bitmap, X, Y);
            DoneDrawing();
        }

        public void SetSize(int width, int height)
        {
            Canvas.SetSize(width, height);
        }
    }
}
