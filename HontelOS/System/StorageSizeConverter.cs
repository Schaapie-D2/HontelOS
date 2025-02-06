/*
* PROJECT:          HontelOS
* CONTENT:          Storage size converter
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System;

namespace HontelOS.System
{
    public class StorageSizeConverter
    {
        public static long Convert(StorageSize oldStorageSize, long size, StorageSize newStorageSize)
        {
            if (oldStorageSize == newStorageSize) return size;

            int sizeDifference = newStorageSize - oldStorageSize;
            return sizeDifference > 0
                ? size / (long)Math.Pow(1024, sizeDifference)
                : size * (long)Math.Pow(1024, -sizeDifference);
        }

        public static (long, StorageSize, string) AutoConvert(StorageSize oldStorageSize, long size)
        {
            StorageSize newStorageSize = oldStorageSize;

            while (size >= 1024 && newStorageSize < StorageSize.Geopbyte)
            {
                size /= 1024;
                newStorageSize++;
            }

            return (size, newStorageSize, $"{size}{StorageSizeToString(newStorageSize)}");
        }

        public static string StorageSizeToString(StorageSize storageSize)
        {
            return storageSize switch
            {
                StorageSize.Byte => "B",
                StorageSize.Kilobyte => "KB",
                StorageSize.Megabyte => "MB",
                StorageSize.Gigabyte => "GB",
                StorageSize.Terabyte => "TB",
                StorageSize.Petabyte => "PB",
                StorageSize.Exabyte => "EB",
                StorageSize.Zettabyte => "ZB",
                StorageSize.Yottabyte => "YB",
                StorageSize.Brontobyte => "BB",
                StorageSize.Geopbyte => "GBB",
                _ => ""
            };
        }
    }

    public enum StorageSize
    {
        Byte = 0,
        Kilobyte = 1,
        Megabyte = 2,
        Gigabyte = 3,
        Terabyte = 4,
        Petabyte = 5,
        Exabyte = 6,
        Zettabyte = 7,
        Yottabyte = 8,
        Brontobyte = 9,
        Geopbyte = 10
    }
}