using System;
using System.Collections.Generic;

namespace HontelOS.System
{
    public class Logs
    {
        public static List<Action<string>> OnNewLog = new();

        public static List<string> LogList = new List<string>();

        public static void Log(string message)
        {
            LogList.Add(message);
            foreach (var a in OnNewLog) a.Invoke(message);

            if(LogList.Count > 1000)
                LogList.RemoveAt(0);
        }
    }
}
