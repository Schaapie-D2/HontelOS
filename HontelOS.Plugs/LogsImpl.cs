using HontelOS.System;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HontelOS.Plugs
{
    [Plug(Target = typeof(global::System.Console))]
    public class LogsImpl
    {
        public static void WriteLine(string message)
        {
            Logs.Log(message);

            foreach (var c in message)
            {
                if (c == '\n')
                    global::System.Console.WriteLine();
                else
                    global::System.Console.Write(c);
            }

            global::System.Console.WriteLine();
        }
    }
}
