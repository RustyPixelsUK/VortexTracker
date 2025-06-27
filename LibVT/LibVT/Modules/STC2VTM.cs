using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ST
    {
        public byte Delay;
        public ushort PositionsPointer;
        public ushort OrnamentsPointer;
        public ushort PatternsPointer;
        // 18 bytes (0..17)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] Name;
        public ushort Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STCPat
    {
        public int Numb;
        public int Trans;
    }

    public class STC2VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private sbyte[] _skip = new sbyte[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private bool[] _isOrnament = new bool[16];
        private bool[] _isSample = new bool[17];
        private STCPat _cPat;
        private byte[] _orn2Sam = new byte[16];
        private byte[] _cSam = new byte[3];
        private byte[] _cOrn = new byte[3];
        private int _pos;
        private int _patMax;
        private STCPat[] _pats = new STCPat[VTModule.MaxPatternIndex + 1];
        private ST _stc;
        private VTM _vtm;

        public STC2VTM()
        {
        }

        public bool Initialize(byte[] data, int fSize, VTM vtm)
        {
            _stc = Helpers.CastToStruct<ST>(data);
            _vtm = vtm;
            bool result = true;
            bool quit = false;
            int i, j, k, l, n;

            // Move(STC.ST_Name, _vtm.Title[1], 18);
            _vtm.Title = Helpers.CopyBytesToString(_stc.Name, 0, 18);
            if ((_vtm.Title == "SONG BY ST COMPILE") ||
                (_vtm.Title == "SONG BY MB COMPILE") ||
                (_vtm.Title == "SONG BY ST-COMPILE") ||
                (_vtm.Title == "SOUND TRACKER v1.1") ||
                (_vtm.Title == "S.T.FULL EDITION  ") ||
                (_vtm.Title == "S.T.FULL EDITION ") ||
                (_vtm.Title == "SOUND TRACKER v1.3"))
            {
                _vtm.Title = "";
            }
            else
            {
                if (_stc.Size != fSize)
                {
                    if ((_stc.Size & 255) >= 32 && (_stc.Size & 255) <= 127)
                    {
                        _vtm.Title = _vtm.Title + ((char)_stc.Size & 255);
                        if ((_stc.Size >> 8) >= 32 && (_stc.Size >> 8) <= 127)
                        {
                            _vtm.Title = _vtm.Title + ((char)_stc.Size >> 8);
                        }
                    }
                }

                _vtm.Title = _vtm.Title.TrimEnd();
            }

            _vtm.Author = "";
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = _stc.Delay;
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

            _patMax = 0;
            _pos = 0;

            while (_pos <= data[_stc.PositionsPointer])
            {
                _cPat.Numb = data[_stc.PositionsPointer + 1 + _pos * 2];
                _cPat.Trans = data[_stc.PositionsPointer + 2 + _pos * 2];
                
                j = _patMax;

                for (i = 0; i < _patMax; i++)
                {
                    if ((_pats[i].Numb == _cPat.Numb) && (_pats[i].Trans == _cPat.Trans))
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
                    k = 0;
                    _vtm.Patterns[j] = new Pattern();

                    for (k = 0; k < 3; k++)
                    {
                        _skipCounter[k] = 0;
                        _skip[k] = 0;
                    }

                    k = 0;
                    n = _pats[j].Numb;

                    while (data[_stc.PatternsPointer + k * 7] != n)
                    {
                        k++;

                        if (_stc.PatternsPointer + k * 7 > data.Length)
                            return result;
                    }
                    // Move(STC.Index[STC.ST_PatternsPointer + k * 7 + 1], ChPtr, 6);
                    Helpers.Move(data, _stc.PatternsPointer + k * 7 + 1, _chPtr, 0, 6);

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

                                PatternInterpreter(data, j, i, k, fSize);
                            }
                        }

                        i++;
                    }

                    _vtm.Patterns[j].Length = i;
                }
            }

            _vtm.Positions.Length = _pos;
            
            for (i = 0; i < 16; i++)
            {
                j = data[_stc.OrnamentsPointer + 0x21 * i];

                if ((j > 0) && (j <= 15) && _isOrnament[j])
                {
                    _isOrnament[j] = false;
                    _vtm.Ornaments[j] = new Ornament();

                    k = _orn2Sam[j] - 1;
                    l = 0;
                    n = 0;

                    if (k >= 0)
                    {
                        while (data[0x1B + 0x63 * n] != k)
                        {
                            n++;

                            if (0x1B + 0x63 * n > data.Length)
                                return result;
                        }

                        l = data[0x1B + 0x63 * n + 0x61];
                    }

                    if (l == 0)
                    {
                        _vtm.Ornaments[j].Loop = 0;
                        _vtm.Ornaments[j].Length = 32;
                    }
                    else
                    {
                        _vtm.Ornaments[j].Loop = l - 1;

                        if (_vtm.Ornaments[j].Loop > 31)
                            _vtm.Ornaments[j].Loop = 31;

                        _vtm.Ornaments[j].Length = l + data[0x1B + 0x63 * n + 0x62];

                        if (_vtm.Ornaments[j].Length > 32)
                            _vtm.Ornaments[j].Length = 32;

                        if (_vtm.Ornaments[j].Length == 0)
                            _vtm.Ornaments[j].Length++;

                        if (_vtm.Ornaments[j].Loop >= _vtm.Ornaments[j].Length)
                            _vtm.Ornaments[j].Loop = _vtm.Ornaments[j].Length - 1;

                        l = _vtm.Ornaments[j].Loop + 1;

                        if (_vtm.Ornaments[j].Length < 32)
                        {
                            _vtm.Ornaments[j].Length += 33 - l;
                            _vtm.Ornaments[j].Loop = 32;
                        }
                    }
                    for (k = 0; k < 32; k++)
                        _vtm.Ornaments[j].Offsets[k] = (sbyte)data[_stc.OrnamentsPointer + 0x21 * i + 1 + k];

                    for (k = 32; k < _vtm.Ornaments[j].Length; k++)
                        _vtm.Ornaments[j].Offsets[k] = _vtm.Ornaments[j].Offsets[k + l - 33];
                }
            }

            for (i = 0; i < 16; i++)
            {
                j = data[0x1B + 0x63 * i] + 1;

                if ((j > 0) && (j <= 16) && _isSample[j])
                {
                    _isSample[j] = false;
                    _vtm.Samples[j] = new Sample();
                    l = data[0x1B + 0x63 * i + 0x61];

                    if (l == 0)
                    {
                        _vtm.Samples[j].Length = 33;
                        _vtm.Samples[j].Loop = 32;
                    }
                    else
                    {
                        _vtm.Samples[j].Loop = (byte)(l - 1);

                        if (_vtm.Samples[j].Loop > 31)
                            _vtm.Samples[j].Loop = 31;

                        _vtm.Samples[j].Length = (byte)(l + data[0x1B + 0x63 * i + 0x62]);

                        if (_vtm.Samples[j].Length > 32)
                            _vtm.Samples[j].Length = 32;

                        if (_vtm.Samples[j].Length == 0)
                            _vtm.Samples[j].Length++;

                        if (_vtm.Samples[j].Loop >= _vtm.Samples[j].Length)
                            _vtm.Samples[j].Loop = (byte)(_vtm.Samples[j].Length - 1);

                        l = _vtm.Samples[j].Loop + 1;

                        if (_vtm.Samples[j].Length < 32)
                        {
                            _vtm.Samples[j].Length += (byte)(33 - l);
                            _vtm.Samples[j].Loop = 32;
                        }
                    }
                    for (k = 0; k < 32; k++)
                    {
                        _vtm.Samples[j].Ticks[k] = new SampleTick();
                        _vtm.Samples[j].Ticks[k].Mixer_Noise = (data[0x1B + 0x63 * i + 1 + k * 3 + 1] & 128) == 0;
                        
                        if (_vtm.Samples[j].Ticks[k].Mixer_Noise)
                            _vtm.Samples[j].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(data[0x1B + 0x63 * i + 1 + k * 3 + 1] & 0x1F);

                        if ((_vtm.Samples[j].Ticks[k].Add_to_Envelope_or_Noise & 0x10) != 0)
                            _vtm.Samples[j].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[j].Ticks[k].Add_to_Envelope_or_Noise | 0xF0);

                        _vtm.Samples[j].Ticks[k].Mixer_Ton = (data[0x1B + 0x63 * i + 1 + k * 3 + 1] & 64) == 0;
                        _vtm.Samples[j].Ticks[k].Amplitude = (byte)(data[0x1B + 0x63 * i + 1 + k * 3] & 15);
                        _vtm.Samples[j].Ticks[k].AddToTone =(short)(data[0x1B + 0x63 * i + 1 + k * 3 + 2] + (data[0x1B + 0x63 * i + 1 + k * 3] & 0xF0) << 4);

                        if ((data[0x1B + 0x63 * i + 1 + k * 3 + 1] & 0x20) == 0)
                            _vtm.Samples[j].Ticks[k].AddToTone = (short)-_vtm.Samples[j].Ticks[k].AddToTone;

                        _vtm.Samples[j].Ticks[k].Envelope_Enabled = true;
                    }

                    if (l == 0)
                        _vtm.Samples[j].Ticks[32] = new SampleTick();
                    else
                    {
                        for (k = 32; k < _vtm.Samples[j].Length; k++)
                            _vtm.Samples[j].Ticks[k] = _vtm.Samples[j].Ticks[k + l - 33];
                    }
                }
            }

            return result;
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum, int fSize)
        {
            byte nt;

            do
            {
                if (data[_chPtr[chNum]] >= 0x00 && data[_chPtr[chNum]] <= 0x5F)
                {
                    nt = (byte)(data[_chPtr[chNum]] + _cPat.Trans);

                    if (nt > 0x5F)
                        nt = 0x5F;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)nt;
                    _chPtr[chNum]++;
                    break;
                }
                else if (data[_chPtr[chNum]] >= 0x60 && data[_chPtr[chNum]] <= 0x6F)
                {
                    _cSam[chNum] = (byte)(data[_chPtr[chNum]] - 0x5F);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = _cSam[chNum];
                    _isSample[_cSam[chNum]] = true;
                }
                else if (data[_chPtr[chNum]] >= 0x70 && data[_chPtr[chNum]] <= 0x7F)
                {
                    _cOrn[chNum] = (byte)(data[_chPtr[chNum]] - 0x70);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _cOrn[chNum];
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _isOrnament[_cOrn[chNum]] = true;
                }
                else if (data[_chPtr[chNum]] == 0x80)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    _chPtr[chNum]++;
                    break;
                }
                else if (data[_chPtr[chNum]] == 0x81)
                {
                    _chPtr[chNum]++;
                    break;
                }
                else if (data[_chPtr[chNum]] == 0x82)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = 0;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                }
                else if (data[_chPtr[chNum]] >= 0x83 && data[_chPtr[chNum]] <= 0x8E)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0x80);
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = data[_chPtr[chNum]];
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = 0;
                }
                else
                    _skip[chNum] = (sbyte)(data[_chPtr[chNum]] - 0xA1);

                _chPtr[chNum]++;
            }
            while (true);

            if ((_cOrn[chNum] > 0) && (_orn2Sam[_cOrn[chNum]] == 0))
            {
                _orn2Sam[_cOrn[chNum]] = _cSam[chNum];
            }

            _skipCounter[chNum] = _skip[chNum];
        }
    }
}
