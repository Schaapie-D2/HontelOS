using Cosmos.Core;
using Cosmos.HAL.Audio;
using System;
using System.Runtime.InteropServices;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.HAL;

namespace HontelOS.Drivers.Audio
{
    /// <summary>
    /// Handles ES1371-compatible sound cards at a low-level.
    /// </summary>
    public sealed unsafe class ES1371 : AudioDriver
    {
        public override IAudioBufferProvider BufferProvider { get; set; }

        /// <summary>
        /// Describes a single entry in the Buffer Descriptor List of a
        /// ES1371-compatible sound card.
        /// </summary>
        private unsafe struct BufferDescriptorListEntry
        {
            /// <summary>
            /// The pointer to the buffer's sample data in memory.
            /// </summary>
            public byte* pointer;

            /// <summary>
            /// The number of samples in this buffer.
            /// </summary>
            public ushort bufferSize;

            /// <summary>
            /// Describes the configuration of this buffer.
            /// <list type="bullet">
            ///     <item><b>Bits 0 - 13:</b> Reserved</item>
            ///     <item><b>Bit 14:</b> Last entry of the BDL; stop playing</item>
            ///     <item><b>Bit 15:</b> Fire an IRQ when data from this entry is transferred</item>
            /// </list>
            /// </summary>
            public ushort configuration;
        }

        /// <summary>
        /// PCI information for the sound card handled by this driver.
        /// </summary>
        public PCIDevice PCI { get; set; }

        AudioBuffer transferBuffer; // Acts as an intermediary buffer to convert between sample formats.
        byte[][] buffers;           // The buffers in memory. These buffers are described in the bufferDescriptorList.
        BufferDescriptorListEntry[] bufferDescriptorList; // Buffer information for the ES1371.

        byte lastValidIdx;
        int bufferSizeSamples, bufferSizeBytes;

        // Private variables prefixed with a "p" indicate an I/O port.
        readonly ushort pTransferControl;    // Allows us to start/stop transfers and control what IRQs are fired
        readonly ushort pMasterVolume;       // Mixer volume setting
        readonly ushort pPCMOutVolume;       // Speaker output volume setting
        readonly ushort pBufferDescriptors;  // Buffer Descriptor List base address
        readonly ushort pLastValidEntry;     // Contains the number of the last Buffer Entry that will be processed
        readonly ushort pTransferStatus;     // Used to query the DMA transfer status and for IRQ acknowledgment
        readonly ushort pGlobalControl;      // Controls basic ES1371 functions
        readonly ushort pResetRegister;      // Writing any value to port will cause a register reset

        const uint RESET_POLL_LIMIT = 1000; // The maximum amount of polls for a reset the driver can perform.

        const int TC_RUN_OR_PAUSE = 1 << 0;
        const int TC_TRANSFER_RESET = 1 << 1;
        const int TC_ENABLE_LAST_VALID_BUF_INTERRUPT = 1 << 2;
        const int TC_ENABLE_FIFO_ERROR_INTERRUPT = 1 << 3;
        const int TC_ENABLE_COMPLETION_INTERRUPT = 1 << 4;

        const ushort IRQ_LVBCI = 1 << 2;
        const ushort IRQ_BCIS = 1 << 3;
        const ushort IRQ_FIFO_ERROR = 1 << 4;

        const ushort BD_FIRE_INTERRUPT_ON_CLEAR = 1 << 15;

        const int GC_GLOBAL_INTERRUPT_ENABLE = 0x1;

        const byte ES1371_VOLUME_MAX = 0;
        const byte ES1371_VOLUME_MIN = 63;

        private const ushort ES1371_REG_CONTROL = 0x00;
        private const ushort ES1371_REG_STATUS = 0x04;
        private const ushort ES1371_REG_UART_DATA = 0x08;
        private const ushort ES1371_REG_UART_STATUS = 0x09;
        private const ushort ES1371_REG_UART_CONTROL = 0x09;
        private const ushort ES1371_REG_UART_CLOCK = 0x0A;
        private const ushort ES1371_REG_SAMPLE_RATE = 0x10;
        private const ushort ES1371_REG_RING_BUS_CONTROL = 0x14;
        private const ushort ES1371_REG_DAC1_SCOUNT = 0x20;
        private const ushort ES1371_REG_DAC2_SCOUNT = 0x24;
        private const ushort ES1371_REG_ADC_SCOUNT = 0x28;
        private const ushort ES1371_REG_DAC1_FRAME = 0x2C;
        private const ushort ES1371_REG_DAC2_FRAME = 0x30;
        private const ushort ES1371_REG_ADC_FRAME = 0x34;
        private const ushort ES1371_REG_DAC1_CONTROL = 0x40;
        private const ushort ES1371_REG_DAC2_CONTROL = 0x44;
        private const ushort ES1371_REG_ADC_CONTROL = 0x48;
        private const ushort ES1371_REG_UART_CONTROL2 = 0x4C;
        private const ushort ES1371_REG_UART_STATUS2 = 0x4E;
        private const ushort ES1371_REG_MEMORY_PAGE = 0x54;
        private const ushort ES1371_REG_UART_CLOCK2 = 0x55;
        private const ushort ES1371_REG_JOYSTICK = 0x58;
        private const ushort ES1371_REG_DAC1_SCOUNTER = 0x5C;
        private const ushort ES1371_REG_DAC2_SCOUNTER = 0x60;
        private const ushort ES1371_REG_ADC_SCOUNTER = 0x64;
        private const ushort ES1371_REG_HOST_IO_BASE = 0x68;


