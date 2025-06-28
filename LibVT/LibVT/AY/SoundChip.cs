// 
// This is part of Vortex Tracker II project
// 
// (c)2000-2009 S.V.Bulba
// Author: Sergey Bulba, vorobey@mail.khstu.ru
// Support page: http://bulba.untergrund.net/
// 
// Version 1.5 - 2.6
// (c)2017-2021 Ivan Pirog, ivan.pirog@gmail.com
// 
// Version 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using System;
using System.Runtime.InteropServices;

namespace LibVT
{
    public enum SpecType
    {
        ChannelA = 0,
        ChannelB = 1,
        ChannelC = 2,
        Envelope = 3
    }

    public class AYRegisters
    {
        public ushort ToneA;
        public ushort ToneB;
        public ushort ToneC;
        public byte Noise;
        public byte Mixer;
        public byte AmplitudeA;
        public byte AmplitudeB;
        public byte AmplitudeC;
        public ushort Envelope;

        private byte _envType;
        private bool _envTypeRepeat;

        public void Clear()
        {
            ToneA = 0;
            ToneB = 0;
            ToneC = 0;
            Noise = 0;
            Mixer = 0;
            AmplitudeA = 0;
            AmplitudeB = 0;
            AmplitudeC = 0;
            Envelope = 0;
            EnvType = 0;
            _envTypeRepeat = false;
        }

        public byte[] Bytes
        {
            get
            {
                return new byte[]
                {
                    (byte)(ToneA & 0xFF),
                    (byte)(ToneA >> 8),
                    (byte)(ToneB & 0xFF),
                    (byte)(ToneB >> 8),
                    (byte)(ToneC & 0xFF),
                    (byte)(ToneC >> 8),
                    Noise,
                    Mixer,
                    AmplitudeA,
                    AmplitudeB,
                    AmplitudeC,
                    (byte)(Envelope & 0xFF),
                    (byte)(Envelope >> 8),
                    _envType
                };
            }
        }

