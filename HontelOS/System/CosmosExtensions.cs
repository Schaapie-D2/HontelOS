/*
* EXTENSIONS:       PCI
*/

using Cosmos.HAL;
using System.Collections.Generic;

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

        public static PCIDevice GetDeviceClass(byte classCode, byte subClassCode, byte ProgIF)
        {
            foreach (var device in PCI.Devices)
                if (device.ClassCode == classCode && device.Subclass == subClassCode && device.ProgIF == ProgIF)
                    return device;
            return null;
        }

        public static PCIDevice[] GetDevicesClass(byte classCode, byte subClassCode, byte ProgIF)
        {
            List<PCIDevice> devices = new List<PCIDevice>();

            foreach (var device in PCI.Devices)
                if (device.ClassCode == classCode && device.Subclass == subClassCode && device.ProgIF == ProgIF)
                    devices.Add(device);
            return devices.ToArray();
        }

        public static PCIDevice GetDevice(ushort VendorID, ushort DeviceID)
        {
            foreach (var device in PCI.Devices)
                if (device.VendorID == VendorID && device.DeviceID == DeviceID)
                    return device;
            return null;
        }
    }
}
