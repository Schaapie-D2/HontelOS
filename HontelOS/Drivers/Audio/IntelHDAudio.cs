// DOES NOT WORK YET

using System;
using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL.Audio;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.HAL;
using System.Linq;
using HontelOS.System;

namespace HontelOS.Drivers.Audio
{
    public unsafe sealed class IntelHDAudio : AudioDriver
    {
        public override IAudioBufferProvider BufferProvider { get; set; }

        private unsafe struct BufferDescriptor
        {
            public uint Address;  // Physical address of the buffer
            public uint Control;  // Buffer size and flags
        }

        public PCIDevice PCI { get; private set; }

        private const int BUFFER_COUNT = 32; // Number of buffers
        private BufferDescriptor[] BDL; // Buffer Descriptor List
        private byte[][] Buffers; // Audio buffers
        private int BufferSizeBytes;

        private uint* MMIOBase; // MMIO Base Address
        private uint StreamBase; // Base address of the stream descriptor
        private byte LastValidIdx;

        private readonly byte InterruptLine;

        private const int STREAM_ID = 1; // ID for audio output stream
        private const uint STREAM_DESC_BASE = 0x80; // Stream Descriptor Base Offset
        private const uint RESET_TIMEOUT = 500; // Reset timeout for the controller

        public static IntelHDAudio Instance { get; private set; } = null;

        public static IntelHDAudio Initialize(ushort bufferSize)
        {
            if (Instance == null)
            {
                Instance = new IntelHDAudio(bufferSize);
            }

            return Instance;
        }

        private IntelHDAudio(ushort bufferSize)
        {
            if (bufferSize % 2 != 0)
                throw new ArgumentException("Buffer size must be even.", nameof(bufferSize));

            PCIDevice pci = Cosmos.HAL.PCI.GetDeviceClass(0x04, 0x01);
            //foreach (var _pci in Cosmos.HAL.PCI.Devices)
            //{
            //    if (_pci.VendorID == 0x8086 &&
            //        (_pci.DeviceID == 0x293E || _pci.DeviceID == 0x293F || _pci.DeviceID == 0x1C20))
            //    {
            //        pci = _pci;
            //        break;
            //    }
            //}

            if (pci == null || !pci.DeviceExists || pci.InterruptLine > 0xF)
                throw new InvalidOperationException("No Intel HD Audio device found.");

            PCI = pci;

            PCI.EnableBusMaster(true);
            PCI.EnableMemory(true);

            MMIOBase = (uint*)PCI.BaseAddressBar[0].BaseAddress;
            StreamBase = PCI.BaseAddressBar[0].BaseAddress + STREAM_DESC_BASE + (uint)(STREAM_ID * 0x20);

            // Setup interrupt handler
            InterruptLine = PCI.InterruptLine;
            INTs.SetIrqHandler(InterruptLine, HandleInterrupt);

            // Reset and initialize controller
            ResetController();

            // Allocate buffers and BDL
            CreateBuffers(bufferSize);
            ProvideBuffers();

            Enable();
        }

        private void ResetController()
        {
            Write32(0x08, 0x0); // GCTL - Set reset
            WaitForRegisterClear(0x08, 0x1, RESET_TIMEOUT);

            Write32(0x08, 0x1); // GCTL - Clear reset
            WaitForRegisterSet(0x08, 0x1, RESET_TIMEOUT);
        }

        private void CreateBuffers(ushort bufferSize)
        {
            BufferSizeBytes = bufferSize * 4; // 16-bit stereo = 4 bytes per sample
            BDL = new BufferDescriptor[BUFFER_COUNT];
            Buffers = new byte[BUFFER_COUNT][];

            for (int i = 0; i < BUFFER_COUNT; i++)
            {
                Buffers[i] = new byte[BufferSizeBytes];

                // Manually align memory to 128 bytes (common DMA alignment)
                uint alignedAddress = (uint)Heap.Alloc((uint)(BufferSizeBytes + 127));
                alignedAddress = (alignedAddress + 127) & ~127U; // Align to 128 bytes

                BDL[i].Address = alignedAddress;

                // Copy buffer data
                fixed (byte* ptr = Buffers[i])
                {
                    MemoryOperations.Copy((byte*)BDL[i].Address, ptr, BufferSizeBytes);
                }

                // Configure BDL
                BDL[i].Control = (uint)(BufferSizeBytes & 0xFFFF); // Set buffer size
                if (i == BUFFER_COUNT - 1)
                {
                    BDL[i].Control |= 1U << 31; // Mark as the last buffer
                }
            }
        }

