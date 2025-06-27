using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static System.Net.WebRequestMethods;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GTR
    {
        public byte Delay;
        // 4 bytes (0..3)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ID;
        public ushort Address;
        // 32 bytes (0..31)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public ushort[] SamplesPointers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] OrnamentsPointers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public PatternTriple[] PatternsPointers;
        public byte NumberOfPositions;
        public byte LoopPosition;
        // 0..(65536 - 295 - 1): 65240 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65240)]
        public byte[] Positions;
    }

    public class GTR2VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private bool[] _isOrnament = new bool[16];
        private bool[] _isSample = new bool[17];
        private byte[] _cOrn = new byte[3];
        private bool[] _envEn = new bool[3];
        private int _envT;
        private int _pos;
        private GTR _gtr;
        private VTM _vtm;

        public GTR2VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _gtr = Helpers.CastToStruct<GTR>(data);
            _vtm = vtm;

            bool result = true;
            bool quit = false;
            int i, j, k;

            // Move(GTR.GTR_Name, _vtm.Title[1], 32);
            _vtm.Title = Helpers.CopyCharsToString(_gtr.Name, 0, 32);
            _vtm.Title = _vtm.Title.TrimEnd();
            _vtm.Author = "";
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = _gtr.Delay;
            _vtm.Positions.Loop = _gtr.LoopPosition;

            for (i = 0; i < 16; i++)
                _vtm.Ornaments[i] = null;

            for (i = 1; i < 16; i++)
                _isOrnament[i] = false;

            for (i = 1; i < 32; i++)
                _vtm.Samples[i] = null;

            for (i = 1; i < 17; i++)
                _isSample[i] = false;

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                _vtm.Patterns[i] = null;

            _pos = 0;

            while (_pos <= _gtr.NumberOfPositions)
            {
                j = _gtr.Positions[_pos] / 6;
                _vtm.Positions.Value[_pos] = j;
                _pos++;

                if (_vtm.Patterns[j] == null)
                {
                    _vtm.Patterns[j] = new Pattern();

                    for (k = 0; k < 3; k++)
                    {
                        _cOrn[k] = 0;
                        _envEn[k] = false;
                        _envT = 15;
                        _skipCounter[k] = 0;
                    }

                    // Move(GTR.GTR_PatternsPointers[j], ChPtr, 6);
                    Helpers.Move(_gtr.PatternsPointers[j], _chPtr, 0);

                    i = 0;
                    quit = false;

                    while ((i < VTModule.MaxPatternLength) && !quit)
                    {
                        for (k = 0; k < 3; k++)
                        {
                            _skipCounter[k]--;

                            if (_skipCounter[k] < 0)
                            {
                                if ((k == 0) && (data[_chPtr[0]] == 255))
                                {
                                    i--;
                                    quit = true;
                                    break;
                                }

                                PatternInterpreter(data, j, i, k);
                            }
                        }

                        i++;
                    }

                    _vtm.Patterns[j].Length = i;
                }
            }

            _vtm.Positions.Length = _pos;

            for (i = 1; i < 16; i++)
            {
                if (_isOrnament[i])
                {
                    _vtm.Ornaments[i] = new Ornament();
                    j = _gtr.OrnamentsPointers[i];
                    _vtm.Ornaments[i].Loop = data[j];
                    _vtm.Ornaments[i].Length = data[j + 1];

                    j += 2;
                    
                    for (k = 0; k < _vtm.Ornaments[i].Length; k++)
                        _vtm.Ornaments[i].Offsets[k] = (sbyte)data[j + k];
                }
            }

            for (i = 1; i < 17; i++)
            {
                if (_isSample[i])
                {
                    j = _gtr.SamplesPointers[i - 1];

                    if ((data[j] / 4 > VTModule.MaxSampleLength - 1) || (data[j + 1] / 4 > VTModule.MaxSampleLength))
                        continue;

                    _vtm.Samples[i] = new Sample();
                    _vtm.Samples[i].Loop = (byte)(data[j] / 4);
                    _vtm.Samples[i].Length = (byte)(data[j + 1] / 4);

                    j += 2;

                    for (k = 0; k < _vtm.Samples[i].Length; k++)
                    {
                        _vtm.Samples[i].Ticks[k] = new SampleTick();
                        // _vtm.Samples[i].Items[k].Add_to_Ton := WordPtr(@GTR.Index[j + k * 4 + 2])^;
                        _vtm.Samples[i].Ticks[k].AddToTone = BitConverter.ToInt16(data, j + k * 4 + 2);
                        _vtm.Samples[i].Ticks[k].Mixer_Noise = (((sbyte)data[j + k * 4 + 1]) & 64) == 0;
                        
                        if (_vtm.Samples[i].Ticks[k].Mixer_Noise)
                        {
                            _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(data[j + k * 4 + 1] & 0x1F);
                            
                            if ((_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise & 0x10) != 0)
                                _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise | 0xF0);
                        }

                        _vtm.Samples[i].Ticks[k].Amplitude = (byte)(data[j + k * 4] & 15);
                        _vtm.Samples[i].Ticks[k].Mixer_Ton = (data[j + k * 4 + 1] & 32) == 0;
                        _vtm.Samples[i].Ticks[k].Envelope_Enabled = ((sbyte)data[j + k * 4 + 1]) < 0;
                    }
                }
            }

            return result;
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            int i;

            _skipCounter[chNum] = 0;

            do
            {
                int val = data[_chPtr[chNum]];

                if (val >= 0x00 && val <= 0x5F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)val;
                    _chPtr[chNum]++;
                    return;
                }
                else if (val >= 0x60 && val <= 0x6F)
                {
                    i = val - 0x5F;
                    _isSample[i] = true;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)i;
                }
                else if (val >= 0x70 && val <= 0x7F)
                {
                    i = val - 0x70;
                    _cOrn[chNum] = (byte)i;

                    if (i > 0)
                        _isOrnament[i] = true;

                    if (_envEn[chNum] && (_gtr.ID[3] == '\x10'))
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)_envT;
                    else
                    {
                        _envEn[chNum] = false;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    }

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)i;
                }
                else if (val >= 0x80 && val <= 0xBF)
                {
                    _skipCounter[chNum] = (sbyte)(val - 0x80);
                }
                else if (val >= 0xC0 && val <= 0xCF)
                {
                    _envEn[chNum] = true;
                    i = val - 0xC0;

                    if (i == 0)
                        i = 9;
                    else if (i == 15)
                        i = 7;

                    _envT = i;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)i;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _cOrn[chNum];
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = data[_chPtr[chNum]];
                }
                else if (val >= 0xD0 && val <= 0xDF)
                {
                    _chPtr[chNum]++;
                    return;
                }
                else if (val >= 0xE0 && val <= 0xEF)
                {
                    if (val == 0xE0)
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;

                        if (_gtr.ID[3] != '\x10')
                        {
                            _chPtr[chNum]++;
                            return;
                        }
                    }
                    else // covers 0xE1 .. 0xEF
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)(val - 0xE0);
                    }
                }

                _chPtr[chNum]++;
            }
            while (true);
        }
    }
}
