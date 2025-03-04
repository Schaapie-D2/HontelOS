using Cosmos.System;
using Cosmos.System.Graphics;
using HontelOS.System.Graphics.Controls;
using System;
using System.Collections.Generic;

namespace HontelOS.System.Graphics
{
    public abstract class Control
    {
        public IControlContainer Container;

        public DirectBitmap c { get; private set; }
        public Style Style = StyleManager.Style;

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public bool IsDisabled = false;
        public bool IsSelected = false;
        public bool IsHovering = false;

        public bool IsDirty = true;
        public bool UpdateWhileInvisable = false;

        public Cursor Cursor = Cursor.Default;
        public ContextMenu ContextMenu;
        public ToolTip ToolTip;

        public List<Action> OnClick = new();
        public List<Action> OnEndClick = new();
        public List<Action> OnClickSec = new();
        public List<Action> OnStartHover = new();
        public List<Action> OnEndHover = new();
        public List<Action> OnMouseMove = new();

        public Control(IControlContainer container)
        {
            Container = container;
            c = Container.canvas;

            SystemEvents.OnStyleChanged.Add(() => { Style = StyleManager.Style; IsDirty = true; });

            Container.Controls.Add(this);
        }

        public virtual void Draw()
        {
            X += Container.OffsetX;
            Y += Container.OffsetY;
        }
        public void DoneDrawing()
        {
            X -= Container.OffsetX;
            Y -= Container.OffsetY;

            IsDirty = false;
        }
        public virtual void Update()
        {
            if ((!UpdateWhileInvisable && !Container.IsVisible) || IsDisabled)
                return;

            if (Kernel.MouseClick() && Container.HandleInput)
                IsSelected = false;

            if (Kernel.MouseInArea(Container.ContainerX + X, Container.ContainerY + Y, Container.ContainerX + X + Width, Container.ContainerY + Y + Height) && Container.HandleInput)
            {
                Kernel.cursor = Cursor;

                if (ToolTip != null)
                    ToolTip.Show();

                if (!IsHovering)
                    foreach (var a in OnStartHover) a.Invoke();

                if (MouseManager.DeltaX != 0 || MouseManager.DeltaY != 0)
                    foreach (var a in OnMouseMove) a.Invoke();

                IsHovering = true;
            }
            else
            {
                if (ToolTip != null)
                    ToolTip.Hide();

                if (IsHovering)
                    foreach (var a in OnEndHover) a.Invoke();

                IsHovering = false;
            }

            if (IsHovering && Kernel.MouseClick())
            {
                foreach(var a in OnClick) a.Invoke();
                IsSelected = true;
            }

            if (IsSelected)
            {
                if (Kernel.MouseClickSec())
                {
                    foreach (var a in OnClickSec) a.Invoke();

                    if(ContextMenu != null)
                        ContextMenu.Show();
                }

                if(MouseManager.MouseState != MouseState.Left && MouseManager.LastMouseState == MouseState.Left)
                    foreach (var a in OnEndClick) a.Invoke();
            } 
        }
    }
}