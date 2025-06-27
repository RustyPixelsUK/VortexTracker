using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    public class RegisterEventArgs : EventArgs
    {
        public int ChipIndex { get; }
        public byte[] Registers { get; }
        public int SlotUs { get; }
        public long PlayAtUs { get; }      // absolute target time in µs

        public RegisterEventArgs(int chipIndex, byte[] registers, int slotUs, long playAtUs)
        {
            ChipIndex = chipIndex;
            Registers = registers;
            SlotUs = slotUs;
            PlayAtUs = playAtUs;
        }

        public override string ToString()
        {
            string regs = Registers.Length <= 16
                ? string.Join(' ', Registers.Select(b => b.ToString("X2")))
                : string.Join(' ', Registers.Take(16).Select(b => b.ToString("X2"))) + " …";

            return $"Chip={ChipIndex}, SlotUs={SlotUs}, PlayAtUs={PlayAtUs}, Regs=[{regs}]";
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Tik
    {
        [FieldOffset(0)]
        public ushort Lo;
        [FieldOffset(2)]
        public ushort Hi;
        [FieldOffset(0)]
        public uint Re;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TikCount
    {
        [FieldOffset(0)]
        public uint Lo;
        [FieldOffset(4)]
        public uint Hi;
        [FieldOffset(0)]
        public long Re;
    }

    // Available soundchips
    public enum ChipType
    {
        None,
        AY,
        YM
    }

    public enum PlayModes
    {
        PlayModule,
        PlayPattern,
        PlayLine
    }

    public class AY
    {
        public const int AYFreqDefault = 1750000; // 1773400;
        public const int InterruptFreqDefault = 48828; // 50000;
        public const int NumberOfChannelsDefault = 2;
        public const int SampleRateDefault = 44100;
        public const int SampleBitDefault = 16;
        public const int IndexA_L_Default = 255;
        public const int IndexA_R_Default = 13;
        public const int IndexB_L_Default = 170;
        public const int IndexB_R_Default = 170;
        public const int IndexC_L_Default = 13;
        public const int IndexC_R_Default = 255;
        public const int StdChannelsAllocationDefault = 1;
        public const int FilterLengthDefault = 32; // powers of 2

        public static int FilterLength = FilterLengthDefault;
        public static bool FilterEnabled;
        public static int[] FilterKernel;
        public static int[] DelayLineL;
        public static int[] DelayLineR;
        public static int FilterIndex = 0;
        public static PlayModes PlayMode;
        public static int DCType = 1;
        public static int DCCutOff = 3;
        public static int ChipCount = VTModule.MaxSoundChipCount; // Sound chip parameters
        public static SoundChip[] SoundChip = new SoundChip[VTModule.MaxSoundChipCount];
        public static Ayumi[] AyumiChip = new Ayumi[VTModule.MaxSoundChipCount];
        // Parameters for all sound chips
        //public static byte[] IndexL = new byte[3];
        //public static byte[] IndexR = new byte[3];
        public static byte IndexA_L;
        public static byte IndexA_R;
        public static byte IndexB_L;
        public static byte IndexB_R;
        public static byte IndexC_L;
        public static byte IndexC_R;
        public static ChipType EmulatingChip = ChipType.AY;
        public static int AYFreq;
        //public static int[,] LevelL = new int[3, 32];
        //public static int[,] LevelR = new int[3, 32];
        public static int[] LevelA_L = new int[32];
        public static int[] LevelA_R = new int[32];
        public static int[] LevelB_L = new int[32];
        public static int[] LevelB_R = new int[32];
        public static int[] LevelC_L = new int[32];
        public static int[] LevelC_R = new int[32];
        public static int ChannelL;
        public static int ChannelR;
        public static byte TickCounter;
        public static Tik Tik;
        public static int DelayInTiks;
        public static uint CurrentTik;
        public static TikCount TikCount;
        public static bool IntFlag;
        public static uint AYTiksInInterrupt;
        public static SynthesizerDelegate Synthesizer;
        public static int StdChannelsAllocation;
        public static bool[] RealEnd = new bool[VTModule.MaxSoundChipCount];
        public static VTM ActiveModule = null;
        public static VTM[] PlayingModule = new VTM[VTModule.MaxSoundChipCount];
        public static int PlayingModuleCount = 0;
        public static bool RealEndAll;
        public static bool LoopAllowed;
        public static bool IsFinished = false;
        public static int RenderEngine = 2;

        public static Stopwatch SongClock = new Stopwatch();
        private static long _nextPlayUs = 0;          // initialised on first call
        public static int SlotUs;

        public static event EventHandler<RegisterEventArgs>? RegisterEvent;

        public delegate void SynthesizerDelegate(byte[] buf);

        // Two amplitude tables of sound chips (c) Introspec
        public static ushort[] Amplitudes_AY =  
        {
            0x0000, 0x028F, 0x03B3, 0x0564, 0x07DC, 0x0BA9, 0x1083, 0x1B7C, 0x2068, 0x347A, 0x4ACE,
            0x5F72, 0x7E16, 0xA2A4, 0xCE3A, 0xFFFF
        };

        public static ushort[] Amplitudes_YM =
        {
            0x0000, 0x0000, 0x0131, 0x01FA, 0x02CE, 0x0393, 0x045A, 0x0520, 0x063D, 0x079A, 0x08FA,
            0x0A57, 0x0C6D, 0x0EEF, 0x116C, 0x13E9, 0x17AF, 0x1C70, 0x2137, 0x2603, 0x2D3A, 0x3628,
            0x3F13, 0x47F6, 0x556F, 0x6682, 0x77A6, 0x88D0, 0xA29A, 0xC20C, 0xE142, 0xFFFF
        };
 
        public static int NoiseGenerator(int seed)
        {
            int high16 = seed >> 16;
            int high13 = seed >> 13;
            int xorBit = (high16 ^ high13) & 1;
            int result = ((seed * 2) & 0x1FFFF) + 1;

            return result ^ xorBit;
        }

        public static int ApplyFilter(int level, int[] delayLine)
        {
            delayLine[FilterIndex] = level;

            long accumulator = (long)level * FilterKernel[0];
            int index = FilterIndex;

            for (int i = 1; i <= FilterLength; i++)
            {
                if (--index < 0)
                    index = FilterLength;

                accumulator += (long)delayLine[index] * FilterKernel[i];
            }

            FilterIndex = index;

            if (accumulator < 0)
                accumulator += 0xFFFFFF;

            int result = (int)(((ulong)accumulator) >> 24);

            return result;
        }

        public static void InitPlayGrid()
        {
            WaveOutAPI.PlayGridLength = (uint)(WaveOutAPI.BufferLength * WaveOutAPI.BufferCount * 2);
            WaveOutAPI.PlayGrid = new PlayGrid[WaveOutAPI.PlayGridLength];

            for (int i = 0; i < WaveOutAPI.PlayGridLength; i++)
                WaveOutAPI.PlayGrid[i] = new PlayGrid();
        }

        public static void FillPlayGrid()
        {
            uint k = WaveOutAPI.TickCount / WaveOutAPI.PlayGridLength;
            WaveOutAPI.PlayGridIndex = WaveOutAPI.TickCount - (WaveOutAPI.PlayGridLength * k);

            for (int i = 0; i < ChipCount; i++)
            {
                PlayArgs playArgs = VTModule.PlayArgs[i];
                PlayInfo playInfo = WaveOutAPI.PlayGrid[WaveOutAPI.PlayGridIndex].Module[i];

                playInfo.Position = playArgs.PositionIndex;
                playInfo.Pattern = playArgs.PatternIndex;
                playInfo.Line = playArgs.LineIndex - 1;
            }
        }

        public static void SetAyumiRegisters(Ayumi ayumi, AYRegisters regs)
        {
            ayumi.SetTone(0, regs.ToneA);
            ayumi.SetTone(1, regs.ToneB);
            ayumi.SetTone(2, regs.ToneC);

            ayumi.SetNoise(regs.Noise);

            ayumi.SetMixer(0, regs.Mixer & 1, (regs.Mixer >> 3) & 1, regs.AmplitudeA >> 4);
            ayumi.SetMixer(1, (regs.Mixer >> 1) & 1, (regs.Mixer >> 4) & 1, (regs.AmplitudeB >> 4) & 1);
            ayumi.SetMixer(2, (regs.Mixer >> 2) & 1, (regs.Mixer >> 5) & 1, (regs.AmplitudeC >> 4) & 1);

            ayumi.SetVolume(0, regs.AmplitudeA & 0x0F);
            ayumi.SetVolume(1, regs.AmplitudeB & 0x0F);
            ayumi.SetVolume(2, regs.AmplitudeC & 0x0F);

            ayumi.SetEnvelope(regs.Envelope);

            if (regs.EnvType != 0xFF)
                ayumi.SetEnvelopeShape(regs.EnvType);

            regs.EnvType = 0xFF;
        }

        public static void Synthesizer_Stereo16(byte[] buffer)
        {
            do
            {
                int levelL = 0;
                int levelR = 0;

                for (int i = 0; i < ChipCount; i++)
                {
                    SoundChip[i].Synthesizer_Logic_Q();
                    SoundChip[i].Synthesizer_Mixer_Q(ref levelL, ref levelR);
                }

                if (FilterEnabled)
                {
                    int filterIndex = FilterIndex;
                    levelL = ApplyFilter(levelL, DelayLineL);
                    FilterIndex = filterIndex;
                    levelR = ApplyFilter(levelR, DelayLineR);
                }

                ChannelL += levelL;
                ChannelR += levelR;

                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    short sampleLeft = (short)Math.Clamp(ChannelL / TickCounter, -32768, 32767);
                    short sampleRight = (short)Math.Clamp(ChannelR / TickCounter, -32768, 32767);

                    int bufferPosition = WaveOutAPI.BufferPosition * 4;
                    buffer[bufferPosition + 0] = (byte)(sampleLeft & 0xFF);
                    buffer[bufferPosition + 1] = (byte)((sampleLeft >> 8) & 0xFF);
                    buffer[bufferPosition + 2] = (byte)(sampleRight & 0xFF);
                    buffer[bufferPosition + 3] = (byte)((sampleRight >> 8) & 0xFF);

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    ChannelL = 0;
                    ChannelR = 0;

                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void Synthesizer_Stereo8(byte[] buffer)
        {
            do
            {
                int levelL = 0;
                int levelR = 0;

                for (int i = 0; i < ChipCount; i++)
                {
                    SoundChip[i].Synthesizer_Logic_Q();
                    SoundChip[i].Synthesizer_Mixer_Q(ref levelL, ref levelR);
                }

                if (FilterEnabled)
                {
                    int filterIndex = FilterIndex;
                    levelL = ApplyFilter(levelL, DelayLineL);
                    FilterIndex = filterIndex;
                    levelR = ApplyFilter(levelR, DelayLineR);
                }

                ChannelL += levelL;
                ChannelR += levelR;

                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    int bufferPosition = WaveOutAPI.BufferPosition * 2;

                    buffer[bufferPosition + 0] = (byte)(128 + Math.Clamp(ChannelL / TickCounter, -128, 127));
                    buffer[bufferPosition + 1] = (byte)(128 + Math.Clamp(ChannelR / TickCounter, -128, 127));

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    ChannelL = 0;
                    ChannelR = 0;
                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);
            
            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void OutputRegisters(AYRegisters regs)
        {
            string s = string.Format($"TA={regs.ToneA:X4}, TB={regs.ToneB:X4}, TC={regs.ToneC:X4}, N={regs.Noise:X2}, M={regs.Mixer:X2}, AA={regs.AmplitudeA:X2}, AB={regs.AmplitudeB:X2}, AC={regs.AmplitudeC:X2}, E={regs.Envelope:X4}, ET={regs.EnvType:X2}");

            Console.WriteLine(s);
        }

        public static void Synthesizer_Ayumi(byte[] buffer)
        {
            int volume = 0;
            double levelRatio = 0;
            const double minPeak = -1.6;
            const double maxPeak = 1.6;

            for (int i = 0; i < ChipCount; i++)
            {
                AYRegisters regs = SoundChip[i].AYRegisters;

                SetAyumiRegisters(AyumiChip[i], regs);

                //OutputRegisters(regs);
            }

            do
            {
                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    double levelL = 0;
                    double levelR = 0;

                    for (int i = 0; i < ChipCount; i++)
                    {
                        AyumiChip[i].Process();
                        AyumiChip[i].RemoveDC();

                        levelL += AyumiChip[i].Left;
                        levelR += AyumiChip[i].Right;
                    }

                    if (Main.GlobalVolume != volume)
                    {
                        volume = Main.GlobalVolume;
                        levelRatio = Math.Exp(volume * Math.Log(2.0) / Main.GlobalVolumeMax) - 1;
                    }

                    levelL *= levelRatio;
                    levelR *= levelRatio;

                    levelL = Math.Clamp(levelL, minPeak, maxPeak);
                    levelR = Math.Clamp(levelR, minPeak, maxPeak);

                    switch (WaveOutAPI.NumberOfChannels)
                    {
                        // MONO
                        case 1:
                            switch (WaveOutAPI.SampleBit)
                            {
                                case 16:
                                    {
                                        int bufferPosition = WaveOutAPI.BufferPosition * 2; // 2 bytes per sample
                                        short sample = (short)Math.Round(((levelL + levelR) / 2) * 0x4FFF);
                                        buffer[bufferPosition + 0] = (byte)(sample & 0xFF);
                                        buffer[bufferPosition + 1] = (byte)((sample >> 8) & 0xFF);
                                    }
                                    break;

                                case 24:
                                    {
                                        int bufferPosition = WaveOutAPI.BufferPosition * 3; // 3 bytes per sample
                                        int sample = (int)Math.Round(((levelL + levelR) / 2) * 0x4FFFFF);
                                        buffer[bufferPosition + 0] = (byte)(sample & 0xFF);
                                        buffer[bufferPosition + 1] = (byte)((sample >> 8) & 0xFF);
                                        buffer[bufferPosition + 2] = (byte)((sample >> 16) & 0xFF);
                                    }
                                    break;

                                case 32:
                                    {
                                        int bufferPosition = WaveOutAPI.BufferPosition * 4; // 4 bytes per sample
                                        int sample = (int)Math.Round(((levelL + levelR) / 2) * 0x4FFFFFFF);
                                        buffer[bufferPosition + 0] = (byte)(sample & 0xFF);
                                        buffer[bufferPosition + 1] = (byte)((sample >> 8) & 0xFF);
                                        buffer[bufferPosition + 2] = (byte)((sample >> 16) & 0xFF);
                                        buffer[bufferPosition + 3] = (byte)((sample >> 24) & 0xFF);
                                    }
                                    break;
                            }
                            break;
                        // STEREO
                        case 2:
                            switch (WaveOutAPI.SampleBit)
                            {
                                case 16:
                                    {
                                        int bufferPosition = WaveOutAPI.BufferPosition * 4; // 2 channels * 2 bytes = 4 bytes per frame
                                        short sampleLeft = (short)Math.Round(levelL * 0x4FFF);
                                        short sampleRight = (short)Math.Round(levelR * 0x4FFF);
                                        buffer[bufferPosition + 0] = (byte)(sampleLeft & 0xFF);
                                        buffer[bufferPosition + 1] = (byte)((sampleLeft >> 8) & 0xFF);
                                        buffer[bufferPosition + 2] = (byte)(sampleRight & 0xFF);
                                        buffer[bufferPosition + 3] = (byte)((sampleRight >> 8) & 0xFF);
                                    }
                                    break;

                                case 24:
                                    {
                                        int bufferPosition = WaveOutAPI.BufferPosition * 6; // 3 bytes per sample * 2 channels
                                        int sampleLeft = (int)Math.Round(levelL * 0x4FFFFF);
                                        int sampleRight = (int)Math.Round(levelR * 0x4FFFFF);
                                        buffer[bufferPosition + 0] = (byte)(sampleLeft & 0xFF);
                                        buffer[bufferPosition + 1] = (byte)((sampleLeft >> 8) & 0xFF);
                                        buffer[bufferPosition + 2] = (byte)((sampleLeft >> 16) & 0xFF);
                                        buffer[bufferPosition + 3] = (byte)(sampleRight & 0xFF);
                                        buffer[bufferPosition + 4] = (byte)((sampleRight >> 8) & 0xFF);
                                        buffer[bufferPosition + 5] = (byte)((sampleRight >> 16) & 0xFF);
                                    }
                                    break;

                                case 32:
                                    {
                                        int bufferPosition = WaveOutAPI.BufferPosition * 8; // 4 bytes per sample * 2 channels
                                        int sampleLeft = (int)Math.Round(levelL * 0x4FFFFFFF);
                                        int sampleRight = (int)Math.Round(levelR * 0x4FFFFFFF);
                                        buffer[bufferPosition + 0] = (byte)(sampleLeft & 0xFF);
                                        buffer[bufferPosition + 1] = (byte)((sampleLeft >> 8) & 0xFF);
                                        buffer[bufferPosition + 2] = (byte)((sampleLeft >> 16) & 0xFF);
                                        buffer[bufferPosition + 3] = (byte)((sampleLeft >> 24) & 0xFF);
                                        buffer[bufferPosition + 4] = (byte)(sampleRight & 0xFF);
                                        buffer[bufferPosition + 5] = (byte)((sampleRight >> 8) & 0xFF);
                                        buffer[bufferPosition + 6] = (byte)((sampleRight >> 16) & 0xFF);
                                        buffer[bufferPosition + 7] = (byte)((sampleRight >> 24) & 0xFF);
                                    }
                                    break;
                            }
                            break;
                    }

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void Synthesizer_Mono16(byte[] buffer)
        {
            do
            {
                int level = 0;

                for (int i = 0; i < ChipCount; i++)
                {
                    SoundChip[i].Synthesizer_Logic_Q();
                    SoundChip[i].Synthesizer_Mixer_Q_Mono(ref level);
                }

                if (FilterEnabled)
                    level = ApplyFilter(level, DelayLineL);

                ChannelL += level;

                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    int bufferPosition = WaveOutAPI.BufferPosition * 2;
                    short sample = (short)Math.Clamp(ChannelL / TickCounter, -32768, 32767);
                    buffer[bufferPosition + 0] = (byte)(sample & 0xFF);
                    buffer[bufferPosition + 1] = (byte)((sample >> 8) & 0xFF);

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    ChannelL = 0;
                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void Synthesizer_Mono8(byte[] buffer)
        {
            do
            {
                int level = 0;

                for (int i = 0; i < ChipCount; i++)
                {
                    SoundChip[i].Synthesizer_Logic_Q();
                    SoundChip[i].Synthesizer_Mixer_Q_Mono(ref level);
                }

                if (FilterEnabled)
                    level = ApplyFilter(level, DelayLineL);

                ChannelL += level;

                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    int bufferPosition = WaveOutAPI.BufferPosition;
                    buffer[bufferPosition] = (byte)(128 + Math.Clamp(ChannelL / TickCounter, -128, 127));

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    ChannelL = 0;
                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void Synthesizer_Stereo16_P(byte[] buffer)
        {
            do
            {
                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    for (int i = 0; i < ChipCount; i++)
                        SoundChip[i].Synthesizer_Logic_P();

                    int levelL = 0;
                    int levelR = 0;

                    for (int i = 0; i < ChipCount; i++)
                    {
                        int channelGateA = 1;

                        if (SoundChip[i].ToneEnvA)
                            channelGateA = SoundChip[i].ToneA;

                        if (SoundChip[i].NoiseEnvA)
                            channelGateA = (int)(channelGateA & SoundChip[i].Noise.Value);

                        if (channelGateA != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvA)
                            {
                                levelL += LevelA_L[SoundChip[i].AYRegisters.AmplitudeA * 2 + 1];
                                levelR += LevelA_R[SoundChip[i].AYRegisters.AmplitudeA * 2 + 1];
                            }
                            else
                            {
                                levelL += LevelA_L[SoundChip[i].Amplitude];
                                levelR += LevelA_R[SoundChip[i].Amplitude];
                            }
                        }

                        int channelGateB = 1;

                        if (SoundChip[i].ToneEnvB)
                            channelGateB = SoundChip[i].ToneB;

                        if (SoundChip[i].NoiseEnvB)
                            channelGateB = (int)(channelGateB & SoundChip[i].Noise.Value);

                        if (channelGateB != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvB)
                            {
                                levelL += LevelB_L[SoundChip[i].AYRegisters.AmplitudeB * 2 + 1];
                                levelR += LevelB_R[SoundChip[i].AYRegisters.AmplitudeB * 2 + 1];
                            }
                            else
                            {
                                levelL += LevelB_L[SoundChip[i].Amplitude];
                                levelR += LevelB_R[SoundChip[i].Amplitude];
                            }
                        }

                        int channelGateC = 1;

                        if (SoundChip[i].ToneEnvC)
                            channelGateC = SoundChip[i].ToneC;

                        if (SoundChip[i].NoiseEnvC)
                            channelGateC = (int)(channelGateC & SoundChip[i].Noise.Value);

                        if (channelGateC != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvC)
                            {
                                levelL += LevelC_L[SoundChip[i].AYRegisters.AmplitudeC * 2 + 1];
                                levelR += LevelC_R[SoundChip[i].AYRegisters.AmplitudeC * 2 + 1];
                            }
                            else
                            {
                                levelL += LevelC_L[SoundChip[i].Amplitude];
                                levelR += LevelC_R[SoundChip[i].Amplitude];
                            }
                        }
                    }

                    if (levelL > 32767)
                        levelL = 32767;

                    if (levelR > 32767)
                        levelR = 32767;

                    int bufferPosition = WaveOutAPI.BufferPosition * 4;
                    short left = (short)levelL;
                    short right = (short)levelR;
                    buffer[bufferPosition + 0] = (byte)(left & 0xFF);
                    buffer[bufferPosition + 1] = (byte)((left >> 8) & 0xFF);
                    buffer[bufferPosition + 2] = (byte)(right & 0xFF);
                    buffer[bufferPosition + 3] = (byte)((right >> 8) & 0xFF);

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void Synthesizer_Stereo8_P(byte[] buffer)
        {
            do
            {
                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    for (int i = 0; i < ChipCount; i++)
                        SoundChip[i].Synthesizer_Logic_P();

                    int levelL = 128;
                    int levelR = 128;

                    for (int i = 0; i < ChipCount; i++)
                    {
                        int channelGateA = 1;

                        if (SoundChip[i].ToneEnvA)
                            channelGateA = SoundChip[i].ToneA;

                        if (SoundChip[i].NoiseEnvA)
                            channelGateA = (int)(channelGateA & SoundChip[i].Noise.Value);

                        if (channelGateA != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvA)
                            {
                                levelL += LevelA_L[SoundChip[i].AYRegisters.AmplitudeA * 2 + 1];
                                levelR += LevelA_R[SoundChip[i].AYRegisters.AmplitudeA * 2 + 1];
                            }
                            else
                            {
                                levelL += LevelA_L[SoundChip[i].Amplitude];
                                levelR += LevelA_R[SoundChip[i].Amplitude];
                            }
                        }

                        int channelGateB = 1;

                        if (SoundChip[i].ToneEnvB)
                            channelGateB = SoundChip[i].ToneB;

                        if (SoundChip[i].NoiseEnvB)
                            channelGateB = (int)(channelGateB & SoundChip[i].Noise.Value);

                        if (channelGateB != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvB)
                            {
                                levelL += LevelB_L[SoundChip[i].AYRegisters.AmplitudeB * 2 + 1];
                                levelR += LevelB_R[SoundChip[i].AYRegisters.AmplitudeB * 2 + 1];
                            }
                            else
                            {
                                levelL += LevelB_L[SoundChip[i].Amplitude];
                                levelR += LevelB_R[SoundChip[i].Amplitude];
                            }
                        }

                        int channelGateC = 1;

                        if (SoundChip[i].ToneEnvC)
                            channelGateC = SoundChip[i].ToneC;

                        if (SoundChip[i].NoiseEnvC)
                            channelGateC = (int)(channelGateC & SoundChip[i].Noise.Value);

                        if (channelGateC != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvC)
                            {
                                levelL += LevelC_L[SoundChip[i].AYRegisters.AmplitudeC * 2 + 1];
                                levelR += LevelC_R[SoundChip[i].AYRegisters.AmplitudeC * 2 + 1];
                            }
                            else
                            {
                                levelL += LevelC_L[SoundChip[i].Amplitude];
                                levelR += LevelC_R[SoundChip[i].Amplitude];
                            }
                        }
                    }

                    if (levelL > 255)
                        levelL = 255;

                    if (levelR > 255)
                        levelR = 255;

                    int bufferPosition = WaveOutAPI.BufferPosition * 2;
                    buffer[bufferPosition + 0] = (byte)levelL;
                    buffer[bufferPosition + 1] = (byte)levelR;

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void Synthesizer_Mono16_P(byte[] buffer)
        {
            do
            {
                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    for (int i = 0; i < ChipCount; i++)
                        SoundChip[i].Synthesizer_Logic_P();

                    int level = 0;

                    for (int i = 0; i < ChipCount; i++)
                    {
                        int channelGateA = 1;

                        if (SoundChip[i].ToneEnvA)
                            channelGateA = SoundChip[i].ToneA;

                        if (SoundChip[i].NoiseEnvA)
                            channelGateA = (int)(channelGateA & SoundChip[i].Noise.Value);

                        if (channelGateA != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvA)
                                level += LevelA_L[SoundChip[i].AYRegisters.AmplitudeA * 2 + 1];
                            else
                                level += LevelA_L[SoundChip[i].Amplitude];
                        }

                        int channelGateB = 1;

                        if (SoundChip[i].ToneEnvB)
                            channelGateB = SoundChip[i].ToneB;

                        if (SoundChip[i].NoiseEnvB)
                            channelGateB = (int)(channelGateB & SoundChip[i].Noise.Value);

                        if (channelGateB != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvB)
                                level += LevelB_L[SoundChip[i].AYRegisters.AmplitudeB * 2 + 1];
                            else
                                level += LevelB_L[SoundChip[i].Amplitude];
                        }

                        int channelGateC = 1;

                        if (SoundChip[i].ToneEnvC)
                            channelGateC = SoundChip[i].ToneC;

                        if (SoundChip[i].NoiseEnvC)
                            channelGateC = (int)(channelGateC & SoundChip[i].Noise.Value);

                        if (channelGateC != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvC)
                                level += LevelC_L[SoundChip[i].AYRegisters.AmplitudeC * 2 + 1];
                            else
                                level += LevelC_L[SoundChip[i].Amplitude];
                        }
                    }

                    if (level > 32767)
                        level = 32767;

                    int bufferPosition = WaveOutAPI.BufferPosition * 2;
                    short sample = (short)level;
                    buffer[bufferPosition + 0] = (byte)(sample & 0xFF);
                    buffer[bufferPosition + 1] = (byte)((sample >> 8) & 0xFF);

                    FillPlayGrid();
                    
                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;
                    
                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void Synthesizer_Mono8_P(byte[] buffer)
        {
            do
            {
                CurrentTik++;
                TickCounter++;

                if (TickCounter >= Tik.Hi)
                {
                    Tik.Re += (uint)DelayInTiks;
                    Tik.Hi -= TickCounter;

                    for (int i = 0; i < ChipCount; i++)
                        SoundChip[i].Synthesizer_Logic_P();

                    int level = 128;

                    for (int i = 0; i < ChipCount; i++)
                    {
                        int channelGateA = 1;

                        if (SoundChip[i].ToneEnvA)
                            channelGateA = SoundChip[i].ToneA;

                        if (SoundChip[i].NoiseEnvA)
                            channelGateA = (int)(channelGateA & SoundChip[i].Noise.Value);

                        if (channelGateA != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvA)
                                level += LevelA_L[SoundChip[i].AYRegisters.AmplitudeA * 2 + 1];
                            else
                                level += LevelA_L[SoundChip[i].Amplitude];
                        }

                        int channelGateB = 1;
                        
                        if (SoundChip[i].ToneEnvB)
                            channelGateB = SoundChip[i].ToneB;

                        if (SoundChip[i].NoiseEnvB)
                            channelGateB = (int)(channelGateB & SoundChip[i].Noise.Value);

                        if (channelGateB != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvB)
                                level += LevelB_L[SoundChip[i].AYRegisters.AmplitudeB * 2 + 1];
                            else
                                level += LevelB_L[SoundChip[i].Amplitude];
                        }

                        int channelGateC = 1;

                        if (SoundChip[i].ToneEnvC)
                            channelGateC = SoundChip[i].ToneC;

                        if (SoundChip[i].NoiseEnvC)
                            channelGateC = (int)(channelGateC & SoundChip[i].Noise.Value);

                        if (channelGateC != 0)
                        {
                            if (SoundChip[i].EnvelopeEnvC)
                                level += LevelC_L[SoundChip[i].AYRegisters.AmplitudeC * 2 + 1];
                            else
                                level += LevelC_L[SoundChip[i].Amplitude];
                        }
                    }
                    if (level > 255)
                        level = 255;

                    int bufferPosition = WaveOutAPI.BufferPosition;
                    buffer[bufferPosition] = (byte)level;

                    FillPlayGrid();

                    WaveOutAPI.BufferPosition++;
                    WaveOutAPI.TickCount++;

                    TickCounter = 0;

                    if (WaveOutAPI.BufferPosition == WaveOutAPI.BufferLength)
                    {
                        if (CurrentTik < TikCount.Hi)
                            IntFlag = true;

                        return;
                    }
                }
            }
            while (CurrentTik < TikCount.Hi);

            TikCount.Hi = 0;
            CurrentTik = 0;
        }

        public static void UpdatePanoram()
        {
            CalculateLevelTables();

            // Ayumi render
            if (RenderEngine == 2)
            {
                for (int i = 0; i < ChipCount; i++)
                {
                    if (AyumiChip[i] == null)
                        continue;

                    AyumiChip[i].SetPan(0, Main.Panoram[0] / 255.0, false);
                    AyumiChip[i].SetPan(1, Main.Panoram[1] / 255.0, false);
                    AyumiChip[i].SetPan(2, Main.Panoram[2] / 255.0, false);
                }

                return;
            }
            
            //TChildWin.PlayingWindow[0].StopAndRestart();
            AppEvents.SendEvent(EventType.StopAndRestart, 0);
        }

        public static void FrameSynthesizer(byte[] buf)
        {
            if (IntFlag)
                IntFlag = false;
            else
                TikCount.Hi = AYTiksInInterrupt;

            Synthesizer(buf);
        }

        public static void ClearRegisters()
        {
            for (int i = 0; i < ChipCount; i++)
                SoundChip[i].ClearRegisters();
        }

        public static void ClearSpec()
        {
            for (int i = 0; i < ChipCount; i++)
                SoundChip[i].ClearSpec();
        }

        public static void UpdateSpec()
        {
            UpdateSpec(VTModule.ChipIndex);
        }

        public static void UpdateSpec(int chip)
        {
            SoundChip[chip].SpecAddAY(AYFreq);
            SoundChip[chip].UpdateSpec();
        }

        public static void Get_Registers()
        {
            if (RealEnd[VTModule.ChipIndex])
            {
                SoundChip[VTModule.ChipIndex].SetAmplitudeA(0);
                SoundChip[VTModule.ChipIndex].SetAmplitudeB(0);
                SoundChip[VTModule.ChipIndex].SetAmplitudeC(0);

                return;
            }

            switch (PlayMode)
            {
                case PlayModes.PlayModule:
                    if (VTModule.Module_PlayCurrentLine() == PlayLineResult.AllPatternsEnded)
                    {
                        if (WaveOutAPI.ExportStarted)
                        {
                            // Export loops checker
                            AY.LoopAllowed = WaveOutAPI.ExportLoops > 0;

                            if (AY.LoopAllowed)
                                WaveOutAPI.ExportLoops--;
                            else
                                IsFinished = true;
                        }
                        else
                        {
                            IsFinished = !AY.LoopAllowed && (!Main.LoopAllAllowed || PlayingModuleCount != 1);
                        }

                        // STOP playing!
                        if (IsFinished)
                        {
                            WaveOutAPI.ExportFinished = true;
                            RealEnd[VTModule.ChipIndex] = true;
                            SoundChip[VTModule.ChipIndex].SetAmplitudeA(0);
                            SoundChip[VTModule.ChipIndex].SetAmplitudeB(0);
                            SoundChip[VTModule.ChipIndex].SetAmplitudeC(0);
                        }
                    }
                    break;
                case PlayModes.PlayPattern:
                    if (VTModule.Pattern_PlayCurrentLine() == PlayLineResult.PatternEnded)
                    {
                        if (Main.MoveBetweenPatterns) // && ((Win32.GetKeyState(Keys.Return) & 0x8000) == 0x8000))
                            VTModule.Module_GoNextPosition();
                        else if (!LoopAllowed && !Main.LoopAllAllowed)
                        {
                            RealEnd[VTModule.ChipIndex] = true;
                            SoundChip[VTModule.ChipIndex].SetAmplitudeA(0);
                            SoundChip[VTModule.ChipIndex].SetAmplitudeB(0);
                            SoundChip[VTModule.ChipIndex].SetAmplitudeC(0);
                        }
                        else
                        {
                            // Play the same pattern from start if Loop active
                            VTModule.Pattern_SetCurrentLine(0);
                            VTModule.Pattern_PlayCurrentLine();
                        }
                    }
                    break;
                case PlayModes.PlayLine:
                    VTModule.Pattern_PlayOnlyCurrentLine();
                    break;
            }
        }

        public static void MakeBuffer(byte[] buf)
        {
            WaveOutAPI.BufferPosition = 0;

            if (IntFlag)
                FrameSynthesizer(buf);

            if (IntFlag)
                return;

            if (WaveOutAPI.LineReady)
            {
                WaveOutAPI.LineReady = false;
                FrameSynthesizer(buf);
            }

            while (!RealEndAll && WaveOutAPI.BufferPosition < WaveOutAPI.BufferLength)
            {
                RealEndAll = true;

                for (int i = 0; i < ChipCount; i++)
                {
                    VTModule.Module_SetPointer(PlayingModule[i], i);

                    Get_Registers();

                    byte[] regs = SoundChip[VTModule.ChipIndex].AYRegisters.Bytes;

                    if (_nextPlayUs == 0)                           // first time
                        _nextPlayUs = SongClock.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
                    long playAtUs = _nextPlayUs;
                    _nextPlayUs += SlotUs;                         // step for **next** frame

                    RegisterEvent?.Invoke(null, new RegisterEventArgs(VTModule.ChipIndex, regs, SlotUs, playAtUs));

                    UpdateSpec();

                    RealEndAll = RealEndAll && RealEnd[i];
                }

                if (!RealEndAll)
                    FrameSynthesizer(buf);
            }
        }

        public static void CalculateLevelTables()
        {
            int level;
            
            int indexA = IndexA_L;
            int indexB = IndexB_L;
            int indexC = IndexC_L;

            int left = indexA + indexB + indexC;
            int right = IndexA_R + IndexB_R + IndexC_R;

            if (WaveOutAPI.NumberOfChannels == 2)
            {
                if (left < right)
                    left = right;
            }
            else
            {
                left = left + right;
                indexA += IndexA_R;
                indexB += IndexB_R;
                indexC += IndexC_R;
            }

            if (left == 0)
                left++;
 
            if (WaveOutAPI.SampleBit == 8)
                right = 127;
            else
                right = 32767;

            left = 255 * right / left;

            switch (EmulatingChip)
            {
                case ChipType.AY:
                    for (int i = 0; i < 16; i++)
                    {
                        // Channel A
                        level = (int)Math.Round(indexA / 255.0 * Amplitudes_AY[i]);
                        level = (int)Math.Round(level / 65535.0 * left);
                        LevelA_L[i * 2] = level;
                        LevelA_L[i * 2 + 1] = level;
                        level = (int)Math.Round(IndexA_R / 255.0 * Amplitudes_AY[i]);
                        level = (int)Math.Round(level / 65535.0 * left);
                        LevelA_R[i * 2] = level;
                        LevelA_R[i * 2 + 1] = level;
                        
                        // Channel B
                        level = (int)Math.Round(indexB / 255.0 * Amplitudes_AY[i]);
                        level = (int)Math.Round(level / 65535.0 * left);
                        LevelB_L[i * 2] = level;
                        LevelB_L[i * 2 + 1] = level;
                        level = (int)Math.Round(IndexB_R / 255.0 * Amplitudes_AY[i]);
                        level = (int)Math.Round(level / 65535.0 * left);
                        LevelB_R[i * 2] = level;
                        LevelB_R[i * 2 + 1] = level;
                        
                        // Channel C
                        level = (int)Math.Round(indexC / 255.0 * Amplitudes_AY[i]);
                        level = (int)Math.Round(level / 65535.0 * left);
                        LevelC_L[i * 2] = level;
                        LevelC_L[i * 2 + 1] = level;
                        level = (int)Math.Round(IndexC_R / 255.0 * Amplitudes_AY[i]);
                        level = (int)Math.Round(level / 65535.0 * left);
                        LevelC_R[i * 2] = level;
                        LevelC_R[i * 2 + 1] = level;
                    }
                    break;
                case ChipType.YM:
                    for (int i = 0; i < 32; i++)
                    {
                        // Channel A
                        level = (int)Math.Round(indexA / 255.0 * Amplitudes_YM[i]);
                        LevelA_L[i] = (int)Math.Round(level / 65535.0 * left);
                        level = (int)Math.Round(IndexA_R / 255.0 * Amplitudes_YM[i]);
                        LevelA_R[i] = (int)Math.Round(level / 65535.0 * left);
                        
                        // Channel B
                        level = (int)Math.Round(indexB / 255.0 * Amplitudes_YM[i]);
                        LevelB_L[i] = (int)Math.Round(level / 65535.0 * left);
                        level = (int)Math.Round(IndexB_R / 255.0 * Amplitudes_YM[i]);
                        LevelB_R[i] = (int)Math.Round(level / 65535.0 * left);
                        
                        // Channel C
                        level = (int)Math.Round(indexC / 255.0 * Amplitudes_YM[i]);
                        LevelC_L[i] = (int)Math.Round(level / 65535.0 * left);
                        level = (int)Math.Round(IndexC_R / 255.0 * Amplitudes_YM[i]);
                        LevelC_R[i] = (int)Math.Round(level / 65535.0 * left);
                    }
                    break;
            }

            double globalVolume = Math.Exp(Main.GlobalVolume * Math.Log(2.0) / Main.GlobalVolumeMax) - 1;

            for (int i = 0; i < 32; i++)
            {
                LevelA_L[i] = (int)Math.Round(LevelA_L[i] * globalVolume);
                LevelA_R[i] = (int)Math.Round(LevelA_R[i] * globalVolume);
                LevelB_L[i] = (int)Math.Round(LevelB_L[i] * globalVolume);
                LevelB_R[i] = (int)Math.Round(LevelB_R[i] * globalVolume);
                LevelC_L[i] = (int)Math.Round(LevelC_L[i] * globalVolume);
                LevelC_R[i] = (int)Math.Round(LevelC_R[i] * globalVolume);
            }
        }

        public static void SetDefault(int samRate, int nChan, int samBit)
        {
            WaveOutAPI.SampleRate = samRate;

            AYFreq = AYFreqDefault;
            
            WaveOutAPI.InterruptFreq = InterruptFreqDefault;

            double tickHz = WaveOutAPI.InterruptFreq / 1000.0;
            SlotUs = (int)(1000000.0 / tickHz);

            DelayInTiks = (int)Math.Round(8192.0 / WaveOutAPI.SampleRate * AYFreq);
            AYTiksInInterrupt = (uint)Math.Round(AYFreq / (tickHz * 8.0));
            
            WaveOutAPI.SampleBit = samBit;
            WaveOutAPI.BufferCount = WaveOutAPI.BufferCountDefault;
            WaveOutAPI.BufferLengthMs = WaveOutAPI.BufferLengthMsDefault;
            WaveOutAPI.BufferLength = (int)Math.Round(WaveOutAPI.BufferLengthMs * WaveOutAPI.SampleRate / 1000.0);
            WaveOutAPI.NumberOfChannels = nChan;
            
            StdChannelsAllocation = StdChannelsAllocationDefault;

            IndexA_L = IndexA_L_Default;
            IndexA_R = IndexA_R_Default;
            IndexB_L = IndexB_L_Default;
            IndexB_R = IndexB_R_Default;
            IndexC_L = IndexC_L_Default;
            IndexC_R = IndexC_R_Default;

            EmulatingChip = ChipType.YM;
            
            CalculateLevelTables();
            InitPlayGrid();

            FilterEnabled = true;
            FilterLength = FilterLengthDefault;
            Array.Resize(ref FilterKernel, FilterLength + 1);
            
            CalcFiltCoefs();

            Array.Resize(ref DelayLineL, FilterLength + 1);
            Array.Resize(ref DelayLineR, FilterLength + 1);
            FilterIndex = 0;

            SoundChip[0] = null;

            for (int i = 0; i < ChipCount; i++)
                SoundChip[i] = new SoundChip();
        }

        public static string ToggleChanMode()
        {
            StdChannelsAllocation++;
            
            if (StdChannelsAllocation > 3)
                StdChannelsAllocation = 0;

            return SetStdChannelsAllocation(StdChannelsAllocation);
        }

        public static string SetStdChannelsAllocation(int ca)
        {
            string result = "";

            StdChannelsAllocation = ca;

            switch (StdChannelsAllocation)
            {
                case 0:
                    VTModule.CenterChannel = 0;
                    result = "Mono";
                    Main.Panoram[0] = 128;
                    Main.Panoram[1] = 128;
                    Main.Panoram[2] = 128;
                    break;
                case 1:
                    VTModule.CenterChannel = 1;
                    result = "ABC";
                    Main.Panoram[0] = 64;
                    Main.Panoram[1] = 128;
                    Main.Panoram[2] = 192;
                    break;
                case 2:
                    VTModule.CenterChannel = 2;
                    result = "ACB";
                    Main.Panoram[0] = 64;
                    Main.Panoram[1] = 192;
                    Main.Panoram[2] = 128;
                    break;
                case 3:
                    VTModule.CenterChannel = 0;
                    result = "BAC";
                    Main.Panoram[0] = 128;
                    Main.Panoram[1] = 64;
                    Main.Panoram[2] = 192;
                    break;
                case 4:
                    VTModule.CenterChannel = 2;
                    result = "BCA";
                    Main.Panoram[0] = 192;
                    Main.Panoram[1] = 64;
                    Main.Panoram[2] = 128;
                    break;
                case 5:
                    VTModule.CenterChannel = 0;
                    result = "CAB";
                    Main.Panoram[0] = 128;
                    Main.Panoram[1] = 192;
                    Main.Panoram[2] = 64;
                    break;
                case 6:
                    VTModule.CenterChannel = 1;
                    result = "CBA";
                    Main.Panoram[0] = 192;
                    Main.Panoram[1] = 128;
                    Main.Panoram[2] = 64;
                    break;
            }

            IndexA_L = (byte)(0xFF - Main.Panoram[0]);
            IndexA_R = Main.Panoram[0];
            IndexB_L = (byte)(0xFF - Main.Panoram[1]);
            IndexB_R = Main.Panoram[1];
            IndexC_L = (byte)(0xFF - Main.Panoram[2]);
            IndexC_R = Main.Panoram[2];

            CalculateLevelTables();

            return result;
        }

        public static void SetIntFreq(int frequency)
        {
            bool r;

            if (frequency < 1000 || frequency > 2000000)
                return;

            r = WaveOutAPI.IsPlaying && !WaveOutAPI.HasReset && PlayMode == PlayModes.PlayModule;

            if (!r && WaveOutAPI.IsPlaying && !WaveOutAPI.HasReset)
                WaveOutAPI.StopPlaying();

            if (r)
                WaveOutAPI.ResetPlaying();

            WaveOutAPI.InterruptFreq = frequency;

            AYTiksInInterrupt = (uint)Math.Round(AYFreq / (WaveOutAPI.InterruptFreq / 1000.0 * 8.0));

            InitPlayGrid();

            if (r)
            {
                //TChildWin.PlayingWindow[0].RerollToLine(0);
                AppEvents.SendEvent(EventType.RerollToLine, 0);
                WaveOutAPI.UnResetPlaying();
            }
        }

        public static void SetSampleRate(int sampleRate)
        {
            if (sampleRate == WaveOutAPI.SampleRate)
                return;

            if (RenderEngine < 2 && sampleRate > 96000)
                sampleRate = 96000;

            WaveOutAPI.SampleRate = sampleRate;

            DelayInTiks = (int)Math.Round(8192.0 / WaveOutAPI.SampleRate * AYFreq);

            WaveOutAPI.BufferLength = (int)Math.Round(WaveOutAPI.BufferLengthMs * WaveOutAPI.SampleRate / 1000.0);

            InitPlayGrid();

            CalcFiltCoefs();
        }

        public static void SetBuffers(int length, int numberOfBuffers)
        {
            if (WaveOutAPI.BufferLengthMs == length && WaveOutAPI.BufferCount == numberOfBuffers)
                return;
 
            WaveOutAPI.BufferLengthMs = length;
            WaveOutAPI.BufferCount = numberOfBuffers;
            WaveOutAPI.BufferLength = (int)Math.Round(WaveOutAPI.BufferLengthMs * WaveOutAPI.SampleRate / 1000.0);

            InitPlayGrid();
        }

        public static void SetBitRate(int sampleBit)
        {
            WaveOutAPI.SampleBit = sampleBit;

            if (RenderEngine == 2)
            {
                Synthesizer = Synthesizer_Ayumi;

                if (WaveOutAPI.SampleBit == 8)
                    WaveOutAPI.SampleBit = 16;
            }
            else if (RenderEngine == 0)
            {
                if (WaveOutAPI.SampleBit > 16)
                    WaveOutAPI.SampleBit = 16;

                if (WaveOutAPI.NumberOfChannels == 2)
                    Synthesizer = sampleBit == 8 ? Synthesizer_Stereo8 : Synthesizer_Stereo16;
                else
                    Synthesizer = sampleBit == 8 ? Synthesizer_Mono8 : Synthesizer_Mono16;
            }
            else
            {
                if (WaveOutAPI.SampleBit > 16)
                    WaveOutAPI.SampleBit = 16;

                if (WaveOutAPI.NumberOfChannels == 2)
                    Synthesizer = sampleBit == 8 ? Synthesizer_Stereo8_P : Synthesizer_Stereo16_P;
                else
                    Synthesizer = sampleBit == 8 ? Synthesizer_Mono8_P : Synthesizer_Mono16_P;
            }

            CalculateLevelTables();
        }

        public static void SetNChans(int numberOfChannels)
        {
            WaveOutAPI.NumberOfChannels = numberOfChannels;
            
            if (RenderEngine == 2)
                Synthesizer = Synthesizer_Ayumi;
            else if (RenderEngine == 0)
            {
                if (numberOfChannels == 2)
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Stereo8 : Synthesizer_Stereo16;
                else
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Mono8 : Synthesizer_Mono16;
            }
            else
            {
                if (numberOfChannels == 2)
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Stereo8_P : Synthesizer_Stereo16_P;
                else
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Mono8_P : Synthesizer_Mono16_P;
            }

            CalculateLevelTables();
        }

        public static void Set_Engine(int engineIndex)
        {
            bool isPlaying = WaveOutAPI.IsPlaying;

            if (isPlaying)
                WaveOutAPI.StopPlaying();

            RenderEngine = engineIndex;
            CurrentTik = (uint)Math.Round(CurrentTik / WaveOutAPI.SampleRate * (AYFreq / 8.0));
            TikCount.Re = (long)Math.Round(TikCount.Re / WaveOutAPI.SampleRate * (AYFreq / 8.0));

            // Ayumi render
            if (engineIndex == 2)
                Synthesizer = Synthesizer_Ayumi;
            else if (engineIndex == 0)
            {
                if (WaveOutAPI.NumberOfChannels == 2)
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Stereo8 : Synthesizer_Stereo16;
                else
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Mono8 : Synthesizer_Mono16;
            }
            else
            {
                if (WaveOutAPI.NumberOfChannels == 2)
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Stereo8_P : Synthesizer_Stereo16_P;
                else
                    Synthesizer = WaveOutAPI.SampleBit == 8 ? Synthesizer_Mono8_P : Synthesizer_Mono16_P;
            }

            if (isPlaying)
            {
                //TChildWin.PlayingWindow[0].RerollToLine(0);
                AppEvents.SendEvent(EventType.RerollToLine, 0);
                WaveOutAPI.StartWOThread();
            }
        }

        public static void SetAYFreq(int frequency)
        {
            bool r;

            if (frequency < 700000 || frequency > 3546800)
                return;

            r = WaveOutAPI.IsPlaying && RenderEngine != 2;

            if (r)
                WaveOutAPI.StopPlaying();

            AYFreq = frequency;
            DelayInTiks = (int)Math.Round(8192.0 / WaveOutAPI.SampleRate * AYFreq);
            AYTiksInInterrupt = (uint)Math.Round(AYFreq / (WaveOutAPI.InterruptFreq / 1000.0 * 8.0));

            CalcFiltCoefs();

            if (RenderEngine == 2)
            {
                for (int i = 0; i < ChipCount; i++)
                {
                    if (AyumiChip[i] == null)
                        continue;

                    AyumiChip[i].SetChipFreq(AYFreq);
                }
            }

            if (r)
            {
                //TChildWin.PlayingWindow[0].RerollToLine(0);
                AppEvents.SendEvent(EventType.RerollToLine, 0);
                WaveOutAPI.StartWOThread();
            }
        }

        public static void CalcFiltCoefs()
        {
            int halfLength = FilterLength / 2;
            double sumCoeffs = 0;
            double omegaC = Math.PI * WaveOutAPI.SampleRate / (AYFreq / 8.0);
            double[] coeffTemp = new double[FilterLength + 1];

            for (int i = 0; i <= FilterLength; i++)
            {
                int relTap = i - halfLength;
                double coeff = relTap == 0 ? omegaC : Math.Sin(omegaC * relTap) / relTap * (0.54 - 0.46 * Math.Cos(2.0 * Math.PI / FilterLength * i));
                coeffTemp[i] = coeff;
                sumCoeffs += coeff;
            }

            for (int i = 0; i <= FilterLength; i++)
                FilterKernel[i] = (int)Math.Round(coeffTemp[i] / sumCoeffs * 0x1000000);
        }

        public static void SetFilter(bool filt, int m)
        {
            if (FilterLength == m && FilterEnabled == filt)
                return;

            bool r = WaveOutAPI.IsPlaying && !WaveOutAPI.HasReset && PlayMode == PlayModes.PlayModule;

            if (!r && WaveOutAPI.IsPlaying && !WaveOutAPI.HasReset)
                WaveOutAPI.StopPlaying();

            if (r)
                WaveOutAPI.ResetPlaying();

            FilterEnabled = filt;

            if (FilterLength != m)
            {
                FilterLength = m;
                Array.Resize(ref FilterKernel, m + 1);

                CalcFiltCoefs();

                Array.Resize(ref DelayLineL, m + 1);
                Array.Resize(ref DelayLineR, m + 1);
                FilterIndex = 0;
            }

            if (r)
            {
                //TChildWin.PlayingWindow[0].RerollToLine(0);
                AppEvents.SendEvent(EventType.RerollToLine, 0);
                WaveOutAPI.UnResetPlaying();
            }
        }
    }
}
