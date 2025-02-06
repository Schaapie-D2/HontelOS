/*
* PROJECT:          HontelOS
* CONTENT:          Audio driver extension
* PROGRAMMERS:      Jort van Dalen
* 
* MIGHT BE INCOMPLETE
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.HAL.Drivers.Audio;
using System;
using Cosmos.HAL;

namespace HontelOS.Drivers.Audio
{
    public static class AudioDriverExt
    {
        public static AudioDriver GetAudioDriver()
        {
            Console.WriteLine("Detecting audio devices...");
            foreach (var pci in PCI.Devices)
            {
                // Intel HD Audio devices
                if (pci.VendorID == 0x8086 &&  // Intel
                    (pci.DeviceID == 0x2804 || // Intel 82801I (ICH9 Family) HD Audio Controller
                    pci.DeviceID == 0x2812 ||  // Intel 82801H (ICH8 Family) HD Audio Controller
                    pci.DeviceID == 0x1C20 ||  // Intel 6 Series/C200 Series Chipset HD Audio Controller
                    pci.DeviceID == 0x1C21 ||  // Intel 7 Series/C210 Series Chipset HD Audio Controller
                    pci.DeviceID == 0x8C20 ||  // Intel 9 Series Chipset HD Audio Controller
                    pci.DeviceID == 0xA170 ||  // Intel Skylake U/D/Y Series HD Audio Controller
                    pci.DeviceID == 0xA1C0))   // Intel Kaby Lake HD Audio Controller
                {
                    Console.WriteLine("Found Intel HD Audio device");
                    return IntelHDAudio.Initialize(4096);
                }
                // AC'97 Audio devices
                else if ((pci.VendorID == 0x8086 && // Intel
                    (pci.DeviceID == 0x24C6 ||      // Intel 82801AB AC'97 Audio Controller
                    pci.DeviceID == 0x24C7 ||       // Intel 82801AC AC'97 Audio Controller
                    pci.DeviceID == 0x24D5 ||       // Intel 82801BA AC'97 Audio Controller
                    pci.DeviceID == 0x2668 ||       // Intel 82801EB AC'97 Audio Controller
                    pci.DeviceID == 0x27D8 ||       // Intel 82801G AC'97 Audio Controller
                    pci.DeviceID == 0x27D9 ||       // Intel 82801GBM AC'97 Audio Controller
                    pci.DeviceID == 0x24C5)) ||     // Intel 82801AA AC'97 Audio Controller
                    (pci.VendorID == 0x1106 &&      // VIA
                    (pci.DeviceID == 0x3059 ||      // VIA AC'97 Audio Controller
                    pci.DeviceID == 0x3038)) ||     // VIA VT1618 AC'97 Audio Controller
                    (pci.VendorID == 0x1002 &&      // AMD
                    pci.DeviceID == 0x4353))        // AMD AC'97 Audio Controller
                {
                    Console.WriteLine("Found AC'97 Audio device");
                    return AC97.Initialize(4096);
                }
                // Sound Blaster 16 PCI devices
                else if (pci.VendorID == 0x1102 && // Creative Labs
                    (pci.DeviceID == 0x0004 ||     // Sound Blaster 16
                    pci.DeviceID == 0x0005 ||      // Sound Blaster 16 (OEM Version)
                    pci.DeviceID == 0x0020))       // Sound Blaster 16 with Plug and Play
                {
                    Console.WriteLine("Found Sound Blaster 16 PCI device");
                    return SoundBlaster16.Initialize(4096);
                }
            }
            Console.WriteLine("No audio devices found");
            return null;
        }
    }
}