using HontelOS.System.Applications.Files;
using HontelOS.System.Graphics;
using HontelOS.System.User;
using System.IO;

namespace HontelOS.System.Applications.Terminal
{
    public class shutdown : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            Kernel.Shutdown();
        }

        public string GetCMDName() => "shutdown";
        public string GetHelpText() => "shutsdown the OS";
    }
    public class reboot : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            Kernel.Reboot();
        }

        public string GetCMDName() => "reboot";
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

        public string GetCMDName() => "msgbox";
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

        public string GetCMDName() => "ls";
        public string GetHelpText() => "lists the content of a directory";
    }
    public class lspci : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var c = terminal.console;
            string action = args[0];
            if(action == "-exists")
            {
                string devID = args[1];
                string venID = args[2];

                if (PCIExt.GetDevice(ushort.Parse(devID), ushort.Parse(venID)) != null)
                    c.WriteLine("PCI device exists!");
                else
                    c.WriteLine("PCI device does not exist!");

            }
            else if (action == "-search")
            {
                c.WriteLine("PCI devices:");
                foreach (var device in Cosmos.HAL.PCI.Devices)
                {
                    c.WriteLine($"Vendor: {device.VendorID:X4} Device: {device.DeviceID:X4} Class: {device.ClassCode:X2} Subclass: {device.Subclass:X2} ProgIF: {device.ProgIF:X2}");
                }
            }
            else
            {
                c.WriteLine("Usage: lspci <-exists or -search> <vendorID (only when using -exists)> <deviceID (only when using -exists)>");
            }
        }

        public string GetCMDName() => "lspci";
        public string GetHelpText() => "search all PCI devices on the device";
    }
    public class resetsettings : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            User.Settings.Reset();
        }

        public string GetCMDName() => "resetsettings";
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

        public string GetCMDName() => "showdir";
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

        public string GetCMDName() => "rm";
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

        public string GetCMDName() => "create";
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

        public string GetCMDName() => "createdir";
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

        public string GetCMDName() => "rmdir";
        public string GetHelpText() => "deletes a directory";
    }
    public class user : ICommand
    {
        public void Execute(string[] args, TerminalProgram terminal)
        {
            var action = args[0];
            var username = args[1];
            string password = args[2];
            var c = terminal.console;

            if (action == "-create")
            {
                if (username == null)
                {
                    c.WriteLine("username was not given");
                    return;
                }
                if (password == null)
                {
                    c.WriteLine("password was not given");
                    return;
                }

                User.User.Create(username, password);
            }
            else if (action == "-delete")
            {
                if (username == null)
                {
                    c.WriteLine("username was not given");
                    return;
                }

                User.User.Delete(username);
            }
            else if (action == "-list")
            {
                foreach(string user in User.User.GetUsers())
                    c.WriteLine(user);
            }
            else
            {
                c.WriteLine("Usage: user <-create or -delete> <username> <password (only when using -create)>");
            }
        }

        public string GetCMDName() => "user";
        public string GetHelpText() => "create and delete a user";
    }
}