        private void ProvideBuffers()
        {
            // Write the BDL address to the stream descriptor
            fixed (BufferDescriptor* bdlPtr = BDL)
            {
                Write32(StreamBase + 0x18, (uint)bdlPtr); // DMA base address
                Write32(StreamBase + 0x1C, (uint)(BUFFER_COUNT * sizeof(BufferDescriptor))); // BDL size
            }

            // Set the last valid buffer index
            LastValidIdx = (byte)(BUFFER_COUNT - 1);
            Write8(StreamBase + 0x15, LastValidIdx);
        }

        private void HandleInterrupt(ref INTs.IRQContext aContext)
        {
            uint status = Read32(StreamBase + 0x24); // Stream status register
            if ((status & 0x4) != 0) // Buffer completion interrupt
            {
                int nextBuffer = (LastValidIdx + 1) % BUFFER_COUNT;

                // Request a new buffer from the buffer provider
                var audioBuffer = new AudioBuffer(BufferSizeBytes / 4, new SampleFormat(AudioBitDepth.Bits16, 2, true));
                BufferProvider.RequestBuffer(audioBuffer);

                fixed (byte* audioData = audioBuffer.RawData)
                {
                    MemoryOperations.Copy((byte*)BDL[nextBuffer].Address, audioData, BufferSizeBytes);
                }

                LastValidIdx = (byte)nextBuffer;

                // Acknowledge the interrupt
                Write32(StreamBase + 0x24, status);
            }
        }

        private void WaitForRegisterClear(uint reg, uint mask, uint timeout)
        {
            uint time = 0;
            while ((Read32(reg) & mask) != 0)
            {
                if (time++ > timeout)
                {
                    throw new InvalidOperationException($"Timeout clearing register {reg:X}");
                }
            }
        }

        private void WaitForRegisterSet(uint reg, uint mask, uint timeout)
        {
            uint time = 0;
            while ((Read32(reg) & mask) == 0)
            {
                if (time++ > timeout)
                {
                    throw new InvalidOperationException($"Timeout setting register {reg:X}");
                }
            }
        }

        public override void Enable()
        {
            if (Enabled)
                return;

            // Enable the stream
            Write8(StreamBase + 0x08, 0x02); // Set RUN bit in control register
        }

        public override void Disable()
        {
            // Disable the stream
            Write8(StreamBase + 0x08, 0x00); // Clear RUN bit in control register
        }

        public override bool Enabled => (Read8(StreamBase + 0x08) & 0x02) != 0;

        public override SampleFormat[] GetSupportedSampleFormats()
        {
            return new[]
            {
                new SampleFormat(AudioBitDepth.Bits16, 1, true),  // 16-bit PCM, mono
                new SampleFormat(AudioBitDepth.Bits24, 1, true),  // 24-bit PCM, mono
                new SampleFormat(AudioBitDepth.Bits32, 1, true),  // 32-bit PCM, mono
                new SampleFormat(AudioBitDepth.Bits16, 2, true),  // 16-bit PCM, stereo
                new SampleFormat(AudioBitDepth.Bits24, 2, true),  // 24-bit PCM, stereo
                new SampleFormat(AudioBitDepth.Bits32, 2, true),  // 32-bit PCM, stereo
                new SampleFormat(AudioBitDepth.Bits16, 6, true),  // 16-bit PCM, 5.1 surround
                new SampleFormat(AudioBitDepth.Bits24, 6, true),  // 24-bit PCM, 5.1 surround
                new SampleFormat(AudioBitDepth.Bits32, 6, true),  // 32-bit PCM, 5.1 surround
                new SampleFormat(AudioBitDepth.Bits16, 8, true),  // 16-bit PCM, 7.1 surround
                new SampleFormat(AudioBitDepth.Bits24, 8, true),  // 24-bit PCM, 7.1 surround
                new SampleFormat(AudioBitDepth.Bits32, 8, true),  // 32-bit PCM, 7.1 surround
            };
        }

        public override void SetSampleFormat(SampleFormat format)
        {
            if (!GetSupportedSampleFormats().Contains(format))
            {
                throw new NotSupportedException("Unsupported sample format!");
            }
        }

        private uint Read32(uint offset) => MMIOBase[offset / 4];
        private void Write32(uint offset, uint value) => MMIOBase[offset / 4] = value;

        private byte Read8(uint offset) => *((byte*)MMIOBase + offset);
        private void Write8(uint offset, byte value) => *((byte*)MMIOBase + offset) = value;
    }
}