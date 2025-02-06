/*
* PROJECT:          HontelOS
* CONTENT:          System information class
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

namespace HontelOS.System
{
    public static class SystemInfo
    {
        public static string Kernel = "CosmosOS";
        public static string OperatingSystem = "HontelOS";
        public static VersionInfo Version = new VersionInfo();
    }
}
