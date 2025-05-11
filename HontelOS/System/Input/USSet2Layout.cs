using Cosmos.System;
using System.Collections.Generic;

namespace HontelOS.System.Input
{
    /// <summary>
    /// US Set 2 Keyboard Layout
    /// </summary>
    public class USSet2Layout : ScanMapBase
    {
        protected override void InitKeys()
        {
            Keys = new List<KeyMapping>(100);

            #region Keys
            /*     Scan  Norm Shift Ctrl Alt     Num  Caps ShCaps ShNum ConsoleKeyEx */
            Keys.Add(new KeyMapping(0x00, ConsoleKeyEx.NoName));
            Keys.Add(new KeyMapping(0x76, ConsoleKeyEx.Escape));
            /* 1 -> 9 */
            Keys.Add(new KeyMapping(0x16, '1', '!', '1', '1', '!', '1', ConsoleKeyEx.D1));
            Keys.Add(new KeyMapping(0x1E, '2', '@', '2', '2', '@', '2', ConsoleKeyEx.D2));
            Keys.Add(new KeyMapping(0x26, '3', '#', '3', '3', '#', '3', ConsoleKeyEx.D3));
            Keys.Add(new KeyMapping(0x25, '4', '$', '4', '4', '$', '4', ConsoleKeyEx.D4));
            Keys.Add(new KeyMapping(0x2E, '5', '%', '5', '5', '%', '5', ConsoleKeyEx.D5));
            Keys.Add(new KeyMapping(0x36, '6', '^', '6', '6', '^', '6', ConsoleKeyEx.D6));
            Keys.Add(new KeyMapping(0x3D, '7', '&', '7', '7', '&', '7', ConsoleKeyEx.D7));
            Keys.Add(new KeyMapping(0x3E, '8', '*', '8', '8', '*', '8', ConsoleKeyEx.D8));
            Keys.Add(new KeyMapping(0x46, '9', '(', '9', '9', '(', '9', ConsoleKeyEx.D9));
            Keys.Add(new KeyMapping(0x45, '0', ')', '0', '0', ')', '0', ConsoleKeyEx.D0));
            /* -, =, Bksp, Tab */
            Keys.Add(new KeyMapping(0x4E, '-', '_', '-', '-', '_', '-', ConsoleKeyEx.Minus));
            Keys.Add(new KeyMapping(0x55, '=', '+', '=', '=', '+', '=', ConsoleKeyEx.Equal));
            Keys.Add(new KeyMapping(0x66, ConsoleKeyEx.Backspace));
            Keys.Add(new KeyMapping(0x0D, '\t', ConsoleKeyEx.Tab));
            /*      QWERTYUIOP[] */
            Keys.Add(new KeyMapping(0x15, 'q', 'Q', 'q', 'Q', 'q', 'Q', ConsoleKeyEx.Q));
            Keys.Add(new KeyMapping(0x1D, 'w', 'W', 'w', 'W', 'w', 'W', ConsoleKeyEx.W));
            Keys.Add(new KeyMapping(0x24, 'e', 'E', 'e', 'E', 'e', 'E', ConsoleKeyEx.E));
            Keys.Add(new KeyMapping(0x2D, 'r', 'R', 'r', 'R', 'r', 'R', ConsoleKeyEx.R));
            Keys.Add(new KeyMapping(0x2C, 't', 'T', 't', 'T', 't', 'T', ConsoleKeyEx.T));
            Keys.Add(new KeyMapping(0x35, 'y', 'Y', 'y', 'Y', 'y', 'Y', ConsoleKeyEx.Y));
            Keys.Add(new KeyMapping(0x3C, 'u', 'U', 'u', 'U', 'u', 'U', ConsoleKeyEx.U));
            Keys.Add(new KeyMapping(0x43, 'i', 'I', 'i', 'I', 'i', 'I', ConsoleKeyEx.I));
            Keys.Add(new KeyMapping(0x44, 'o', 'O', 'o', 'O', 'o', 'O', ConsoleKeyEx.O));
            Keys.Add(new KeyMapping(0x4D, 'p', 'P', 'p', 'P', 'p', 'P', ConsoleKeyEx.P));
            Keys.Add(new KeyMapping(0x54, '[', '{', '[', '{', '[', '{', ConsoleKeyEx.LBracket));
            Keys.Add(new KeyMapping(0x5B, ']', '}', ']', '}', ']', '}', ConsoleKeyEx.RBracket));
            /* ENTER, CTRL */
            Keys.Add(new KeyMapping(0x5A, ConsoleKeyEx.Enter));
            Keys.Add(new KeyMapping(0x14, ConsoleKeyEx.LCtrl));
            /* ASDFGHJKL;'` */
            Keys.Add(new KeyMapping(0x1C, 'a', 'A', 'a', 'A', 'a', 'A', ConsoleKeyEx.A));
            Keys.Add(new KeyMapping(0x1B, 's', 'S', 's', 'S', 's', 'S', ConsoleKeyEx.S));
            Keys.Add(new KeyMapping(0x23, 'd', 'D', 'd', 'D', 'd', 'D', ConsoleKeyEx.D));
            Keys.Add(new KeyMapping(0x2B, 'f', 'F', 'f', 'F', 'f', 'F', ConsoleKeyEx.F));
            Keys.Add(new KeyMapping(0x34, 'g', 'G', 'g', 'G', 'g', 'G', ConsoleKeyEx.G));
            Keys.Add(new KeyMapping(0x33, 'h', 'H', 'h', 'H', 'h', 'H', ConsoleKeyEx.H));
            Keys.Add(new KeyMapping(0x3B, 'j', 'J', 'j', 'J', 'j', 'J', ConsoleKeyEx.J));
            Keys.Add(new KeyMapping(0x42, 'k', 'K', 'k', 'K', 'k', 'K', ConsoleKeyEx.K));
            Keys.Add(new KeyMapping(0x4B, 'l', 'L', 'l', 'L', 'l', 'L', ConsoleKeyEx.L));
            Keys.Add(new KeyMapping(0x4C, ';', ':', ';', ';', ':', ':', ConsoleKeyEx.Semicolon));
            Keys.Add(new KeyMapping(0x28, '\'', '"', '\'', '\'', '"', '"', ConsoleKeyEx.Apostrophe));
            Keys.Add(new KeyMapping(0x0E, '`', '~', '`', '`', '~', '~', ConsoleKeyEx.Backquote));
            /* Left Shift*/
            Keys.Add(new KeyMapping(0x12, ConsoleKeyEx.LShift));
            /* \ZXCVBNM,./ */
            Keys.Add(new KeyMapping(0x5D, '\\', '|', '\\', '\\', '|', '|', ConsoleKeyEx.Backslash));
            Keys.Add(new KeyMapping(0x1A, 'z', 'Z', 'z', 'Z', 'z', 'Z', ConsoleKeyEx.Z));
            Keys.Add(new KeyMapping(0x22, 'x', 'X', 'x', 'X', 'x', 'X', ConsoleKeyEx.X));
            Keys.Add(new KeyMapping(0x21, 'c', 'C', 'c', 'C', 'c', 'C', ConsoleKeyEx.C));
            Keys.Add(new KeyMapping(0x2A, 'v', 'V', 'v', 'V', 'v', 'V', ConsoleKeyEx.V));
            Keys.Add(new KeyMapping(0x32, 'b', 'B', 'b', 'B', 'b', 'B', ConsoleKeyEx.B));
            Keys.Add(new KeyMapping(0x31, 'n', 'N', 'n', 'N', 'n', 'N', ConsoleKeyEx.N));
            Keys.Add(new KeyMapping(0x3A, 'm', 'M', 'm', 'M', 'm', 'M', ConsoleKeyEx.M));
            Keys.Add(new KeyMapping(0x41, ',', '<', ',', ',', '<', '<', ConsoleKeyEx.Comma));
            Keys.Add(new KeyMapping(0x49, '.', '>', '.', '.', '>', '>', ConsoleKeyEx.Period));
            Keys.Add(new KeyMapping(0x4A, '/', '?', '/', '/', '?', '/', ConsoleKeyEx.Slash)); // also numpad divide
            /* Right Shift */
            Keys.Add(new KeyMapping(0x59, ConsoleKeyEx.RShift));
            /* Print Screen */
            Keys.Add(new KeyMapping(0x7C, '*', '*', '*', '*', '*', '*', ConsoleKeyEx.NumMultiply));
            // also numpad multiply
            /* Alt  */
            Keys.Add(new KeyMapping(0x11, ConsoleKeyEx.LAlt));
            /* Space */
            Keys.Add(new KeyMapping(0x29, ' ', ConsoleKeyEx.Spacebar));
            /* Caps */
            Keys.Add(new KeyMapping(0x58, ConsoleKeyEx.CapsLock));
            /* F1-F12 */
            Keys.Add(new KeyMapping(0x05, ConsoleKeyEx.F1));
            Keys.Add(new KeyMapping(0x06, ConsoleKeyEx.F2));
            Keys.Add(new KeyMapping(0x04, ConsoleKeyEx.F3));
            Keys.Add(new KeyMapping(0x0C, ConsoleKeyEx.F4));
            Keys.Add(new KeyMapping(0x03, ConsoleKeyEx.F5));
            Keys.Add(new KeyMapping(0x0B, ConsoleKeyEx.F6));
            Keys.Add(new KeyMapping(0x83, ConsoleKeyEx.F7));
            Keys.Add(new KeyMapping(0x0A, ConsoleKeyEx.F8));
            Keys.Add(new KeyMapping(0x01, ConsoleKeyEx.F9));
            Keys.Add(new KeyMapping(0x09, ConsoleKeyEx.F10));
            Keys.Add(new KeyMapping(0x78, ConsoleKeyEx.F11));
            Keys.Add(new KeyMapping(0x07, ConsoleKeyEx.F12));
            /* Num Lock, Scrl Lock */
            Keys.Add(new KeyMapping(0x77, ConsoleKeyEx.NumLock));
            Keys.Add(new KeyMapping(0x7E, ConsoleKeyEx.ScrollLock));
            /* HOME, Up, Pgup, -kpad, left, center, right, +keypad, end, down, pgdn, ins, del */
            Keys.Add(new KeyMapping(0x6C, '\0', '\0', '7', '\0', '\0', '\0', ConsoleKeyEx.Home, ConsoleKeyEx.Num7));
            Keys.Add(new KeyMapping(0x75, '\0', '\0', '8', '\0', '\0', '\0', ConsoleKeyEx.UpArrow, ConsoleKeyEx.Num8));
            Keys.Add(new KeyMapping(0x7D, '\0', '\0', '9', '\0', '\0', '\0', ConsoleKeyEx.PageUp, ConsoleKeyEx.Num9));
            Keys.Add(new KeyMapping(0x7B, '-', '-', '-', '-', '-', '-', ConsoleKeyEx.NumMinus));
            Keys.Add(new KeyMapping(0x6B, '\0', '\0', '4', '\0', '\0', '\0', ConsoleKeyEx.LeftArrow, ConsoleKeyEx.Num4));
            Keys.Add(new KeyMapping(0x73, '\0', '\0', '5', '\0', '\0', '\0', ConsoleKeyEx.Num5));
            Keys.Add(new KeyMapping(0x74, '\0', '\0', '6', '\0', '\0', '\0', ConsoleKeyEx.RightArrow, ConsoleKeyEx.Num6));
            Keys.Add(new KeyMapping(0x79, '+', '+', '+', '+', '+', '+', ConsoleKeyEx.NumPlus));
            Keys.Add(new KeyMapping(0x69, '\0', '\0', '1', '\0', '\0', '\0', ConsoleKeyEx.End, ConsoleKeyEx.Num1));
            Keys.Add(new KeyMapping(0x72, '\0', '\0', '2', '\0', '\0', '\0', ConsoleKeyEx.DownArrow, ConsoleKeyEx.Num2));
            Keys.Add(new KeyMapping(0x7A, '\0', '\0', '3', '\0', '\0', '\0', ConsoleKeyEx.PageDown, ConsoleKeyEx.Num3));
            Keys.Add(new KeyMapping(0x70, '\0', '\0', '0', '\0', '\0', '\0', ConsoleKeyEx.Insert, ConsoleKeyEx.Num0));
            Keys.Add(new KeyMapping(0x71, '\0', '\0', '.', '\0', '\0', '\0', ConsoleKeyEx.Delete,
                ConsoleKeyEx.NumPeriod));

            Keys.Add(new KeyMapping(0x1F, ConsoleKeyEx.LWin));
            Keys.Add(new KeyMapping(0x27, ConsoleKeyEx.RWin));

            #endregion
        }
    }
}
