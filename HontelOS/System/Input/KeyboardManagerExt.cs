using Cosmos.System;

namespace HontelOS.System.Input
{
    public class KeyboardManagerExt
    {
        public static bool KeyAvailable = false;

        static KeyEvent KeyEvent;

        public static void Update()
        {
            KeyAvailable = false;
            if (KeyboardManager.KeyAvailable)
            {
                KeyEvent = KeyboardManager.ReadKey();
                KeyAvailable = true;
            }   
        }

        public static bool GetKey(ConsoleKeyEx key)
        {
            return KeyEvent.Key == key;
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
