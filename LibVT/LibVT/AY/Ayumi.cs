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
// Version 2.6 - 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LibVT
{
    public enum EnvelopeShape
    {
        SlideUp,
        SlideDown,
        HoldBottom,
        HoldTop
    }

    public class ToneChannel
    {
        public int TonePeriod;
        public int ToneCounter;
        public int Tone;
        public int ToneOff;
        public int NoiseOff;
        public int EnvelopeOn;
        public int Volume;
        public double PanLeft;
        public double PanRight;

        public ToneChannel()
        {
            TonePeriod = 1;
            ToneCounter = 0;
            Tone = 0;
            ToneOff = 0;
            NoiseOff = 0;
            EnvelopeOn = 0;
            Volume = 0;
            PanLeft = 0.0;
            PanRight = 0.0;
        }
    }

    public class Interpolator
    {
        public double[] Coeffs;
        public double[] History;
    }

    public class DCFilterFIR
    {
        public double Sum;
        public double[] Delay;
    }

    public class DCFilterIir
    {
        public double PoleR;
        public double PrevInput;
        public double PrevOutput;
    }

    public class Ayumi /* : IDisposable */
    {
        public const int ToneChannels = 3;
        public const int DecimateFactor = 8;
        public const int FIRSize = 192;
        public const int DCFilterSize = 1024;
        // DC_CUTOFF = 3;
        public static double[] AYDacTable = { 0.0, 0.0, 0.00999465934234, 0.00999465934234, 0.0144502937362, 0.0144502937362, 0.0210574502174, 0.0210574502174, 0.0307011520562, 0.0307011520562, 0.0455481803616, 0.0455481803616, 0.0644998855573, 0.0644998855573, 0.107362478065, 0.107362478065, 0.126588845655, 0.126588845655, 0.20498970016, 0.20498970016, 0.292210269322, 0.292210269322, 0.372838941024, 0.372838941024, 0.492530708782, 0.492530708782, 0.635324635691, 0.635324635691, 0.805584802014, 0.805584802014, 1.0, 1.0 };
        public static double[] YMDacTable = { 0.0, 0.0, 0.00465400167849, 0.00772106507973, 0.0109559777218, 0.0139620050355, 0.0169985503929, 0.0200198367285, 0.024368657969, 0.029694056611, 0.0350652323186, 0.0403906309606, 0.0485389486534, 0.0583352407111, 0.0680552376593, 0.0777752346075, 0.0925154497597, 0.111085679408, 0.129747463188, 0.148485542077, 0.17666895552, 0.211551079576, 0.246387426566, 0.281101701381, 0.333730067903, 0.400427252613, 0.467383840696, 0.53443198291, 0.635172045472, 0.75800717174, 0.879926756695, 1.0 };
        public static EnvelopeShape[][] EnvelopeShapes = { new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.SlideDown }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.HoldBottom }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.SlideUp }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideDown, LibVT.EnvelopeShape.HoldTop }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.SlideUp }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.HoldTop }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.SlideDown }, new EnvelopeShape[] { LibVT.EnvelopeShape.SlideUp, LibVT.EnvelopeShape.HoldBottom } };
        public static double[] CoeffsTable = { -0.0000046183113992051936, -0.00001117761640887225, -0.000018610264502005432, -0.000025134586135631012, -0.000028494281690666197, -0.000026396828793275159, -0.000017094212558802156, 0.000023798193576966866, 0.000051281160242202183, 0.00007762197826243427, 0.000096759426664120416, 0.00010240229300393402, 0.000089344614218077106, 0.000054875700118949183, -0.000069839082210680165, -0.0001447966132360757, -0.00021158452917708308, -0.00025535069106550544, -0.00026228714374322104, -0.00022258805927027799, -0.00013323230495695704, 0.00016182578767055206, 0.00032846175385096581, 0.00047045611576184863, 0.00055713851457530944, 0.00056212565121518726, 0.00046901918553962478, 0.00027624866838952986, -0.00032564179486838622, -0.00065182310286710388, -0.00092127787309319298, -0.0010772534348943575, -0.0010737727700273478, -0.00088556645390392634, -0.00051581896090765534, 0.00059548767193795277, 0.0011803558710661009, 0.0016527320270369871, 0.0019152679330965555, 0.0018927324805381538, 0.0015481870327877937, 0.00089470695834941306, -0.0010178225878206125, -0.0020037400552054292, -0.0027874356824117317, -0.003210329988021943, -0.0031540624117984395, -0.0025657163651900345, -0.0014750752642111449, 0.0016624165446378462, 0.0032591192839069179, 0.0045165685815867747, 0.0051838984346123896, 0.0050774264697459933, 0.0041192521414141585, 0.0023628575417966491, -0.0026543507866759182, -0.0051990251084333425, -0.0072020238234656924, -0.0082672928192007358, -0.0081033739572956287, -0.006583111539570221, -0.0037839040415292386, 0.0042781252851152507, 0.0084176358598320178, 0.01172566057463055, 0.013550476647788672, 0.013388189369997496, 0.010979501242341259, 0.006381274941685413, -0.007421229604153888, -0.01486456304340213, -0.021143584622178104, -0.02504275058758609, -0.025473530942547201, -0.021627310017882196, -0.013104323383225543, 0.017065133989980476, 0.036978919264451952, 0.05823318062093958, 0.079072012081405949, 0.097675998716952317, 0.11236045936950932, 0.12176343577287731, 0.125 };

        public int SamRate = 0;
        public ToneChannel[] Channels = new ToneChannel[ToneChannels + 1];
        public int NoisePeriod = 0;
        public int NoiseCounter = 0;
        public int Noise = 0;
        public int EnvelopeCounter = 0;
        public int EnvelopePeriod = 0;
        public int EnvelopeShape = 0;
        public int EnvelopeSegment = 0;
        public int Envelope = 0;
        public double[] DACTable = new double[32];
        public double Step = 0;
        public double Position = 0;
        public Interpolator InterpolatorLeft = new Interpolator();
        public Interpolator InterpolatorRight = new Interpolator();
        public double[] FIRLeft = new double[FIRSize * 2 + 1];
        public double[] FIRRIght = new double[FIRSize * 2 + 1];
        public int FIRIndex = 0;
        public int DCType = 0;
        public int DCCutOff = 0;
        public DCFilterFIR DCLeftFIR = new DCFilterFIR();
        public DCFilterFIR DCRightFIR = new DCFilterFIR();
        public DCFilterIir DCLeftIir = new DCFilterIir();
        public DCFilterIir DCRightIir = new DCFilterIir();
        public int DCIndex = 0;
        public double Left = 0.0;
        public double Right = 0.0;

        public Ayumi()
        {
            for (int i = 0; i < Channels.Length; i++)
                Channels[i] = new ToneChannel();

            DCLeftFIR.Delay = new double[DCFilterSize + 1];
            DCRightFIR.Delay = new double[DCFilterSize + 1];

            InterpolatorLeft.Coeffs = new double[5];
            InterpolatorLeft.History = new double[5];

            InterpolatorRight.Coeffs = new double[5];
            InterpolatorRight.History = new double[5];
        }

        ~Ayumi()
        {
        }

        public void Configure(bool isYM, double clockRate, int sampleRate, int adcType)
        {
            Noise = 1;
            SamRate = sampleRate;
            DCType = adcType;
            DCCutOff = 3;

            SetEnvelope(1);
            SetChipType(isYM);
            SetChipFreq(clockRate);

            if (DCType == 2)
            {
                DCLeftIir.PoleR = 1.0 - (2.0 * Math.PI * DCCutOff / SamRate);
                DCRightIir.PoleR = DCLeftIir.PoleR;
            }

            for (int i = 0; i < Channels.Length; i++)
                SetTone(i, 1);
        }

        public void SetDCType(int value)
        {
            DCType = value;
        }

        public void SetDCCutoff(int value)
        {
            if (value < 3 || value > 10)
                return;

            DCCutOff = value;
            DCLeftIir.PrevInput = 0;
            DCLeftIir.PrevOutput = 0;
            DCRightIir.PrevInput = 0;
            DCRightIir.PrevOutput = 0;
            DCLeftIir.PoleR = 1.0 - (2.0 * Math.PI * DCCutOff / SamRate);
            DCRightIir.PoleR = DCLeftIir.PoleR;
        }

        public void ResetChip()
        {
            Position = 0;
            NoisePeriod = 0;
            NoiseCounter = 0;
            Noise = 1;
            EnvelopeCounter = 0;
            EnvelopePeriod = 0;
            EnvelopeShape = 0;
            EnvelopeSegment = 0;
            Envelope = 0;

            SetEnvelope(1);

            Array.Clear(InterpolatorLeft.Coeffs);
            Array.Clear(InterpolatorLeft.History);
            Array.Clear(InterpolatorRight.Coeffs);
            Array.Clear(InterpolatorRight.History);

            Array.Clear(FIRLeft);
            Array.Clear(FIRRIght);

            FIRIndex = 0;
            DCIndex = 0;

            DCLeftFIR.Sum = 0;
            DCRightFIR.Sum = 0;

            Array.Clear(DCLeftFIR.Delay);
            Array.Clear(DCRightFIR.Delay);

            DCLeftIir.PrevInput = 0;
            DCLeftIir.PrevOutput = 0;
            DCRightIir.PrevInput = 0;
            DCRightIir.PrevOutput = 0;

            Left = 0.0;
            Right = 0.0;
        }

        public void SetChipType(bool isYM)
        {
            Array.Copy(isYM ? YMDacTable : AYDacTable, DACTable, 32);
        }

        public void SetChipFreq(double clockRate)
        {
            Step = clockRate / (SamRate * 8 * DecimateFactor);
        }

        public void SetPan(int channelIndex, double pan, bool isEqp)
        {
            if (isEqp)
            {
                Channels[channelIndex].PanLeft = Math.Sqrt(1.0 - pan);
                Channels[channelIndex].PanRight = Math.Sqrt(pan);
            }
            else
            {
                Channels[channelIndex].PanLeft = 1.0 - pan;
                Channels[channelIndex].PanRight = pan;
            }
        }

        public void SetTone(int channelIndex, int period)
        {
            period &= 0x0FFF;
            Channels[channelIndex].TonePeriod = (period == 0 ? 1 : 0) | period;
        }

        public void SetNoise(int period)
        {
            period &= 0x1F;
            NoisePeriod = (period == 0 ? 1 : 0) | period;
        }

        public void SetMixer(int channelIndex, int toneOff, int noiseOff, int envelopeOn)
        {
            Channels[channelIndex].ToneOff = toneOff & 1;
            Channels[channelIndex].NoiseOff = noiseOff & 1;
            Channels[channelIndex].EnvelopeOn = envelopeOn;
        }

        public void SetVolume(int channelIndex, int volume)
        {
            Channels[channelIndex].Volume = volume & 0x0F;
        }

        public void SetEnvelope(int period)
        {
            period &= 0xFFFF;
            EnvelopePeriod = (period == 0 ? 1 : 0) | period;
        }

        public void ResetSegment()
        {
            EnvelopeShape envProc = EnvelopeShapes[EnvelopeShape][EnvelopeSegment];
            Envelope = (envProc == LibVT.EnvelopeShape.SlideDown || envProc == LibVT.EnvelopeShape.HoldTop) ? 31 : 0;
        }

        public void SetEnvelopeShape(int shape)
        {
            EnvelopeShape = shape & 0x0F;
            EnvelopeCounter = 0;
            EnvelopeSegment = 0;

            ResetSegment();
        }

        public void SlideUp()
        {
            if (++Envelope > 31)
            {
                EnvelopeSegment ^= 1;
                ResetSegment();
            }
        }

        public void SlideDown()
        {
            if (--Envelope < 0)
            {
                EnvelopeSegment ^= 1;
                ResetSegment();
            }
        }

        public void HoldTop()
        {
        }

        public void HoldBottom()
        {
        }

        public int UpdateTone(int channelIndex)
        {
            if (++Channels[channelIndex].ToneCounter >= Channels[channelIndex].TonePeriod)
            {
                Channels[channelIndex].ToneCounter = 0;
                Channels[channelIndex].Tone ^= 1;
            }

            return Channels[channelIndex].Tone;
        }

        public int UpdateNoise()
        {
            if (++NoiseCounter >= (NoisePeriod << 1))
            {
                NoiseCounter = 0;
                int bit0x3 = (Noise ^ (Noise >> 3)) & 1;
                Noise = (Noise >> 1) | (bit0x3 << 16);
            }

            return Noise & 1;
        }

        public void UpdateEnvelope()
        {
            if (++EnvelopeCounter >= EnvelopePeriod)
            {
                EnvelopeCounter = 0;

                switch (EnvelopeShapes[EnvelopeShape][EnvelopeSegment])
                {
                    case LibVT.EnvelopeShape.SlideUp:
                        SlideUp();
                        break;
                    case LibVT.EnvelopeShape.SlideDown:
                        SlideDown();
                        break;
                    case LibVT.EnvelopeShape.HoldTop:
                        HoldTop();
                        break;
                    case LibVT.EnvelopeShape.HoldBottom:
                        HoldBottom();
                        break;
                }
            }
        }

        public void UpdateMixer()
        {
            int iNoise = UpdateNoise();
            UpdateEnvelope();

            Left = 0.0;
            Right = 0.0;

            for (int i = 0; i < ToneChannels; i++)
            {
                int tone = UpdateTone(i);
                int res = (tone | Channels[i].ToneOff) & (iNoise | Channels[i].NoiseOff);
                res = (Channels[i].EnvelopeOn != 0) ? res * Envelope : res * Channels[i].Volume * 2 + 1;
                Left += DACTable[res] * Channels[i].PanLeft;
                Right += DACTable[res] * Channels[i].PanRight;
            }
        }

        public double Decimate(double[] x, int index)
        {
            double result =
                CoeffsTable[0] * (x[index + 1] + x[index + 191]) +
                CoeffsTable[1] * (x[index + 2] + x[index + 190]) +
                CoeffsTable[2] * (x[index + 3] + x[index + 189]) +
                CoeffsTable[3] * (x[index + 4] + x[index + 188]) +
                CoeffsTable[4] * (x[index + 5] + x[index + 187]) +
                CoeffsTable[5] * (x[index + 6] + x[index + 186]) +
                CoeffsTable[6] * (x[index + 7] + x[index + 185]) +
                CoeffsTable[7] * (x[index + 9] + x[index + 183]) +
                CoeffsTable[8] * (x[index + 10] + x[index + 182]) +
                CoeffsTable[9] * (x[index + 11] + x[index + 181]) +
                CoeffsTable[10] * (x[index + 12] + x[index + 180]) +
                CoeffsTable[11] * (x[index + 13] + x[index + 179]) +
                CoeffsTable[12] * (x[index + 14] + x[index + 178]) +
                CoeffsTable[13] * (x[index + 15] + x[index + 177]) +
                CoeffsTable[14] * (x[index + 17] + x[index + 175]) +
                CoeffsTable[15] * (x[index + 18] + x[index + 174]) +
                CoeffsTable[16] * (x[index + 19] + x[index + 173]) +
                CoeffsTable[17] * (x[index + 20] + x[index + 172]) +
                CoeffsTable[18] * (x[index + 21] + x[index + 171]) +
                CoeffsTable[19] * (x[index + 22] + x[index + 170]) +
                CoeffsTable[20] * (x[index + 23] + x[index + 169]) +
                CoeffsTable[21] * (x[index + 25] + x[index + 167]) +
                CoeffsTable[22] * (x[index + 26] + x[index + 166]) +
                CoeffsTable[23] * (x[index + 27] + x[index + 165]) +
                CoeffsTable[24] * (x[index + 28] + x[index + 164]) +
                CoeffsTable[25] * (x[index + 29] + x[index + 163]) +
                CoeffsTable[26] * (x[index + 30] + x[index + 162]) +
                CoeffsTable[27] * (x[index + 31] + x[index + 161]) +
                CoeffsTable[28] * (x[index + 33] + x[index + 159]) +
                CoeffsTable[29] * (x[index + 34] + x[index + 158]) +
                CoeffsTable[30] * (x[index + 35] + x[index + 157]) +
                CoeffsTable[31] * (x[index + 36] + x[index + 156]) +
                CoeffsTable[32] * (x[index + 37] + x[index + 155]) +
                CoeffsTable[33] * (x[index + 38] + x[index + 154]) +
                CoeffsTable[34] * (x[index + 39] + x[index + 153]) +
                CoeffsTable[35] * (x[index + 41] + x[index + 151]) +
                CoeffsTable[36] * (x[index + 42] + x[index + 150]) +
                CoeffsTable[37] * (x[index + 43] + x[index + 149]) +
                CoeffsTable[38] * (x[index + 44] + x[index + 148]) +
                CoeffsTable[39] * (x[index + 45] + x[index + 147]) +
                CoeffsTable[40] * (x[index + 46] + x[index + 146]) +
                CoeffsTable[41] * (x[index + 47] + x[index + 145]) +
                CoeffsTable[42] * (x[index + 49] + x[index + 143]) +
                CoeffsTable[43] * (x[index + 50] + x[index + 142]) +
                CoeffsTable[44] * (x[index + 51] + x[index + 141]) +
                CoeffsTable[45] * (x[index + 52] + x[index + 140]) +
                CoeffsTable[46] * (x[index + 53] + x[index + 139]) +
                CoeffsTable[47] * (x[index + 54] + x[index + 138]) +
                CoeffsTable[48] * (x[index + 55] + x[index + 137]) +
                CoeffsTable[49] * (x[index + 57] + x[index + 135]) +
                CoeffsTable[50] * (x[index + 58] + x[index + 134]) +
                CoeffsTable[51] * (x[index + 59] + x[index + 133]) +
                CoeffsTable[52] * (x[index + 60] + x[index + 132]) +
                CoeffsTable[53] * (x[index + 61] + x[index + 131]) +
                CoeffsTable[54] * (x[index + 62] + x[index + 130]) +
                CoeffsTable[55] * (x[index + 63] + x[index + 129]) +
                CoeffsTable[56] * (x[index + 65] + x[index + 127]) +
                CoeffsTable[57] * (x[index + 66] + x[index + 126]) +
                CoeffsTable[58] * (x[index + 67] + x[index + 125]) +
                CoeffsTable[59] * (x[index + 68] + x[index + 124]) +
                CoeffsTable[60] * (x[index + 69] + x[index + 123]) +
                CoeffsTable[61] * (x[index + 70] + x[index + 122]) +
                CoeffsTable[62] * (x[index + 71] + x[index + 121]) +
                CoeffsTable[63] * (x[index + 73] + x[index + 119]) +
                CoeffsTable[64] * (x[index + 74] + x[index + 118]) +
                CoeffsTable[65] * (x[index + 75] + x[index + 117]) +
                CoeffsTable[66] * (x[index + 76] + x[index + 116]) +
                CoeffsTable[67] * (x[index + 77] + x[index + 115]) +
                CoeffsTable[68] * (x[index + 78] + x[index + 114]) +
                CoeffsTable[69] * (x[index + 79] + x[index + 113]) +
                CoeffsTable[70] * (x[index + 81] + x[index + 111]) +
                CoeffsTable[71] * (x[index + 82] + x[index + 110]) +
                CoeffsTable[72] * (x[index + 83] + x[index + 109]) +
                CoeffsTable[73] * (x[index + 84] + x[index + 108]) +
                CoeffsTable[74] * (x[index + 85] + x[index + 107]) +
                CoeffsTable[75] * (x[index + 86] + x[index + 106]) +
                CoeffsTable[76] * (x[index + 87] + x[index + 105]) +
                CoeffsTable[77] * (x[index + 89] + x[index + 103]) +
                CoeffsTable[78] * (x[index + 90] + x[index + 102]) +
                CoeffsTable[79] * (x[index + 91] + x[index + 101]) +
                CoeffsTable[80] * (x[index + 92] + x[index + 100]) +
                CoeffsTable[81] * (x[index + 93] + x[index + 99]) +
                CoeffsTable[82] * (x[index + 94] + x[index + 98]) +
                CoeffsTable[83] * (x[index + 95] + x[index + 97]) +
                CoeffsTable[84] * x[index + 96];

            for (int i = 0; i < DecimateFactor; i++)
                x[index + FIRSize - DecimateFactor + i] = x[index + i];

            return result;
        }

        public double DCFilterFIR(DCFilterFIR dc, int index, double x)
        {
            dc.Sum += x - dc.Delay[index];
            dc.Delay[index] = x;

            return x - (dc.Sum / DCFilterSize);
        }

        public double DCFilterIir(DCFilterIir dc, double x)
        {
            double y = x - dc.PrevInput + dc.PoleR * dc.PrevOutput;

            dc.PrevInput = x;
            dc.PrevOutput = y;

            return y;
        }

        public void RemoveDC()
        {
            switch (DCType)
            {
                case 1:
                    Left = DCFilterFIR(DCLeftFIR, DCIndex, Left);
                    Right = DCFilterFIR(DCRightFIR, DCIndex, Right);
                    DCIndex = (DCIndex + 1) & (DCFilterSize - 1);
                    break;
                case 2:
                    Left = DCFilterIir(DCLeftIir, Left);
                    Right = DCFilterIir(DCRightIir, Right);
                    break;
            }
        }

        public void Process()
        {
            double deltaY;
            double[] coeffsLeft = InterpolatorLeft.Coeffs;
            double[] historyLeft = InterpolatorLeft.History;
            double[] coeffsRight = InterpolatorRight.Coeffs;
            double[] historyRight = InterpolatorRight.History;

            int firOffset = FIRSize - (FIRIndex * DecimateFactor);

            FIRIndex = (FIRIndex + 1) % ((FIRSize / DecimateFactor) - 1);

            for (int i = DecimateFactor - 1; i >= 0; i--)
            {
                Position += Step;

                while (Position >= 1)
                {
                    Position -= 1;
                    historyLeft[0] = historyLeft[1];
                    historyLeft[1] = historyLeft[2];
                    historyLeft[2] = historyLeft[3];

                    historyRight[0] = historyRight[1];
                    historyRight[1] = historyRight[2];
                    historyRight[2] = historyRight[3];

                    UpdateMixer();

                    historyLeft[3] = Left;
                    historyRight[3] = Right;

                    deltaY = historyLeft[2] - historyLeft[0];
                    coeffsLeft[0] = 0.5 * historyLeft[1] + 0.25 * (historyLeft[0] + historyLeft[2]);
                    coeffsLeft[1] = 0.5 * deltaY;
                    coeffsLeft[2] = 0.25 * (historyLeft[3] - historyLeft[1] - deltaY);

                    deltaY = historyRight[2] - historyRight[0];
                    coeffsRight[0] = 0.5 * historyRight[1] + 0.25 * (historyRight[0] + historyRight[2]);
                    coeffsRight[1] = 0.5 * deltaY;
                    coeffsRight[2] = 0.25 * (historyRight[3] - historyRight[1] - deltaY);
                }

                FIRLeft[firOffset + i] = (coeffsLeft[2] * Position + coeffsLeft[1]) * Position + coeffsLeft[0];
                FIRRIght[firOffset + i] = (coeffsRight[2] * Position + coeffsRight[1]) * Position + coeffsRight[0];
            }

            Left = Decimate(FIRLeft, firOffset);
            Right = Decimate(FIRRIght, firOffset);
        }
    }
}