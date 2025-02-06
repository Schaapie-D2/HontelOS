/*
* PROJECT:          HontelOS
* CONTENT:          About HontelOS program for HontelOS
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
using HontelOS.System.Graphics;
using HontelOS.Resources;
using Cosmos.System.Graphics;

namespace HontelOS.System.Applications.About
{
    public class AboutHontelOSProgram : Window
    {
        PictureBox logoCon;

        public AboutHontelOSProgram() : base("About HontelOS", WindowStyle.Dialog, (int)Kernel.screenWidth / 2 - 150, (int)Kernel.screenHeight / 2 - 250, 600, 450)
        {
            Bitmap logo = Style.StyleType == StyleType.Light ? ResourceManager.HontelOSLogoBlack : ResourceManager.HontelOSLogoWhite;
            Page p = Pages[0];

            logoCon = new PictureBox(logo, 25, 25, 550, 195, p);

            new Label("HontelOS " + VersionInfo.Version + $" ({VersionInfo.VersionNumber})", null, Color.Empty, 25, 235, p);

            new Label("Developed by Jort van Dalen", null, Color.Empty, 25, 235 + Style.SystemFont.Height * 2, p);
            new Label("Source code is available at:", null, Color.Empty, 25, 235 + Style.SystemFont.Height * 3, p);
            new Label("https://github.com/Schaapie-D2/HontelOS", null, Color.Empty, 25, 235 + Style.SystemFont.Height * 4, p);

            new Label("Copyright (c) 2025 Jort van Dalen", null, Color.Empty, 25, 235 + Style.SystemFont.Height * 6, p);
            new Label("All rights reserved", null, Color.Empty, 25, 235 + Style.SystemFont.Height * 7, p);

            new Label("Made with Cosmos:", null, Color.Empty, 25, 235 + Style.SystemFont.Height * 9, p);
            new Label("https://github.com/CosmosOS/Cosmos", null, Color.Empty, 25, 235 + Style.SystemFont.Height * 10, p);

            WindowManager.Register(this);

            SystemEvents.OnStyleChanged.Add(() => { logoCon.Image = Style.StyleType == StyleType.Light ? ResourceManager.HontelOSLogoBlack : ResourceManager.HontelOSLogoWhite; });
        }
    }
}
