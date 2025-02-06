/*
* PROJECT:          HontelOS
* CONTENT:          HontelOS file explorer program
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System;
using HontelOS.System.Input;
using HontelOS.System.Graphics;
using HontelOS.System.Graphics.Controls;
using System;
using System.Collections.Generic;
using System.IO;

namespace HontelOS.System.Applications.Files
{
    public class FilesProgram : Window
    {
        public string workingDirectory = "0:\\";
        string oldWorkingDirectory = "";

        ItemsList itemsList;
        TextBox pathTextBox;

        public FilesProgram() : base("Files", WindowStyle.Normal, (int)Kernel.screenWidth / 2 - 450, (int)Kernel.screenHeight / 2 - 300, 900, 600)
        {
            Page p = Pages[0];

            string[] _items = { "File", "Folder" };
            Action<int>[] _actions =
            {
                index => CreateFile(),
                index => CreateFolder()
            };
            new ContextMenuButton("Create", _items, _actions, 5, 5, 100, 25, p);

            new Button("Delete", new Action(Delete), 110, 5, 100, 25, p);

            for (int i = 0; i < Kernel.fileSystem.GetVolumes().Count; i++)
            {
                string volumePath = Kernel.fileSystem.GetVolumes()[i].mFullPath;
                new Button(volumePath, () => GoToPath(volumePath), 5, 35 + i * 30, 100, 25, p);
            }

            pathTextBox = new TextBox("path to file/folder...", new Action<string>(GoToPath), 110, 35, 785, 25, p);
            pathTextBox.Text = workingDirectory.Replace("\\", "/");

            itemsList = new ItemsList(new List<string> { "" }, 110, 65, 785, 530, p);
            itemsList.OnSubmit.Add(GoToPathFromItemsList);

            WindowManager.Register(this);
        }
        public FilesProgram(string path) : base("Files", WindowStyle.Normal, (int)Kernel.screenWidth / 2 - 450, (int)Kernel.screenHeight / 2 - 300, 900, 600)
        {
            Page p = Pages[0];

            if (Directory.Exists(path))
                workingDirectory = path;

            string[] _items = { "File", "Folder" };
            Action<int>[] _actions =
            {
                index => CreateFile(),
                index => CreateFolder()
            };
            new ContextMenuButton("Create", _items, _actions, 5, 5, 100, 25, p);

            new Button("Delete", new Action(Delete), 110, 5, 100, 25, p);

            for (int i = 0; i < Kernel.fileSystem.GetVolumes().Count; i++)
            {
                string volumePath = Kernel.fileSystem.GetVolumes()[i].mFullPath;
                new Button(volumePath, () => GoToPath(volumePath), 5, 35 + i * 30, 100, 25, p);
            }

            pathTextBox = new TextBox("path to file/folder...", new Action<string>(GoToPath), 110, 35, 785, 25, p);
            pathTextBox.Text = workingDirectory.Replace("\\", "/");

            itemsList = new ItemsList(new List<string> { "" }, 110, 65, 785, 530, p);
            itemsList.OnSubmit.Add(GoToPathFromItemsList);

            WindowManager.Register(this);
        }

        void CreateFile()
        {
            
        }

        void CreateFolder()
        {
            
        }

        void Delete()
        {
            if (itemsList.SelectedIndex != -1)
            {
                File.Delete(Path.Combine(workingDirectory, itemsList.Items[itemsList.SelectedIndex]));
                oldWorkingDirectory = "";
            }  
        }

        void GoToPath(string path)
        {
            path = path.Replace("/", "\\");

            if (Directory.Exists(path))
            {
                workingDirectory = path;
                pathTextBox.Text = path.Replace("\\", "/");
            }
            else if (File.Exists(path))
                OpenFile(path);
            else
                pathTextBox.Text = workingDirectory.Replace("\\", "/");
        }

        void GoToPath(DirectoryInfo path)
        {
            if(path == null) return;

            GoToPath(path.FullName);
        }

        void GoToPathFromItemsList(int selectedIndex)
        {
            GoToPath(Path.Combine(workingDirectory, itemsList.Items[selectedIndex]));
        }

        void OpenFile(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".txt":
                    new TextEditor.TextEditorProgram(path);
                    break;
                default:
                    new TextEditor.TextEditorProgram(path);
                    break;
            }
        }

        public override void CustomUpdate()
        {
            if (workingDirectory != oldWorkingDirectory)
            {
                itemsList.Items.Clear();
                foreach (string directory in Directory.GetDirectories(workingDirectory))
                    itemsList.Items.Add(Path.GetFileName(directory));
                foreach (string file in Directory.GetFiles(workingDirectory))
                    itemsList.Items.Add(Path.GetFileName(file));
            }
            oldWorkingDirectory = workingDirectory;

            if(KeyboardManagerExt.KeyAvailable)
            {
                var key = KeyboardManagerExt.ReadKey().Key;

                if (key == ConsoleKeyEx.Delete)
                    Delete();
                else if (key == ConsoleKeyEx.Backspace)
                    GoToPath(Directory.GetParent(workingDirectory));
            }
        }
    }
}
