/*
* PROJECT:          HontelOS
* CONTENT:          HontelOS crash screen
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using CS = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;
using System;

namespace HontelOS.System
{
    public class Crash
    {
        /// <summary>
        /// Stop the kernel and display exception
        /// </summary>
        /// <param name="ex">Exception that stop the kernel</param>
        public static void StopKernel(string exception, string description, string lastknowaddress, string ctxinterrupt)
        {
            if(Kernel.canvas != null)
                ShowOnCanvas(exception, description, lastknowaddress, ctxinterrupt);
            else
                ShowOnConsole(exception, description, lastknowaddress, ctxinterrupt);
        }

        static void ShowOnCanvas(string exception, string description, string lastknowaddress, string ctxinterrupt)
        {
            Canvas c = Kernel.canvas;

            c.Clear(Color.Blue);

            c.DrawString("An exception occured in HontelOS!!!", PCScreenFont.Default, Color.White, 25, 25 + PCScreenFont.Default.Height * 1);
            c.DrawString($"Version: {VersionInfo.Version} ({VersionInfo.VersionNumber})", PCScreenFont.Default, Color.White, 25, 25 + PCScreenFont.Default.Height * 2);

            c.DrawString($"Exception: {exception}", PCScreenFont.Default, Color.White, 25, 25 + PCScreenFont.Default.Height * 4);
            c.DrawString($"Description: {description}", PCScreenFont.Default, Color.White, 25, 25 + PCScreenFont.Default.Height * 5);

            c.DrawString($"Last known address: {lastknowaddress}", PCScreenFont.Default, Color.White, 25, 25 + PCScreenFont.Default.Height * 7);
            c.DrawString($"Interrupt: {ctxinterrupt}", PCScreenFont.Default, Color.White, 25, 25 + PCScreenFont.Default.Height * 8);

            c.DrawString("Press any key to reboot.", PCScreenFont.Default, Color.White, 25, 25 + PCScreenFont.Default.Height * 10);

            c.Display();

            CS.KeyboardManager.ReadKey();

            CS.Power.Reboot();
        }

        static void ShowOnConsole(string exception, string description, string lastknowaddress, string ctxinterrupt)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("An exception occured in HontelOS!!!");
            Console.WriteLine($"Version: {VersionInfo.Version} ({VersionInfo.VersionNumber})\n");

            Console.WriteLine($"Exception: {exception}");
            Console.WriteLine($"Description: {description}\n");

            Console.WriteLine($"Last known address: {lastknowaddress}");
            Console.WriteLine($"Interrupt: {ctxinterrupt}\n");

            Console.WriteLine("Press any key to reboot");

            Console.ForegroundColor = ConsoleColor.White;

            CS.KeyboardManager.ReadKey();

            CS.Power.Reboot();
        }
    }
}
