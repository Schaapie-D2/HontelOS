/*
* PROJECT:          HontelOS
* CONTENT:          Sound Blaster 16 Audio driver
* PROGRAMMERS:      Jort van Dalen
* 
* https://gist.github.com/ascpixi/af4f138bf61a2a3f62c670a34ee1f7ce
* gist by ascpixi and modified to work with the new Cosmos Audio Infrastructure
* 
* DOES NOT WORK YET
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System;
using Cosmos.Core;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.HAL.Audio;

namespace HontelOS.Drivers.Audio
{
    /// <summary>
    /// Handles Sound Blaster 16-compatible sound cards at a low level.
    /// </summary>
    public unsafe sealed class SoundBlaster16 : AudioDriver
    {
        // NOTE: You should probably change this to a buffer received from an allocator instead.
        //       This WILL cause problems if this address happens to be taken up by another resource, or if it isn't RAM.
        private const int AUDIO_BUFFER_ADDRESS = 0x4D8BBB;

        AudioBuffer buffer;
        ushort bufferSize;

        readonly ushort pMixer = 0x224;
        readonly ushort pMixerData = 0x225;
        readonly ushort pReset = 0x226;
        readonly ushort pRead = 0x22A;
        readonly ushort pWrite = 0x22C;
        readonly ushort p8BitIRQAck = 0x22E;
        readonly ushort p16BitIRQAck = 0x22F;

        readonly ushort pPICInterruptAcknowledge = 0x20;
        readonly ushort pPICExtendedInterruptAcknowledge = 0xA0;

        const int DSP_SET_TIME_CONSTANT = 0x40;
        const int DSP_SET_OUTPUT_SAMPLE_RATE = 0x41;
        const int DSP_TURN_SPEAKER_ON = 0xD1;
        const int DSP_TURN_SPEAKER_OFF = 0xD3;
        const int DSP_STOP_8BIT_CHANNEL = 0xD0;
        const int DSP_RESUME_8BIT_CHANNEL = 0xD4;
        const int DSP_STOP_16BIT_CHANNEL = 0xD5;
        const int DSP_RESUME_16BIT_CHANNEL = 0xD6;
        const int DSP_GET_VERSION = 0xE1;

        const int MIXER_SET_MASTER_VOLUME = 0x22;
        const int MIXER_SET_IRQ = 0x80;

        const byte MIXER_IRQ_2 = 0x01;
        const byte MIXER_IRQ_5 = 0x02;
        const byte MIXER_IRQ_7 = 0x04;
        const byte MIXER_IRQ_10 = 0x08;

        public override IAudioBufferProvider BufferProvider { get; set; }

        /// <summary>
        /// The sampling rate of the driver.
        /// </summary>
        public ushort SampleRate { get; set; } = 44100;

        /// <summary>
        /// The version of the Digital Signal Processor of the Sound Blaster 16. 
        /// </summary>
        public Version DSPVersion { get; private set; }

        private bool enabled;
        public override bool Enabled => enabled;

        private SoundBlaster16(ushort bufferSize)
        {
            this.bufferSize = bufferSize;

            if (!ResetDSP())
                throw new InvalidOperationException("No Sound Blaster 16 device could be found - the DSP reset check has failed.");

            Console.WriteLine("The DSP has been reset.");

            IOPort.Write16(pWrite, DSP_GET_VERSION);
            byte versionMajor = IOPort.Read8(pRead);
            byte versionMinor = IOPort.Read8(pRead);
            DSPVersion = new Version(versionMajor, versionMinor);

            // Set the IRQ the SB16 will trigger
            IOPort.Write8(pMixer, MIXER_SET_IRQ);
            IOPort.Write8(pMixerData, MIXER_IRQ_10);

            // Set the IRQ handler itself
            INTs.SetIrqHandler(10, HandleInterrupt);
        }

        /// <summary>
        /// The global instance of the Sound Blaster 16 driver. This property will return
        /// <see langword="null"/> if the driver has not been initialized.
        /// </summary>
        public static SoundBlaster16 Instance { get; private set; } = null;

        /// <summary>
        /// Initializes the Sound Blaster 16 driver. This method will return
        /// an existing instance if the driver is already initialized
        /// and has a running instance.
        /// </summary>
        /// <param name="bufferSize">The buffer size in samples to use.</param>
        /// <param name="format">The audio format the Sound Blaster 16 should operate on.</param>
        /// <exception cref="InvalidOperationException">Thrown when no Sound Blaster 16 sound card is present.</exception>
        public static SoundBlaster16 Initialize(ushort bufferSize)
        {
            if (Instance != null)
            {
                if (Instance.bufferSize != bufferSize)
                {
                    Instance.ChangeBufferSize(bufferSize);
                }

                return Instance;
            }

            Instance = new SoundBlaster16(bufferSize);
            return Instance;
        }

        private void HandleInterrupt(ref INTs.IRQContext aContext)
        {
            if (!enabled)
            {
                IOPort.Write8(pWrite, 0xDA); // stop any 8-bit output
                IOPort.Write8(pWrite, 0xD9); // stop any 16-bit output
                return;
            }

            // Acknowledge the IRQ
            if (buffer.Format.BitDepth == AudioBitDepth.Bits8)
            {
                ReadIOPort(p8BitIRQAck);
            }
            else
            {
                ReadIOPort(p16BitIRQAck);
            }

            // Acknowledge the PIC interrupt
            IOPort.Write8(pPICInterruptAcknowledge, 0x20);
        }

        public void ChangeBufferSize(ushort newSize)
        {
            // Validate the new buffer size (ensure it's within a reasonable range)
            if (newSize < 64 || newSize > 65536)
            {
                throw new InvalidOperationException($"Invalid buffer size: {newSize}. Must be between 64 and 65536.");
            }

            Disable();

            bufferSize = newSize;
            buffer = new AudioBuffer(newSize, buffer.Format);

            Enable();
        }

        public override void Enable()
        {
            // Make the SB16 play audio at max volume
            IOPort.Write8(pMixer, MIXER_SET_MASTER_VOLUME);
            IOPort.Write8(pMixerData, 0xFF);

            enabled = true;
            Start();
            Console.WriteLine("Enabled.");
        }

        public override void Disable()
        {
            IOPort.Write8(pWrite, DSP_STOP_16BIT_CHANNEL);
            IOPort.Write8(pWrite, DSP_STOP_8BIT_CHANNEL);
            enabled = false;
            Console.WriteLine("Disabled.");
        }

        /// <summary>
        /// Resets the DSP of the Sound Blaster 16.
        /// </summary>
        /// <returns>A value indicating whether the operation has succeeded.</returns>
        private bool ResetDSP()
        {
            IOPort.Write8(pReset, 1);
            Cosmos.HAL.Global.PIT.Wait(3);
            IOPort.Write8(pReset, 0);

            return IOPort.Read8(pRead) == 0xAA;
        }

        /// <summary>
        /// Starts the data transfer operation.
        /// </summary>
        private void Start()
        {
            Console.WriteLine("Starting the SB16...");
            IOPort.Write8(pWrite, DSP_TURN_SPEAKER_ON);
            ProgramDMATransfer(buffer.Format.BitDepth);

            // Set the sample rate
            IOPort.Write8(pWrite, DSP_SET_OUTPUT_SAMPLE_RATE);
            IOPort.Write8(pWrite, Hi(SampleRate));
            IOPort.Write8(pWrite, Lo(SampleRate));

            // Set the bit-depth
            IOPort.Write8(pWrite, buffer.Format.BitDepth == AudioBitDepth.Bits8 ? (byte)0xC6 : (byte)0xB6);

            // Set the channel number (stereo or mono) and if it's signed
            SetChannelFormat();

            // Set data size (the amount of *samples* to transfer)
            IOPort.Write8(pWrite, Lo((uint)bufferSize - 1));
            IOPort.Write8(pWrite, Hi((uint)bufferSize - 1));
            Console.WriteLine("Start routine complete.");
        }

        private void SetChannelFormat()
        {
            switch (buffer.Format.Channels)
            {
                case 1:
                    IOPort.Write8(pWrite, buffer.Format.Signed ? (byte)0b00_01_0000 : (byte)0b00_00_0000);
                    break;
                case 2:
                    IOPort.Write8(pWrite, buffer.Format.Signed ? (byte)0b00_11_0000 : (byte)0b00_10_0000);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported number of channels ({buffer.Format.Channels}). The Sound Blaster 16 only supports up to 2 channels.");
            }
        }

        /// <summary>
        /// Programs the DMA channel 1 for audio buffer transfer.
        /// </summary>
        private void ProgramDMATransfer(AudioBitDepth bitDepth)
        {
            switch (bitDepth)
            {
                case AudioBitDepth.Bits8:
                    ProgramDMATransfer(0x0A, 0x0C, 0x0B, 0x83, 0x02, 0x03);
                    break;
                case AudioBitDepth.Bits16:
                    ProgramDMATransfer(0xD4, 0xD8, 0xD6, 0x8B, 0xC4, 0xC6);
                    break;
                default:
                    throw new NotSupportedException("The Sound Blaster 16 only supports 8-bit and 16-bit DMA transfers.");
            }
        }

        private unsafe void ProgramDMATransfer(
            ushort enableDisablePort,
            ushort flipFlopPort,
            ushort transferModePort,
            ushort pagePort,
            ushort addressPort,
            ushort lengthPort
        )
        {
            // Fill both the main and the back-buffer
            BufferProvider.RequestBuffer(buffer);

            Console.WriteLine("Programming the DMA transfer.");

            // Turn the channel off
            IOPort.Write8(enableDisablePort, 0x05);

            // Flip-flop
            IOPort.Write8(flipFlopPort, 1);

            // Set transfer-mode
            IOPort.Write8(transferModePort, 0x58 + 1);

            fixed (byte* bufferPtr = buffer.RawData)
            {
                uint address = (uint)bufferPtr;

                Console.WriteLine($"The buffer is at 0x{address:X8}.");

                IOPort.Write8(pagePort, (byte)(address >> 4 >> 12));
                IOPort.Write8(addressPort, Lo(address));
                IOPort.Write8(addressPort, Hi(address));
            }

            // Set the data length
            IOPort.Write8(lengthPort, Lo((uint)buffer.RawData.Length));
            IOPort.Write8(lengthPort, Hi((uint)buffer.RawData.Length));

            IOPort.Write8(enableDisablePort, 1); // DMA channel 1
            Console.WriteLine("The DMA has been programmed.");
        }

        static byte ReadIOPort(ushort port)
        {
            return IOPort.Read8(port);
        }

        static byte Hi(ushort val) => (byte)(val >> 8);
        static byte Hi(uint val) => (byte)((val >> 8) & 0xff);
        static byte Lo(uint val) => (byte)(val & 0xff);
        static byte Lo(ushort val) => (byte)(val & 0xff);

        public override SampleFormat[] GetSupportedSampleFormats()
            => new SampleFormat[]
            {
                new SampleFormat(AudioBitDepth.Bits8, 1, true),
                new SampleFormat(AudioBitDepth.Bits8, 2, true),
                new SampleFormat(AudioBitDepth.Bits8, 1, false),
                new SampleFormat(AudioBitDepth.Bits8, 2, false),
                new SampleFormat(AudioBitDepth.Bits16, 1, true),
                new SampleFormat(AudioBitDepth.Bits16, 2, true),
                new SampleFormat(AudioBitDepth.Bits16, 1, false),
                new SampleFormat(AudioBitDepth.Bits16, 2, false),
            };

        public override void SetSampleFormat(SampleFormat format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            buffer = new AudioBuffer(bufferSize, format);
        }
    }
}
