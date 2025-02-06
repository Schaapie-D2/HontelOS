/*
* PROJECT:          HontelOS
* CONTENT:          About this PC program for HontelOS
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System.Drawing;
using HontelOS.System.Graphics.Controls;
using Cosmos.Core;
using HontelOS.System.Graphics;

namespace HontelOS.System.Applications.About
{
    public class AboutProgram : Window
    {
        public AboutProgram() : base("About this PC", WindowStyle.Dialog, (int)Kernel.screenWidth / 2 - 150, (int)Kernel.screenHeight / 2 - 250, 600, 300)
        {
            Page p = Pages[0];

            new Label("HontelOS " + VersionInfo.Version + $" ({VersionInfo.VersionNumber})", null, Color.Empty, 25, 25, p);

            new Label("CPU: " + CPU.GetCPUBrandString(), null, Color.Empty, 25, 25 + Style.SystemFont.Height * 2, p);
            new Label("RAM: " + StorageSizeConverter.AutoConvert(StorageSize.Megabyte, (long)GCImplementation.GetAvailableRAM()).Item3, null, Color.Empty, 25, 25 + Style.SystemFont.Height * 3, p);
            new Label("Storage: " + StorageSizeConverter.AutoConvert(StorageSize.Byte, Kernel.fileSystem.GetTotalSize("0:\\")).Item3, null, Color.Empty, 25, 25 + Style.SystemFont.Height * 4, p);

            new Button("About HontelOS", () => new AboutHontelOSProgram(), 10, Height - 35, 125, 25, p);

            WindowManager.Register(this);
        }
    }
}
