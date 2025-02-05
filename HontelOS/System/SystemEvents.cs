/*
* PROJECT:          HontelOS
* CONTENT:          All HontelOS system events
* PROGRAMMERS:      Jort van Dalen
*/

using System;
using System.Collections.Generic;

namespace HontelOS.System
{
    public class SystemEvents
    {
        public static List<Action> OnStyleChanged = new();
        public static List<Action> OnCanvasChanged = new();

        public static List<Action> SecondPassed = new();
        public static List<Action> MinutePassed = new();
    }
}
