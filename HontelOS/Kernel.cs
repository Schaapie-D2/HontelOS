﻿/*
* PROJECT:          HontelOS
* CONTENT:          HontelOS kernel
* PROGRAMMERS:      Jort van Dalen
*/

using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System;
using HontelOS.System;
using System.Collections.Generic;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.Core;
using Cosmos.System.Graphics.Fonts;
using Cosmos.Core.Memory;
using HontelOS.Resources;
using System;
using HontelOS.System.Applications.PasswordWindow;
using HontelOS.System.Graphics;
using HontelOS.System.User;
using HontelOS.System.Processing;
using HontelOS.System.Input;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System.Audio;
using HontelOS.Drivers.Audio;
using Cosmos.HAL.Audio;
using Cosmos.HAL;

namespace HontelOS
{
    public class Kernel : Sys.Kernel
    {
        public static Canvas canvas;
        static Dock dock;
        static TopBar topBar;
        public static CosmosVFS fileSystem;

        public static AudioDriver audioDriver;
        public static AudioMixer audioMixer;
        public static AudioManager audioManager;

        public static Cursor cursor = Cursor.Default;

        public static uint screenWidth = 1920;
        public static uint screenHeight = 1200;

        static Bitmap logo = ResourceManager.HontelLogo;
        static Bitmap BG1 = ResourceManager.Background1;

        public static List<SystemControl> systemControls = new List<SystemControl>();
        public static List<Process> Processes = new List<Process>();

        public static bool appListVisable;
        public static bool isDEBUGMode = true;

        static bool mouseClickNotice = false;
        static bool mouseClickNotice1 = false;
        static bool mouseClickSecNotice = false;
        static bool mouseClickSecNotice1 = false;

        static int heapCounter = 4;

        int _deltaT = 0;
        int frames = 0;
        int fps = 0;

        //LockScreen
        public static bool isUnlocked { get; internal set; }

        protected override void BeforeRun()
        {
            try
            {
                fileSystem = new CosmosVFS();
                VFSManager.RegisterVFS(fileSystem);

                //Settings.Reset();
                Settings.Load();

                StyleManager.Init();

                // I don't know how to use the Cosmos Audio interface this correctly, i'll look into it later
                audioMixer = new AudioMixer();
                audioDriver = AudioDriverExt.GetAudioDriver();
                if (audioDriver != null)
                {
                    audioDriver.SetSampleFormat(new SampleFormat(AudioBitDepth.Bits16, 2, true));
                    audioManager = new AudioManager()
                    {
                        Stream = audioMixer,
                        Output = audioDriver
                    };
                    audioManager.Enable();
                }

                string resFromSettings = Settings.Get("Resolution");
                if (resFromSettings != null)
                {
                    string[] splitResFromSettings = resFromSettings.Split('x');
                    screenWidth = uint.Parse(splitResFromSettings[0]);
                    screenHeight = uint.Parse(splitResFromSettings[1]);
                }

                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(screenWidth, screenHeight, ColorDepth.ColorDepth32));

                // Boot progress image
                canvas.DrawImage(logo, (int)screenWidth / 2 - (int)screenHeight / 8, (int)screenHeight / 2 - (int)screenHeight / 8, (int)screenHeight / 4, (int)screenHeight / 4);
                canvas.Display();

                audioMixer.Streams.Add(ResourceManager.BootSound);

                MouseManager.ScreenWidth = screenWidth;
                MouseManager.ScreenHeight = screenHeight;
                MouseManager.X = screenWidth / 2;
                MouseManager.Y = screenHeight / 2;

                dock = new Dock();
                topBar = new TopBar();

                if (audioDriver == null)
                    new MessageBox("Audio", "There were no compatible sound cards found on your system!", null, MessageBoxButtons.Ok);

                new PasswordWindow();

                appListVisable = false;
                isUnlocked = false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Crash.StopKernel(ex.Message, ex.InnerException.Message, "0x00000000", "0");
                else
                    Crash.StopKernel("Fatal dotnet exception occured.", ex.Message, "0x00000000", "0");
            }
        }

