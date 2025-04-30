using System;
using System.Collections.Generic;
using System.Linq;

namespace HontelOS.System.Graphics
{
    public class WindowManager
    {
        public static Dictionary<int, IWindow> Windows = new Dictionary<int, IWindow>();
        public static List<int> ZOrder = new();

        public static int? FocusedWindow { get; private set; }

        public static List<Action> OnWindowsListUpdate = new();

        static int WIDcounter = -1;

        static bool WinDubFocusPrevention = false;

        public static int Register(IWindow window)
        {
            WIDcounter++;
            window.WID = WIDcounter;
            Windows.Add(WIDcounter, window);
            ZOrder.Add(WIDcounter);
            SetFocused(WIDcounter);
            foreach (var a in OnWindowsListUpdate) a.Invoke();
            return WIDcounter;
        }

        public static void Unregister(int WID)
        {
            if (Windows.ContainsKey(WID))
            {
                Windows.Remove(WID);
                ZOrder.Remove(WID);
                FocusedWindow = ZOrder.LastOrDefault();
                foreach (var a in OnWindowsListUpdate) a.Invoke();
            }
        }

        public static void Update()
        {
            WinDubFocusPrevention = false;

            foreach (var wid in ZOrder.Reverse<int>())
                Windows[wid].UpdateWindow();
        }

        public static void Draw()
        {
            foreach (var wid in ZOrder)
                Windows[wid].DrawWindow();
        }

        public static void SetFocused(int WID)
        {
            if (!WinDubFocusPrevention && Windows.ContainsKey(WID))
            {
                FocusedWindow = WID;
                ZOrder.Remove(WID);
                ZOrder.Add(WID);
            }
            WinDubFocusPrevention = true;
        }

        public static bool IsAlive(int WID) { return Windows.ContainsKey(WID); }
    }
}
