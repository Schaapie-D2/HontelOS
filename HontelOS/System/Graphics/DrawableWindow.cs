/*
* PROJECT:          HontelOS
* CONTENT:          Drawable window
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HontelOS.System.Graphics
{
    public class DrawableWindow : IWindow
    {
        Canvas c = Kernel.canvas;
        public DirectBitmap canvas;
        public Style Style = StyleManager.Style;

        public string Title { get; set; }
        public Image Icon { get; set; }
        public WindowStyle WindowStyle;
        public int WID { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool IsVisible { get; set; } = true;
        public bool CanClose { get; set; } = true;

        public bool IsDirty { get; set; } = true;

        public bool DisableMaximizeButton = false;
        public bool DisableMinimizeButton = false;

        public List<Action> OnClose = new();

        bool isHoldingHandel = false;

        Bitmap cachedTop = null;
        bool isTopDirty = true;

        int oldX;
        int oldY;
        int oldWidth;
        int oldHeight;

        int dragOffsetX;
        int dragOffsetY;

        public DrawableWindow(string title, WindowStyle windowStyle, int x, int y, int width, int height)
        {
            Title = title;
            WindowStyle = windowStyle;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            canvas = new DirectBitmap(width, height);
            canvas.Clear(Color.Black);

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; IsDirty = true; isTopDirty = true; });
            SystemEvents.OnCanvasChanged.Add(() => { c = Kernel.canvas; IsDirty = true; isTopDirty = true; });
        }

        public virtual void Draw(DirectBitmap canvas) { return; }
        public virtual void CustomUpdate() { return; }

        public void DrawWindow()
        {
            if (!IsVisible)
                return;

            // Border
            if (WindowStyle != WindowStyle.Borderless)
            {
                c.DrawRoundedRectangle(Color.Black, X - 1, Y - 1, Width + 1, 33, 10);
                c.DrawRectangle(Color.Black, X - 1, Y + 16, Width + 1, Height + 16);
            }
            else
                c.DrawRectangle(Color.Black, X - 1, Y - 1, Width + 1, Height + 1);

            // Top
            if (isTopDirty)
            {
                if (WindowStyle == WindowStyle.Normal)
                {
                    c.DrawFilledTopRoundedRectangle(Style.Window_HandleColor, X, Y, Width, 32, 10);//handel

                    if (Icon != null)
                    {
                        c.DrawImage(Icon, X + 10, Y + 10, 16, 16, true);//icon
                        c.DrawString(Title, PCScreenFont.Default, Style.Window_HandleTextColor, X + 32, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title
                    }
                    else
                        c.DrawString(Title, PCScreenFont.Default, Style.Window_HandleTextColor, X + 10, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title

                    if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32))
                        c.DrawFilledRectangle(Color.Red, X + Width - 32, Y, 32, 32, true);//red glow
                    c.DrawString("X", PCScreenFont.Default, Style.Window_HandleTextColor, X + Width - 32 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//close
                    if (Kernel.MouseInArea(X + Width - 64, Y, X + Width - 32, Y + 32))
                        c.DrawFilledRectangle(Style.Window_HandleButtonGlowColor, X + Width - 64, Y, 32, 32, true);//gray glow
                    c.DrawString("+", PCScreenFont.Default, Style.Window_HandleTextColor, X + Width - 64 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//maximize
                    if (Kernel.MouseInArea(X + Width - 96, Y, X + Width - 64, Y + 32))
                        c.DrawFilledRectangle(Style.Window_HandleButtonGlowColor, X + Width - 96, Y, 32, 32, true);//gray glow
                    c.DrawString("-", PCScreenFont.Default, Style.Window_HandleTextColor, X + Width - 96 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//minimize
                }
                else if (WindowStyle == WindowStyle.Dialog)
                {
                    c.DrawRectangle(Color.Black, X - 1, Y + 16, Width + 1, Height + 16);
                    c.DrawFilledTopRoundedRectangle(Style.Window_HandleColor, X, Y, Width, 32, 10);//handel

                    if (Icon != null)
                    {
                        c.DrawImage(Icon, X + 10, Y + 10, 16, 16, true);//icon
                        c.DrawString(Title, PCScreenFont.Default, Style.Window_HandleTextColor, X + 32, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title
                    }
                    else
                        c.DrawString(Title, PCScreenFont.Default, Style.Window_HandleTextColor, X + 10, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title

                    if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32))
                        c.DrawFilledRectangle(Color.Red, X + Width - 32, Y, 32, 32, true);//red glow
                    c.DrawString("X", PCScreenFont.Default, Style.Window_HandleTextColor, X + Width - 32 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//close
                }
                else if (WindowStyle == WindowStyle.Borderless)
                {

                }

                if (WindowStyle != WindowStyle.Borderless)
                    cachedTop = c.GetImage(X, Y, Width, 32);

                isTopDirty = false;
            }
            else
            {
                if (WindowStyle != WindowStyle.Borderless)
                    c.DrawImage(cachedTop, X, Y, true);
            }

            // Content
            Draw(canvas);

            IsDirty = false;
        }

        public void UpdateWindow()
        {
            if (IsVisible)
            {
                if (WindowStyle == WindowStyle.Normal)
                {
                    if (isHoldingHandel)
                    { X = (int)MouseManager.X + dragOffsetX; Y = (int)MouseManager.Y + dragOffsetY; }

                    if (Kernel.MouseInArea(X, Y, X + Width, Y + Height + 32) && IsVisible)
                    {
                        if (Kernel.MouseClick())
                            WindowManager.SetFocused(WID);

                        if (Kernel.MouseInArea(X, Y, X + Width, Y + 32))
                        {
                            if (Kernel.MouseInArea(X, Y, X + Width - 96, Y + 32) && MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left && !isHoldingHandel && WindowManager.FocusedWindow == WID)
                            { dragOffsetX = X - (int)MouseManager.X; dragOffsetY = Y - (int)MouseManager.Y; isHoldingHandel = true; }

                            if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32) && Kernel.MouseClick())
                                Close();
                            if (Kernel.MouseInArea(X + Width - 64, Y, X + Width - 32, Y + 32) && Kernel.MouseClick())
                                Maximize();
                            if (Kernel.MouseInArea(X + Width - 96, Y, X + Width - 64, Y + 32) && Kernel.MouseClick())
                                Minimize();

                            isTopDirty = true;
                        }
                    }
                }
                else if (WindowStyle == WindowStyle.Dialog)
                {
                    if (isHoldingHandel)
                    { X = (int)MouseManager.X - dragOffsetX; Y = (int)MouseManager.Y - dragOffsetY; }

                    if (Kernel.MouseInArea(X, Y, X + Width, Y + Height + 32) && IsVisible)
                    {
                        if (Kernel.MouseClick())
                            WindowManager.SetFocused(WID);

                        if (Kernel.MouseInArea(X, Y, X + Width, Y + 32))
                        {
                            if (Kernel.MouseInArea(X, Y, X + Width - 32, Y + 32) && MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left && !isHoldingHandel && WindowManager.FocusedWindow == WID)
                            { dragOffsetX = (int)MouseManager.X - X; dragOffsetY = (int)MouseManager.Y - Y; isHoldingHandel = true; }

                            if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32) && Kernel.MouseClick())
                                Close();

                            isTopDirty = true;
                        }
                    }
                }
                else if (WindowStyle == WindowStyle.Borderless)
                {
                    if (isHoldingHandel)
                    { X = (int)MouseManager.X - dragOffsetX; Y = (int)MouseManager.Y - dragOffsetY; }

                    if (Kernel.MouseInArea(X, Y, X + Width, Y + Height) && IsVisible)
                    {
                        if (Kernel.MouseClick())
                            WindowManager.SetFocused(WID);
                    }
                }

                if (Y < 32)
                    Y = 32;

                if (MouseManager.MouseState != MouseState.Left && isHoldingHandel)
                    isHoldingHandel = false;
            }

            CustomUpdate();
        }

        public void Close()
        {
            if (CanClose)
            {
                foreach(var a in OnClose) a.Invoke();
                WindowManager.Unregister(WID);
            }
        }

        public void ForceClose()
        {
            foreach (var a in OnClose) a.Invoke();
            WindowManager.Unregister(WID);
        }

        public void Maximize()
        {
            if (X == 0 && Y == 32 && Width == Kernel.screenWidth && Height == Kernel.screenHeight - 32)
                Resize(oldX, oldY, oldWidth, oldHeight);
            else
            {
                oldX = X; oldY = Y; oldWidth = Width; oldHeight = Height;
                Resize(0, 32, (int)Kernel.screenWidth, (int)Kernel.screenHeight - 32);
            }
            WindowManager.SetFocused(WID);
            IsDirty = true;
        }

        public void Minimize() => IsVisible = false;

        public void Resize(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            canvas = new DirectBitmap(width, height);
            IsDirty = true;
        }
    }
}