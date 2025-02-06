/*
* PROJECT:          HontelOS
* CONTENT:          Terminal program for HontelOS
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using HontelOS.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Cosmos.System;
using HontelOS.System.Applications.Files;

namespace HontelOS.System.Applications.Terminal
{
    public class TerminalProgram : ConsoleWindow
    {

        public string WorkingDirectory = "0:\\";

        public TerminalProgram() : base("Terminal", (int)Kernel.screenWidth / 2 - 450, (int)Kernel.screenHeight / 2 - 300, 900, 600)
        {
            WindowManager.Register(this);

            console.WriteLine($"HontelOS {VersionInfo.Version} ({VersionInfo.VersionNumber})");
            console.WriteLine("Copyright (c) 2025 Jort van Dalen. All rights reserved.");
            console.WriteLine();
            console.Write(WorkingDirectory + "> ");
            console.ReadLine();
            console.OnSubmitReadLine.Add((string input) =>
            {
                ExecuteCommand(input);
                console.Write(WorkingDirectory + "> ");
                console.ReadLine();
            });
        }

        public void ExecuteCommand(string fullCommand)
        {
            string command = fullCommand.Split(' ')[0].ToLower();
            string[] args = RestoreArgs(fullCommand.Split(' ').Skip(1).ToArray());

            if(args.Length == 0)
                args = new string[] { null };

            switch (command)
            {
                case "shutdown": // shut device down
                    Kernel.Shutdown();
                    break;
                case "reboot": // reboot device
                    Kernel.Reboot();
                    break;
                case "msgbox": // display messagebox
                    MSGBOX(args);
                    break;
                case "ls": // list directory
                    LS();
                    break;
                case "lspci": // list PCI devices
                    LSPCI();
                    break;
                case "resetsettings": // reset settings
                    User.Settings.Reset();
                    break;
                case "showdir": // open Files in directory
                    SHOWDIR(args[0]);
                    break;
                case "rm": // delete file
                    RM(args[0]);
                    break;
                case "create": // create file
                    CREATE(args[0]);
                    break;
                case "createdir": // create directory
                    CREATEDIR(args[0]);
                    break;
                case "rmdir": // delete directory
                    RMDIR(args[0]);
                    break;
                case "cd": // jump to
                    CD(args[0]);
                    break;
                case "exit": // close terminal
                    Close();
                    break;
                default:
                    console.WriteLine($"Command '{command}' not found.");
                    break;
            }
        }

        public void LSPCI()
        {
            console.WriteLine("PCI devices:");
            foreach (var device in Cosmos.HAL.PCI.Devices)
            {
                console.WriteLine($"Vendor: {device.VendorID:X4} Device: {device.DeviceID:X4} Class: {device.ClassCode:X2} Subclass: {device.Subclass:X2} ProgIF: {device.ProgIF:X2}");
            }
        }

        public string[] RestoreArgs(string[] args)
        {
            List<string> newArgs = new List<string>();
            bool stringStarted = false;
            StringBuilder currentString = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith('"') && !stringStarted)
                {
                    stringStarted = true;
                    currentString.Append(args[i].TrimStart('"'));
                }
                else if (args[i].EndsWith('"') && stringStarted)
                {
                    stringStarted = false;
                    currentString.Append(" ").Append(args[i].TrimEnd('"'));
                    newArgs.Add(currentString.ToString());
                    currentString.Clear();
                }
                else if (stringStarted)
                    currentString.Append(" ").Append(args[i]);
                else
                    newArgs.Add(args[i]);
            }

            if (stringStarted)
                newArgs.Add(currentString.ToString());

            return newArgs.ToArray();
        }

        public void CD(string path)
        {
            if(File.Exists(path))
            {
                console.WriteLine("Can not move into a file.");
            }
            else if (path == "..")
            {
                if (WorkingDirectory != "0:\\")
                {
                    WorkingDirectory = WorkingDirectory.Substring(0, WorkingDirectory.LastIndexOf("\\"));
                    if (WorkingDirectory == "0:")
                        WorkingDirectory += "\\";
                }
            }
            else
            {
                if (Directory.Exists(Path.Combine(WorkingDirectory, path)))
                    WorkingDirectory = Path.Combine(WorkingDirectory, path);
                else if (Directory.Exists(path))
                    WorkingDirectory = path;
                else
                    console.WriteLine("Directory not found.");
            }
        }

        public void RM(string path)
        {
            if (File.Exists(Path.Combine(WorkingDirectory, path)))
                File.Delete(Path.Combine(WorkingDirectory, path));
            else if (File.Exists(path))
                File.Delete(path);
            else
                console.WriteLine("File not found.");
        }

        public void RMDIR(string path)
        {
            if (path == null)
            {
                Directory.Delete(WorkingDirectory);
                CD("..");
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path);
                CD("..");
            }
            else
                console.WriteLine("Directory not found.");
        }

        public void SHOWDIR(string path)
        {
            if (path == null)
                new FilesProgram(WorkingDirectory);
            else if (Directory.Exists(path))
                new FilesProgram(path);
            else
                console.WriteLine("Directory not found.");
        }

        public void LS()
        {
            foreach (string dir in Directory.GetDirectories(WorkingDirectory))
                console.WriteLine(dir);
            foreach (string file in Directory.GetFiles(WorkingDirectory))
                console.WriteLine(file);
        }

        public void MSGBOX(string[] args)
        {
            if(args.Length < 3)
            {
                console.WriteLine("Usage: msgbox <title> <message> <buttons (0 - 7)>");
                return;
            }

            new MessageBox(args[0], args[1], null, (MessageBoxButtons)int.Parse(args[2]));
        }

        public void CREATE(string filename)
        {
            var str = File.Create(Path.Combine(WorkingDirectory, filename));
            str.Dispose();
        }

        public void CREATEDIR(string dirname)
        {
            Directory.CreateDirectory(Path.Combine(WorkingDirectory, dirname));
        }
    }
}
