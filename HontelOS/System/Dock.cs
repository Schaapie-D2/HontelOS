/*
* PROJECT:          HontelOS
* CONTENT:          Dock element
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System.Graphics;
using HontelOS.Resources;
using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;
using System.Collections.Generic;
using System.Linq;

namespace HontelOS.System
{
    public class Dock
    {
        Canvas c = Kernel.canvas;
        Style Style = StyleManager.Style;

        public int dockWidth = 74;
        public int items = 1;
        public Dictionary<int, ToolTip> windowTooltips = new Dictionary<int, ToolTip>();
        ToolTip launchPadToolTip = new ToolTip("Launch pad", ToolTip.ToolTipOrginDirection.Down, 0, 0);

        Bitmap appList = ResourceManager.SystemAppListIcon;
        Bitmap applicationIcon = ResourceManager.SystemApplicationIcon;

        Bitmap cached = null;
        bool IsDirty = true;

        public Dock()
        {
            WindowManager.OnWindowsListUpdate.Add(OnItemsUpdate);

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; IsDirty = true; });
            SystemEvents.OnCanvasChanged.Add(() => { c = Kernel.canvas; OnItemsUpdate(); IsDirty = true; });
        }

        public void Draw()
        {
            int posX = (int)Kernel.screenWidth / 2 - dockWidth / 2;
            int posY = (int)Kernel.screenHeight - 94;
            int sizX = dockWidth;
            int sizY = 84;

            if (!IsDirty)
            {
                c.DrawImage(cached, posX, posY, sizX, sizY);
                return;
            }

            c.DrawFilledRoundedRectangle(Style.Dock_BackgroundColor, posX, posY, sizX, sizY, 10);

            c.DrawImage(appList, posX + 10, posY + 10, 64, 64);

            for (int i = 0; i < WindowManager.Windows.Count; i++)
            {
                if (WindowManager.Windows.Values.ToList()[i].Icon != null)
                    c.DrawImage(WindowManager.Windows.Values.ToList()[i].Icon, posX + 84 + i * 74, posY + 10, 64, 64);
                else
                    c.DrawImage(applicationIcon, posX + 84 + i * 74, posY + 10, 64, 64);
            }

            cached = c.GetImage(posX, posY, sizX, sizY);
            IsDirty = false;
        }

        public void Update()
        {
            items = WindowManager.Windows.Count + 1;
            dockWidth = items * 74 + 10;
            int posX = (int)Kernel.screenWidth / 2 - dockWidth / 2;
            int posY = (int)Kernel.screenHeight - 94;

            launchPadToolTip.Hide();

            foreach (var tt in windowTooltips.Values)
                tt.Hide();

            if (!Kernel.MouseInArea(posX, posY, posX + dockWidth, posY + 20 + 64))
                return;

            if (Kernel.MouseInArea(posX + 10, posY + 10, posX + 10 + 64, posY + 10 + 64))
            {
                launchPadToolTip.Show();

                if (Kernel.MouseClick())
                    Kernel.appListVisable = !Kernel.appListVisable;

                IsDirty = true;
            }

            for (int i = 0; i < WindowManager.Windows.Count; i++)
            {
                int iconPosX = posX + 84 + i * 74;
                int iconPosY = posY + 10;

                if (Kernel.MouseInArea(iconPosX, iconPosY, iconPosX + 64, iconPosY + 64))
                {
                    windowTooltips[WindowManager.Windows.Keys.ToList()[i]].Show();

                    if (Kernel.MouseClick())
                    {
                        WindowManager.SetFocused(WindowManager.Windows.Values.ToList()[i].WID);
                        WindowManager.Windows.Values.ToList()[i].IsVisable = !WindowManager.Windows.Values.ToList()[i].IsVisable;
                    }
                }
            }
        }

        void OnItemsUpdate()
        {
            items = WindowManager.Windows.Count + 1;
            dockWidth = items * 74 + 10;
            int posX = (int)Kernel.screenWidth / 2 - dockWidth / 2;
            int posY = (int)Kernel.screenHeight - 104;

            windowTooltips.Clear();

            launchPadToolTip.OrginX = posX + 38;
            launchPadToolTip.OrginY = posY;

            for (int i = 0; i < WindowManager.Windows.Count; i++)
                windowTooltips.Add(WindowManager.Windows.Keys.ToList()[i], new ToolTip(WindowManager.Windows.Values.ToList()[i].Title, ToolTip.ToolTipOrginDirection.Down, posX + 79 + 32 + i * 74, posY));

            IsDirty = true;
        }
    }
}
