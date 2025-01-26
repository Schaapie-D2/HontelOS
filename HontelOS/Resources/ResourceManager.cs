﻿/*
* PROJECT:          HontelOS
* CONTENT:          Resource manager
* PROGRAMMERS:      Jort van Dalen
*/


using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace HontelOS.Resources
{
    public class ResourceManager
    {
        #region Images
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.Hontel_Logo.bmp")] readonly static byte[] Image__Hontel_LogoRaw; public readonly static Bitmap HontelLogo = new Bitmap(Image__Hontel_LogoRaw, ColorOrder.RGB);
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.HontelOS_Logo_Black.bmp")] readonly static byte[] Image__HontelOS_Logo_BlackRaw; public readonly static Bitmap HontelOSLogoBlack = new Bitmap(Image__HontelOS_Logo_BlackRaw, ColorOrder.RGB);
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.HontelOS_Logo_White.bmp")] readonly static byte[] Image__HontelOS_Logo_WhiteRaw; public readonly static Bitmap HontelOSLogoWhite = new Bitmap(Image__HontelOS_Logo_WhiteRaw, ColorOrder.RGB);

        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.BG1.bmp")] readonly static byte[] Image__Background_1Raw; public readonly static Bitmap Background1 = new Bitmap(Image__Background_1Raw, ColorOrder.RGB);
        #endregion
        #region Audio
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Audio.boot.wav")] readonly static byte[] Audio__BootRaw; public readonly static AudioStream BootSound = MemoryAudioStream.FromWave(Audio__BootRaw);
        #endregion
        #region System
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System_App_List.bmp")] public readonly static byte[] Image__System_App_ListRaw; public readonly static Bitmap SystemAppListIcon = new Bitmap(Image__System_App_ListRaw, ColorOrder.RGB);

        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System_Application.bmp")] public readonly static byte[] Image__System_ApplicationRaw; public readonly static Bitmap SystemApplicationIcon = new Bitmap(Image__System_ApplicationRaw, ColorOrder.RGB);
        #endregion
    }
}
