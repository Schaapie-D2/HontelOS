﻿using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HontelOS.System.Graphics
{
    public class ConsoleWindow : IWindow
    {
        Canvas c = Kernel.canvas;
        public Console console;
        public Style Style = StyleManager.Style;

        public string Title { get; set; }
        public Image Icon { get; set; }
        public int WID { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int ControlDrawOffsetX;
        public int ControlDrawOffsetY;
        public int ControlDrawOffsetXUpd;
        public int ControlDrawOffsetYUpd;

        public bool IsVisible { get; set; } = true;
        public bool CanClose { get; set; } = true;

        public bool IsDirty { get; set; } = true;

        public bool DisableMaximizeButton = false;
        public bool DisableMinimizeButton = false;

        public List<Action> OnClose { get; set; } = new();
        public List<Action> OnResize { get; set; } = new();

        bool isHoldingHandel = false;

        Bitmap cachedTop = null;
        bool isTopDirty = true;

        int oldX;
        int oldY;
        int oldWidth;
        int oldHeight;

        int dragOffsetX;
        int dragOffsetY;

        public ConsoleWindow(string title, int x, int y, int width, int height)
        {
            Title = title;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            console = new Console(width, height);

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; IsDirty = true; isTopDirty = true; });
            SystemEvents.OnCanvasChanged.Add(() => { c = Kernel.canvas; IsDirty = true; isTopDirty = true; });
        }

        public virtual void CustomUpdate() { return; }

        public void DrawWindow()
        {
            if (!IsVisible)
                return;

            // Border
            c.DrawRoundedRectangle(Color.Black, X - 1, Y - 1, Width + 1, 33, 10);
            c.DrawRectangle(Color.Black, X - 1, Y + 16, Width + 1, Height + 16);

            // Top
            if (isTopDirty)
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

                cachedTop = c.GetImage(X, Y, Width, 32);

                isTopDirty = false;
            }
            else
            {
                c.DrawImage(cachedTop, X, Y, true);
            }

            // Content
            if (IsDirty)
                console.Draw();

            c.DrawImage(console.canvas.Bitmap, X, Y + 32, true);

            console.IsDirty = false;
        }

        public void UpdateWindow()
        {
            if (IsVisible)
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

                ControlDrawOffsetX = X; ControlDrawOffsetY = Y + 32;

                if (MouseManager.MouseState != MouseState.Left && isHoldingHandel)
                    isHoldingHandel = false;

                console.IsSelected = WindowManager.FocusedWindow == WID;

                //if(MouseManager.ScrollDelta != 0 && console.IsSelected)
                //{
                //    if (MouseManager.ScrollDelta == 1)
                //        console.ScrollDown();
                //    else
                //        console.ScrollUp();
                //}

                console.Update();
            }

            IsDirty = console.IsDirty;
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
            if (X == 0 && Y == 0 && Width == Kernel.screenWidth && Height == Kernel.screenHeight - Kernel.taskBar.taskBarHeight)
                Resize(oldX, oldY, oldWidth, oldHeight);
            else
            {
                oldX = X; oldY = Y; oldWidth = Width; oldHeight = Height;
                Resize(0, 0, (int)Kernel.screenWidth, (int)Kernel.screenHeight - Kernel.taskBar.taskBarHeight);
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
            console.Resize(width, height);
            foreach (var a in OnResize) a.Invoke();
            IsDirty = true;
        }
    }
}