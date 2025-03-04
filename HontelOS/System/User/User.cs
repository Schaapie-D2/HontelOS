using HontelOS.System.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HontelOS.System.User
{
    public class User
    {
        public static string path = "0:\\HontelOS\\users.ini";
        public static string directory = "0:\\HontelOS";

        public static string Username;
        
        public static bool Login(string username, string password)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (!File.Exists(path))
            { File.WriteAllText(path, null); return false; }

            var usersFile = File.ReadAllLines(path);

            if (usersFile.Contains($"{username};{SHA256.hash(password)}"))
            {
                Username = username;
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            Username = "";
        }

        public static bool Create(string username, string password)
        {
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (!File.Exists(path))
                File.WriteAllText(path, null);

            List<string> usersFile = File.ReadAllLines(path).ToList();

            foreach (string line in usersFile)
                if (line.Split(';')[0] == username)
                    return false;

            Directory.CreateDirectory($"0:\\Users\\{username}\\Applications");
            Directory.CreateDirectory($"0:\\Users\\{username}\\Documents");
            Directory.CreateDirectory($"0:\\Users\\{username}\\Downloads");
            Directory.CreateDirectory($"0:\\Users\\{username}\\Desktop");

            usersFile.Add($"{username};{SHA256.hash(password)}");

            File.WriteAllLines(path, usersFile.ToArray());

            return true;
        }

        public static void Delete(string username)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (!File.Exists(path))
            { File.WriteAllText(path, null); return; }

            var usersFile = File.ReadAllLines(path);
            List<string> newFile = new List<string>();

            foreach (string line in usersFile)
                if (line.Split(';')[0] != username)
                    newFile.Add(line);
                else
                    Directory.Delete(Path.Combine(directory, username), true);

            if (newFile.Count == 0)
                newFile.Add("");

            File.WriteAllLines(path, newFile.ToArray());
        }

        public static string[] GetUsers()
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (!File.Exists(path))
            { File.WriteAllText(path, null); return Array.Empty<string>(); }

            var usersFile = File.ReadAllLines(path);
            List<string> users = new List<string>();

            foreach(string line in usersFile)
                users.Add(line.Split(';')[0]);

            return users.ToArray();
        }
    }
}
