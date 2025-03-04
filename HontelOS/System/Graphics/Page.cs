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
        public bool IsVisible { get; set; }
        public bool HandleInput { get; set; }

        public bool FullRedrawNeeded = true;

        public Window Window;

        public Page(string title, Window window)
        {
            Title = title;
            Window = window;
            canvas = window.canvas;

            SystemEvents.OnStyleChanged.Add(() => FullRedrawNeeded = true);
        }

        public void Draw()
        {
            foreach (Control control in Controls)
                if(control.IsDirty || FullRedrawNeeded)
                    control.Draw();

            IsDirty = false;
            FullRedrawNeeded = false;
        }

        public void Update()
        {
            if (IControlContainer.ForceHandleInput)
                HandleInput = true;

            ContainerX = Window.ContainerX;
            ContainerY = Window.ContainerY;
            IsVisible = Window.IsVisible;

            OffsetX = 0; OffsetY = 0;
            if (Window.Pages.Count > 1)
                OffsetX = Window.NavBar.Width;

            foreach (Control control in Controls)
            {
                control.Update();
                if (control.IsDirty) IsDirty = true;
            }

            Window.IsDirty = IsDirty;
        }
    }
}
