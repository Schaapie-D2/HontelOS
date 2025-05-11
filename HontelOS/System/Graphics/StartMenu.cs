using System;
using System.Collections.Generic;
using System.Linq;
using Cosmos.System;
using Cosmos.System.Graphics;
using HontelOS.Resources;
using HontelOS.System.Applications.About;
using HontelOS.System.Applications.Files;
using HontelOS.System.Applications.Settings;
using HontelOS.System.Applications.Terminal;
using HontelOS.System.Applications.Debugging;
using HontelOS.System.Graphics.Controls;
using HontelOS.System.Input;

namespace HontelOS.System.Graphics
{
    public class StartMenu
    {
        Canvas c = Kernel.canvas;
        Style Style = StyleManager.Style;

        public int MenuWidth = 800;
        public int MenuHeight = 600;

        public bool IsVisible = false;
        public bool OldIsVisible = false;

        public List<(Bitmap icon, string title, string execPath)> Apps = new List<(Bitmap icon, string title, string execPath)>
        {
            (ResourceManager.FolderIcon, "Files", "HontelOS.Files"),
            (null, "Terminal", "HontelOS.Terminal"),
            (null, "Settings", "HontelOS.Settings"),
            (null, "About this PC", "HontelOS.About"),
            (null, "About HontelOS", "HontelOS.AboutHontelOS"),
            (null, "Logs", "HontelOS.Logs")
        };

        Bitmap cached = null;

        bool IsDirty = true;

        string searchText = "";
        bool isSearchBarSelected = false;

        int appsListHoverIndex = -1;

        public StartMenu()
        {
            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; IsDirty = true; });
            SystemEvents.OnCanvasChanged.Add(() => { c = Kernel.canvas; IsDirty = true; });
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            int X = 10;
            int Y = (int)(Kernel.screenHeight - Kernel.taskBar.taskBarHeight - 10 - MenuHeight);

            if (!IsDirty)
            {
                c.DrawImage(cached, X, Y);
                return;
            }

            // Background
            c.DrawFilledRoundedRectangle(Style.StartMenu_BackgroundColor, X, Y, MenuWidth, MenuHeight, 10);

            // Search bar
            c.DrawFilledRoundedRectangle(Style.StartMenu_ContentColor, X + 20 + Kernel.taskBar.taskBarHeight, Y + 10, MenuWidth - 30 - Kernel.taskBar.taskBarHeight, 32, 10);
            if (searchText.Length != 0)
                c.DrawString(searchText, Style.SystemFont, Style.TextBox_TextColor, X + 20 + Kernel.taskBar.taskBarHeight + 5, Y + 10 + 16 - Style.SystemFont.Height / 2);
            else
                c.DrawString("Search...", Style.SystemFont, Style.TextBox_PlaceholderTextColor, X + 20 + Kernel.taskBar.taskBarHeight + 5, Y + 10 + 16 - Style.SystemFont.Height / 2);

            // Apps
            c.DrawFilledRoundedRectangle(Style.StartMenu_ContentColor, X + 20 + Kernel.taskBar.taskBarHeight, Y + 20 + 32, (MenuWidth - 30 - Kernel.taskBar.taskBarHeight) / 3, MenuHeight - 30 - 32, 10);
            for(int i = 0; i < Search().Count; i++)
            {
                int appX = X + 20 + Kernel.taskBar.taskBarHeight + 5;
                int appY = Y + 20 + 32 + 5 + (32 + 5) * i;

                if(i == appsListHoverIndex)
                    c.DrawFilledRoundedRectangle(Style.StartMenu_BackgroundColor, appX, appY, (MenuWidth - 30 - Kernel.taskBar.taskBarHeight) / 3 - 10, 32, 10);

                c.DrawImage(Search()[i].icon ?? ResourceManager.ApplicationIcon, appX + 5, appY + 5, 32 - 10, 32 - 10);
                c.DrawString(Search()[i].title, Style.SystemFont, Style.DefaultTextColor, appX + 5 + 32 - 10 + 5, appY + 16 - Style.SystemFont.Height / 2);
            }

            // Power button
            c.DrawFilledRoundedRectangle(Style.StartMenu_ContentColor, X + 10, Y + MenuHeight - 10 - Kernel.taskBar.taskBarHeight, Kernel.taskBar.taskBarHeight, Kernel.taskBar.taskBarHeight, 10);
            c.DrawImage(ResourceManager.PowerIcon, X + 15, Y + MenuHeight - 5 - Kernel.taskBar.taskBarHeight, Kernel.taskBar.taskBarHeight - 10, Kernel.taskBar.taskBarHeight - 10);

            // Files app
            c.DrawFilledRoundedRectangle(Style.StartMenu_ContentColor, X + 10, Y + MenuHeight - 20 - Kernel.taskBar.taskBarHeight * 2, Kernel.taskBar.taskBarHeight, Kernel.taskBar.taskBarHeight, 10);
            c.DrawImage(ResourceManager.FolderIcon, X + 15, Y + MenuHeight - 15 - Kernel.taskBar.taskBarHeight * 2, Kernel.taskBar.taskBarHeight - 10, Kernel.taskBar.taskBarHeight - 10);