        /// <summary>
        /// Creates a mixer volume value to provide to an I/O port to set
        /// the output volume of the ES1371.
        /// </summary>
        /// <param name="right">The right channel volume. Must be between 0 and 63.</param>
        /// <param name="left">The left chanel volume. Must be between 0 and 63.</param>
        /// <param name="mute">Whether to mute all output of the channel.</param>
        private static ushort CreateMixerVolumeValue(byte right, byte left, bool mute)
            => (ushort)((right & 0x3f) | ((left & 0x3f) << 8) | ((mute ? 1 : 0) & (1 << 15)));

        /// <summary>
        /// Creates a new instance of the <see cref="ES1371"/> class, with the
        /// given buffer size.
        /// </summary>
        /// <param name="bufferSize">The buffer size in samples to use. This value cannot be an odd number, as per the ES1371 specification.</param>
        /// <exception cref="ArgumentException">Thrown when the given buffer size is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no ES1371-compatible sound card is present.</exception>
        ushort NAMbar, NABMbar;

        public static byte* Buffer;
        public const int SampleRate = 44100;
        public const int SizePerPacket = SampleRate * 2;
        public const int CacheSize = 1024 * 86;
        int BAR0 = 0;
        public byte[] newBuffer;
        public byte[] CurrentBuffer { get; private set; }

        public unsafe void ChangeBuffer(byte[] target)
        {
            target = new byte[CacheSize];
            fixed (byte* ptr = target)
            {
                IOPort.Write32(BAR0 + 0x38, (uint)ptr);
            }

            CurrentBuffer = target;
        }
        bool CanTake;
        int bytesWritten;
        byte[] _pcm;
        int _index;
        WAV.Header _header;
        public string _song_name;

