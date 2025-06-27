using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PT2
    {
        public byte Delay;
        public byte NumberOfPositions;
        public byte LoopPosition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ushort[] SamplePointers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] OrnamentPointers;
        public ushort PatternsPointer;
        // 30 bytes (0..29)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public byte[] MusicName;
        // 0..(65535 - 131): 65405 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65405)]
        public byte[] PositionList;
    }

    public class PT22VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private sbyte[] _skip = new sbyte[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private byte[] _prevOrn = new byte[3];
        private int _nsBase;
        private int _pos;
        private PT2 _pt2;
        private VTM _vtm;

        public PT22VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _pt2 = Helpers.CastToStruct<PT2>(data);
            _vtm = vtm;

            bool result = true;
            bool quit = false;
            int i, j, k;

            if (VTModule.DetectFeaturesLevel)
                _vtm.FeaturesLevel = 0;
 
            if (VTModule.DetectModuleHeader)
                _vtm.HasHeader = false;

            // Move(PT2.PT2_MusicName, _vtm.Title[1], 30);

            _vtm.Title = Helpers.CopyCharsToString(_pt2.MusicName, 0, 30);
            _vtm.Title = _vtm.Title.TrimEnd();
            _vtm.Author = "";
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = _pt2.Delay;
            _vtm.Positions.Loop = _pt2.LoopPosition;

            for (i = 0; i < 256; i++)
                _vtm.Positions.Value[i] = 0;

            _vtm.Ornaments[0] = null;

            for (i = 1; i < 16; i++)
            {
                if (_pt2.OrnamentPointers[i] == 0)
                    _vtm.Ornaments[i] = null;
                else
                {
                    _vtm.Ornaments[i] = new Ornament();
                    _vtm.Ornaments[i].Loop = data[_pt2.OrnamentPointers[i] + 1];
                    _vtm.Ornaments[i].Length = data[_pt2.OrnamentPointers[i]];
                    
                    if ((_vtm.Ornaments[i].Length == 0) || (_vtm.Ornaments[i].Length > VTModule.MaxOrnamentLength))
                        _vtm.Ornaments[i].Length = VTModule.MaxOrnamentLength;

                    if (_vtm.Ornaments[i].Loop >= _vtm.Ornaments[i].Length)
                        _vtm.Ornaments[i].Loop = _vtm.Ornaments[i].Length - 1;

                    for (j = 0; j < _vtm.Ornaments[i].Length; j++)
                        _vtm.Ornaments[i].Offsets[j] = (sbyte)data[_pt2.OrnamentPointers[i] + 2 + j];
                }
            }

            for (i = 1; i < 32; i++)
            {
                if (_pt2.SamplePointers[i] == 0)
                    _vtm.Samples[i] = null;
                else
                {
                    if (data[_pt2.SamplePointers[i] + 1] > VTModule.MaxSampleLength - 1)
                        continue;

                    if (data[_pt2.SamplePointers[i]] > VTModule.MaxSampleLength)
                        continue;

                    _vtm.Samples[i] = new Sample();
                    _vtm.Samples[i].Loop = data[_pt2.SamplePointers[i] + 1];
                    _vtm.Samples[i].Length = data[_pt2.SamplePointers[i]];

                    if ((_vtm.Samples[i].Length == 0) || (_vtm.Samples[i].Length > VTModule.MaxSampleLength))
                        _vtm.Samples[i].Length = VTModule.MaxSampleLength;

                    if (_vtm.Samples[i].Loop >= _vtm.Samples[i].Length)
                        _vtm.Samples[i].Loop = (byte)(_vtm.Samples[i].Length - 1);

                    for (j = 0; j < _vtm.Samples[i].Length; j++)
                    {
                        _vtm.Samples[i].Ticks[j] = new SampleTick();
                        _vtm.Samples[i].Ticks[j].Envelope_Enabled = true;
                        _vtm.Samples[i].Ticks[j].AddToTone = (short)(data[_pt2.SamplePointers[i] + j * 3 + 4] + (data[_pt2.SamplePointers[i] + j * 3 + 3] & 15) << 8);
                        
                        if ((data[_pt2.SamplePointers[i] + j * 3 + 2] & 4) == 0)
                            _vtm.Samples[i].Ticks[j].AddToTone = (short)-_vtm.Samples[i].Ticks[j].AddToTone;

                        _vtm.Samples[i].Ticks[j].Amplitude = (byte)(data[_pt2.SamplePointers[i] + j * 3 + 3] >> 4);
                        _vtm.Samples[i].Ticks[j].Mixer_Noise = (data[_pt2.SamplePointers[i] + j * 3 + 2] & 1) == 0;

                        if (_vtm.Samples[i].Ticks[j].Mixer_Noise)
                            _vtm.Samples[i].Ticks[j].Add_to_Envelope_or_Noise = (sbyte)((data[_pt2.SamplePointers[i] + j * 3 + 2] >> 3) & 31);

                        if ((_vtm.Samples[i].Ticks[j].Add_to_Envelope_or_Noise & 0x10) != 0)
                            _vtm.Samples[i].Ticks[j].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[i].Ticks[j].Add_to_Envelope_or_Noise | 0xF0);

                        _vtm.Samples[i].Ticks[j].Mixer_Ton = (data[_pt2.SamplePointers[i] + j * 3 + 2] & 2) == 0;
                    }
                }
            }
            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                _vtm.Patterns[i] = null;

            _pos = 0;

            while ((_pos < 256) && (_pt2.PositionList[_pos] < 128))
            {
                j = _pt2.PositionList[_pos];

                _vtm.Positions.Value[_pos] = j;

                _pos++;

                if (_vtm.Patterns[j] == null)
                {
                    _vtm.Patterns[j] = new Pattern();

                    for (k = 0; k < 3; k++)
                    {
                        _prevOrn[k] = 0;
                        _skipCounter[k] = 0;
                        _skip[k] = 0;
                    }

                    // Move(PT2.Index[PT2.PT2_PatternsPointer + j * 6], ChPtr, 6);
                    Helpers.Move(data, _pt2.PatternsPointer + j * 6, _chPtr, 6);
                    
                    _nsBase = 0;
                    i = 0;
                    
                    quit = false;
                    
                    while ((i < VTModule.MaxPatternLength) && !quit)
                    {
                        for (k = 0; k < 3; k++)
                        {
                            _skipCounter[k]--;

                            if (_skipCounter[k] < 0)
                            {
                                if ((k == 0) && (data[_chPtr[0]] == 0))
                                {
                                    i--;
                                    quit = true;
                                    break;
                                }

                                PatternInterpreter(data, j, i, k);
                            }
                        }
                        if (i >= 0)
                            _vtm.Patterns[j].Lines[i].Noise = (byte)_nsBase;

                        i++;
                    }

                    _vtm.Patterns[j].Length = i;
                }
            }
            _vtm.Positions.Length = _pos;

            return result;
        }

        private void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool quit = false;

            do
            {
                if (data[_chPtr[chNum]] >= 0xE1 && data[_chPtr[chNum]] <= 0xFF)
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)(data[_chPtr[chNum]] - 0xE0);
                else if (data[_chPtr[chNum]] == 0xE0)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x80 && data[_chPtr[chNum]] <= 0xDF)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)(data[_chPtr[chNum]] - 0x80);
                    quit = true;
                }
                else if (data[_chPtr[chNum]] == 0x7F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                }
                else if (data[_chPtr[chNum]] >= 0x71 && data[_chPtr[chNum]] <= 0x7E)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0x70);
                    
                    _chPtr[chNum]++;

                    // _vtm.Patterns[PatNum].Items[LnNum].Envelope := WordPtr(@PT2.Index[ChPtr[ChNum]]) ^;
                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = BitConverter.ToUInt16(data, _chPtr[chNum]);

                    _chPtr[chNum]++;
                }
                else if (data[_chPtr[chNum]] == 0x70)
                    quit = true;
                else if (data[_chPtr[chNum]] >= 0x60 && data[_chPtr[chNum]] <= 0x6F)
                {
                    _prevOrn[chNum] = (byte)(data[_chPtr[chNum]] - 0x60);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];
                    
                    if (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope == 0)
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                }
                else if (data[_chPtr[chNum]] >= 0x20 && data[_chPtr[chNum]] <= 0x5F)
                    _skip[chNum] = (sbyte)(data[_chPtr[chNum]] - 0x20);
                else if (data[_chPtr[chNum]] >= 0x10 && data[_chPtr[chNum]] <= 0x1F)
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)(data[_chPtr[chNum]] - 0x10);
                else if (data[_chPtr[chNum]] == 0xF)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 11;
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xE)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _chPtr[chNum]++;
                    
                    if (((sbyte)data[_chPtr[chNum]]) >= 0)
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                    }
                    else
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 2;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)-data[_chPtr[chNum]];
                    }
                }
                else if (data[_chPtr[chNum]] == 0xD)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 3;
                    
                    _chPtr[chNum]++;
                    
                    if (((sbyte)data[_chPtr[chNum]]) >= 0)
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                    else
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)-data[_chPtr[chNum]];

                    _chPtr[chNum] += 2;
                }
                else if (data[_chPtr[chNum]] == 0xC)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 0;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = 0;
                }
                else
                {
                    _chPtr[chNum]++;
                    _nsBase = data[_chPtr[chNum]];
                }

                _chPtr[chNum]++;
            }
            while (!quit);

            _skipCounter[chNum] = _skip[chNum];
        }
    }
}
