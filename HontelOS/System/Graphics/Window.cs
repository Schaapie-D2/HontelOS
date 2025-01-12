﻿/*
* PROJECT:          HontelOS
* CONTENT:          Control window
* PROGRAMMERS:      Jort van Dalen
*/

using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HontelOS.System.Graphics
{
    public class Window
    {
        Canvas c = Kernel.canvas;
        public Style Style = Kernel.style;

        public string Title;
        public Image Icon;
        public WindowStyle WindowStyle;
        public int WID;

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int ViewX;
        public int ViewY;

        public bool IsVisable = true;
        public bool CanClose = true;

        public bool IsDirty = true;

        public bool DisableMaximizeButton = false;
        public bool DisableMinimizeButton = false;

        public List<Control> Controls = new List<Control>();

        public List<Action> OnClose = new();

        public Color BackgroundColor;

        bool isHoldingHandel = false;

        Bitmap cachedViewArea = null;
        bool isCacheInCue = false;
        bool isReadyToCache = true;
        bool isReadyToDrawCache = true;

        int oldX;
        int oldY;
        int oldWidth;
        int oldHeight;

        int dragOffsetX;
        int dragOffsetY;

        public Window(string title, WindowStyle windowStyle, int x, int y, int width, int height)
        {
            Title = title;
            WindowStyle = windowStyle;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            BackgroundColor = Style.Window_BackgroundColor;
        }

        public virtual void CustomUpdate() { return; }

        public void DrawWindow()
        {
            if (!IsVisable)
                return;

            c.DrawRoundedRectangle(Color.Black, X - 1, Y - 1, Width + 2, Height + 34, 10);

            if (WindowStyle == WindowStyle.Normal)
            {
                c.DrawFilledRectangle(BackgroundColor, X, Y + 32, Width, Height, true);//view area
                c.DrawFilledTopRoundedRectangle(Style.Window_HandleColor, X, Y, Width, 32, 10);//handel

                if (Icon != null)
                {
                    c.DrawImage(Icon, X + 10, Y + 10, 16, 16, true);//icon
                    c.DrawString(Title, PCScreenFont.Default, Color.Black, X + 32, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title
                }
                else
                    c.DrawString(Title, PCScreenFont.Default, Color.Black, X + 10, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title

                if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32))
                    c.DrawFilledRectangle(Color.Red, X + Width - 32, Y, 32, 32, true);//red glow
                c.DrawString("X", PCScreenFont.Default, Color.Black, X + Width - 32 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//close
                if (Kernel.MouseInArea(X + Width - 64, Y, X + Width - 32, Y + 32))
                    c.DrawFilledRectangle(Style.Window_HandleButtonGlowColor, X + Width - 64, Y, 32, 32, true);//gray glow
                c.DrawString("+", PCScreenFont.Default, Color.Black, X + Width - 64 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//maximize
                if (Kernel.MouseInArea(X + Width - 96, Y, X + Width - 64, Y + 32))
                    c.DrawFilledRectangle(Style.Window_HandleButtonGlowColor, X + Width - 96, Y, 32, 32, true);//gray glow
                c.DrawString("-", PCScreenFont.Default, Color.Black, X + Width - 96 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//minimize
            }
            else if (WindowStyle == WindowStyle.Dialog)
            {
                c.DrawFilledRectangle(BackgroundColor, X, Y + 32, Width, Height, true);//view area
                c.DrawFilledTopRoundedRectangle(Style.Window_HandleColor, X, Y, Width, 32, 10);//handel

                if (Icon != null)
                {
                    c.DrawImage(Icon, X + 10, Y + 10, 16, 16, true);//icon
                    c.DrawString(Title, PCScreenFont.Default, Color.Black, X + 32, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title
                }
                else
                    c.DrawString(Title, PCScreenFont.Default, Color.Black, X + 10, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//title

                if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32))
                    c.DrawFilledRectangle(Color.Red, X + Width - 32, Y, 32, 32, true);//red glow
                c.DrawString("X", PCScreenFont.Default, Color.Black, X + Width - 32 + 16 - PCScreenFont.Default.Width / 2, Y + 32 / 2 - PCScreenFont.Default.Height / 2);//close
            }
            else if (WindowStyle == WindowStyle.Borderless)
            {
                c.DrawFilledRectangle(BackgroundColor, X, Y, Width, Height + 32, true);//view area
            }

            if (!IsDirty && cachedViewArea != null && isReadyToDrawCache)
            {
                if (WindowStyle != WindowStyle.Borderless)
                    c.DrawImage(cachedViewArea, X, Y + 32, true);
                else
                    c.DrawImage(cachedViewArea, X, Y, true);

                return;
            }

            foreach (Control c in Controls)
                c.Draw();

            if (IsVisable || IsVisable && isCacheInCue)
            {
                if (isReadyToCache)
                {
                    if (WindowStyle != WindowStyle.Borderless)
                        cachedViewArea = c.GetImage(X, Y + 32, Width, Height);
                    else
                        cachedViewArea = c.GetImage(X, Y, Width, Height);

                    isReadyToDrawCache = true;
                }
            }

            IsDirty = false;
        }

        public void UpdateWindow()
        {
            if (Kernel.MouseInArea(X, Y, X + Width, Y + Height) && Kernel.MouseClick())
                WindowManager.SetFocused(WID);

            if (WindowStyle == WindowStyle.Normal)
            {
                isReadyToCache = !Kernel.MouseInArea(X, Y + 32, X + Width, Y + Height + 32);
                if (!isReadyToCache)
                    isReadyToDrawCache = false;

                if (Kernel.MouseInArea(X, Y, X + Width, Y + Height))
                {
                    if (Kernel.MouseInArea(X, Y, X + Width - 96, Y + 32) && MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left && !isHoldingHandel && WindowManager.FocusedWindow == WID)
                    { dragOffsetX = X - (int)MouseManager.X; dragOffsetY = Y - (int)MouseManager.Y; isHoldingHandel = true; }

                    if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32) && Kernel.MouseClick())
                        Close();
                    if (Kernel.MouseInArea(X + Width - 64, Y, X + Width - 32, Y + 32) && Kernel.MouseClick())
                        Maximize();
                    if (Kernel.MouseInArea(X + Width - 96, Y, X + Width - 64, Y + 32) && Kernel.MouseClick())
                        Minimize();
                }

                if (isHoldingHandel)
                { X = (int)MouseManager.X + dragOffsetX; Y = (int)MouseManager.Y + dragOffsetY; }
            }
            else if (WindowStyle == WindowStyle.Dialog)
            {
                isReadyToCache = !Kernel.MouseInArea(X, Y + 32, X + Width, Y + Height + 32);
                if (!isReadyToCache)
                    isReadyToDrawCache = false;

                if (Kernel.MouseInArea(X, Y, X + Width, Y + Height))
                {
                    if (Kernel.MouseInArea(X, Y, X + Width - 32, Y + 32) && MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState != MouseState.Left && !isHoldingHandel && WindowManager.FocusedWindow == WID)
                    { dragOffsetX = (int)MouseManager.X - X; dragOffsetY = (int)MouseManager.Y - Y; isHoldingHandel = true; }

                    if (Kernel.MouseInArea(X + Width - 32, Y, X + Width, Y + 32) && Kernel.MouseClick())
                        Close();
                }

                if (isHoldingHandel)
                { X = (int)MouseManager.X - dragOffsetX; Y = (int)MouseManager.Y - dragOffsetY; }
            }
            else if (WindowStyle == WindowStyle.Borderless)
            {
                isReadyToCache = !Kernel.MouseInArea(X, Y, X + Width, Y + Height);
                if (!isReadyToCache)
                    isReadyToDrawCache = false;
            }

            if (Y < 32)
                Y = 32; 

            ViewX = X; ViewY = Y + 32;

            if (MouseManager.MouseState != MouseState.Left && isHoldingHandel)
                isHoldingHandel = false;

            foreach (Control c in Controls)
                c.Update();

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
            { X = oldX; Y = oldY; Width = oldWidth; Height = oldHeight; }
            else
            {
                oldX = X; oldY = Y; oldWidth = Width; oldHeight = Height;
                X = 0; Y = 32; Width = (int)Kernel.screenWidth; Height = (int)Kernel.screenHeight - 32;
            }
            WindowManager.SetFocused(WID);
            IsDirty = true;
        }

        public void Minimize() => IsVisable = false;
    }

    public enum WindowStyle
    {
        /// <summary>
        /// A window with a handel and view area with the minimize, maximize and close buttons
        /// </summary>
        Normal,
        /// <summary>
        /// The same as normal but only with the close button
        /// </summary>
        Dialog,
        /// <summary>
        /// A window only with a view area
        /// </summary>
        Borderless
    }
}