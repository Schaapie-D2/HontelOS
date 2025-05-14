using Cosmos.Core;
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
            if (Cosmos.HAL.Global.PS2Controller.FirstDevice is PS2Keyboard kb)
                SetScanMap(kb);
            if (Cosmos.HAL.Global.PS2Controller.SecondDevice is PS2Keyboard kb2)
                SetScanMap(kb2);
        }

        static void SetScanMap(PS2Keyboard kb)
        {
            var scanCodeSet = kb.GetScanCodeSet();
            switch (scanCodeSet)
            {
                case 1:
                    Logs.Log($"Keyboard scan code set {scanCodeSet} on PS/2 port: {kb.PS2Port}");
                    KeyboardManager.SetKeyLayout(new USStandardLayout());
                    break;
                case 2:
                    Logs.Log($"Keyboard scan code set {scanCodeSet} on PS/2 port: {kb.PS2Port}");
                    KeyboardManager.SetKeyLayout(new USPS2Set2Layout());
                    break;
                default:
                    Logs.Log($"Trying to set scan code set {scanCodeSet} to 1 on PS/2 port: {kb.PS2Port}");
                    kb.SetScanCodeSet(1);
                    if(kb.GetScanCodeSet() == 1)
                    {
                        Logs.Log($"Keyboard scan code set {scanCodeSet} successfully set to 1 on PS/2 port: {kb.PS2Port}");
                    }
                    else
                    {
                        Logs.Log($"Failed to set scan code set {scanCodeSet} to 1 on PS/2 port: {kb.PS2Port} and is currently {kb.GetScanCodeSet()}");
                    }
                    KeyboardManager.SetKeyLayout(new USStandardLayout());
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
                        MouseManager.HandleMouse(0, -5, 0, 0);
                    else if (key.Key == ConsoleKeyEx.F2)
                        MouseManager.HandleMouse(0, 5, 0, 0);
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
