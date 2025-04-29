/*
* USES CODE FROM:
* https://github.com/Szymekk44/Waterfall-Core
*/

using System;

namespace HontelOS.Drivers
{
    public static class Native
    {
        public static unsafe void Movsb(byte* dest, byte* src, ulong count)
        {
            Buffer.MemoryCopy(src, dest, count, count);
        }

        public static unsafe void MemSet(byte* dest, byte value, ulong count)
        {
            for (ulong i = 0; i < count; i++)
            {
                dest[i] = value;
            }
        }

        public static unsafe void MemSet(void* dest, byte value, ulong count)
        {
            var byteDest = (byte*)dest;

            for (ulong i = 0; i < count; i++)
            {
                byteDest[i] = value;
            }
        }

        public static unsafe void Stosb(byte* p, byte value, ulong count)
        {
            if (count == 0) return;

            byte* block = stackalloc byte[8];
            for (int i = 0; i < 8; i++)
            {
                block[i] = value;
            }

            ulong blockSize = 8;
            ulong fullBlocks = count / blockSize;
            ulong remainingBytes = count % blockSize;

            for (ulong i = 0; i < fullBlocks; i++)
            {
                Buffer.MemoryCopy(block, p + (i * blockSize), blockSize, blockSize);
            }

            for (ulong i = 0; i < remainingBytes; i++)
            {
                p[(fullBlocks * blockSize) + i] = value;
            }
        }

        public static unsafe void Stosb(void* p, byte value, ulong count)
        {
            if (count == 0) return;

            var byteP = (byte*)p;

            byte* block = stackalloc byte[8];
            for (int i = 0; i < 8; i++)
            {
                block[i] = value;
            }

            ulong blockSize = 8;
            ulong fullBlocks = count / blockSize;
            ulong remainingBytes = count % blockSize;

            for (ulong i = 0; i < fullBlocks; i++)
            {
                Buffer.MemoryCopy(block, byteP + (i * blockSize), blockSize, blockSize);
            }

            for (ulong i = 0; i < remainingBytes; i++)
            {
                byteP[(fullBlocks * blockSize) + i] = value;
            }
        }
    }
}