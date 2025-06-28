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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PT1
    {
        public byte Delay;
        public byte NumberOfPositions;
        public byte LoopPosition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] SamplesPointers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] OrnamentsPointers;
        public ushort PatternsPointer;
        // 30 bytes (0..29)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public byte[] MusicName;
        // 0..(65535 - 99): 65437 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65437)]
        public byte[] PositionList;
    }

    public class PT12VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private sbyte[] _skip = new sbyte[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private bool[] _isOrnament = new bool[16];
        private bool[] _isSample = new bool[17];
        private byte[] _orn2Sam = new byte[16];
        private byte[] _cSam = new byte[3];
        private byte[] _cOrn = new byte[3];
        private int _pos;
        private PT1 _pt1;
        private VTM _vtm;

        public PT12VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _pt1 = Helpers.CastToStruct<PT1>(data);
            _vtm = vtm;

            bool result = true;
            int i, j, k;

            // Move(PT1.PT1_MusicName, _vtm.Title[1], 30);
            _vtm.Title = Helpers.CopyCharsToString(_pt1.MusicName, 0, 30);
            _vtm.Title = _vtm.Title.TrimEnd();
            _vtm.Author = "";
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = _pt1.Delay;
            _vtm.Positions.Loop = _pt1.LoopPosition;

            for (i = 0; i < 16; i++)
                _vtm.Ornaments[i] = null;

            for (i = 1; i < 16; i++)
            {
                _isOrnament[i] = false;
                _orn2Sam[i] = 0;
            }

            for (i = 1; i < 32; i++)
                _vtm.Samples[i] = null;

            for (i = 1; i < 17; i++)
                _isSample[i] = false;

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                _vtm.Patterns[i] = null;

            for (k = 0; k < 3; k++)
            {
                _cSam[k] = 0;
                _cOrn[k] = 0;
            }

            _pos = 0;

            while (_pos < _pt1.NumberOfPositions)
            {
                j = _pt1.PositionList[_pos];
                _vtm.Positions.Value[_pos] = j;

                _pos++;

                if (_vtm.Patterns[j] == null)
                {
                    _vtm.Patterns[j] = new Pattern();

                    for (k = 0; k < 3; k++)
                    {
                        _skipCounter[k] = 0;
                        _skip[k] = 0;
                    }

                    // Move(PT1.Index[PT1.PT1_PatternsPointer + j * 6], ChPtr, 6);
                    Helpers.Move(data, _pt1.PatternsPointer + j * 6, _chPtr, 0, 6);

                    i = 0;
                    bool quit = false;

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
                    j = 0;

                    _vtm.Ornaments[i] = new Ornament();
                    k = _orn2Sam[i] - 1;

                    if (k < 0)
                    {
                        _vtm.Ornaments[i].Loop = 0;
                        _vtm.Ornaments[i].Length = 32;
                    }
                    else
                    {
                        j = _pt1.SamplesPointers[k];
                        _vtm.Ornaments[i].Length = data[j];
                        _vtm.Ornaments[i].Loop = data[j + 1];
                    }

                    j = _pt1.OrnamentsPointers[i];

                    for (k = 0; k < _vtm.Ornaments[i].Length; k++)
                        _vtm.Ornaments[i].Offsets[k] = (sbyte)data[j + k];
                }
            }

            for (i = 1; i < 17; i++)
            {
                if (_isSample[i])
                {
                    j = _pt1.SamplesPointers[i - 1];

                    if ((data[j] > VTModule.MaxSampleLength) || (data[j + 1] > VTModule.MaxSampleLength - 1))
                        continue;

                    _vtm.Samples[i] = new Sample();
                    _vtm.Samples[i].Length = data[j];
                    _vtm.Samples[i].Loop = data[j + 1];

                    j += 2;
                    
                    for (k = 0; k < _vtm.Samples[i].Length; k++)
                    {
                        _vtm.Samples[i].Ticks[k] = new SampleTick();
                        _vtm.Samples[i].Ticks[k].AddToTone = (short)((data[j + k * 3] & 0xF0) << 4 + data[j + k * 3 + 2]);
                        
                        if ((data[j + k * 3 + 1] & 32) == 0)
                            _vtm.Samples[i].Ticks[k].AddToTone = (short)-_vtm.Samples[i].Ticks[k].AddToTone;

                        _vtm.Samples[i].Ticks[k].Amplitude = (byte)(data[j + k * 3] & 15);
                        _vtm.Samples[i].Ticks[k].Mixer_Noise = ((sbyte)data[j + k * 3 + 1]) >= 0;

                        if (_vtm.Samples[i].Ticks[k].Mixer_Noise)
                        {
                            _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(data[j + k * 3 + 1] & 0x1F);
                        
                            if ((_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise & 0x10) != 0)
                                _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise | 0xF0);
                        }

                        _vtm.Samples[i].Ticks[k].Mixer_Ton = (data[j + k * 3 + 1] & 64) == 0;
                        _vtm.Samples[i].Ticks[k].Envelope_Enabled = true;
                    }
                }
            }
            return result;
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool quit = false;
            int i;

            do
            {
                if (data[_chPtr[chNum]] >= 0x00 && data[_chPtr[chNum]] <= 0x5F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)data[_chPtr[chNum]];
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x60 && data[_chPtr[chNum]] <= 0x6F)
                {
                    i = data[_chPtr[chNum]] - 0x5F;
                    _cSam[chNum] = (byte)i;
                    _isSample[i] = true;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)i;
                }
                else if (data[_chPtr[chNum]] >= 0x70 && data[_chPtr[chNum]] <= 0x7F)
                {
                    i = data[_chPtr[chNum]] - 0x70;
                    _cOrn[chNum] = (byte)i;

                    if (i > 0)
                        _isOrnament[i] = true;

                    if (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope == 0)
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)i;
                }
                else if (data[_chPtr[chNum]] == 0x80)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    quit = true;
                }
                else if (data[_chPtr[chNum]] == 0x81)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _cOrn[chNum];
                }
                else if (data[_chPtr[chNum]] >= 0x82 && data[_chPtr[chNum]] <= 0x8F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0x81);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _cOrn[chNum];
                    _chPtr[chNum]++;
                    // _vtm.Patterns[PatNum].Items[LnNum].Envelope := WordPtr(@PT1.Index[ChPtr[ChNum]]) ^;
                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = BitConverter.ToUInt16(data, _chPtr[chNum]);
                    _chPtr[chNum]++;
                }
                else if (data[_chPtr[chNum]] == 0x90)
                {
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x91 && data[_chPtr[chNum]] <= 0xA0)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 11;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)(data[_chPtr[chNum]] - 0x91);
                }
                else if (data[_chPtr[chNum]] >= 0xA1 && data[_chPtr[chNum]] <= 0xB0)
                {
                    i = data[_chPtr[chNum]] - 0xA1;

                    if (i == 0)
                        i++;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)i;
                }
                else
                {
                    _skip[chNum] = (sbyte)(data[_chPtr[chNum]] - 0xB1);
                }

                _chPtr[chNum]++;
            }
            while (!quit);

            if ((_cOrn[chNum] > 0) && (_orn2Sam[_cOrn[chNum]] == 0))
                _orn2Sam[_cOrn[chNum]] = _cSam[chNum];

            _skipCounter[chNum] = _skip[chNum];
        }
    }
}
