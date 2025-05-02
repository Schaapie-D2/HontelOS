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
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.Hontel_Logo.png")] readonly static byte[] HontelLogoRaw; public static Bitmap HontelLogo { get; private set; } = ImageFormatHelper.GetBitmap(HontelLogoRaw, ".png");
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.HontelOS_Logo_Black.png")] readonly static byte[] HontelOSLogoBlackRaw; public static Bitmap HontelOSLogoBlack { get; private set; }
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Branding.HontelOS_Logo_White.png")] readonly static byte[] HontelOSLogoWhiteRaw; public static Bitmap HontelOSLogoWhite { get; private set; }

        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Backgrounds.BG1.png")] readonly static byte[] Background1Raw; public static Bitmap Background1 { get; private set; }
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Backgrounds.BG2.png")] readonly static byte[] Background2Raw; public static Bitmap Background2 { get; private set; }
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.Backgrounds.BG3.png")] readonly static byte[] Background3Raw; public static Bitmap Background3 { get; private set; }
        #endregion
        #region Audio
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Audio.boot.wav")] readonly static byte[] BootSoundRaw; public static AudioStream BootSound { get; private set; }
        #endregion
        #region System
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System_App_List.bmp")] public readonly static byte[] SystemAppListIconRaw; public static Bitmap SystemAppListIcon { get; private set; }

        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System_Application.bmp")] public readonly static byte[] SystemApplicationIconRaw; public static Bitmap SystemApplicationIcon { get; private set; }
        #endregion

        public static void LoadResources()
        {
            HontelOSLogoBlack = ImageFormatHelper.GetBitmap(HontelOSLogoBlackRaw, ".png");
            HontelOSLogoWhite = ImageFormatHelper.GetBitmap(HontelOSLogoWhiteRaw, ".png");

            Background1 = ImageFormatHelper.GetBitmap(Background1Raw, ".png");
            Background2 = ImageFormatHelper.GetBitmap(Background2Raw, ".png");
            Background3 = ImageFormatHelper.GetBitmap(Background3Raw, ".png");

            BootSound = MemoryAudioStream.FromWave(BootSoundRaw);

            SystemAppListIcon = ImageFormatHelper.GetBitmap(SystemAppListIconRaw, ".bmp");
            SystemApplicationIcon = ImageFormatHelper.GetBitmap(SystemApplicationIconRaw, ".bmp");
        }
    }
}
