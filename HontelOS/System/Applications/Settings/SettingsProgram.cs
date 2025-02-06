/*
* PROJECT:          HontelOS
* CONTENT:          Settings program for HontelOS
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using HontelOS.Resources;
using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;

namespace HontelOS.System.Applications.Settings
{
    public class SettingsProgram : Window
    {
        Dictionary<string, string> _Settings = new Dictionary<string, string>();

        Page[] pages;

        public SettingsProgram() : base("Settings", WindowStyle.Normal, (int)Kernel.screenWidth / 2 - 450, (int)Kernel.screenHeight / 2 - 300, 900, 600)
        {
            Pages.Clear();
            Pages.Add(new Page("Home", this));
            Pages.Add(new Page("System", this));
            Pages.Add(new Page("Network and Internet", this));
            Pages.Add(new Page("Appearance", this));
            Pages.Add(new Page("Time and Language", this));
            Pages.Add(new Page("Accessibility", this));
            Pages.Add(new Page("Security", this));
            Pages.Add(new Page("Updates", this));
            pages = Pages.ToArray();

            NavBar.Width = 250;

            SetupPageHome();
            SetupPageSystem();
            SetupPageNetworkNInternet();
            SetupPageAppearance();
            SetupPageTimeNLanguage();
            SetupPageAccessibility();
            SetupPageSecurity();
            SetupPageUpdates();

            OnClose.Add(PushSettings);
            WindowManager.Register(this);
        }

        void SetupPageHome()
        {
            var p = pages[0];
            var w = Width - NavBar.Width - 10;

            new Label("Home", null, Color.Empty, 5, 5, p);

            new Label("The settings program is not done yet, more things will be added later!", null, Color.Empty, 5, 30, p);

            for (int i = 1; i < pages.Length; i++)
            {
                int index = i;
                new Button(pages[i].Title, () => { CurrentPage = index; }, 5, 60 + (i - 1) * 45, w, 40, p);
            }
        }

        void SetupPageSystem()
        {
            var p = pages[1];
            var w = Width - NavBar.Width - 10;
            
            new Label("System", null, Color.Empty, 5, 5, p);

            Action<int>[] _resolutionActions = new Action<int>[Kernel.canvas.AvailableModes.Count];
            for (int i = 0; i < _resolutionActions.Length; i++)
            {
                int index = i;
                _resolutionActions[i] = (int sel) =>
                {
                    var m = Kernel.canvas.AvailableModes[index];
                    Kernel.SetResolution(m);
                    Set("Resolution", $"{m.Width}x{m.Height}");
                };
            }
            string[] _resolutionItems = new string[Kernel.canvas.AvailableModes.Count];
            for (int i = 0; i < _resolutionItems.Length; i++)
                _resolutionItems[i] = Kernel.canvas.AvailableModes[i].ToString();
            new DropdownButton(_resolutionItems, 0, _resolutionActions, 5, 30, 300, 30, p);
        }

        void SetupPageNetworkNInternet()
        {
            var p = pages[2];
            var w = Width - NavBar.Width - 10;

            new Label("Network and Internet", null, Color.Empty, 5, 5, p);
        }

        void SetupPageAppearance()
        {
            var p = pages[3];
            var w = Width - NavBar.Width - 10;

            new Label("Appearance", null, Color.Empty, 5, 5, p);

            Action<int>[] _themeActions =
            {
                index => { StyleManager.SetStyle(new LightStyle()); Set("Style", "L"); },
                index => { StyleManager.SetStyle(new DarkStyle()); Set("Style", "D"); }
            };
            new DropdownButton(new[] { "Light", "Dark" }, 0, _themeActions, 5, 30, 300, 30, p);

            Action<int>[] _BGActions =
            {
                index => { StyleManager.SetBackground(ResourceManager.Background1); Set("BackgroundType", "builtin"); Set("Background", "1"); },
                index => { StyleManager.SetBackground(ResourceManager.Background2); Set("BackgroundType", "builtin"); Set("Background", "2"); },
                index => { StyleManager.SetBackground(ResourceManager.Background3); Set("BackgroundType", "builtin"); Set("Background", "3"); }
            };
            new DropdownButton(new[] { "Background 1", "Background 2", "Background 3" }, 0, _BGActions, 5, 65, 300, 30, p);
        }

        void SetupPageTimeNLanguage()
        {
            var p = pages[4];
            var w = Width - NavBar.Width - 10;

            new Label("Time and Language", null, Color.Empty, 5, 5, p);
        }

        void SetupPageAccessibility()
        {
            var p = pages[5];
            var w = Width - NavBar.Width - 10;

            new Label("Accessibility", null, Color.Empty, 5, 5, p);
        }

        void SetupPageSecurity()
        {
            var p = pages[6];
            var w = Width - NavBar.Width - 10;

            new Label("Security", null, Color.Empty, 5, 5, p);
        }

        void SetupPageUpdates()
        {
            var p = pages[7];
            var w = Width - NavBar.Width - 10;

            new Label("Updates", null, Color.Empty, 5, 5, p);
        }

        void Set(string key, string value) => _Settings.Add(key, value);

        void PushSettings()
        {
            foreach(var setting in _Settings)
                User.Settings.Set(setting.Key, setting.Value);
        }
    }
}
