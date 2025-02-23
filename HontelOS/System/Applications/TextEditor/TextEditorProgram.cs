/*
* PROJECT:          HontelOS
* CONTENT:          HontelOS text editor program
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;

namespace HontelOS.System.Applications.TextEditor
{
    public class TextEditorProgram : Window
    {
        public TextAreaBox textArea;
        public string filePath = "";

        public bool IsReadOnly = false;

        public TextEditorProgram(string arg) : base("Text Editor", WindowStyle.Normal, (int)Kernel.screenWidth / 2 - 450, (int)Kernel.screenHeight / 2 - 300, 900, 600)
        {
            Page p = Pages[0];

            textArea = new TextAreaBox("", 0, 35, Width, Height - 35, p);

            if (File.Exists(arg))
            {
                var lines = File.ReadAllLines(arg).ToList();
                if (lines.Count == 0)
                    lines.Add("");
                textArea.Text = lines;
                filePath = arg;

                if(Kernel.fileSystem.GetFileSystemType(arg) == "ISO9660")
                {
                    IsReadOnly = true;
                    textArea.IsDisabled = true;
                    new Label("File is Read-Only", null, Color.Empty, 5, 35 / 2 - Style.SystemFont.Height / 2, p);
                }
                else
                {
                    new Button("Save", new Action(Save), 5, 5, 100, 25, p);
                }
            }
            else
            {
                new MessageBox("Error!", "This file doesn't exist!", null, MessageBoxButtons.Ok);
                Close();
            }

            WindowManager.Register(this);
        }

        void Save()
        {
            try
            {
                string[] file = textArea.Text.ToArray();
                File.WriteAllLines(filePath, file);
            }
            catch (Exception ex)
            {
                new MessageBox("Error!", $"Failed to save: {ex.Message}", null, MessageBoxButtons.Ok);
            }
        }
    }
}
