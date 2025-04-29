using Cosmos.HAL;
using System;
using System.Collections.Generic;

namespace HontelOS.System.Multithreading
{
    public static class InterruptManager
    {
        public static PIT PIT = new PIT();
        public static List<PIT.PITTimer> Tasks = new List<PIT.PITTimer>();
        public static int Start(PIT.PITTimer timer)
        {
            Tasks.Add(timer);
            return PIT.RegisterTimer(timer);
        }

        public static int Start(Action action)
        {
            return Start(new PIT.PITTimer(action, 0, false));
        }

        public static int Start(Action action, ulong nanosecondsTimeout, bool recurring)
        {
            return Start(new PIT.PITTimer(action, nanosecondsTimeout, recurring));
        }

        public static int StartAfter(Action action, ulong nanosecondsTimeout, ulong startAfterNanoseconds, bool recurring)
        {
            var timer = new PIT.PITTimer(action, nanosecondsTimeout, startAfterNanoseconds);
            timer.Recurring = recurring;
            return Start(new PIT.PITTimer(action, nanosecondsTimeout, startAfterNanoseconds));
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