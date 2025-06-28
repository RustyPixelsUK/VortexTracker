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
using static System.Net.WebRequestMethods;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct STP
    {
        public byte Delay;
        public ushort PositionsPointer;
        public ushort PatternsPointer;
        public ushort OrnamentsPointer;
        public ushort SamplesPointer;
        public byte Init_Id;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct STPPat
    {
        public int Numb;
        public int Trans;
    }

    public class STP2VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private sbyte[] _gliss = new sbyte[3];
        private sbyte[] _skip = new sbyte[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private bool[] _isOrnament = new bool[16];
        private bool[] _isSample = new bool[16];
        private STPPat _cPat;
        private int _pos;
        private int _patMax;
        //private string _ksaId2;
        private STPPat[] _pats = new STPPat[VTModule.MaxPatternIndex + 1];
        private STP _stp;
        private VTM _vtm;

        public STP2VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _stp = Helpers.CastToStruct<STP>(data);
            _vtm = vtm;

            bool result = true;
            int i, j, k;

            // Move(STP.Index[10], KsaId2[1], 28);
            string ksaId2 = Helpers.CopyBytesToString(data, 10, 28);

            if (ksaId2 == VTModule.KsaId)
            {
                // Move(STP.Index[38], _vtm.Title[1], 25);
                _vtm.Title = Helpers.CopyBytesToString(data, 38, 25).TrimEnd();
            }
            else
                _vtm.Title = "";

            _vtm.Author = "";
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = _stp.Delay;
            _vtm.Positions.Loop = data[_stp.PositionsPointer + 1];

            for (i = 0; i < 16; i++)
                _vtm.Ornaments[i] = null;
            
            for (i = 1; i < 16; i++)
                _isOrnament[i] = false;

            for (i = 1; i < 32; i++)
                _vtm.Samples[i] = null;

            for (i = 1; i < 16; i++)
                _isSample[i] = false;

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                _vtm.Patterns[i] = null;

            for (k = 0; k < 3; k++)
                _gliss[k] = 0;

            _patMax = 0;
            _pos = 0;

            while (_pos < data[_stp.PositionsPointer])
            {
                _cPat.Numb = data[_stp.PositionsPointer + 2 + _pos * 2] / 6;
                _cPat.Trans = data[_stp.PositionsPointer + 3 + _pos * 2];

                j = _patMax;

                for (i = 0; i < _patMax; i++)
                {
                    if ((_pats[i].Numb == _cPat.Numb) &&
                        (_pats[i].Trans == _cPat.Trans))
                    {
                        j = i;
                        break;
                    }
                }

                if (j == _patMax)
                {
                    _patMax++;
                    _pats[j] = _cPat;
                }

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

                    // Move(STP.Index[STP.STP_PatternsPointer + CPat.Numb * 6], ChPtr, 6);
                    Helpers.Move(data, _stp.PatternsPointer + _cPat.Numb * 6, _chPtr, 0, 6);

                    i = 0;
                    bool quit = false;

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
                        i++;
                    }

                    _vtm.Patterns[j].Length = i;
                }
                else
                {
                    for (k = 0; k < 3; k++)
                        _gliss[k] = 0;
                }
            }

            _vtm.Positions.Length = _pos;
            
            if (_vtm.Positions.Loop >= _pos)
                _vtm.Positions.Loop = _pos - 1;

            for (i = 1; i < 16; i++)
            {
                if (_isOrnament[i])
                {
                    _vtm.Ornaments[i] = new Ornament();
                    // j := WordPtr(@STP.Index[STP.STP_OrnamentsPointer + i * 2])^;
                    j = BitConverter.ToUInt16(data, _stp.OrnamentsPointer + i * 2);
                    _vtm.Ornaments[i].Loop = data[j];
                    j++;
                    _vtm.Ornaments[i].Length = data[j];

                    for (k = 0; k < _vtm.Ornaments[i].Length; k++)
                    {
                        j++;
                        _vtm.Ornaments[i].Offsets[k] = (sbyte)data[j];
                    }
                }
            }

            for (i = 1; i < 16; i++)
            {
                if (_isSample[i])
                {
                    // j := WordPtr(@STP.Index[STP.STP_SamplesPointer + (i - 1) * 2])^;
                    j = BitConverter.ToUInt16(data, _stp.SamplesPointer + (i - 1) * 2);

                    //Console.WriteLine($"data[j]: {data[j]} data[j + 1]: {data[j + 1]} MaxSamLen: {MaxSamLen}");
                    // data[j]: 255 data[j + 1]: 32 MaxSamLen: 64

                    //if ((data[j] > TVTM.MaxSamLen - 1) || (data[j + 1] > TVTM.MaxSamLen))
                    //    continue;

                    if ((data[j] != 255 && data[j] > VTModule.MaxSampleLength - 1) || data[j + 1] > VTModule.MaxSampleLength)
                        continue;

                    _vtm.Samples[i] = new Sample();
                    _vtm.Samples[i].Loop = data[j];
                    j++;
                    _vtm.Samples[i].Length = data[j];
                    j++;

                    for (k = 0; k < _vtm.Samples[i].Length; k++)
                    {
                        _vtm.Samples[i].Ticks[k] = new SampleTick();
                        // _vtm.Samples[i].Items[k].Add_to_Ton := WordPtr(@STP.Index[j + k * 4 + 2])^;
                        _vtm.Samples[i].Ticks[k].AddToTone = (short)BitConverter.ToUInt16(data, j + k * 4 + 2);
                        _vtm.Samples[i].Ticks[k].Amplitude = (byte)(data[j + k * 4] & 15);
                        _vtm.Samples[i].Ticks[k].Envelope_Enabled = (data[j + k * 4 + 1] & 1) != 0;
                        _vtm.Samples[i].Ticks[k].Mixer_Ton = (data[j + k * 4] & 0x10) == 0;
                        _vtm.Samples[i].Ticks[k].Mixer_Noise = (data[j + k * 4] & 0x80) == 0;

                        if (_vtm.Samples[i].Ticks[k].Envelope_Enabled ||
                            _vtm.Samples[i].Ticks[k].Mixer_Noise)
                        {
                            _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)((data[j + k * 4 + 1] >> 1) & 31);
                            
                            if ((_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise & 0x10) != 0)
                                _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise | 0xF0);
                        }
                    }

                    if ((sbyte)_vtm.Samples[i].Loop < 0)
                    {
                        _vtm.Samples[i].Loop = _vtm.Samples[i].Length;
                        _vtm.Samples[i].Length++;
                        _vtm.Samples[i].Ticks[_vtm.Samples[i].Loop] = new SampleTick();
                    }
                }
            }

            return result;
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool quit = false;
            bool stopGliss = false;
            byte nt;
            int i;

            do
            {
                if (data[_chPtr[chNum]] >= 0x1 && data[_chPtr[chNum]] <= 0x60)
                {
                    nt = (byte)(data[_chPtr[chNum]] - 1 + _cPat.Trans);

                    if (nt > 0x5F)
                        nt = 0x5F;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)nt;
                    
                    if (!stopGliss)
                    {
                        i = _gliss[chNum];

                        if ((i != 0) && (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number == 0))
                        {
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                            
                            if (i > 0)
                            {
                                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)i;
                            }
                            else
                            {
                                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 2;
                                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)-i;
                            }
                        }
                    }
                    else
                    {
                        stopGliss = false;
                        _gliss[chNum] = 0;
                    }

                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x61 && data[_chPtr[chNum]] <= 0x6F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)(data[_chPtr[chNum]] - 0x60);
                    _isSample[_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample] = true;
                }
                else if (data[_chPtr[chNum]] >= 0x70 && data[_chPtr[chNum]] <= 0x7F)
                {
                    stopGliss = true;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)(data[_chPtr[chNum]] - 0x70);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _isOrnament[_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament] = true;
                }
                else if (data[_chPtr[chNum]] >= 0x80 && data[_chPtr[chNum]] <= 0xBF)
                {
                    _skip[chNum] = (sbyte)(data[_chPtr[chNum]] - 0x80);
                }
                else if (data[_chPtr[chNum]] >= 0xC0 && data[_chPtr[chNum]] <= 0xCF)
                {
                    stopGliss = true;

                    if (data[_chPtr[chNum]] != 0xC0)
                    {
                        if (data[_chPtr[chNum]] != 0xCF)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0xC0);
                        else
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 7;

                        _chPtr[chNum]++;
                        _vtm.Patterns[patNum].Lines[lnNum].Envelope = data[_chPtr[chNum]];
                    }
                    else
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = 0;
                }
                else if (data[_chPtr[chNum]] >= 0xD0 && data[_chPtr[chNum]] <= 0xDF)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0xE0 && data[_chPtr[chNum]] <= 0xEF)
                    quit = true;
                else if (data[_chPtr[chNum]] == 0xF0)
                {
                    _chPtr[chNum]++;

                    i = ((sbyte)data[_chPtr[chNum]]);

                    if (i == 0)
                        stopGliss = true;
                    else
                    {
                        _gliss[chNum] = (sbyte)i;

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                        
                        if (i >= 0)
                        {
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)i;
                        }
                        else
                        {
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 2;
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)-i;
                        }
                    }
                }
                else if (data[_chPtr[chNum]] >= 0xF1 && data[_chPtr[chNum]] <= 0xFF)
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)(256 - data[_chPtr[chNum]]);

                _chPtr[chNum]++;
            }
            while (!quit);

            if (stopGliss && (_gliss[chNum] != 0))
            {
                _gliss[chNum] = 0;

                if (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number == 0)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 0;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = 0;
                }
            }

            _skipCounter[chNum] = _skip[chNum];
        }
    }
}
