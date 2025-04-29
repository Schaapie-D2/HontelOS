using HontelOS.System.Graphics.Controls;
using HontelOS.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HontelOS.System.Applications.Debugging
{
    public class LogsProgram : ConsoleWindow
    {
        public LogsProgram() : base("Logs", 0, 0, 600, 1000)//base("Logs", (int)Kernel.screenWidth / 2 - 300, (int)Kernel.screenHeight / 2 - 500, 600, 1000)
        {
            foreach (var log in Logs.LogList)
                console.WriteLine(log);

            Logs.OnNewLog.Add((string msg) => console.WriteLine(msg));

            WindowManager.Register(this);
            WindowManager.SetFocused(WID);
        }
    }
}
