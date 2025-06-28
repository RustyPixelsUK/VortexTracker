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
    public struct FLS
    {
        public ushort PositionsPointer;
        public ushort OrnamentsPointer;
        public ushort SamplesPointer;
        // Delphi: array[1..(65536 - 6) div 6] of record ... 
        // Use 10921 elements (converted to 0-indexed array)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10921)]
        public PatternTriple[] PatternsPointers;
    }

    public class FLS2VTM
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
        private FLS _fls;
        private VTM _vtm;

        public FLS2VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _fls = Helpers.CastToStruct<FLS>(data);
            _vtm = vtm;

            bool result = true;
            bool quit = false;
            int i, j, k, l;

            _vtm.Title = "";
            _vtm.Author = "";
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = data[_fls.PositionsPointer];
            _vtm.Positions.Loop = 0;

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

            while ((_pos < 256) && (data[_pos + _fls.PositionsPointer + 1] != 0))
            {
                j = data[_pos + _fls.PositionsPointer + 1];
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

                    // Move(FLS.FLS_PatternsPointers[j], ChPtr, 6);
                    Helpers.Move(_fls.PatternsPointers[j], _chPtr, 0);
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
                    k = _orn2Sam[i] - 1;
                    j = _fls.SamplesPointer + k * 4;
                    l = 0;

                    if (k >= 0)
                        l = data[j];

                    if (l == 0)
                    {
                        _vtm.Ornaments[i].Loop = 0;
                        _vtm.Ornaments[i].Length = 32;
                    }
                    else
                    {
                        _vtm.Ornaments[i].Loop = l - 1;

                        if (_vtm.Ornaments[i].Loop > 31)
                            _vtm.Ornaments[i].Loop = 31;

                        _vtm.Ornaments[i].Length = l - 1 + data[j + 1];

                        if (_vtm.Ornaments[i].Length > 32)
                            _vtm.Ornaments[i].Length = 32;

                        if (_vtm.Ornaments[i].Length == 0)
                            _vtm.Ornaments[i].Length++;

                        if (_vtm.Ornaments[i].Loop >= _vtm.Ornaments[i].Length)
                            _vtm.Ornaments[i].Loop = _vtm.Ornaments[i].Length - 1;

                        l = _vtm.Ornaments[i].Loop + 1;

                        if (_vtm.Ornaments[i].Length < 32)
                        {
                            _vtm.Ornaments[i].Length += 33 - l;
                            _vtm.Ornaments[i].Loop = 32;
                        }
                    }

                    // j := WordPtr(@FLS.Index[FLS.FLS_OrnamentsPointer + (i - 1) * 2])^;
                    j = BitConverter.ToUInt16(data, _fls.OrnamentsPointer + (i - 1) * 2);

                    for (k = 0; k < 32; k++)
                        _vtm.Ornaments[i].Offsets[k] = (sbyte)data[j + k];

                    for (k = 32; k < _vtm.Ornaments[i].Length; k++)
                        _vtm.Ornaments[i].Offsets[k] = _vtm.Ornaments[i].Offsets[k + l - 33];
                }
            }

            for (i = 1; i < 17; i++)
            {
                if (_isSample[i])
                {
                    _vtm.Samples[i] = new Sample();
                    j = _fls.SamplesPointer + (i - 1) * 4;
                    l = data[j];

                    if (l == 0)
                    {
                        _vtm.Samples[i].Length = 33;
                        _vtm.Samples[i].Loop = 32;
                    }
                    else
                    {
                        _vtm.Samples[i].Loop = (byte)(l - 1);

                        if (_vtm.Samples[i].Loop > 31)
                            _vtm.Samples[i].Loop = 31;

                        _vtm.Samples[i].Length = (byte)(l - 1 + data[j + 1]);

                        if (_vtm.Samples[i].Length > 32)
                            _vtm.Samples[i].Length = 32;

                        if (_vtm.Samples[i].Length == 0)
                            _vtm.Samples[i].Length++;

                        if (_vtm.Samples[i].Loop >= _vtm.Samples[i].Length)
                            _vtm.Samples[i].Loop = (byte)(_vtm.Samples[j].Length - 1);
 
                        l = _vtm.Samples[i].Loop + 1;
                        
                        if (_vtm.Samples[i].Length < 32)
                        {
                            _vtm.Samples[i].Length += (byte)(33 - l);
                            _vtm.Samples[i].Loop = 32;
                        }
                    }

                    // j := WordPtr(@FLS.Index[j + 2])^;
                    j = BitConverter.ToUInt16(data, j + 2);

                    for (k = 0; k < 32; k++)
                    {
                        _vtm.Samples[i].Ticks[k] = new SampleTick();
                        _vtm.Samples[i].Ticks[k].Amplitude = (byte)(data[j + k * 3] & 15);
                        _vtm.Samples[i].Ticks[k].Mixer_Noise = ((sbyte)data[j + k * 3 + 1]) >= 0;
                        
                        if (_vtm.Samples[i].Ticks[k].Mixer_Noise)
                        {
                            _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(data[j + k * 3 + 1] & 0x1F);

                            if ((_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise & 0x10) != 0)
                                _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise | 0xF0);
                        }

                        _vtm.Samples[i].Ticks[k].Mixer_Ton = (data[j + k * 3 + 1] & 64) == 0;
                        _vtm.Samples[i].Ticks[k].AddToTone = (short)(((ushort)data[j + k * 3] & 0xF0) << 4 + data[j + k * 3 + 2]);

                        if ((data[j + k * 3 + 1] & 32) == 0)
                            _vtm.Samples[i].Ticks[k].AddToTone = (short)-_vtm.Samples[i].Ticks[k].AddToTone;

                        _vtm.Samples[i].Ticks[k].Envelope_Enabled = true;
                    }

                    if (l == 0)
                        _vtm.Samples[i].Ticks[32] = new SampleTick();
                    else
                    {
                        for (k = 32; k < _vtm.Samples[i].Length; k++)
                            _vtm.Samples[i].Ticks[k] = _vtm.Samples[i].Ticks[k + l - 33];
                    }
                }
            }

            return result;
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool quit;
            int i;
            quit = false;

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
                else if (data[_chPtr[chNum]] == 0x70)
                {
                    _cOrn[chNum] = 0;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = 0;
                }
                else if (data[_chPtr[chNum]] >= 0x71 && data[_chPtr[chNum]] <= 0x7F)
                {
                    i = data[_chPtr[chNum]] - 0x70;
                    _cOrn[chNum] = (byte)i;
                    _isOrnament[i] = true;
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
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x82 && data[_chPtr[chNum]] <= 0x8E)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0x80);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = 0;
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = data[_chPtr[chNum]];
                }
                else
                    _skip[chNum] = (sbyte)(data[_chPtr[chNum]] - 0xA1);

                _chPtr[chNum]++;
            }
            while (!quit);
            
            if ((_cOrn[chNum] > 0) && (_orn2Sam[_cOrn[chNum]] == 0))
                _orn2Sam[_cOrn[chNum]] = _cSam[chNum];
 
            _skipCounter[chNum] = _skip[chNum];
        }
    }
}
