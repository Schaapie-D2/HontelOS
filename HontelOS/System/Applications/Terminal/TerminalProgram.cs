using HontelOS.System.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HontelOS.System.Applications.Terminal
{
    public class TerminalProgram : ConsoleWindow
    {

        public string WorkingDirectory = "0:\\";

        public List<ICommand> Commands = new List<ICommand>();

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

            Commands.Add(new shutdown());
            Commands.Add(new reboot());
            Commands.Add(new msgbox());
            Commands.Add(new ls());
            Commands.Add(new lspci());
            if (Kernel.fileSystem != null)
            {
                Commands.Add(new resetsettings());
                Commands.Add(new showdir());
                Commands.Add(new rm());
                Commands.Add(new rmdir());
                Commands.Add(new create());
                Commands.Add(new createdir());
                Commands.Add(new user());
            }
        }

        public void Help(string arg)
        {
            var c = console;
            if (arg == null)
            {
                foreach (var com in Commands)
                    c.WriteLine($"{com.GetCMDName()} = {com.GetHelpText()}");
            }
            else
            {
                foreach (var com in Commands)
                {
                    if(com.GetCMDName() == arg)
                    {
                        c.WriteLine(com.GetHelpText());
                        break;
                    }
                }
            }
        }

        public void ExecuteCommand(string fullCommand)
        {
            string command = fullCommand.Split(' ')[0].ToLower();
            string[] args = RestoreArgs(fullCommand.Split(' ').Skip(1).ToArray());

            if(args.Length == 0)
                args = new string[] { null };

            switch(command)
            {
                case "help":
                    Help(args[0]);
                    return;
                case "cd":
                    CD(args[0]);
                    return;
                case "exit":
                    Close();
                    return;
            }

            foreach (var com in Commands)
            {
                if (com.GetCMDName() == command)
                {
                    com.Execute(args, this);
                    return;
                }
            }

            console.WriteLine($"Command '{command}' not found.");
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
    }
}
