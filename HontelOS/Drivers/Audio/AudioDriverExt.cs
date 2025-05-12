// MIGHT BE INCOMPLETE

using Cosmos.HAL.Drivers.Audio;
using Cosmos.HAL;
using Cosmos.System;
using HontelOS.System;

namespace HontelOS.Drivers.Audio
{
    public static class AudioDriverExt
    {
        public static AudioDriver GetAudioDriver(ushort bufferSize)
        {
            global::System.Console.WriteLine("Detecting audio devices...");

            if (VMTools.IsVMWare)
            {
                var Dev = PCI.GetDevice(0x1274, 0x1371);
                if (Dev != null && Dev.DeviceExists)
                    return ES1371.Initialize(bufferSize);
                else
                    return null;
            }
            else if (VMTools.IsVirtualBox)
                return AC97.Initialize(bufferSize);

            foreach (var pci in PCI.Devices)
            {
                // AC'97 Audio devices
                if ((pci.VendorID == 0x8086 && // Intel
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
                    global::System.Console.WriteLine("Found AC'97 Audio device");
                    return AC97.Initialize(bufferSize);
                }
                else if (pci.VendorID == 0x1274 && // Ensoniq / Creative Labs
                    (pci.DeviceID == 0x1371 ||     // Ensoniq ES1371 AudioPCI
                    pci.DeviceID == 0x5000 ||      // Ensoniq ES1370 AudioPCI
                    pci.DeviceID == 0x5880 ||      // Creative Sound Blaster AudioPCI 64V
                    pci.DeviceID == 0x5882 ||      // Creative Sound Blaster PCI128
                    pci.DeviceID == 0x5883 ||      // Creative Sound Blaster Live!
                    pci.DeviceID == 0x8938 ||      // Creative SB PCI 128 CT4750
                    pci.DeviceID == 0x8939))       // Creative Sound Blaster PCI128 CT4700
                {
                    global::System.Console.WriteLine("Found ES1371 Audio Device");
                    return ES1371.Initialize(bufferSize);
                }
                // Intel HD Audio devices
                else if (pci.VendorID == 0x8086 &&  // Intel
                    (pci.DeviceID == 0x2804 || // Intel 82801I (ICH9 Family) HD Audio Controller
                    pci.DeviceID == 0x2812 ||  // Intel 82801H (ICH8 Family) HD Audio Controller
                    pci.DeviceID == 0x1C20 ||  // Intel 6 Series/C200 Series Chipset HD Audio Controller
                    pci.DeviceID == 0x1C21 ||  // Intel 7 Series/C210 Series Chipset HD Audio Controller
                    pci.DeviceID == 0x8C20 ||  // Intel 9 Series Chipset HD Audio Controller
                    pci.DeviceID == 0xA170 ||  // Intel Skylake U/D/Y Series HD Audio Controller
                    pci.DeviceID == 0xA1C0))   // Intel Kaby Lake HD Audio Controller
                {
                    global::System.Console.WriteLine("Found Intel HD Audio device");
                    return IntelHDAudio.Initialize(bufferSize);
                }
                // Sound Blaster 16 PCI devices
                else if (pci.VendorID == 0x1102 && // Creative Labs
                    (pci.DeviceID == 0x0004 ||     // Sound Blaster 16
                    pci.DeviceID == 0x0005 ||      // Sound Blaster 16 (OEM Version)
                    pci.DeviceID == 0x0020))       // Sound Blaster 16 with Plug and Play
                {
                    global::System.Console.WriteLine("Found Sound Blaster 16 PCI device");
                    return SoundBlaster16.Initialize(bufferSize);
                }
            }
            global::System.Console.WriteLine("No audio devices found");
            return null;
        }
    }
}