        private ES1371(ushort bufferSize)
        {
            if (bufferSize % 2 != 0)
                // As per the ES1371 specification, the buffer size cannot be odd.
                // (1.2.4.2 PCM Buffer Restrictions, Intel document 302349-003)
                throw new ArgumentException("The buffer size must be an even number.", nameof(bufferSize));

            PCIDevice pci = Cosmos.HAL.PCI.GetDevice(
                VendorID.VMWare, // 0x04
                DeviceID.VBVGA         // 0x01
            ); ;

            if (pci == null || !pci.DeviceExists || pci.InterruptLine > 0xF)
                throw new InvalidOperationException("No ES1371-compatible device could be found. " + pci.InterruptLine);

            pci.WriteRegister16(0x04, 0x04 | 0x02 | 0x01);
            BAR0 = (int)(pci.BAR0 & ~0x3);
            Buffer = (byte*)NativeMemory.Alloc(CacheSize);
            CanTake = true;
            bytesWritten = 0;

            Console.WriteLine($"[ES1371] BAR0:{BAR0}");

            // Create all needed buffers
            CreateBuffers(bufferSize);

            // Initialization done - driver can now be activated by using Enable()
        }
        public int snd_write(byte* buffer, int len)
        {
            CanTake = false;
            if (bytesWritten + len > CacheSize)
            {
                MemoryOperations.Copy(Buffer + bytesWritten - len, Buffer + bytesWritten, len);
                bytesWritten -= len;
            }

            MemoryOperations.Copy(Buffer + bytesWritten, buffer, len);
            bytesWritten += len;
            CanTake = true;
            snd_clear();
            return len;
        }
        public void snd_clear()
        {
            bytesWritten = 0;
        }
        public bool require(byte* buffer)
        {
            if (CanTake && bytesWritten > 0)
            {
                int size = SizePerPacket > bytesWritten ? bytesWritten : SizePerPacket;

                MemoryOperations.Copy(buffer, Buffer, size);
                bytesWritten -= size;
                if (bytesWritten > SizePerPacket)
                {
                    MemoryOperations.Copy(Buffer, Buffer + size, bytesWritten);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The global instance of the ES1371. This property will return
        /// <see langword="null"/> if the driver has not been initialized.
        /// </summary>
        public static ES1371 Instance { get; private set; } = null;

        /// <summary>
        /// Initializes the ES1371 driver. This method will return
        /// an existing instance if the driver is already initialized
        /// and has a running instance.
        /// </summary>
        /// <param name="bufferSize">The buffer size in samples to use. This value cannot be an odd number, as per the ES1371 specification.</param>
        /// <exception cref="ArgumentException">Thrown when the given buffer size is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no ES1371-compatible sound card is present.</exception>
        public static ES1371 Initialize(ushort bufferSize)
        {
            if (Instance != null)
            {
                if (Instance.bufferSizeSamples != bufferSize)
                    Instance.ChangeBufferSize(bufferSize);

                return Instance;
            }

            Instance = new ES1371(bufferSize);
            return Instance;
        }

        const int BUFFER_COUNT = 32;
        private unsafe void CreateBuffers(ushort bufferSize)
        {
            transferBuffer = new AudioBuffer(bufferSize, new SampleFormat(AudioBitDepth.Bits16, 2, true));
            bufferSizeSamples = bufferSize;
            bufferSizeBytes = bufferSize * transferBuffer.Format.Size;

            bufferDescriptorList = new BufferDescriptorListEntry[BUFFER_COUNT];
            buffers = new byte[BUFFER_COUNT][];

            for (int i = 0; i < BUFFER_COUNT; i++)
            {
                buffers[i] = new byte[bufferSizeBytes];
                fixed (byte* ptr = buffers[i])
                {
                    bufferDescriptorList[i].pointer = ptr;
                }

                bufferDescriptorList[i].bufferSize = bufferSize;
                bufferDescriptorList[i].configuration |= BD_FIRE_INTERRUPT_ON_CLEAR;
            }
        }

        /// <summary>
        /// Changes the size of the internal buffers. This will result
        /// in a slight interruption in audio.
        /// </summary>
        /// <param name="newSize">The new buffer size, in samples. This value cannot be an odd number, as per the ES1371 specification.</param>
        /// <exception cref="ArgumentException">Thrown when the given buffer size is invalid.</exception>
        public void ChangeBufferSize(ushort newSize)
        {
            if (newSize % 2 != 0)
                throw new ArgumentException("The new buffer size must be an even number.", nameof(newSize));

            if (newSize == bufferSizeSamples)
                return; // No action needed

            CreateBuffers(newSize);
            ProvideBuffers();
        }

        /// <summary>
        /// Provides the buffers to the sound card.
        /// </summary>
        private void ProvideBuffers()
        {
            // Tell BDL location
            fixed (BufferDescriptorListEntry* ptr = bufferDescriptorList)
            {
                IOPort.Write32(pBufferDescriptors, (uint)ptr);
            }

            // Set last valid index
            lastValidIdx = 2; // Start at the 3rd buffer. This will give us some headroom and will decrease clicks.
            IOPort.Write8(pLastValidEntry, lastValidIdx);
        }

        public void HandleInterrupt(ref INTs.IRQContext context)
        {
            uint sts = IOPort.Read32(BAR0 + 0x04);
            if (BitHelpers.IsBitSet(sts, 1))
            {
                Console.WriteLine("can play");
                IOPort.Write32(BAR0 + 0x20, IOPort.Read32(BAR0 + 0x20) & 0xFFFFFDFF);

                Native.Stosb(Buffer, 0, CacheSize);
                require(Buffer);
            }
            else
            {
                Console.WriteLine("cannot play");
                throw new Exception("cannot play");
            }
        }
        public void DoPlay(ref INTs.IRQContext context)
        {
            if (bytesWritten != 0) return;
            if (_index + CacheSize > _pcm.Length) _index = 0;

            fixed (byte* buffer = _pcm)
            {
                _index += CacheSize;
                snd_write(buffer + _index, CacheSize);
            }
        }

        public override SampleFormat[] GetSupportedSampleFormats()
            => new SampleFormat[]
            {
                    new SampleFormat(AudioBitDepth.Bits16, 2, true)
				// TODO: Implement more channels, as the ES1371 spec defines 2/4/6 channel audio
			};

        public override void SetSampleFormat(SampleFormat sampleFormat)
        {
            if (sampleFormat.BitDepth != AudioBitDepth.Bits16)
                throw new NotSupportedException("The ES1371 driver only supports 16-bit audio.");

            if (sampleFormat.Channels != 2)
                throw new NotSupportedException("The ES1371 driver only supports stereo audio.");

            if (!sampleFormat.Signed)
                throw new NotSupportedException("The ES1371 driver does not support unsigned audio.");

            // TODO: The ES1371 specification defines support 2/4/6 channel audio. Currently, only stereo audio output is supported.
        }

        public override bool Enabled =>
            (IOPort.Read8(pTransferControl) & TC_RUN_OR_PAUSE) != 0;
        public override void Enable()
        {
            if (Enabled)
            {
                //WaterfallBoot.CLILogs.WriteInfo("Already enabled");
                return; // Ignore calls to Enable() if the driver is already enabled
            }
            _index = 0;
            WAV.Decode(transferBuffer.RawData, out var pcm, out var hdr);
            //wav.Dispose();
            _pcm = pcm;
            _header = hdr;
            IOPort.Write32(BAR0 + 0x04, 2);
            //transferBuffer.ReadSample(0, Buffer);
            IOPort.Write32(BAR0 + 0x14, 0x00020000);
            IOPort.Write32(BAR0 + 0x14, 0x00180000);
            IOPort.Write32(BAR0 + 0x10, 0xeb403800);
            SetPlayback2SampleRate(44100);
            IOPort.Write32(BAR0 + 0x0c, 0x0c);
            IOPort.Write32(BAR0 + 0x38, (uint)Buffer);
            ChangeBuffer(CurrentBuffer);
            IOPort.Write32(BAR0 + 0x3c, 4096);
            IOPort.Write32(BAR0 + 0x28, 0x7FFF);
            IOPort.Write32(BAR0 + 0x20, 0x0020020C);
            IOPort.Write32(BAR0 + 0x00, 0x00000020);
            var context = new INTs.IRQContext();
            INTs.SetIntHandler(0x20, HandleInterrupt); // The issue starts here somewhere
            INTs.HandleInterrupt_20(ref context);
            INTs.SetIntHandler(0x20, DoPlay);
            while (true)
            {
                INTs.HandleInterrupt_20(ref context);
            }
        }

        void SetPlayback2SampleRate(int rate)
        {
            long frequency = (rate << 16) / 3000;

            IOPort.Write32(BAR0 + 0x75, (uint)((frequency >> 6) & 0xfc00));
            IOPort.Write32(BAR0 + 0x77, (uint)(frequency >> 1));
        }

        public override void Disable()
        {
            // Set audio to paused by clearing the run bit
            byte transferControl = IOPort.Read8(pTransferControl);
            IOPort.Write8(pTransferControl, transferControl);

            // Optionally, disable all interrupts
            IOPort.Write8(pTransferControl, 0x0); //Disable all interrupts by clearing the control register

            // Reset the global control register if necessary
            uint globalControl = IOPort.Read32(pGlobalControl);
            IOPort.Write32(pGlobalControl, globalControl);

        }
    }
    public static class BitHelpers
    {
        public static bool IsBitSet(sbyte val, int pos) => (((byte)val) & (1U << pos)) != 0;
        public static bool IsBitSet(byte val, int pos) => (val & (1U << pos)) != 0;
        public static bool IsBitSet(short val, int pos) => (((ushort)val) & (1U << pos)) != 0;
        public static bool IsBitSet(ushort val, int pos) => (val & (1U << pos)) != 0;
        public static bool IsBitSet(int val, int pos) => (((uint)val) & (1U << pos)) != 0;
        public static bool IsBitSet(uint val, int pos) => (val & (1U << pos)) != 0;
        public static bool IsBitSet(long val, int pos) => (((ulong)val) & (1UL << pos)) != 0;
        public static bool IsBitSet(ulong val, int pos) => (val & (1UL << pos)) != 0;
    }
    public static unsafe class WAV
    {
        public struct Header
        {
            public uint ChunkID;
            public uint ChunkSize;
            public uint Format;
            public uint Subchunk1ID;
            public uint Subchunk1Size;
            public ushort AudioFormat;
            public ushort NumChannels;
            public uint SampleRate;
            public uint ByteRate;
            public ushort BlockAlign;
            public ushort BitsPerSample;
            public uint Subchunk2ID;
            public uint Subchunk2Size;
        }

        public static void Decode(byte[] WAV, out byte[] PCM, out Header header)
        {
            fixed (byte* PWAV = WAV)
            {
                Header* hdr = (Header*)PWAV;

                if (hdr->AudioFormat != 1)
                {
                    PCM = null;
                    header = default;
                    return;
                }
                PCM = new byte[hdr->Subchunk2Size];
                fixed (byte* PPCM = PCM)
                {
                    MemoryOperations.Copy(PPCM, PWAV + sizeof(Header), PCM.Length);
                }
                header = *hdr;
            }
        }
    }

}