using Cosmos.System.Graphics;
using HontelOS.Resources;
using HontelOS.System.Graphics;
using HontelOS.System.User;
using Cosmos.Debug.Kernel;
using System.Diagnostics;
using System.IO;
using Cosmos.Core;
using System;

namespace HontelOS.System.Graphics
{
    public class StyleManager
    {
        public static Style Style { get; private set; }
        public static Style PreviousStyle { get; private set; }

        public static Bitmap Background;
        public static Bitmap ScalledBackground;
        public static Bitmap BlurredBackground;

        public static void Init()
        {
            SystemEvents.OnCanvasChanged.Add(() => { SetBackground(Background); });

            var s = Settings.Get("Style");
            Style ns = new LightStyle();

            if(s == "D")
                ns = new DarkStyle();

            Style = ns;
            PreviousStyle = ns;

            if(Settings.Get("BackgroundType") == "builtin")
            {
                switch (Settings.Get("Background"))
                {
                    case "1":
                        SetBackground(ResourceManager.Background1);
                        break;
                    case "2":
                        SetBackground(ResourceManager.Background2);
                        break;
                    case "3":
                        SetBackground(ResourceManager.Background3);
                        break;
                    default:
                        SetBackground(ResourceManager.Background1);
                        break;
                }
            }
            else if (Settings.Get("BackgroundType") == "file")
            {
                SetBackground(Settings.Get("Background"));
            }
            else
            {
                SetBackground(ResourceManager.Background1);
            }
        }

        public static void SetStyle(Style style)
        {
            PreviousStyle = Style;
            Style = style;
            foreach (var a in SystemEvents.OnStyleChanged)
                a.Invoke();
        }

        public static void SetBackground(Bitmap background)
        {
            Background = background;
            Bitmap scBG = CanvasUtils.ScaleImage(background, (int)Kernel.screenWidth, (int)Kernel.screenHeight);
            ScalledBackground = scBG;

            Bitmap blurBG = new Bitmap(scBG.Width, scBG.Height, ColorDepth.ColorDepth32); // For some reason
            Array.Copy(scBG.RawData, blurBG.RawData, scBG.RawData.Length); // This prevents the ScalledBackground to be blurry??????
            BlurredBackground = Blur.GenerateFastBlur(blurBG, 20);

            foreach (var a in SystemEvents.OnStyleChanged)
                a.Invoke();
        }

        public static void SetBackground(string path)
        {
            Bitmap background = null;
            if (File.Exists(path))
                background = new Bitmap(path);
            else
                background = ResourceManager.Background1;

            SetBackground(background);
        }
    }
}
