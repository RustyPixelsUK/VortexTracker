using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASC0
    {
        public byte Delay;
        public ushort PatternsPointers;
        public ushort SamplesPointers;
        public ushort OrnamentsPointers;
        public byte NumberOfPositions;
        // 0..(65535 - 8): 65528 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65528)]
        public byte[] Positions;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ASC1
    {
        public byte Delay;
        public byte LoopingPosition;
        public ushort PatternsPointers;
        public ushort SamplesPointers;
        public ushort OrnamentsPointers;
        public byte NumberOfPositions;
        // 0..(65535 - 9): 65527 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65527)]
        public byte[] Positions;
    }

    public class ASC2VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private bool[] _envEn = new bool[3];
        private sbyte[] _skip = new sbyte[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private sbyte[] _tsCnt = new sbyte[3];
        private int _envT;
        private int _cDelay;
        private int _nOrns;
        private int _nSams;
        private int _ns;
        private int[] _orns = new int[32];
        private int[] _sams = new int[32];
        private sbyte[] _prevOrn = new sbyte[3];
        private sbyte[] _prevNote = new sbyte[3];
        private sbyte[] _prevVol = new sbyte[3];
        private sbyte[] _vol = new sbyte[3];
        private sbyte[] _patVol = new sbyte[3];
        private int[] _volumeCounter = new int[3];
        private int[] _vCDop = new int[3];
        private short[] _ts = new short[3];
        private short[] _tsAdd = new short[3];
        private int _pos;
        private VTM _vtm;
        private ASC1 _asc;

        public bool Initialize(byte[] data, VTM vtm)
        {
            _asc = Helpers.CastToStruct<ASC1>(data);
            _vtm = vtm;

            bool result = true;
            bool quit = false;
            int i, j, k, l, jl, nb, ns, n, tmp, zo;

            if (_asc.PatternsPointers - _asc.NumberOfPositions == 72)
            {
                // Move(ASC.Index[ASC.ASC1_PatternsPointers - 44], _vtm.Title[1], 20);
                // Move(ASC.Index[ASC.ASC1_PatternsPointers - 20], _vtm.Author[1], 20);
                
                _vtm.Title = Helpers.CopyCharsToString(data, _asc.PatternsPointers - 44, 20).TrimEnd();
                _vtm.Author = Helpers.CopyCharsToString(data, _asc.PatternsPointers - 20, 20).TrimEnd();
            }
            else
            {
                _vtm.Title = "";
                _vtm.Author = "";
            }
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = _asc.Delay;
            _cDelay = _asc.Delay;
            _vtm.Positions.Loop = _asc.LoopingPosition;

            for (i = 0; i < 256; i++)
                _vtm.Positions.Value[i] = 0;

            for (i = 0; i < 16; i++)
                _vtm.Ornaments[i] = null;

            for (i = 1; i < 32; i++)
                _vtm.Samples[i] = null;

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                _vtm.Patterns[i] = null;

            for (i = 0; i < 32; i++)
                _orns[i] = -1;

            for (i = 0; i < 32; i++)
                _sams[i] = -1;

            for (k = 0; k < 3; k++)
            {
                _tsCnt[k] = 0;
                _ts[k] = 0;
                _volumeCounter[k] = 0;
                _prevNote[k] = 0;
                _vol[k] = 0;
                _prevVol[k] = 0;
            }

            _envT = 0;
            _nOrns = 0;
            _nSams = 0;
            _pos = 0;

            while (_pos < _asc.NumberOfPositions)
            {
                j = data[_pos + 9];
                _vtm.Positions.Value[_pos] = j;
                _pos++;

                if (_vtm.Patterns[j] == null)
                {
                    _vtm.Patterns[j] = new Pattern();

                    for (k = 0; k < 3; k++)
                    {
                        _envEn[k] = false;
                        _patVol[k] = 0;
                        _prevOrn[k] = 0;
                        _skipCounter[k] = 0;
                        _skip[k] = 0;
                    }
                    // Move(WordPtr(@ASC.Index[ASC.ASC1_PatternsPointers + 6 * j]) ^, ChPtr, 6);
                    // Inc(ChPtr[0], ASC.ASC1_PatternsPointers);
                    // Inc(ChPtr[1], ASC.ASC1_PatternsPointers);
                    // Inc(ChPtr[2], ASC.ASC1_PatternsPointers);
                    Helpers.Move(data, _asc.PatternsPointers + 6 * j, _chPtr, 6);
                    _chPtr[0] += _asc.PatternsPointers;
                    _chPtr[1] += _asc.PatternsPointers;
                    _chPtr[2] += _asc.PatternsPointers;

                    ns = 0;
                    i = 0;
                    quit = false;

                    while ((i < VTModule.MaxPatternLength) && !quit)
                    {
                        if (_cDelay == 0)
                            _cDelay = 256;

                        for (k = 0; k < 3; k++)
                        {
                            if (_volumeCounter[k] != 0)
                            {
                                if (_volumeCounter[k] > 0)
                                {
                                    n = (_cDelay + _vCDop[k]) / _volumeCounter[k];
                                    _vCDop[k] = (_cDelay + _vCDop[k]) % _volumeCounter[k];
                                    n += _prevVol[k];
                                    
                                    if (n > 15)
                                        n = 15;

                                    if (n != _prevVol[k])
                                    {
                                        _vtm.Patterns[j].Lines[i].Channel[k].Volume = (sbyte)n;
                                        _prevVol[k] = (sbyte)n;
                                    }
                                }
                                else
                                {
                                    n = (_cDelay + _vCDop[k]) / -_volumeCounter[k];
                                    _vCDop[k] = (_cDelay + _vCDop[k]) % -_volumeCounter[k];
                                    n = _prevVol[k] - n;

                                    if (n < 0)
                                        n = 0;

                                    if (n != _prevVol[k])
                                    {
                                        _vtm.Patterns[j].Lines[i].Channel[k].Volume = (sbyte)n;
                                        
                                        if (n != 0)
                                            _prevVol[k] = (sbyte)n;
                                    }
                                }
                            }
                        }
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

                            for (l = 0; l < _cDelay; l++)
                            {
                                if (_tsCnt[k] != 0)
                                {
                                    if (_tsCnt[k] > 0)
                                        _tsCnt[k]--;

                                    _ts[k] -= _tsAdd[k];
                                }
                            }
                        }

                        _vtm.Patterns[j].Lines[i].Noise = (byte)ns;
                        i++;
                    }

                    _vtm.Patterns[j].Length = i;
                }
            }

            _vtm.Positions.Length = _pos;

            if (_vtm.Positions.Loop >= _pos)
                _vtm.Positions.Loop = _pos - 1;

            zo = 0;

            for (i = 0; i < 32; i++)
            {
                l = _orns[i];
                if ((l > 0) && (l <= _vtm.Ornaments.Length))
                {
                    // j := WordPtr(@ASC.Index[i * 2 + ASC.ASC1_OrnamentsPointers]) ^ + ASC.ASC1_OrnamentsPointers;
                    j = BitConverter.ToUInt16(data, i * 2 + _asc.OrnamentsPointers) + _asc.OrnamentsPointers;
                    jl = j;
                    k = nb = n = 0;

                    do
                    {
                        j = jl;
                        do
                        {
                            tmp = n;
                            n += ((sbyte)data[j + 1]);

                            if ((n < -0x55) || (n > 0x55))
                                break;

                            if (((sbyte)data[j]) < 0)
                            {
                                nb = tmp;
                                jl = j;
                            }

                            k++;
                            j += 2;

                            if (k == VTModule.MaxOrnamentLength)
                                break;
                        }
                        while ((data[j - 2] & 64) == 0);
                    }
                    while ((k != VTModule.MaxOrnamentLength) && (n != nb) && (n >= -0x55) && (n <= 0x55));

                    if ((k == 1) && (n == 0))
                    {
                        zo = l;
                        _orns[i] = -1;
                        break;
                    }
                }
            }

            if (zo == 0)
            {
                if (_nOrns == 16)
                {
                    for (i = 0; i < 32; i++)
                    {
                        if (_orns[i] == 16)
                        {
                            _orns[i] = -1;
                            break;
                        }
                    }

                    for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                    {
                        if (_vtm.Patterns[i] != null)
                        {
                            for (j = 0; j < _vtm.Patterns[i].Length; j++)
                            {
                                for (k = 0; k < 3; k++)
                                {
                                    if (_vtm.Patterns[i].Lines[j].Channel[k].Ornament == 16)
                                        _vtm.Patterns[i].Lines[j].Channel[k].Ornament = 0;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (i = 0; i < 32; i++)
                {
                    if (_orns[i] > zo)
                        _orns[i]--;
                }

                for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                {
                    if (_vtm.Patterns[i] != null)
                    {
                        for (j = 0; j < _vtm.Patterns[i].Length; j++)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                if (_vtm.Patterns[i].Lines[j].Channel[k].Ornament > zo)
                                    _vtm.Patterns[i].Lines[j].Channel[k].Ornament--;
                                else if (_vtm.Patterns[i].Lines[j].Channel[k].Ornament == zo)
                                    _vtm.Patterns[i].Lines[j].Channel[k].Ornament = 0;
                            }
                        }
                    }
                }
            }

            for (i = 0; i < 32; i++)
            {
                l = _orns[i];

                if ((l > 0) && (l <= _vtm.Ornaments.Length))
                {
                    _vtm.Ornaments[l] = new Ornament();
                    _vtm.Ornaments[l].Loop = 0;
                    // j := WordPtr(@ASC.Index[i * 2 + ASC.ASC1_OrnamentsPointers]) ^ + ASC.ASC1_OrnamentsPointers;
                    j = BitConverter.ToUInt16(data, i * 2 + _asc.OrnamentsPointers) + _asc.OrnamentsPointers;
                    jl = j;
                    k = nb = n = 0;

                    do
                    {
                        j = jl;

                        do
                        {
                            tmp = n;
                            n += (sbyte)data[j + 1];

                            if ((n < -0x55) || (n > 0x55))
                                break;

                            if ((sbyte)data[j] < 0)
                            {
                                _vtm.Ornaments[l].Loop = k;
                                nb = tmp;
                                jl = j;
                            }

                            k++;

                            _vtm.Ornaments[l].Offsets[k - 1] = (sbyte)n;
                            j += 2;

                            if (k == VTModule.MaxOrnamentLength)
                                break;

                        }
                        while ((data[j - 2] & 64) == 0);
                    }
                    while ((k != VTModule.MaxOrnamentLength) && (n != nb) && (n >= -0x55) && (n <= 0x55));

                    _vtm.Ornaments[l].Length = k;
                }
            }

            for (i = 0; i < 32; i++)
            {
                l = _sams[i];

                if ((l > 0) && (l <= _vtm.Samples.Length))
                {
                    _vtm.Samples[l] = new Sample();
                    _vtm.Samples[l].Loop = 0;
                    // j := WordPtr(@ASC.Index[i * 2 + ASC.ASC1_SamplesPointers])^ + ASC.ASC1_SamplesPointers;
                    j = BitConverter.ToUInt16(data, i * 2 + _asc.SamplesPointers) + _asc.SamplesPointers;
                    k = 0;

                    do
                    {
                        if (((sbyte)data[j]) < 0)
                            _vtm.Samples[l].Loop = (byte)k;
 
                        _vtm.Samples[l].Ticks[k] = new SampleTick();
                        _vtm.Samples[l].Ticks[k].Ton_Accumulation = true;
                        _vtm.Samples[l].Ticks[k].AddToTone = ((sbyte)data[j + 1]);
                        _vtm.Samples[l].Ticks[k].Mixer_Ton = (data[j + 2] & 1) == 0;
                        _vtm.Samples[l].Ticks[k].Mixer_Noise = (data[j + 2] & 8) == 0;
                        _vtm.Samples[l].Ticks[k].Envelope_Enabled = (data[j + 2] & 6) == 2;
                        
                        if ((data[j + 2] & 6) == 4)
                        {
                            _vtm.Samples[l].Ticks[k].Amplitude_Sliding = true;
                            _vtm.Samples[l].Ticks[k].Amplitude_Slide_Up = false;
                        }
                        else if ((data[j + 2] & 6) == 6)
                        {
                            _vtm.Samples[l].Ticks[k].Amplitude_Sliding = true;
                            _vtm.Samples[l].Ticks[k].Amplitude_Slide_Up = true;
                        }

                        _vtm.Samples[l].Ticks[k].Amplitude = (byte)(data[j + 2] >> 4);
                        _vtm.Samples[l].Ticks[k].Envelope_or_Noise_Accumulation = true;
                        
                        if (_vtm.Samples[l].Ticks[k].Envelope_Enabled || _vtm.Samples[l].Ticks[k].Mixer_Noise)
                            _vtm.Samples[l].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)((data[j] << 3) / 8);

                        k++;
                        j += 3;

                        if (k == VTModule.MaxSampleLength)
                            break;
                    }
                    while ((data[j - 3] & (64 + 32)) == 0);

                    if (((data[j - 3] & (64 + 32)) == 32) && (k < VTModule.MaxSampleLength))
                    {
                        _vtm.Samples[l].Loop = (byte)k;
                        k++;
                        _vtm.Samples[l].Ticks[k - 1] = new SampleTick();
                    }

                    _vtm.Samples[l].Length = (byte)k;
                }
            }

            return result;
        }

        public void PatternInterpreter_CalcSlide(byte[] data, int patNum, int lnNum, int chNum)
        {
            short deltaTon;

            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 3;
            _chPtr[chNum]++;
            
            if (data[_chPtr[chNum] + 1] < 0x56)
                deltaTon = (short)((VTModule.PT3NoteTable_ST[_prevNote[chNum]] - VTModule.PT3NoteTable_ST[data[_chPtr[chNum] + 1]]) * 16);
            else
                deltaTon = 0;

            if (VTModule.FeaturesLevel >= FeaturesLevel.VTII_PT36)
                deltaTon += _ts[chNum];

            _tsAdd[chNum] = (short)(deltaTon / data[_chPtr[chNum]]);
            _ts[chNum] = (short)(deltaTon - deltaTon % data[_chPtr[chNum]]);
            _tsCnt[chNum] = (sbyte)data[_chPtr[chNum]];
            
            deltaTon = (short)(deltaTon / 16);

            if (deltaTon < 0)
                deltaTon = (short)-deltaTon;

            if (deltaTon != 0)
            {
                int i = deltaTon / data[_chPtr[chNum]];

                if (i > 255)
                    i = 255;

                if (i > 0)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)i;
                }
                else
                {
                    i = data[_chPtr[chNum]] / deltaTon;

                    if (i > 15)
                        i = 15;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = (byte)i;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = 1;
                }
            }
            else
            {
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 15;
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = 1;
            }
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool initializationOfSampleDisabled = false;
            int i, a;

            _tsCnt[chNum] = 0;
            _volumeCounter[chNum] = 0;

            do
            {
                if (data[_chPtr[chNum]] >= 0x00 && data[_chPtr[chNum]] <= 0x55)
                {
                    _prevNote[chNum] = (sbyte)data[_chPtr[chNum]];
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = _prevNote[chNum];
                    _chPtr[chNum]++;

                    if (_tsCnt[chNum] <= 0)
                        _ts[chNum] = 0;

                    if (_envEn[chNum])
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)_envT;
                        _vtm.Patterns[patNum].Lines[lnNum].Envelope = data[_chPtr[chNum]];
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)_prevOrn[chNum];
                        _chPtr[chNum]++;
                    }

                    if (!initializationOfSampleDisabled)
                    {
                        if (_vol[chNum] != _prevVol[chNum])
                        {
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = _vol[chNum];
                            _patVol[chNum] = _vol[chNum];
                            _prevVol[chNum] = _vol[chNum];
                        }
                    }
                    break;
                }
                else if (data[_chPtr[chNum]] >= 0x56 && data[_chPtr[chNum]] <= 0x5D)
                {
                    _chPtr[chNum]++;
                    break;
                }
                else if (data[_chPtr[chNum]] == 0x5E)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    _chPtr[chNum]++;
                    break;
                }
                else if (data[_chPtr[chNum]] == 0x5F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    _chPtr[chNum]++;
                    break;
                }
                else if (data[_chPtr[chNum]] >= 0x60 && data[_chPtr[chNum]] <= 0x9F)
                {
                    _skip[chNum] = (sbyte)(data[_chPtr[chNum]] - 0x60);
                }
                else if (data[_chPtr[chNum]] >= 0xA0 && data[_chPtr[chNum]] <= 0xBF)
                {
                    a = data[_chPtr[chNum]] - 0xA0;
                    i = _sams[a];

                    if (i < 0)
                    {
                        if (_nSams < 31)
                        {
                            _nSams++;
                            i = _nSams;
                            _sams[a] = i;
                        }
                    }

                    if (i < 0)
                        i = 0;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)i;
                }
                else if (data[_chPtr[chNum]] >= 0xC0 && data[_chPtr[chNum]] <= 0xDF)
                {
                    a = data[_chPtr[chNum]] - 0xC0;
                    i = _orns[a];

                    if (i < 0)
                    {
                        if (_nOrns < 16)
                        {
                            _nOrns++;
                            i = _nOrns;
                            _orns[a] = i;
                        }
                    }

                    if (i < 0)
                        i = 0;

                    _prevOrn[chNum] = (sbyte)i;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)i;
                    
                    if (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope == 0)
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                }
                else if (data[_chPtr[chNum]] == 0xE0)
                {
                    if (_patVol[chNum] != 15)
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = 15;
                        _patVol[chNum] = 15;
                    }
                    _vol[chNum] = 15;
                    _prevVol[chNum] = 15;
                    _envEn[chNum] = true;
                }
                else if (data[_chPtr[chNum]] >= 0xE1 && data[_chPtr[chNum]] <= 0xEF)
                {
                    i = data[_chPtr[chNum]] - 0xE0;
                    if (_patVol[chNum] != i)
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)i;
                        _patVol[chNum] = (sbyte)i;
                    }
                    _vol[chNum] = (sbyte)i;
                    _prevVol[chNum] = (sbyte)i;
                    if (_envEn[chNum])
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)_prevOrn[chNum];
                        _envEn[chNum] = false;
                    }
                }
                else if (data[_chPtr[chNum]] == 0xF0)
                {
                    _chPtr[chNum]++;
                    _ns = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xF1)
                {
                    initializationOfSampleDisabled = true;
                }
                else if (data[_chPtr[chNum]] == 0xF2)
                {
                    // do nothing
                }
                else if (data[_chPtr[chNum]] == 0xF3)
                {
                    initializationOfSampleDisabled = true;
                }
                else if (data[_chPtr[chNum]] == 0xF4)
                {
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 11;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xF5)
                {
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 2;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _tsAdd[chNum] = (short)(data[_chPtr[chNum]] * 16);
                    _tsCnt[chNum] = -1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xF6)
                {
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _tsAdd[chNum] = (short)(-data[_chPtr[chNum]] * 16);
                    _tsCnt[chNum] = -1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xF7)
                {
                    initializationOfSampleDisabled = true;
                    PatternInterpreter_CalcSlide(data, patNum, lnNum, chNum);
                    break;
                }
                else if (data[_chPtr[chNum]] == 0xF8)
                {
                    _envT = 8;
                }
                else if (data[_chPtr[chNum]] == 0xF9)
                {
                    PatternInterpreter_CalcSlide(data, patNum, lnNum, chNum);
                    break;
                }
                else if (data[_chPtr[chNum]] == 0xFA)
                {
                    _envT = 10;
                }
                else if (data[_chPtr[chNum]] == 0xFB)
                {
                    _chPtr[chNum]++;
                    _volumeCounter[chNum] = data[_chPtr[chNum]];
                    if ((_volumeCounter[chNum] & 32) != 0)
                    {
                        _volumeCounter[chNum] = ((sbyte)((byte)_volumeCounter[chNum]) | (128 + 64));
                    }
                    _vCDop[chNum] = 0;
                }
                else if (data[_chPtr[chNum]] == 0xFC)
                {
                    _envT = 12;
                }
                else if (data[_chPtr[chNum]] == 0xFE)
                {
                    _envT = 14;
                }

                _chPtr[chNum]++;
            }
            while (true);

            _skipCounter[chNum] = _skip[chNum];
        }
    }
}
