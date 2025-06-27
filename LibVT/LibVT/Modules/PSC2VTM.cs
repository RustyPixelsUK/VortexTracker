using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PSC
    {
        // 69 bytes (0..68)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 69)]
        public byte[] MusicName;
        public ushort UnknownPointer;
        public ushort PatternsPointer;
        public byte Delay;
        public ushort OrnamentsPointer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ushort[] SamplesPointers;
    }

    public class PSC2VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private byte[] _skipCounter = new byte[3];
        private int[] _orns = new int[32];
        private int[] _sams = new int[32];
        private int _nOrns;
        private int _nSams;
        private int _nsC;
        private int _nsB;
        private bool[] _envEn = new bool[3];
        private bool[] _envSet = new bool[3];
        private bool[] _ntSet = new bool[3];
        private int _envT;
        private int _envP;
        private int _cDelay;
        private sbyte[] _ornSet = new sbyte[3];
        private sbyte[] _ornUsed = new sbyte[3];
        private sbyte[] _prevVol = new sbyte[3];
        private sbyte[] _prevVol1 = new sbyte[3];
        private sbyte[] _initVol = new sbyte[3];
        private bool[] _ornEnabled = new bool[3];
        private int[] _volumeCounter = new int[3];
        private int[] _vCDop = new int[3];
        private int[] _curSam = new int[3];
        private bool _psc1_00;
        private int _pos;
        private int _patMax;
        private ushort _pp;
        private ushort[,] _pats = new ushort[VTModule.MaxPatternCount, 3];
        private PSC _psc;
        private VTM _vtm;

        public PSC2VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _psc = Helpers.CastToStruct<PSC>(data);
            _vtm = vtm;

            bool result = true;
            int i, j, k, l, jl, nb, n, tmp, zo;

            _psc1_00 = _psc.MusicName[8] >= '0' && _psc.MusicName[8] <= '3';
            // Move(PSC.PSC_MusicName[$19], _vtm.Title[1], 20);
            // Move(PSC.PSC_MusicName[$31], _vtm.Author[1], 20);
            _vtm.Title = Helpers.CopyCharsToString(_psc.MusicName, 0x19, 20).TrimEnd();
            _vtm.Author = Helpers.CopyCharsToString(_psc.MusicName, 0x31, 20).TrimEnd();

            _vtm.Author = _vtm.Author.TrimEnd();
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = _psc.Delay;
            _cDelay = _psc.Delay;
            _vtm.Positions.Loop = 0;

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
                _volumeCounter[k] = 0;
                _prevVol[k] = 15;
                _initVol[k] = 15;
                _curSam[k] = 0;
                _envEn[k] = false;
                _ornUsed[k] = 0;
            }

            _envT = 0;
            _envP = 0;
            _nOrns = 0;
            _nSams = 0;
            _pos = 0;
            _patMax = 0;
            _pp = (ushort)(_psc.PatternsPointer + 1);

            while (_pos < 256)
            {
                int nl = data[_pp];
                _pp++;

                if (nl == 255)
                {
                    // j := WordPtr(@PSC.Index[pp])^;
                    j = BitConverter.ToUInt16(data, _pp);
                    i = (j - _psc.PatternsPointer) / 8;

                    if (i <= _pos)
                        _vtm.Positions.Loop = i;

                    break;
                }

                if (nl > VTModule.MaxPatternLength)
                    nl = VTModule.MaxPatternLength;

                // Move(PSC.Index[pp], ChPtr, 6);
                Helpers.Move(data, _pp, _chPtr, 0, 6);

                _pp += 7;
                j = _patMax;

                if (_patMax > _pats.Length)
                    return result;

                for (i = 0; i < _patMax; i++)
                {
                    if (_pats[i, 0] == _chPtr[0] && _pats[i, 1] == _chPtr[1] && _pats[i, 2] == _chPtr[2])
                    {
                        j = i;
                        break;
                    }
                }

                if (j == _patMax)
                {
                    _patMax++;
                    _pats[j, 0] = _chPtr[0];
                    _pats[j, 1] = _chPtr[1];
                    _pats[j, 2] = _chPtr[2];
                }

                _vtm.Positions.Value[_pos] = j;
                _pos++;

                if (_vtm.Patterns[j] == null)
                {
                    _vtm.Patterns[j] = new Pattern();
                    _nsC = 0;
                    _nsB = 0;

                    for (k = 0; k < 3; k++)
                    {
                        _prevVol1[k] = _prevVol[k];
                        _prevVol[k] = 0;
                        _skipCounter[k] = 1;
                        _ornEnabled[k] = false;
                        _ornSet[k] = -1;
                    }

                    _vtm.Patterns[j].Length = nl;
                    i = 0;

                    while (i < nl)
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
                                    
                                    if (_prevVol[k] == 0)
                                        n += _prevVol1[k];
                                    else
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
                                    
                                    if (_prevVol[k] == 0)
                                        n = _prevVol1[k] - n;
                                    else
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
                            _ntSet[k] = false;
                            _skipCounter[k]--;

                            if (_skipCounter[k] == 0)
                                PatternInterpreter(data, j, i, k);
                        }
                        for (k = 0; k < 3; k++)
                        {
                            if (_envEn[k])
                            {
                                if ((_vtm.Patterns[j].Lines[i].Channel[k].Envelope == 15) || !_envSet[k])
                                {
                                    _vtm.Patterns[j].Lines[i].Channel[k].Envelope = (byte)_envT;
                                    _vtm.Patterns[j].Lines[i].Envelope = (ushort)_envP;
                                    _vtm.Patterns[j].Lines[i].Channel[k].Ornament = (byte)_ornUsed[k];
                                    _envSet[k] = true;
                                }
                            }
                        }

                        for (k = 2; k >= 0; k--)
                        {
                            if (_ntSet[k] && IsNsInSam(data, _curSam[k]))
                            {
                                _nsC = 0;
                                break;
                            }
                        }

                        _nsC = (_nsC + _nsB) & 0x1F;
                        _vtm.Patterns[j].Lines[i].Noise = (byte)_nsC;
                        i++;
                    }
                }
            }

            _vtm.Positions.Length = _pos;
            zo = 0;

            for (i = 0; i < 32; i++)
            {
                l = _orns[i];

                if ((l > 0) && (l <= _vtm.Ornaments.Length))
                {
                    // j := WordPtr(@PSC.Index[PSC.PSC_OrnamentsPointer + i * 2])^;
                    j = BitConverter.ToUInt16(data, _psc.OrnamentsPointer + i * 2);

                    if (!_psc1_00)
                        j += _psc.OrnamentsPointer;

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

                            if (((sbyte)data[j]) >= 0)
                            {
                                nb = tmp;
                                jl = j;
                            }

                            k++;
                            j += 2;

                            if (k == VTModule.MaxOrnamentLength)
                            {
                                break;
                            }
                        }
                        while (((data[j - 2] & (64 + 32)) != 0) && ((data[j - 2] & (64 + 32)) != 32) && ((data[j - 2] & (64 + 32)) != 64));

                        if ((data[j - 2] & (64 + 32)) == 64)
                            break;
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
                    // j := WordPtr(@PSC.Index[PSC.PSC_OrnamentsPointer + i * 2])^;
                    j = BitConverter.ToUInt16(data, _psc.OrnamentsPointer + i * 2);
                    
                    if (!_psc1_00)
                        j += _psc.OrnamentsPointer;

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

                            if (((sbyte)data[j]) >= 0)
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
                        while (((data[j - 2] & (64 + 32)) != 0) && ((data[j - 2] & (64 + 32)) != 32) && ((data[j - 2] & (64 + 32)) != 64));

                        if ((data[j - 2] & (64 + 32)) == 64)
                        {
                            _vtm.Ornaments[l].Loop = k - 1;
                            break;
                        }
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
                    j = _psc.SamplesPointers[i];

                    if (!_psc1_00)
                        j += 0x4C;

                    k = 0;

                    do
                    {
                        if ((sbyte)data[j + 4] >= 0)
                            _vtm.Samples[l].Loop = (byte)k;

                        _vtm.Samples[l].Ticks[k] = new SampleTick();
                        _vtm.Samples[l].Ticks[k].Ton_Accumulation = true;
                        // _vtm.Samples[l].Items[k].Add_to_Ton := WordPtr(@PSC.Index[j])^;
                        _vtm.Samples[l].Ticks[k].AddToTone = (short)BitConverter.ToUInt16(data, j);
                        _vtm.Samples[l].Ticks[k].Mixer_Ton = (data[j + 4] & 1) == 0;
                        _vtm.Samples[l].Ticks[k].Mixer_Noise = (data[j + 4] & 8) == 0;
                        _vtm.Samples[l].Ticks[k].Envelope_Enabled = (data[j + 4] & 16) == 0;
                        
                        if ((data[j + 4] & (2 + 4)) == 2)
                        {
                            _vtm.Samples[l].Ticks[k].Amplitude_Sliding = true;
                            _vtm.Samples[l].Ticks[k].Amplitude_Slide_Up = true;
                        }
                        else if ((data[j + 4] & (2 + 4)) == 4)
                        {
                            _vtm.Samples[l].Ticks[k].Amplitude_Sliding = true;
                            _vtm.Samples[l].Ticks[k].Amplitude_Slide_Up = false;
                        }

                        _vtm.Samples[l].Ticks[k].Amplitude = (byte)(data[j + 3] & 15);
                        _vtm.Samples[l].Ticks[k].Envelope_or_Noise_Accumulation = true;
                        
                        if (_vtm.Samples[l].Ticks[k].Envelope_Enabled || _vtm.Samples[l].Ticks[k].Mixer_Noise)
                            _vtm.Samples[l].Ticks[k].Add_to_Envelope_or_Noise = ((sbyte)data[j + 2]);

                        k++;
                        j += 6;

                        if (k == VTModule.MaxSampleLength)
                            break;

                    }
                    while (((data[j - 2] & (64 + 32)) != 0) && ((data[j - 2] & (64 + 32)) != 32) && ((data[j - 2] & (64 + 32)) != 64));

                    if (((data[j - 2] & (64 + 32)) == 64) && (k < VTModule.MaxSampleLength))
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

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool quit = false;
            bool vcSet = false;
            bool ornOff = false;
            bool ornOn = false;
            int a;
            int i;
            int quitCounter = 0;

            do
            {
                quitCounter++;

                if (quitCounter > 65536 * 2)
                    return;

                if (data[_chPtr[chNum]] >= 0xC0 && data[_chPtr[chNum]] <= 0xFF)
                {
                    _skipCounter[chNum] = (byte)(data[_chPtr[chNum]] - 0xBF);
                    i = -1;

                    if (ornOn)
                    {
                        if (!_ornEnabled[chNum] || (_ornSet[chNum] != _ornUsed[chNum]))
                        {
                            if (_ornSet[chNum] >= 0)
                                i = _ornSet[chNum];
                        }

                        _ornEnabled[chNum] = true;
                    }

                    if (ornOff && _ornEnabled[chNum])
                    {
                        i = 0;
                        _ornEnabled[chNum] = false;
                    }

                    if (i >= 0)
                    {
                        _ornUsed[chNum] = (sbyte)i;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)i;
                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope == 0)
                        {
                            _envSet[chNum] = false;
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                        }
                    }

                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0xA0 && data[_chPtr[chNum]] <= 0xBF)
                {
                    a = data[_chPtr[chNum]] - 0xA0;
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

                    _ornSet[chNum] = (sbyte)i;
                }
                else if (data[_chPtr[chNum]] >= 0x7E && data[_chPtr[chNum]] <= 0x9F)
                {
                    if (data[_chPtr[chNum]] >= 0x80)
                    {
                        a = data[_chPtr[chNum]] - 0x80;
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

                        _curSam[chNum] = a;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = (byte)i;
                    }
                }
                else if (data[_chPtr[chNum]] == 0x6B)
                {
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0x6C)
                {
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 2;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0x6D)
                {
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 3;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0x6E)
                {
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 11;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                    _cDelay = data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0x6F)
                {
                    ornOff = true;
                    _chPtr[chNum]++;
                }
                else if (data[_chPtr[chNum]] == 0x70)
                {
                    vcSet = true;
                    _chPtr[chNum]++;
                    _volumeCounter[chNum] = data[_chPtr[chNum]];

                    if ((_volumeCounter[chNum] & 0x40) != 0)
                        _volumeCounter[chNum] = ((sbyte)_volumeCounter[chNum] | 128);

                    _vCDop[chNum] = 0;
                }
                else if (data[_chPtr[chNum]] == 0x71)
                {
                    // Break_Ornament_Loop := True; //not available in PT3
                    _chPtr[chNum]++;
                }
                else if (data[_chPtr[chNum]] == 0x7A)
                {
                    _chPtr[chNum]++;

                    if (chNum == 1)
                    {
                        _envSet[0] = false;
                        _envSet[1] = false;
                        _envSet[2] = false;
                        i = data[_chPtr[1]] & 15;

                        if (i == 0)
                            i = 9;
                        else if (i == 15)
                            i = 7;

                        _envT = i;
                        // EnvP := WordPtr(@PSC.Index[ChPtr[1] + 1])^;
                        _envP = BitConverter.ToUInt16(data, _chPtr[1] + 1);
                        _chPtr[1] += 2;
                    }
                }
                else if (data[_chPtr[chNum]] == 0x7B)
                {
                    _chPtr[chNum]++;

                    if (chNum == 1)
                        _nsB = data[_chPtr[1]] & 0x1F;
                }
                else if (data[_chPtr[chNum]] == 0x7C)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                }
                else if (data[_chPtr[chNum]] == 0x7D)
                {
                    // OrnOff := True;
                    // Break_Sample_Loop := True //not available in PT3
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                }
                else if (data[_chPtr[chNum]] >= 0x58 && data[_chPtr[chNum]] <= 0x66)
                {
                    i = data[_chPtr[chNum]] - 0x57;
                    _initVol[chNum] = (sbyte)i;

                    if (_prevVol[chNum] != i)
                    {
                        _prevVol[chNum] = (sbyte)i;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)i;
                    }

                    if (_envEn[chNum])
                    {
                        _envSet[chNum] = false;
                        _envEn[chNum] = false;

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                        i = 0;

                        if (_ornEnabled[chNum])
                            i = _ornUsed[chNum];

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)i;
                        _ornUsed[chNum] = (sbyte)i;
                    }
                }
                else if (data[_chPtr[chNum]] == 0x57)
                {
                    if (_prevVol[chNum] != 15)
                    {
                        _initVol[chNum] = 15;
                        _prevVol[chNum] = 15;

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = 15;
                    }

                    _envSet[chNum] = false;
                    _envEn[chNum] = true;
                }
                else if (data[_chPtr[chNum]] >= 0x00 && data[_chPtr[chNum]] <= 0x56)
                {
                    ornOn = true;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)data[_chPtr[chNum]];
                    
                    if (_initVol[chNum] != _prevVol[chNum])
                    {
                        _prevVol[chNum] = _initVol[chNum];
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = _initVol[chNum];
                    }

                    if (!vcSet)
                        _volumeCounter[chNum] = 0;

                    _ntSet[chNum] = true;
                }
                else
                    _chPtr[chNum]++;

                _chPtr[chNum]++;
            }
            while (!quit);
        }

        public bool IsNsInSam(byte[] data, int sn)
        {
            bool result = false;
            int j, k;

            j = _psc.SamplesPointers[sn];

            if (!_psc1_00)
                j += 0x4C;

            k = 0;

            do
            {
                if ((data[j + 4] & 8) == 0)
                {
                    result = true;
                    return result;
                }

                k++;
                j += 6;

                if (k == VTModule.MaxSampleLength)
                    break;
            }
            while (((data[j - 2] & (64 + 32)) != 0) &&
            ((data[j - 2] & (64 + 32)) != 32) &&
            ((data[j - 2] & (64 + 32)) != 64));

            return result;
        }
    }
}
