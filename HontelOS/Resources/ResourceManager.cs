using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.Graphics;
using HontelOS.System.Helpers.Graphics;
using IL2CPU.API.Attribs;

namespace HontelOS.Resources
{
    public class ResourceManager
    {
        #region Images
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.Hontel_Logo.png")] readonly static byte[] Image__Hontel_LogoRaw; public readonly static Bitmap HontelLogo = ImageFormatHelper.GetBitmap(Image__Hontel_LogoRaw, ".png");
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.HontelOS_Logo_Black.png")] readonly static byte[] Image__HontelOS_Logo_BlackRaw; public readonly static Bitmap HontelOSLogoBlack = ImageFormatHelper.GetBitmap(Image__HontelOS_Logo_BlackRaw, ".png");
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.HontelOS_Logo_White.png")] readonly static byte[] Image__HontelOS_Logo_WhiteRaw; public readonly static Bitmap HontelOSLogoWhite = ImageFormatHelper.GetBitmap(Image__HontelOS_Logo_WhiteRaw, ".png");

        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Backgrounds.BG1.png")] readonly static byte[] Image__Background_1Raw; public readonly static Bitmap Background1 = ImageFormatHelper.GetBitmap(Image__Background_1Raw, ".png");
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Backgrounds.BG2.png")] readonly static byte[] Image__Background_2Raw; public readonly static Bitmap Background2 = ImageFormatHelper.GetBitmap(Image__Background_2Raw, ".png");
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Backgrounds.BG3.png")] readonly static byte[] Image__Background_3Raw; public readonly static Bitmap Background3 = ImageFormatHelper.GetBitmap(Image__Background_3Raw, ".png");
        #endregion
        #region Audio
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Audio.boot.wav")] readonly static byte[] Audio__BootRaw; public readonly static AudioStream BootSound = MemoryAudioStream.FromWave(Audio__BootRaw);
        #endregion
        #region System
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System_App_List.bmp")] public readonly static byte[] Image__System_App_ListRaw; public readonly static Bitmap SystemAppListIcon = ImageFormatHelper.GetBitmap(Image__System_App_ListRaw, ".bmp");

        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System_Application.bmp")] public readonly static byte[] Image__System_ApplicationRaw; public readonly static Bitmap SystemApplicationIcon = ImageFormatHelper.GetBitmap(Image__System_ApplicationRaw, ".bmp");
        #endregion
    }
}
