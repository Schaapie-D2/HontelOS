/*
* PROJECT:          HontelOS
* CONTENT:          Window page
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System.Collections.Generic;

namespace HontelOS.System.Graphics
{
    public class Page : IControlContainer
    {
        public string Title;
        public DirectBitmap canvas { get; set; }
        public List<Control> Controls { get; set; } = new List<Control>();
        public int ContainerX { get; set; }
        public int ContainerY { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool IsDirty { get; set; } = true;

        public Window Window;

        public Page(string title, Window window)
        {
            Title = title;
            Window = window;
            canvas = window.canvas;
        }

        public void Draw()
        {
            foreach (Control control in Controls)
                control.Draw();

            IsDirty = false;
        }

        public void Update()
        {
            ContainerX = Window.ContainerX;
            ContainerY = Window.ContainerY;

            OffsetX = 0; OffsetY = 0;
            if (Window.Pages.Count > 1)
                OffsetX = Window.NavBar.Width;

            foreach (Control control in Controls)
                control.Update();

            Window.IsDirty = IsDirty;
        }
    }
}
