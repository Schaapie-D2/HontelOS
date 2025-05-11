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
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System.Application.bmp")] public readonly static byte[] ApplicationIconRaw; public static Bitmap ApplicationIcon { get; private set; }
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System.Folder.png")] public readonly static byte[] FolderIconRaw; public static Bitmap FolderIcon { get; private set; }
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System.Power.png")] public readonly static byte[] PowerIconRaw; public static Bitmap PowerIcon { get; private set; }
        [ManifestResourceStream(ResourceName = "HontelOS.Resources.Images.System.StartMenu.bmp")] public readonly static byte[] StartMenuIconRaw; public static Bitmap StartMenuIcon { get; private set; }
        #endregion

        public static void LoadResources()
        {
            HontelOSLogoBlack = ImageFormatHelper.GetBitmap(HontelOSLogoBlackRaw, ".png");
            HontelOSLogoWhite = ImageFormatHelper.GetBitmap(HontelOSLogoWhiteRaw, ".png");

            Background1 = ImageFormatHelper.GetBitmap(Background1Raw, ".png");
            Background2 = ImageFormatHelper.GetBitmap(Background2Raw, ".png");
            Background3 = ImageFormatHelper.GetBitmap(Background3Raw, ".png");

            BootSound = MemoryAudioStream.FromWave(BootSoundRaw);

            ApplicationIcon = ImageFormatHelper.GetBitmap(ApplicationIconRaw, ".bmp");
            FolderIcon = ImageFormatHelper.GetBitmap(FolderIconRaw, ".png");
            PowerIcon = ImageFormatHelper.GetBitmap(PowerIconRaw, ".png");
            StartMenuIcon = ImageFormatHelper.GetBitmap(StartMenuIconRaw, ".bmp");
        }
    }
}
