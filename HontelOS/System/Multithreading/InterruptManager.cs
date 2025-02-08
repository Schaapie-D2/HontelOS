using Cosmos.HAL;
using System.Collections.Generic;

namespace HontelOS.System.Multithreading
{
    public static class InterruptManager
    {
        public static PIT PIT = new PIT();
        public static List<PIT.PITTimer> Tasks = new List<PIT.PITTimer>();
        public static void Start(PIT.PITTimer timer)
        {
            PIT.RegisterTimer(timer);
            Tasks.Add(timer);
        }
        public static void EndAll()
        {
            foreach (PIT.PITTimer timer in Tasks)
            {
                PIT.UnregisterTimer(timer.TimerID);
            }
        }
        public static void End(PIT.PITTimer timer)
        {
            PIT.UnregisterTimer(timer.TimerID);
        }
        public static void End(int id)
        {
            PIT.UnregisterTimer(id);
        }
    }
}