        public byte EnvType
        {
            get => (_envTypeRepeat ? (byte)0xFF : _envType);
            set
            {
                if (value == 0xFF)
                    _envTypeRepeat = true;
                else
                {
                    _envType = value;
                    _envTypeRepeat = false;
                }
            }
        }
    }
 
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Counter32
    {
        [FieldOffset(0)]
        public ushort Lo;
        [FieldOffset(2)]
        public ushort Hi;
        [FieldOffset(0)]
        public uint Re;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Counter64
    {
        [FieldOffset(0)]
        public int Lo;
        [FieldOffset(2)]
        public int Hi;
        [FieldOffset(0)]
        public long Re;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Noise
    {
        [FieldOffset(0)]
        public uint Seed;
        [FieldOffset(0)]
        public ushort Lo;
        [FieldOffset(2)]
        public uint Value;
    }

    public class SoundChip
    {
        public AYRegisters AYRegisters = new AYRegisters();
        public bool FirstPeriod = false;
        public int Amplitude = 0;
        public Counter32 ToneCounterA;
        public Counter32 ToneCounterB;
        public Counter32 ToneCounterC;
        public Counter32 NoiseCounter;
        public Counter64 EnvelopeCounter;
        public int ToneA = 0;
        public int ToneB = 0;
        public int ToneC = 0;
        public Noise Noise;
        public EnvTypeDelegate EnvType = null;
        public bool ToneEnvA = false;
        public bool ToneEnvB = false;
        public bool ToneEnvC = false;
        public bool NoiseEnvA = false;
        public bool NoiseEnvB = false;
        public bool NoiseEnvC = false;
        public bool EnvelopeEnvA = false;
        public bool EnvelopeEnvB = false;
        public bool EnvelopeEnvC = false;

        private const int SpecBands = 42;
        private const int SpecRange = 1000;
        private const float SpecDecay = 0.06f;

        public float[] VULevel = new float[3];
        private const float VUDecay = 0.06f;

        public float[] SpecLevels = new float[SpecBands];
        public SpecType[] SpecTypes = new SpecType[SpecBands];

        public delegate void EnvTypeDelegate();

        public SoundChip()
        {
        }

        public void EnvType_0_3__9()
        {
            if (FirstPeriod)
            {
                if (--Amplitude == 0)
                {
                    FirstPeriod = false;
                }
            }
        }

        public void EnvType_4_7__15()
        {
            if (FirstPeriod)
            {
                if (++Amplitude == 32)
                {
                    FirstPeriod = false;
                    Amplitude = 0;
                }
            }
        }

        public void EnvType_8()
        {
            Amplitude = (Amplitude - 1) & 31;
        }

        public void EnvType_10()
        {
            if (FirstPeriod)
            {
                if (--Amplitude < 0)
                {
                    FirstPeriod = false;
                    Amplitude = 0;
                }
            }
            else
            {
                if (++Amplitude == 32)
                {
                    FirstPeriod = true;
                    Amplitude = 31;
                }
            }
        }

        public void EnvType_11()
        {
            if (FirstPeriod)
            {
                if (--Amplitude < 0)
                {
                    FirstPeriod = false;
                    Amplitude = 31;
                }
            }
        }

        public void EnvType_12()
        {
            Amplitude = (Amplitude + 1) & 31;
        }

        public void EnvType_13()
        {
            if (FirstPeriod)
            {
                if (++Amplitude == 32)
                {
                    FirstPeriod = false;
                    Amplitude = 31;
                }
            }
        }

        public void EnvType_14()
        {
            if (!FirstPeriod)
            {
                if (--Amplitude < 0)
                {
                    FirstPeriod = true;
                    Amplitude = 0;
                }
            }
            else
            {
                if (++Amplitude == 32)
                {
                    FirstPeriod = false;
                    Amplitude = 31;
                }
            }
        }

        public void Synthesizer_Logic_Q()
        {
            ToneCounterA.Hi++;

            if (ToneCounterA.Hi >= AYRegisters.ToneA)
            {
                ToneCounterA.Hi = 0;
                ToneA ^= 1;
            }

            ToneCounterB.Hi++;
            
            if (ToneCounterB.Hi >= AYRegisters.ToneB)
            {
                ToneCounterB.Hi = 0;
                ToneB ^= 1;
            }

            ToneCounterC.Hi++;

            if (ToneCounterC.Hi >= AYRegisters.ToneC)
            {
                ToneCounterC.Hi = 0;
                ToneC ^= 1;
            }

            NoiseCounter.Hi++;
            
            if (((NoiseCounter.Hi & 1) == 0) && (NoiseCounter.Hi >= AYRegisters.Noise << 1))
            {
                NoiseCounter.Hi = 0;
                Noise.Seed = (uint)AY.NoiseGenerator((int)Noise.Seed);
            }

            if (EnvelopeCounter.Hi == 0)
                EnvType();

            EnvelopeCounter.Hi++;

            if (EnvelopeCounter.Hi >= AYRegisters.Envelope)
                EnvelopeCounter.Hi = 0;
        }

        public void Synthesizer_Logic_P()
        {
            ToneCounterA.Re += (uint)AY.DelayInTiks;

            ushort tonePeriodA = AYRegisters.ToneA;

            if (tonePeriodA == 0)
                tonePeriodA++;

            if (tonePeriodA <= ToneCounterA.Hi)
            {
                ToneA ^= (ToneCounterA.Hi / tonePeriodA) & 1;
                ToneCounterA.Hi = (ushort)(ToneCounterA.Hi % tonePeriodA);
            }

            ToneCounterB.Re += (uint)AY.DelayInTiks;

            ushort tonePeriodB = AYRegisters.ToneB;

            if (tonePeriodB == 0)
                tonePeriodB++;

            if (tonePeriodB <= ToneCounterB.Hi)
            {
                ToneB ^= (ToneCounterB.Hi / tonePeriodB) & 1;
                ToneCounterB.Hi = (ushort)(ToneCounterB.Hi % tonePeriodB);
            }

            ToneCounterC.Re += (uint)AY.DelayInTiks;

            ushort tonePeriodC = AYRegisters.ToneC;

            if (tonePeriodC == 0)
                tonePeriodC++;

            if (tonePeriodC <= ToneCounterC.Hi)
            {
                ToneC ^= (ToneCounterC.Hi / tonePeriodC) & 1;
                ToneCounterC.Hi = (ushort)(ToneCounterC.Hi % tonePeriodC);
            }
            
            NoiseCounter.Re += (uint)AY.DelayInTiks;

            ushort noisePeriod = AYRegisters.Noise;
            
            if (noisePeriod == 0)
                noisePeriod++;

            noisePeriod <<= 1;

            if (NoiseCounter.Hi >= noisePeriod)
            {
                NoiseCounter.Hi = (ushort)(NoiseCounter.Hi % noisePeriod);
                Noise.Seed = (uint)AY.NoiseGenerator((int)Noise.Seed);
            }

            uint envelopePeriod = AYRegisters.Envelope;

            if (envelopePeriod == 0)
                envelopePeriod++;

            if (EnvelopeCounter.Hi == 0)
                EnvelopeCounter.Hi += (int)envelopePeriod;

            while (EnvelopeCounter.Hi >= envelopePeriod)
            {
                EnvelopeCounter.Hi -= (int)envelopePeriod;
                EnvType();
            }

            EnvelopeCounter.Re += AY.DelayInTiks << 16;
        }

        public void SetMixerRegister(byte value)
        {
            AYRegisters.Mixer = value;
            ToneEnvA = (value & 1) == 0;
            NoiseEnvA = (value & 8) == 0;
            ToneEnvB = (value & 2) == 0;
            NoiseEnvB = (value & 16) == 0;
            ToneEnvC = (value & 4) == 0;
            NoiseEnvC = (value & 32) == 0;
        }

        public void SetEnvelopeRegister(byte value)
        {
            EnvelopeCounter.Hi = 0;
            FirstPeriod = true;
            Amplitude = (value & 4) == 0 ? 32 : -1;
            AYRegisters.EnvType = value;

            if ((value >= 0 && value <= 3) || value == 9)
                EnvType = EnvType_0_3__9;
            else if ((value >= 4 && value <= 7) || value == 15)
                EnvType = EnvType_4_7__15;
            {
                switch(value)
                {
                    case 8:
                        EnvType = EnvType_8;
                        break;
                    case 10:
                        EnvType = EnvType_10;
                        break;
                    case 11:
                        EnvType = EnvType_11;
                        break;
                    case 12:
                        EnvType = EnvType_12;
                        break;
                    case 13:
                        EnvType = EnvType_13;
                        break;
                    case 14:
                        EnvType = EnvType_14;
                        break;
                }
            }
        }

        public void SetAmplitudeA(byte value)
        {
            AYRegisters.AmplitudeA = value;
            EnvelopeEnvA = (value & 16) == 0;
        }

        public void SetAmplitudeB(byte value)
        {
            AYRegisters.AmplitudeB = value;
            EnvelopeEnvB = (value & 16) == 0;
        }

        public void SetAmplitudeC(byte value)
        {
            AYRegisters.AmplitudeC = value;
            EnvelopeEnvC = (value & 16) == 0;
        }

        public void Synthesizer_Mixer_Q(ref int levelL, ref int levelR)
        {
            int channelGateA = 1;

            if (ToneEnvA)
                channelGateA = ToneA;

            if (NoiseEnvA)
                channelGateA = (int)(channelGateA & Noise.Value);

            if (channelGateA != 0)
            {
                if (EnvelopeEnvA)
                {
                    levelL += AY.LevelA_L[AYRegisters.AmplitudeA * 2 + 1];
                    levelR += AY.LevelA_R[AYRegisters.AmplitudeA * 2 + 1];
                }
                else
                {
                    levelL += AY.LevelA_L[Amplitude];
                    levelR += AY.LevelA_R[Amplitude];
                }
            }

            int channelGateB = 1;

            if (ToneEnvB)
                channelGateB = ToneB;

            if (NoiseEnvB)
                channelGateB = (int)(channelGateB & Noise.Value);

            if (channelGateB != 0)
            {
                if (EnvelopeEnvB)
                {
                    levelL += AY.LevelB_L[AYRegisters.AmplitudeB * 2 + 1];
                    levelR += AY.LevelB_R[AYRegisters.AmplitudeB * 2 + 1];
                }
                else
                {
                    levelL += AY.LevelB_L[Amplitude];
                    levelR += AY.LevelB_R[Amplitude];
                }
            }

            int channelGateC = 1;

            if (ToneEnvC)
                channelGateC = ToneC;

            if (NoiseEnvC)
                channelGateC = (int)(channelGateC & Noise.Value);

            if (channelGateC != 0)
            {
                if (EnvelopeEnvC)
                {
                    levelL += AY.LevelC_L[AYRegisters.AmplitudeC * 2 + 1];
                    levelR += AY.LevelC_R[AYRegisters.AmplitudeC * 2 + 1];
                }
                else
                {
                    levelL += AY.LevelC_L[Amplitude];
                    levelR += AY.LevelC_R[Amplitude];
                }
            }
        }

        public void Synthesizer_Mixer_Q_Mono(ref int levelL)
        {
            int channelGateA = 1;

            if (ToneEnvA)
                channelGateA = ToneA;

            if (NoiseEnvA)
                channelGateA = (int)(channelGateA & Noise.Value);

            if (channelGateA != 0)
                levelL += EnvelopeEnvA ? AY.LevelA_L[AYRegisters.AmplitudeA * 2 + 1] : AY.LevelA_L[Amplitude];

            int channelGateB = 1;

            if (ToneEnvB)
                channelGateB = ToneB;

            if (NoiseEnvB)
                channelGateB = (int)(channelGateB & Noise.Value);

            if (channelGateB != 0)
                levelL += EnvelopeEnvB ? AY.LevelB_L[AYRegisters.AmplitudeB * 2 + 1] : AY.LevelB_L[Amplitude];

            int channelGateC = 1;

            if (ToneEnvC)
                channelGateC = ToneC;

            if (NoiseEnvC)
                channelGateC = (int)(channelGateC & Noise.Value);

            if (channelGateC != 0)
                levelL += EnvelopeEnvC ? AY.LevelC_L[AYRegisters.AmplitudeC * 2 + 1] : AY.LevelC_L[Amplitude];
        }

        public void ClearSpec()
        {
            for (int i = 0; i < SpecBands; ++i)
            {
                SpecLevels[i] = 0;
                SpecTypes[i] = SpecType.ChannelA;
            }

            for (int i = 0; i < 3; i++)
                VULevel[i] = 0f;
        }

        public void UpdateSpec()
        {
            for (int i = 0; i < SpecBands; ++i)
            {
                SpecLevels[i] -= SpecDecay;
                if (SpecLevels[i] < 0)
                    SpecLevels[i] = 0;
            }

            for (int i = 0; i < 3; i++)
            {
                VULevel[i] -= VUDecay;
                VULevel[i] = MathF.Max(VULevel[i], 0f);
            }
        }

        public void SpecAdd(int hz, int level, SpecType type)
        {
            float[] curve = { 0.1f, 0.2f, 0.5f, 0.2f, 0.1f };

            if (hz == 0)
                return;

            int off = hz / (SpecRange / SpecBands) - 2;

            if (off > SpecBands - 1)
                off = SpecBands - 1;

            float normLevel = level / 15f; // Normalize input level (0..15) to 0..1

            for (int i = 0; i < 5; ++i)
            {
                if (off >= 0 && off < SpecBands)
                {
                    SpecLevels[off] += curve[i] * normLevel;

                    if (SpecLevels[off] > 1.0f)
                        SpecLevels[off] = 1.0f;

                    SpecTypes[off] = type;
                }

                ++off;
            }
        }

        public void SpecAddAY(int ayClock)
        {
            // Channel A
            if ((AYRegisters.Mixer & 0x01) == 0 && (AYRegisters.AmplitudeA & 0x10) == 0)
            {
                int toneA = AYRegisters.ToneA;

                if (toneA != 0)
                    SpecAdd(ayClock / 16 / toneA, AYRegisters.AmplitudeA, SpecType.ChannelA);

                VULevel[0] = MathF.Max(VULevel[0], AYRegisters.AmplitudeA / 15f); // Normalize to 0.0f–1.0f
            }

            // Channel B
            if ((AYRegisters.Mixer & 0x02) == 0 && (AYRegisters.AmplitudeB & 0x10) == 0)
            {
                int toneB = AYRegisters.ToneB;

                if (toneB != 0)
                    SpecAdd(ayClock / 16 / toneB, AYRegisters.AmplitudeB, SpecType.ChannelB);

                VULevel[1] = MathF.Max(VULevel[1], AYRegisters.AmplitudeB / 15f);
            }

            // Channel C
            if ((AYRegisters.Mixer & 0x04) == 0 && (AYRegisters.AmplitudeC & 0x10) == 0)
            {
                int toneC = AYRegisters.ToneC;

                if (toneC != 0)
                    SpecAdd(ayClock / 16 / toneC, AYRegisters.AmplitudeC, SpecType.ChannelC);

                VULevel[2] = MathF.Max(VULevel[2], AYRegisters.AmplitudeC / 15f);
            }

            // Envelope
            if ((AYRegisters.AmplitudeA & 0x10) != 0 || (AYRegisters.AmplitudeB & 0x10) != 0 || (AYRegisters.AmplitudeC & 0x10) != 0)
            {
                int envelope = AYRegisters.Envelope;

                if (envelope != 0)
                    SpecAdd(ayClock / 16 / 16 / envelope, 12, SpecType.Envelope);
            }
        }

        public void ClearRegisters()
        {
            AYRegisters.ToneA = 0;
            AYRegisters.ToneB = 0;
            AYRegisters.ToneC = 0;
            AYRegisters.Noise = 0;
            AYRegisters.Mixer = 0;
            AYRegisters.AmplitudeA = 0;
            AYRegisters.AmplitudeB = 0;
            AYRegisters.AmplitudeC = 0;
            AYRegisters.Envelope = 0;
            AYRegisters.EnvType = 0;
        }

        public override string ToString()
        {
            string result = "";
            result += $"AYRegisters.ToneA: {AYRegisters.ToneA}\n";
            result += $"AYRegisters.ToneB: {AYRegisters.ToneB}\n";
            result += $"AYRegisters.ToneC: {AYRegisters.ToneC}\n";
            result += $"AYRegisters.Noise: {AYRegisters.Noise}\n";
            result += $"AYRegisters.Mixer: {AYRegisters.Mixer}\n";
            result += $"AYRegisters.AmplitudeA: {AYRegisters.AmplitudeA}\n";
            result += $"AYRegisters.AmplitudeB: {AYRegisters.AmplitudeB}\n";
            result += $"AYRegisters.AmplitudeC: {AYRegisters.AmplitudeC}\n";
            result += $"AYRegisters.Envelope: {AYRegisters.Envelope}\n";
            result += $"AYRegisters.EnvType: {AYRegisters.EnvType}\n";
            result += $"FirstPeriod: {FirstPeriod}\n";
            result += $"Amplitude: {Amplitude}\n";
            result += $"ToneCounterA.Lo: {ToneCounterA.Lo}\n";
            result += $"ToneCounterA.Hi: {ToneCounterA.Hi}\n";
            result += $"ToneCounterB.Lo: {ToneCounterB.Lo}\n";
            result += $"ToneCounterB.Hi: {ToneCounterB.Hi}\n";
            result += $"ToneCounterC.Lo: {ToneCounterC.Lo}\n";
            result += $"ToneCounterC.Hi: {ToneCounterC.Hi}\n";
            result += $"NoiseCounter.Lo: {NoiseCounter.Lo}\n";
            result += $"NoiseCounter.Hi: {NoiseCounter.Hi}\n";
            result += $"EnvelopeCounter.Lo: {EnvelopeCounter.Lo}\n";
            result += $"EnvelopeCounter.Hi: {EnvelopeCounter.Hi}\n";
            result += $"ToneA: {ToneA}\n";
            result += $"ToneB: {ToneB}\n";
            result += $"ToneC: {ToneC}\n";
            result += $"Noise.Seed: {Noise.Seed}\n";
            result += $"Noise.Lo: {Noise.Lo}\n";
            result += $"Noise.Val: {Noise.Value}\n";
            result += $"EnvType: {EnvType}\n";
            result += $"ToneEnvA: {ToneEnvA}\n";
            result += $"ToneEnvB: {ToneEnvB}\n";
            result += $"ToneEnvC: {ToneEnvC}\n";
            result += $"NoiseEnvA: {NoiseEnvA}\n";
            result += $"NoiseEnvB: {NoiseEnvB}\n";
            result += $"NoiseEnvC: {NoiseEnvC}\n";
            result += $"EnvelopeEnvA: {EnvelopeEnvA}\n";
            result += $"EnvelopeEnvB: {EnvelopeEnvB}\n";
            result += $"EnvelopeEnvC: {EnvelopeEnvC}\n";
            return result;
        }
    }
}
