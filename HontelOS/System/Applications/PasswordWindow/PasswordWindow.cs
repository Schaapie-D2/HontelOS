/*
* PROJECT:          HontelOS
* CONTENT:          Login window
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;
using System;

namespace HontelOS.System.Applications.PasswordWindow
{
    public class PasswordWindow : Window
    {
        protected int attempt = 0;
        protected int attempts = 5;

        public PasswordWindow() : base("Login", WindowStyle.Dialog, (int)Kernel.screenWidth / 2 - 300, (int)Kernel.screenHeight / 2 - 250, 600, 500)
        {
            Page p = Pages[0];

            CanClose = false;

            new Button("Shutdown", new Action(Kernel.Shutdown), 10, Height - 40, Width / 2 - 10, 30, p);
            new Button("Reboot", new Action(Kernel.Reboot), Width / 2 + 10, Height - 40, Width / 2 - 20, 30, p);

            TextBox username = new TextBox("Enter username...", null, Width / 2 - 200, 10, 400, 40, p);
            TextBox password = new TextBox("Enter password...", null, Width / 2 - 200, 60, 400, 40, p);
            new Button("Login", new Action(CheckPasword), Width / 2 - 200, 110, 400, 40, p);

            void CheckPasword()
            {
                if (username.Text == "Admin" && password.Text == "HontelOS")
                {
                    Kernel.isUnlocked = true;
                    ForceClose();
                }
                else
                {
                    new MessageBox("Login", "Wrong username or password!", null, MessageBoxButtons.Ok);
                    attempt++;
                }
            }

            WindowManager.Register(this);
        }
    }
}
