﻿using Cosmos.System.Graphics;
using System;
using HontelOS.Resources;
using HontelOS.System.Applications.About;
using HontelOS.System.Applications.Files;
using HontelOS.System.Applications.Terminal;
using System.Drawing;
using Cosmos.HAL;
using HontelOS.System.Graphics.Controls;
using HontelOS.System.Applications.Settings;

namespace HontelOS.System.Graphics
{
    public class TopBar
    {
        Canvas c = Kernel.canvas;
        Style Style = StyleManager.Style;

        Bitmap logo = ResourceManager.HontelLogo;

        Bitmap cached = null;
        bool IsDirty = true;

        public TopBar()
        {
            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; IsDirty = true; });
            SystemEvents.OnCanvasChanged.Add(() => { c = Kernel.canvas; IsDirty = true; });
            SystemEvents.MinutePassed.Add(() => { IsDirty = true; });
        }

        public void Draw()
        {
            if (!IsDirty)
            {
                c.DrawImage(cached, 0, 0);
                return;
            }

            var BG = StyleManager.BlurredBackground.GetImage(0, 0, (int)Kernel.screenWidth, 32); BG.ModifyColor(Style.TopBar_BackgroundColor, 0.2f);
            c.DrawImage(BG, 0, 0);
            c.DrawLine(Color.Gray, 0, 31, (int)Kernel.screenWidth, 31);
            c.DrawImage(logo, 4, 4, 12, 24);

            string time = $"{RTC.Hour.ToString("00")}:{RTC.Minute.ToString("00")}";
            c.DrawString(time, Style.SystemFont, Style.DefaultTextColor, (int)Kernel.screenWidth - time.Length * Style.SystemFont.Width - 5, 16 - Style.SystemFont.Height / 2);

            cached = c.GetImage(0, 0, (int)Kernel.screenWidth, 32);
            IsDirty = false;
        }

        public void Update()
        {
            if (Kernel.MouseInArea(0, 0, (int)Kernel.screenWidth, 32))
            {
                //IsDirty = true;

                if (Kernel.MouseInArea(0, 0, 32, 32) && Kernel.MouseClick())
                {
                    string[] _items = { "Files", "About", "Settings", "Terminal", "Restart", "Shutdown" };
                    Action<int>[] _actions =
                    {
                        index => new FilesProgram(),
                        index => new AboutProgram(),
                        index => new SettingsProgram(),
                        index => new TerminalProgram(),
                        index => Kernel.Reboot(),
                        index => Kernel.Shutdown(),
                    };
                    ContextMenu menu = new ContextMenu(_items, _actions, 0, 32, 150);
                    menu.Show();
                }
            }
        }
    }
}