        protected override void Run()
        {
            try
            {
                cursor = Cursor.Default;
                UpdateSystem();

                // Drawing GUI
                canvas.DrawImage(BG1, 0, 0, true);

                WindowManager.Draw();

                if (isUnlocked)
                {
                    DrawDesktop();

                    if (appListVisable)
                        DrawAppList();

                    // Top Drawing GUI
                    topBar.Draw();
                    dock.Draw();
                }

                foreach (SystemControl gUIElement in systemControls)
                    gUIElement.Draw();

                DrawCursor(Cursors.GetCursorRawData(cursor), MouseManager.X, MouseManager.Y);

                if(isDEBUGMode)
                    DrawDebugInfo();

                canvas.Display();

                heapCounter--;
                if(heapCounter == 0)
                {
                    heapCounter = 4;
                    Heap.Collect();
                }
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null)
                    Crash.StopKernel(ex.Message, ex.InnerException.Message, "0x00000000", "0");
                else
                    Crash.StopKernel("Fatal dotnet exception occured.", ex.Message, "0x00000000", "0");
            }
        }

        #region Drawing
        void DrawDesktop()
        {
            // TODO
        }

        void DrawAppList()
        {
            canvas.DrawFilledRectangle(Color.FromArgb(64, 128, 128, 128), 0, 0, (int)screenWidth, (int)screenHeight);
        }

        void DrawCursor(int[] cursor, uint x, uint y)
        {
            for (uint h = 0; h < cursor[3]; h++)
            {
                for (uint w = 0; w < cursor[2]; w++)
                {
                    int pixelX = (int)(w + x - cursor[0]);
                    int pixelY = (int)(h + y - cursor[1]);

                    if (pixelX >= 0 && pixelX < screenWidth && pixelY >= 0 && pixelY < screenHeight)
                    {
                        if (cursor[h * cursor[2] + w + 4] == 1)
                        {
                            canvas.DrawPoint(Color.Black, pixelX, pixelY);
                        }
                        else if (cursor[h * cursor[2] + w + 4] == 2)
                        {
                            canvas.DrawPoint(Color.White, pixelX, pixelY);
                        }
                    }
                }
            }
        }
        #endregion

        #region System
        void UpdateSystem()
        {
            if (_deltaT != RTC.Second)
            {
                fps = frames;
                frames = 0;
                _deltaT = RTC.Second;
            }
            frames++;

            mouseClickNotice1 = false;
            mouseClickSecNotice1 = false;
            KeyboardManagerExt.Update();

            if (!appListVisable)
            {
                foreach (SystemControl c in systemControls)
                    c.Update();
                WindowManager.Update();
                foreach (var p in Processes)
                    p.Update();
            }

            // Update top GUI
            if (isUnlocked)
            {
                dock.Update();
                topBar.Update();
            }

            MouseManager.ResetScrollDelta();
        }

        public static void SetResolution(Mode mode)
        {
            if (!canvas.AvailableModes.Contains(mode)) return;

            canvas.Mode = mode;
            screenWidth = mode.Width;
            screenHeight = mode.Height;
            MouseManager.ScreenWidth = mode.Width;
            MouseManager.ScreenHeight = mode.Height;
            foreach (var a in SystemEvents.OnCanvasChanged) a.Invoke();
        }

        public static void Reboot()
        {
            ShutdownPrepare();
            Sys.Power.Reboot();
        }
        public static void Shutdown()
        {
            ShutdownPrepare();
            Sys.Power.Shutdown();
        }

        static void ShutdownPrepare()
        {
            canvas.Clear();
            canvas.DrawImage(logo, (int)screenWidth / 2 - (int)screenHeight / 8, (int)screenHeight / 2 - (int)screenHeight / 8, (int)screenHeight / 4, (int)screenHeight / 4);
            canvas.Display();

            foreach (IWindow w in WindowManager.Windows.Values)
                w.ForceClose();
            foreach (Process p in Processes)
                p.Kill();

            Settings.Push();
        }
        #endregion

        #region Input
        public static bool MouseInArea(int x1, int y1, int x2, int y2)
        {
            int MX = (int)MouseManager.X;
            int MY = (int)MouseManager.Y;
            return (MX >= x1 && MX <= x2 && MY >= y1 && MY <= y2);
        }

        public static bool MouseClick()
        {
            if (MouseManager.MouseState != MouseState.Left)
            { mouseClickNotice = false; mouseClickNotice1 = false; }
            if (MouseManager.MouseState == MouseState.Left && !mouseClickNotice)
            { mouseClickNotice = true; mouseClickNotice1 = true; }

            if (mouseClickNotice1)
                return true;
            return false;
        }
        public static bool MouseClickSec()
        {
            if (MouseManager.MouseState != MouseState.Right)
            { mouseClickSecNotice = false; mouseClickSecNotice1 = false; }
            if (MouseManager.MouseState == MouseState.Right && !mouseClickSecNotice)
            { mouseClickSecNotice = true; mouseClickSecNotice1 = true; }

            if (mouseClickSecNotice1)
                return true;
            return false;
        }
        #endregion

        #region DEBUG
        void DrawDebugInfo()
        {
            canvas.DrawString("RAM usage: " + StorageSizeConverter.Convert(StorageSize.Byte, GCImplementation.GetUsedRAM(), StorageSize.Megabyte).ToString(), PCScreenFont.Default, Color.Blue, 0, 0);
            canvas.DrawString("Canvas Type: " + canvas.Name(), PCScreenFont.Default, Color.Blue, 0, PCScreenFont.Default.Height * 1);
            canvas.DrawString("Processes: " + Processes.Count, PCScreenFont.Default, Color.Blue, 0, PCScreenFont.Default.Height * 2);
            canvas.DrawString("FPS: " + fps, PCScreenFont.Default, Color.Blue, 0, PCScreenFont.Default.Height * 3);
        }
        #endregion
    }
}
