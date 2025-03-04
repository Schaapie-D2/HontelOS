using System.Collections.Generic;

namespace HontelOS.System.Graphics
{
    public interface IControlContainer
    {
        public List<Control> Controls { get; set; }
        public int ContainerX { get; set; }
        public int ContainerY { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool IsDirty { get; set; }
        public bool IsVisible { get; set; }
        public bool HandleInput { get; set; }
        public static bool ForceHandleInput;
        public DirectBitmap canvas { get; set; }
    }
}