            cached = c.GetImage(X, Y, MenuWidth, MenuHeight);

            IsDirty = false;
        }

        public void Update()
        {
            OldIsVisible = IsVisible;

            if (KeyboardManagerExt.GetKey(ConsoleKeyEx.LWin) || KeyboardManagerExt.GetKey(ConsoleKeyEx.RWin))
            {
                ToggleVisibility();
            }

            if (!IsVisible)
                return;

            int X = 10;
            int Y = (int)(Kernel.screenHeight - Kernel.taskBar.taskBarHeight - 10 - MenuHeight);

            if (Kernel.MouseClick())
                isSearchBarSelected = false;

            if (Kernel.MouseInArea(X, Y, X + MenuWidth, Y + MenuHeight))
            {
                if (Kernel.MouseInArea(X + 20 + Kernel.taskBar.taskBarHeight, Y + 10, X + 20 + Kernel.taskBar.taskBarHeight + MenuWidth - 30 - Kernel.taskBar.taskBarHeight, Y + 10 + 32))
                {
                    if (Kernel.MouseClick())
                        isSearchBarSelected = true;

                    Kernel.cursor = Cursor.Text;
                }

                if (KeyboardManagerExt.KeyAvailable && isSearchBarSelected)
                {
                    var key = KeyboardManagerExt.ReadKey();

                    if (key.Key == ConsoleKeyEx.Backspace && searchText.Length != 0)
                        searchText = searchText.Remove(searchText.Length - 1);
                    else if (key.Key == ConsoleKeyEx.Enter)
                        Search();
                    else if (!char.IsControl(key.KeyChar))
                        searchText += key.KeyChar;

                    IsDirty = true;
                }

                if (Kernel.MouseInArea(X + 10, Y + MenuHeight - 10 - Kernel.taskBar.taskBarHeight, X + 10 + Kernel.taskBar.taskBarHeight, Y + MenuHeight - 10))
                {
                    if (Kernel.MouseClick())
                    {
                        string[] _items = { "Shutdown", "Restart" };
                        Action<int>[] _actions =
                        {
                            index => Kernel.Shutdown(),
                            index => Kernel.Reboot(),
                        };
                        ContextMenu menu = new ContextMenu(_items, _actions, (int)MouseManager.X + 1, (int)MouseManager.Y + 1, 150);
                        menu.Show();
                    }
                }

                if(Kernel.MouseInArea(X + 10, Y + MenuHeight - 20 - Kernel.taskBar.taskBarHeight * 2, X + 10 + Kernel.taskBar.taskBarHeight, Y + MenuHeight - 20 - Kernel.taskBar.taskBarHeight))
                {
                    if (Kernel.MouseClick())
                    {
                        new FilesProgram();
                        IsVisible = false;
                    }
                }

                bool wasHoverSet = false;

                for (int i = 0; i < Search().Count; i++)
                {
                    int appX = X + 20 + Kernel.taskBar.taskBarHeight + 5;
                    int appY = Y + 20 + 32 + 5 + (32 + 5) * i;

                    if (Kernel.MouseInArea(appX, appY, appX + (MenuWidth - 30 - Kernel.taskBar.taskBarHeight) / 3 - 10, appY + 32))
                    {
                        if(appsListHoverIndex != i)
                            IsDirty = true;

                        if (Kernel.MouseClick())
                        {
                            OpenApp(Search()[i].execPath);
                            IsVisible = false;
                        }

                        appsListHoverIndex = i;
                        wasHoverSet = true;
                    }
                }

                if (!wasHoverSet && appsListHoverIndex != -1)
                {
                    IsDirty = true;
                    appsListHoverIndex = -1;
                }
            }
            else
            {
                if (Kernel.MouseClick())
                {
                    IsVisible = false;
                }
            }
        }

        public void ToggleVisibility()
        {
            IsVisible = !OldIsVisible;
        }

        List<(Bitmap icon, string title, string execPath)> Search()
        {
            if(string.IsNullOrEmpty(searchText))
                return Apps;

            return Apps.Where(app => app.title.ToLower().Contains(searchText.ToLower())).ToList();
        }

        void OpenApp(string execPath)
        {
            switch (execPath)
            {
                case "HontelOS.Files":
                    new FilesProgram();
                    break;
                case "HontelOS.Terminal":
                    new TerminalProgram();
                    break;
                case "HontelOS.Settings":
                    new SettingsProgram();
                    break;
                case "HontelOS.About":
                    new AboutProgram();
                    break;
                case "HontelOS.AboutHontelOS":
                    new AboutHontelOSProgram();
                    break;
                case "HontelOS.Logs":
                    new LogsProgram();
                    break;
                default:
                    throw new Exception($"App {execPath} not found.");
            }
        }
    }
}