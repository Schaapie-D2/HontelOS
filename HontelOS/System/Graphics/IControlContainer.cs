/*
* PROJECT:          HontelOS
* CONTENT:          Control container interface
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
    public interface IControlContainer
    {
        public List<Control> Controls { get; set; }
        public int ContainerX { get; set; }
        public int ContainerY { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool IsDirty { get; set; }
        public DirectBitmap canvas { get; set; }
    }
}
