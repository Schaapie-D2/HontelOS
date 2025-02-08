/*
* PROJECT:          HontelOS
* CONTENT:          Terminal commands
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using HontelOS.System.Applications.Files;
using HontelOS.System.Graphics;
using System.IO;

namespace HontelOS.System.Applications.Terminal
{
    public class shutdown : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            Kernel.Shutdown();
        }

        public string GetName() => "shutdown";
        public string GetHelpText() => "shutsdown the OS";
    }
    public class reboot : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            Kernel.Reboot();
        }

        public string GetName() => "reboot";
        public string GetHelpText() => "reboots the OS";
    }
    public class msgbox : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            if (args.Length < 3)
            {
                terminal.console.WriteLine("Usage: msgbox <title> <message> <buttons (0 - 7)>");
                return;
            }

            new MessageBox(args[0], args[1], null, (MessageBoxButtons)int.Parse(args[2]));
        }

        public string GetName() => "msgbox";
        public string GetHelpText() => "displays a message box";
    }
    public class ls : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var WD = terminal.WorkingDirectory;
            var c = terminal.console;
            foreach (string dir in Directory.GetDirectories(WD))
                c.WriteLine(dir);
            foreach (string file in Directory.GetFiles(WD))
                c.WriteLine(file);
        }

        public string GetName() => "ls";
        public string GetHelpText() => "lists the content of a directory";
    }
    public class lspci : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var c = terminal.console;
            c.WriteLine("PCI devices:");
            foreach (var device in Cosmos.HAL.PCI.Devices)
            {
                c.WriteLine($"Vendor: {device.VendorID:X4} Device: {device.DeviceID:X4} Class: {device.ClassCode:X2} Subclass: {device.Subclass:X2} ProgIF: {device.ProgIF:X2}");
            }
        }

        public string GetName() => "lspci";
        public string GetHelpText() => "lists all PCI devices on the device";
    }
    public class resetsettings : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            User.Settings.Reset();
        }

        public string GetName() => "resetsettings";
        public string GetHelpText() => "resets the system settings";
    }
    public class showdir : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var path = args[0];
            var WD = terminal.WorkingDirectory;
            var c = terminal.console;
            if (path == null)
                new FilesProgram(WD);
            else if (Directory.Exists(path))
                new FilesProgram(path);
            else
                c.WriteLine("Directory not found.");
        }

        public string GetName() => "showdir";
        public string GetHelpText() => "opens the directory in Files";
    }
    public class rm : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var path = args[0];
            var WD = terminal.WorkingDirectory;
            var c = terminal.console;
            if (File.Exists(Path.Combine(WD, path)))
                File.Delete(Path.Combine(WD, path));
            else if (File.Exists(path))
                File.Delete(path);
            else
                c.WriteLine("File not found.");
        }

        public string GetName() => "rm";
        public string GetHelpText() => "deletes a file";
    }
    public class create : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var filename = args[0];
            var WD = terminal.WorkingDirectory;
            var str = File.Create(Path.Combine(WD, filename));
            str.Dispose();
        }

        public string GetName() => "create";
        public string GetHelpText() => "creates a file";
    }
    public class createdir : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var dirname = args[0];
            var WD = terminal.WorkingDirectory;
            Directory.CreateDirectory(Path.Combine(WD, dirname));
        }

        public string GetName() => "createdir";
        public string GetHelpText() => "creates a directory";
    }
    public class rmdir : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var path = args[0];
            var WD = terminal.WorkingDirectory;
            var c = terminal.console;
            if (path == null)
            {
                Directory.Delete(WD);
                terminal.CD("..");
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path);
                terminal.CD("..");
            }
            else
                c.WriteLine("Directory not found.");
        }

        public string GetName() => "rmdir";
        public string GetHelpText() => "deletes a directory";
    }
}
