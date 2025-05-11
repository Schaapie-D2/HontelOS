using Cosmos.System.Graphics;
using HontelOS.Resources;
using HontelOS.System.Graphics.Controls;
using System.Collections.Generic;
using System.Linq;
using Cosmos.HAL;

namespace HontelOS.System.Graphics
{
    public class TaskBar
    {
        Canvas c = Kernel.canvas;
        Style Style = StyleManager.Style;

        public int taskBarHeight = 48;

        public Dictionary<int, ToolTip> windowTooltips = new Dictionary<int, ToolTip>();
        ToolTip startMenuToolTip = new ToolTip("Start menu", ToolTip.ToolTipOrginDirection.Down, 0, 0);

        Bitmap startMenuIcon = ResourceManager.StartMenuIcon;
        Bitmap applicationIcon = ResourceManager.ApplicationIcon;

        Bitmap cached = null;
        bool IsDirty = true;

        public TaskBar()
        {
            WindowManager.OnWindowsListUpdate.Add(OnItemsUpdate);

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; IsDirty = true; });
            SystemEvents.OnCanvasChanged.Add(() => { c = Kernel.canvas; OnItemsUpdate(); IsDirty = true; });
            SystemEvents.MinutePassed.Add(() => { IsDirty = true; });
        }

        public void Draw()
        {
            int sizY = taskBarHeight;
            int posY = (int)Kernel.screenHeight - sizY;

            if (!IsDirty)
            {
                c.DrawImage(cached, 0, posY - 1);
                return;
            }

            var BG = StyleManager.BlurredBackground.GetImage(0, posY, (int)Kernel.screenWidth, sizY); BG.ModifyColor(Style.TaskBar_BackgroundColor, 0.8f);
            c.DrawImage(BG, 0, posY, (int)Kernel.screenWidth, sizY);

            c.DrawLine(Style.TaskBar_BorderColor, 0, posY - 1, (int)Kernel.screenWidth, posY - 1);

            c.DrawImage(startMenuIcon, 4, posY + 4, sizY - 8, sizY - 8);

            for (int i = 0; i < WindowManager.Windows.Count; i++)
            {
                if (WindowManager.Windows.Values.ToList()[i].Icon != null)
                    c.DrawImage(WindowManager.Windows.Values.ToList()[i].Icon, sizY + 4 + i * sizY, posY + 4, sizY - 8, sizY - 8);
                else
                    c.DrawImage(applicationIcon, sizY + 4 + i * sizY, posY + 4, sizY - 8, sizY - 8);
            }

            string time = $"{RTC.Hour.ToString("00")}:{RTC.Minute.ToString("00")}";
            c.DrawString(time, Style.SystemFont, Style.DefaultTextColor, (int)Kernel.screenWidth - time.Length * Style.SystemFont.Width - 5, posY + (taskBarHeight / 3 - Style.SystemFont.Height / 2));
            string date = $"{RTC.DayOfTheMonth}-{RTC.Month}-{RTC.Century}{RTC.Year}";
            c.DrawString(date, Style.SystemFont, Style.DefaultTextColor, (int)Kernel.screenWidth - date.Length * Style.SystemFont.Width - 5, posY + (taskBarHeight / 3 * 2 - Style.SystemFont.Height / 2));

            cached = c.GetImage(0, posY - 1, (int)Kernel.screenWidth, sizY + 1);
            IsDirty = false;
        }

        public void Update()
        {
            int sizY = taskBarHeight;
            int posY = (int)Kernel.screenHeight - sizY;

            startMenuToolTip.Hide();

            foreach (var tt in windowTooltips.Values)
                tt.Hide();

            if (!Kernel.MouseInArea(0, posY, (int)Kernel.screenWidth, (int)Kernel.screenHeight))
                return;

            if (Kernel.MouseInArea(0, posY, sizY, (int)Kernel.screenHeight))
            {
                startMenuToolTip.Show();

                if (Kernel.MouseClick())
                    Kernel.startMenu.ToggleVisibility();
            }

            for (int i = 0; i < WindowManager.Windows.Count; i++)
            {
                int iconPosX = sizY + i * sizY;
                int iconPosY = posY;
                int iconSize = sizY;

                if (Kernel.MouseInArea(iconPosX, iconPosY, iconPosX + iconSize, iconPosY + iconSize))
                {
                    windowTooltips[WindowManager.Windows.Keys.ToList()[i]].Show();

                    if (Kernel.MouseClick())
                    {
                        WindowManager.SetFocused(WindowManager.Windows.Values.ToList()[i].WID);
                        WindowManager.Windows.Values.ToList()[i].IsVisible = !WindowManager.Windows.Values.ToList()[i].IsVisible;
                    }
                }
            }
        }

        void OnItemsUpdate()
        {
            int sizY = taskBarHeight;
            int posY = (int)Kernel.screenHeight - sizY;

            windowTooltips.Clear();

            startMenuToolTip.OrginX = sizY / 2;
            startMenuToolTip.OrginY = posY;

            for (int i = 0; i < WindowManager.Windows.Count; i++)
                windowTooltips.Add(WindowManager.Windows.Keys.ToList()[i], new ToolTip(WindowManager.Windows.Values.ToList()[i].Title, ToolTip.ToolTipOrginDirection.Down, sizY + sizY / 2 + i * sizY, posY));

            IsDirty = true;
        }
    }
}
