/*
* PROJECT:          HontelOS
* CONTENT:          User settings class
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System.Collections.Generic;
using System.IO;

namespace HontelOS.System.User
{
    public class Settings
    {
        public static string path = "0:\\HontelOS\\settings.ini";
        public static string directory = "0:\\HontelOS";

        static Dictionary<string, string> settingsDic = new Dictionary<string, string>();

        public static string[] Default = new string[]
        {
            "Resolution;1920x1080",
            "Style;L",
            "BackgroundType;builtin",
            "Background;2"
        };

        public static void Reset()
        {
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllLines(path, Default);
        }

        public static void Load()
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (!File.Exists(path))
                File.WriteAllLines(path, Default);

            string[] settingsFile = File.ReadAllLines(path);
            foreach (string line in settingsFile)
            {
                string[] sp = line.Split(';');
                settingsDic.Add(sp[0], sp[1]);
            }
        }

        public static void DiscardChanges() => settingsDic.Clear();

        public static string Get(string key) { if (settingsDic.ContainsKey(key)) return settingsDic[key]; else return null; }

        public static void Set(string key, string value) => settingsDic[key] = value;

        public static void Push()
        {
            List<string> newFile = new List<string>();
            foreach (var setting in settingsDic)
                newFile.Add($"{setting.Key};{setting.Value}");

            File.WriteAllLines(path, newFile.ToArray());
        }
    }
}
