﻿/*
* PROJECT:          HontelOS
* CONTENT:          HontelOS text editor program
* PROGRAMMERS:      Jort van Dalen
*/

using System;
using System.IO;
using System.Linq;
using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;

namespace HontelOS.System.Applications.TextEditor
{
    public class TextEditorProgram : Window
    {
        public TextAreaBox textArea;
        public string filePath = "";

        public TextEditorProgram(string arg) : base("Text Editor", WindowStyle.Normal, (int)Kernel.screenWidth / 2 - 450, (int)Kernel.screenHeight / 2 - 300, 900, 600)
        {
            Page p = Pages[0];

            new Button("Save", new Action(Save), 5, 5, 100, 25, p);

            textArea = new TextAreaBox("", 0, 35, Width, Height - 35, p);

            if (File.Exists(arg))
            {
                var lines = File.ReadAllLines(arg).ToList();
                if (lines.Count == 0)
                    lines.Add("");
                textArea.Text = lines;
                filePath = arg;
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
