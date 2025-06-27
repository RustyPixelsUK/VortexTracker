using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
//using System.Windows.Forms;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PT3
    {
        // Delphi: array[0..$62] of char → 99 bytes (0..98)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 99)]
        public byte[] Name;
        public byte Table;
        public byte Delay;
        public byte NumberOfPositions;
        public byte LoopPosition;
        public ushort PatternsPointer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ushort[] SamplePointers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] OrnamentPointers;
    }

    public class PT32VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private byte[] _skip = new byte[3];
        private byte[] _skipCounter = new byte[3];
        private byte[] _prevOrn = new byte[3];
        private int _nsBase;
        private int _ts;
        private int _pos;
        private PT3 _pt3;
        private VTM _vtm;

        public PT32VTM()
        {
        }

        public bool Initialize(byte[] data, int fSize, VTM vtm1, ref VTM vtm2)
        {
            _pt3 = Helpers.CastToStruct<PT3>(data);
            _vtm = vtm1;

            bool result = true;
            int i, j, k, kk;

            if (VTModule.DetectFeaturesLevel)
            {
                if (Helpers.StrLComp(_pt3.Name.ToString(), "ProTracker 3.", 13) == 0)
                {
                    if (_pt3.Name[13] >= 0x30 && _pt3.Name[13] <= 0x35)
                        vtm1.FeaturesLevel = FeaturesLevel.PT35;
                    else if (_pt3.Name[13] >= 0x37 && _pt3.Name[13] <= 0x39)
                        vtm1.FeaturesLevel = FeaturesLevel.PT37;
                    else
                        vtm1.FeaturesLevel = FeaturesLevel.VTII_PT36;
                }
                else if (Helpers.StrLComp(_pt3.Name.ToString(), "Vortex Tracker II", 17) == 0)
                    vtm1.FeaturesLevel = FeaturesLevel.VTII_PT36;
                else
                    vtm1.FeaturesLevel = FeaturesLevel.PT35;
            }

            if (VTModule.DetectModuleHeader)
                vtm1.HasHeader = Helpers.StrLComp(_pt3.Name.ToString(), "ProTracker 3.", 13) != 0;

            // Move(PT3.PT3_Name[$1E], VTM1.Title[1], 32);
            // Move(PT3.PT3_Name[$42], VTM1.Author[1], 32);

            vtm1.Title = Helpers.CopyBytesToString(_pt3.Name, 0x1E, 32).TrimEnd();
            vtm1.Author = Helpers.CopyBytesToString(_pt3.Name, 0x42, 32).TrimEnd();
            vtm1.NoteTable = (NoteTableType)_pt3.Table;
            vtm1.InitialDelay = _pt3.Delay;
            vtm1.Positions.Loop = _pt3.LoopPosition;

            for (i = 0; i < 256; i++)
                vtm1.Positions.Value[i] = 0;

            vtm1.Ornaments[0] = null;

            for (i = 1; i < 16; i++)
            {
                if (_pt3.OrnamentPointers[i] == 0)
                    vtm1.Ornaments[i] = null;
                else
                {
                    vtm1.Ornaments[i] = new Ornament();
                    vtm1.Ornaments[i].Loop = data[_pt3.OrnamentPointers[i]];
                    vtm1.Ornaments[i].Length = data[_pt3.OrnamentPointers[i] + 1];
                    for (j = 0; j < vtm1.Ornaments[i].Length; j++)
                        vtm1.Ornaments[i].Offsets[j] = (sbyte)data[_pt3.OrnamentPointers[i] + 2 + j];
                }
            }

            for (i = 16; i < 32; i++)
                vtm1.Ornaments[i] = null;

            for (i = 1; i < 32; i++)
            {
                if (_pt3.SamplePointers[i] == 0)
                    vtm1.Samples[i] = null;
                else
                {
                    if ((data[_pt3.SamplePointers[i]] > VTModule.MaxSampleLength - 1) || (data[_pt3.SamplePointers[i] + 1] > VTModule.MaxSampleLength))
                        continue;
                    
                    vtm1.Samples[i] = new Sample();
                    vtm1.Samples[i].Loop = data[_pt3.SamplePointers[i]];
                    vtm1.Samples[i].Length = data[_pt3.SamplePointers[i] + 1];
                    
                    for (j = 0; j < vtm1.Samples[i].Length; j++)
                    {
                        // VTM1.Samples[i].Items[j].Add_to_Ton := WordPtr(@PT3.Index[PT3.PT3_SamplePointers[i] + j * 4 + 4]) ^;
                        vtm1.Samples[i].Ticks[j].AddToTone = (short)BitConverter.ToUInt16(data, _pt3.SamplePointers[i] + j * 4 + 4);
                        vtm1.Samples[i].Ticks[j].Ton_Accumulation = (data[_pt3.SamplePointers[i] + j * 4 + 3] & 0x40) != 0;
                        vtm1.Samples[i].Ticks[j].Amplitude = (byte)(data[_pt3.SamplePointers[i] + j * 4 + 3] & 0xF);
                        vtm1.Samples[i].Ticks[j].Amplitude_Sliding = (data[_pt3.SamplePointers[i] + j * 4 + 2] & 0x80) != 0;
                        vtm1.Samples[i].Ticks[j].Amplitude_Slide_Up = (data[_pt3.SamplePointers[i] + j * 4 + 2] & 0x40) != 0;
                        vtm1.Samples[i].Ticks[j].Envelope_Enabled = (data[_pt3.SamplePointers[i] + j * 4 + 2] & 1) == 0;
                        vtm1.Samples[i].Ticks[j].Envelope_or_Noise_Accumulation = (data[_pt3.SamplePointers[i] + j * 4 + 3] & 0x20) != 0;
                        vtm1.Samples[i].Ticks[j].Add_to_Envelope_or_Noise = (sbyte)(data[_pt3.SamplePointers[i] + j * 4 + 2] >> 1);
                        
                        if ((vtm1.Samples[i].Ticks[j].Add_to_Envelope_or_Noise & 0x10) != 0)
                            vtm1.Samples[i].Ticks[j].Add_to_Envelope_or_Noise = (sbyte)(vtm1.Samples[i].Ticks[j].Add_to_Envelope_or_Noise | 0xF0);
                        else
                            vtm1.Samples[i].Ticks[j].Add_to_Envelope_or_Noise = (sbyte)(vtm1.Samples[i].Ticks[j].Add_to_Envelope_or_Noise & 15);

                        vtm1.Samples[i].Ticks[j].Mixer_Ton = (data[_pt3.SamplePointers[i] + j * 4 + 3] & 0x10) == 0;
                        vtm1.Samples[i].Ticks[j].Mixer_Noise = (data[_pt3.SamplePointers[i] + j * 4 + 3] & 0x80) == 0;
                    }
                }
            }

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                vtm1.Patterns[i] = null;

            vtm2 = null;
            _ts = (byte)_pt3.Name[98];

            if (((_ts != 0x20) && (_pt3.Name[13] >= '7' && _pt3.Name[13] <= '9')) || FoundPT36TS(data, fSize, vtm1, ref vtm2))
                vtm2 = (VTM)vtm1.Clone();

            _pos = 0;

            while (_pos < 256 && data[_pos + 0xC9] != 255)
            {
                j = data[_pos + 0xC9] / 3;
                if (vtm2 != null)
                {
                    j = _ts - j - 1;
                    vtm2.Positions.Value[_pos] = j;
                }

                vtm1.Positions.Value[_pos] = j;

                _pos++;

                if (vtm2 != null)
                {
                    DecodePattern(data, vtm2, j, j);
                    DecodePattern(data, vtm1, j, _ts - j - 1);

                    for (i = 0; i < VTModule.MaxPatternLength; i++)
                    {
                        bool tset = false;

                        for (k = 2; k >= 0; k--)
                        {
                            if (vtm2.Patterns[j].Lines[i].Channel[k].AdditionalCommand.Number == 11)
                            {
                                for (kk = 2; kk >= 0; kk--)
                                {
                                    if (vtm1.Patterns[j].Lines[i].Channel[kk].AdditionalCommand.Number == 0 ||
                                        vtm1.Patterns[j].Lines[i].Channel[kk].AdditionalCommand.Number == 11)
                                    {
                                        vtm1.Patterns[j].Lines[i].Channel[kk].AdditionalCommand = vtm2.Patterns[j].Lines[i].Channel[k].AdditionalCommand;
                                        break;
                                    }
                                }
                                tset = true;
                                break;
                            }
                        }

                        if (!tset)
                        {
                            for (k = 2; k >= 0; k--)
                            {
                                if (vtm1.Patterns[j].Lines[i].Channel[k].AdditionalCommand.Number == 11)
                                {
                                    for (kk = 2; kk >= 0; kk--)
                                    {
                                        if (vtm2.Patterns[j].Lines[i].Channel[kk].AdditionalCommand.Number == 0)
                                        {
                                            vtm2.Patterns[j].Lines[i].Channel[kk].AdditionalCommand = vtm1.Patterns[j].Lines[i].Channel[k].AdditionalCommand;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                    DecodePattern(data, vtm1, j, j);
            }

            vtm1.Positions.Length = _pos;

            if (vtm2 != null)
                vtm2.Positions.Length = _pos;

            return result;
        }

        public void PatternInterpreter(byte[] data, VTM vtm, int patNum, int lnNum, int chNum)
        {
            short tmp = 0;
            bool quit = false;

            do
            {
                if (data[_chPtr[chNum]] >= 0xF0 && data[_chPtr[chNum]] <= 0xFF)
                {
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _prevOrn[chNum] = (byte)(data[_chPtr[chNum]] - 0xF0);
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                    _chPtr[chNum]++;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)(data[_chPtr[chNum]] / 2);
                }
                else if (data[_chPtr[chNum]] >= 0xD1 && data[_chPtr[chNum]] <= 0xEF)
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)(data[_chPtr[chNum]] - 0xD0);
                else if (data[_chPtr[chNum]] == 0xD0)
                    quit = true;
                else if (data[_chPtr[chNum]] >= 0xC1 && data[_chPtr[chNum]] <= 0xCF)
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)(data[_chPtr[chNum]] - 0xC0);
                else if (data[_chPtr[chNum]] == 0xC0)
                {
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0xB2 && data[_chPtr[chNum]] <= 0xBF)
                {
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0xB1);
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                    _chPtr[chNum]++;
                    vtm.Patterns[patNum].Lines[lnNum].Envelope = (ushort)(data[_chPtr[chNum]] << 8);
                    _chPtr[chNum]++;
                    vtm.Patterns[patNum].Lines[lnNum].Envelope += data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xB1)
                {
                    _chPtr[chNum]++;
                    _skip[chNum] = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xB0)
                {
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                }
                else if (data[_chPtr[chNum]] >= 0x50 && data[_chPtr[chNum]] <= 0xAF)
                {
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)(data[_chPtr[chNum]] - 0x50);
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x40 && data[_chPtr[chNum]] <= 0x4F)
                {
                    if (data[_chPtr[chNum]] == 0x40)
                    {
                        // only for Orn #0 from PT3.69
                        if (vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope == 0)
                            vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    }
                    _prevOrn[chNum] = (byte)(data[_chPtr[chNum]] - 0x40);
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                }
                else if (data[_chPtr[chNum]] >= 0x20 && data[_chPtr[chNum]] <= 0x3F)
                    _nsBase = data[_chPtr[chNum]] - 0x20;
                else if (data[_chPtr[chNum]] >= 0x10 && data[_chPtr[chNum]] <= 0x1F)
                {
                    if (data[_chPtr[chNum]] == 0x10)
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    else
                    {
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0x10);
                        _chPtr[chNum]++;
                        vtm.Patterns[patNum].Lines[lnNum].Envelope = (ushort)(data[_chPtr[chNum]] << 8);
                        _chPtr[chNum]++;
                        vtm.Patterns[patNum].Lines[lnNum].Envelope += data[_chPtr[chNum]];
                    }
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                    _chPtr[chNum]++;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)(data[_chPtr[chNum]] / 2);
                }
                else if (data[_chPtr[chNum]] >= 0x8 && data[_chPtr[chNum]] <= 0x9)
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = data[_chPtr[chNum]];
                else if (data[_chPtr[chNum]] >= 0x1 && data[_chPtr[chNum]] <= 0x5)
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = data[_chPtr[chNum]];

                _chPtr[chNum]++;
            }
            while (!quit);
            
            switch (vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number)
            {
                case 1:
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = data[_chPtr[chNum]];
                    _chPtr[chNum]++;
                    // move(PT3.Index[ChPtr[ChNum]], Tmp, 2);
                    tmp = BitConverter.ToInt16(data, _chPtr[chNum]);
                    if (tmp < 0)
                    {
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number++;
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)-tmp;
                    }
                    else
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)tmp;
                    _chPtr[chNum] += 2;
                    break;
                case 2:
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number++;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = data[_chPtr[chNum]];
                    _chPtr[chNum] += 3;
                    // move(PT3.Index[ChPtr[ChNum]], Tmp, 2);
                    tmp = BitConverter.ToInt16(data, _chPtr[chNum]);
                    if (tmp < 0)
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)-tmp;
                    else
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)tmp;
                    _chPtr[chNum] += 2;
                    break;
                case 3:
                case 4:
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number++;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                    _chPtr[chNum]++;
                    break;
                case 5:
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number++;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)(data[_chPtr[chNum]] << 4);
                    _chPtr[chNum]++;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter += data[_chPtr[chNum]];
                    _chPtr[chNum]++;
                    break;
                case 8:
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number++;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = data[_chPtr[chNum]];
                    _chPtr[chNum]++;
                    // move(PT3.Index[ChPtr[ChNum]], Tmp, 2);;
                    tmp = BitConverter.ToInt16(data, _chPtr[chNum]);
                    if (tmp < 0)
                    {
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number++;
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)-tmp;
                    }
                    else
                        vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)tmp;
                    _chPtr[chNum] += 2;
                    break;
                case 9:
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 0xB;
                    vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                    _chPtr[chNum]++;
                    break;
            }

            _skipCounter[chNum] = _skip[chNum];
        }

        public void DecodePattern(byte[] data, VTM vtm, int j, int jj)
        {
            int i, k;
            bool quit = false;

            if (vtm.Patterns[j] == null)
            {
                vtm.Patterns[j] = new Pattern();

                for (k = 0; k < 3; k++)
                {
                    _prevOrn[k] = 0;
                    _skipCounter[k] = 1;
                    _skip[k] = 1;
                }

                // move(PT3.Index[PT3.PT3_PatternsPointer + jj * 6], ChPtr, 6);
                Helpers.Move(data, _pt3.PatternsPointer + jj * 6, _chPtr, 6);

                _nsBase = 0;
                i = 0;

                while (i < VTModule.MaxPatternLength && !quit)
                {
                    for (k = 0; k < 3; k++)
                    {
                        _skipCounter[k]--;

                        if (_skipCounter[k] == 0)
                        {
                            if (k == 0 && data[_chPtr[0]] == 0)
                            {
                                i--;
                                quit = true;
                                break;
                            }

                            PatternInterpreter(data, vtm, j, i, k);
                        }
                    }

                    if (i >= 0)
                        vtm.Patterns[j].Lines[i].Noise = (byte)_nsBase;

                    i++;
                }

                vtm.Patterns[j].Length = i;
            }
        }

        public bool FoundPT36TS(byte[] data, int fSize, VTM vtm1, ref VTM vtm2)
        {
            bool result = false;
            int j1, j2, k;
            int pos = 0;

            if (_pt3.Name[13] != '6')
                return result;

            while (pos < 256 && data[pos + 0xC9] != 255)
            {
                j1 = data[pos + 0xC9] / 3;

                if (j1 < 0x30 / 2)
                    return result;

                j2 = 0x30 - j1 - 1;
                // move(PT3.Index[PT3.PT3_PatternsPointer + j2 * 6], ChPtr, 6);
                Helpers.Move(data, _pt3.PatternsPointer + j2 * 6, _chPtr, 6);

                for (k = 0; k < 3; k++)
                {
                    if (_chPtr[k] < 100 || _chPtr[k] >= fSize - 4)
                        return result;
                }

                pos++;
            }

            if ((int)AppEvents.SendEvent(EventType.MessageBox, "This PT 3.6 Module May Contain Turbo Sound Data. Try To Import?", Main.ProductName, MyMessageBoxButtons.YesNo | MyMessageBoxButtons.YesNo, MyMessageBoxIcon.Question) != 6)
                return result;

            /* if (MessageBox.Show(this, "This PT 3.6 module may contain Turbo Sound data. Try to import?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return result;
            */

            _ts = 0x30;

            return true;
        }
    }
}