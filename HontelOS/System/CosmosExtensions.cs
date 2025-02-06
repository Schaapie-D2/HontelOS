/*
* PROJECT:          HontelOS
* CONTENT:          Cosmos extensions
* PROGRAMMERS:      Jort van Dalen
* 
* EXTENSIONS:       PCI
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.HAL;

namespace HontelOS.System
{
    public static class PCIExt
    {
        public static PCIDevice GetDeviceClass(byte classCode, byte subClassCode)
        {
            foreach (var device in PCI.Devices)
                if (device.ClassCode == classCode && device.Subclass == subClassCode)
                    return device;
            return null;
        }
    }
}
