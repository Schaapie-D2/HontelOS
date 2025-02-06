/*
* PROJECT:          HontelOS
* CONTENT:          All HontelOS system events
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
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
