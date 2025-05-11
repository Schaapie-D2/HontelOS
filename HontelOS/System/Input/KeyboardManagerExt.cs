using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.ScanMaps;

namespace HontelOS.System.Input
{
    public class KeyboardManagerExt
    {
        public static bool KeyAvailable = false;

        static KeyEvent KeyEvent;

        public static void Init()
        {
            if(Cosmos.HAL.Global.PS2Controller.FirstDevice is PS2Keyboard kb)
                SetScanMap(kb);
            if(Cosmos.HAL.Global.PS2Controller.SecondDevice is PS2Keyboard kb2)
                SetScanMap(kb2);
        }

        static void SetScanMap(PS2Keyboard kb)
        {
            var scanCodeSet = kb.GetScanCodeSet();
            switch (scanCodeSet)
            {
                case 1:
                    KeyboardManager.SetKeyLayout(new USStandardLayout());
                    Logs.Log($"Keyboard Layout: {scanCodeSet} on PS/2 port: {kb.PS2Port}");
                    break;
                case 2:
                    KeyboardManager.SetKeyLayout(new USSet2Layout());
                    Logs.Log($"Keyboard Layout: {scanCodeSet} on PS/2 port: {kb.PS2Port}");
                    break;
                default:
                    Logs.Log($"Unknown Keyboard Layout: {scanCodeSet} on PS/2 port: {kb.PS2Port}");
                    break;
            }
        }

        public static void Update()
        {
            KeyAvailable = false;
            if (KeyboardManager.KeyAvailable)
            {
                var key = KeyboardManager.ReadKey();

                if (Kernel.isRealHardwareTest)
                {
                    if (key.Key == ConsoleKeyEx.F1)
                        MouseManager.HandleMouse(0, 5, 0, 0);
                    else if (key.Key == ConsoleKeyEx.F2)
                        MouseManager.HandleMouse(0, -5, 0, 0);
                    else if (key.Key == ConsoleKeyEx.F3)
                        MouseManager.HandleMouse(-5, 0, 0, 0);
                    else if (key.Key == ConsoleKeyEx.F4)
                        MouseManager.HandleMouse(5, 0, 0, 0);
                    else if (key.Key == ConsoleKeyEx.F5)
                        MouseManager.HandleMouse(0, 0, 1, 0);
                }

                KeyEvent = key;
                KeyAvailable = true;
            }   
        }

        public static bool GetKey(ConsoleKeyEx key)
        {
            if(KeyAvailable)
                return KeyEvent.Key == key;

            return false;
        }

        public static KeyEvent ReadKey()
        {
            if (KeyAvailable)
                return KeyEvent;
            else
                return null;
        }

        public static void AppendKey(byte scanKey, bool shift, bool alt, bool control)
        {
            KeyEvent = KeyboardManager.GetKeyLayout().ConvertScanCode(scanKey, control, shift, alt, false, false, false);
            KeyAvailable = true;
        }

        public static void AppendKey(KeyEvent keyEvent)
        {
            KeyEvent = keyEvent;
            KeyAvailable = true;
        }
    }
}
