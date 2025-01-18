﻿using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HontelOS.System.Graphics
{
    public interface IWindow
    {
        public string Title { get; set; }
        public Image Icon { get; set; }
        public int WID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsVisable { get; set; }
        public bool CanClose { get; set; }
        public bool IsDirty { get; set; }

        public void DrawWindow();
        public void UpdateWindow();
        public void Close();
        public void ForceClose();
        public void Maximize();
        public void Minimize();
        public void Resize(int x, int y, int width, int height);
    }
}
