using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FTC
    {
        // 69 bytes (0..68)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 69)]
        public byte[] MusicName;
        public byte Delay;
        public byte Loop_Position;
        public int Slack;
        public ushort PatternsPointer;
        // 5 bytes (0..4)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Slack2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public ushort[] SamplesPointers;
        // 33 words (0..32)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        public ushort[] OrnamentsPointers;
        // ((65536 - 212) div 2) - 1 = 32661 elements
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32661)]
        public FTC_Position[] Positions;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FTC_Position
    {
        public byte Pattern;
        public sbyte Transposition;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FTCPat
    {
        public int Numb;
        public int Trans;
    }

    public class FTC2VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private sbyte[] _prevOrn = new sbyte[3];
        private sbyte[] _tsStep = new sbyte[3];
        private int[] _sams = new int[32];
        private int[] _orns = new int[32 + 1];
        private int _nOrns;
        private int _nSams;
        private int _ns;
        private FTCPat _cPat;
        private int _pos;
        private int _patMax;
        private FTCPat[] _pats = new FTCPat[VTModule.MaxPatternIndex + 1];
        private FTC _ftc;
        private VTM _vtm;

        public FTC2VTM()
        {;
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _ftc = Helpers.CastToStruct<FTC>(data);
            _vtm = vtm;

            bool result = true;
            bool quit = false;
            int i, j, k, l, jl, nb, n, tmp, len, zo;

            // Move(FTC.FTC_MusicName[8], _vtm.Title[1], 42);
            _vtm.Title = Helpers.CopyCharsToString(_ftc.MusicName, 8, 42);
            // Detect FTC version

            if ((data[0x32] != 0x3b) && (data[0x32] < 4))
                _vtm.NoteTable = (NoteTableType)data[0x32];
            else
                _vtm.NoteTable = NoteTableType.SoundTracker;

            _vtm.Title = _vtm.Title.Trim();

            if (_vtm.Title.Length > 32)
            {
                _vtm.Author = _vtm.Title.Substring(32);
                _vtm.Title = _vtm.Title.Substring(0, 32);
            }
            else
                _vtm.Author = "";

            _vtm.InitialDelay = _ftc.Delay;
            _vtm.Positions.Loop = _ftc.Loop_Position;

            for (i = 0; i < 256; i++)
                _vtm.Positions.Value[i] = 0;

            for (i = 0; i < 16; i++)
                _vtm.Ornaments[i] = null;

            for (i = 1; i < 32; i++)
                _vtm.Samples[i] = null;

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                _vtm.Patterns[i] = null;

            for (i = 0; i < 33; i++)
                _orns[i] = -1;

            for (i = 0; i < 32; i++)
                _sams[i] = -1;

            _nOrns = 0;
            _nSams = 0;
            _patMax = 0;
            _pos = 0;

            while ((_pos < 256) && (_ftc.Positions[_pos].Pattern != 255))
            {
                _cPat.Numb = _ftc.Positions[_pos].Pattern;
                _cPat.Trans = _ftc.Positions[_pos].Transposition;
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
                    _vtm.Patterns[j] = new Pattern();

                    for (k = 0; k < 3; k++)
                    {
                        _tsStep[k] = 0;
                        _prevOrn[k] = 0;
                        _skipCounter[k] = 0;
                    }

                    // Move(FTC.Index[FTC.FTC_PatternsPointer + 6 * CPat.Numb], ChPtr, 6);
                    Helpers.Move(data, _ftc.PatternsPointer + 6 * _cPat.Numb, _chPtr, 0, 6);
                    
                    _ns = 0;
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

                        if (i >= 0)
                            _vtm.Patterns[j].Lines[i].Noise = (byte)_ns;

                        i++;
                    }

                    _vtm.Patterns[j].Length = i;
                }
            }
            _vtm.Positions.Length = _pos;

            if (_vtm.Positions.Loop >= _pos)
                _vtm.Positions.Loop = _pos - 1;

            zo = 0;

            for (i = 0; i < 33; i++)
            {
                l = _orns[i];

                if ((l > 0) && (l <= _vtm.Ornaments.Length))
                {
                    j = _ftc.OrnamentsPointers[i] + 3;
                    jl = j + data[j - 2] * 2;
                    len = j + (data[j - 1] + 1) * 2;
                    k = nb = n = 0;

                    do
                    {
                        do
                        {
                            tmp = n;

                            if ((data[j] & 64) != 0)
                            {
                                n += ((sbyte)data[j + 1]);

                                if ((n < -0x5F) || (n > 0x5F))
                                    break;
                            }
                            else
                                n = data[j + 1];

                            if (j == jl)
                                nb = tmp;

                            k++;
                            j += 2;

                            if (k == VTModule.MaxOrnamentLength)
                                break;

                        }
                        while (j < len);
                        
                        j = jl;
                    }
                    while ((k != VTModule.MaxOrnamentLength) && (n != nb) && (n >= -0x5F) && (n <= 0x5F));

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
                    for (i = 0; i < 33; i++)
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
                                    {
                                        _vtm.Patterns[i].Lines[j].Channel[k].Ornament = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (i = 0; i < 33; i++)
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

            for (i = 0; i < 33; i++)
            {
                l = _orns[i];

                if ((l > 0) && (l <= _vtm.Ornaments.Length))
                {
                    _vtm.Ornaments[l] = new Ornament();
                    _vtm.Ornaments[l].Loop = 0;

                    j = _ftc.OrnamentsPointers[i] + 3;
                    jl = j + data[j - 2] * 2;
                    len = j + (data[j - 1] + 1) * 2;
                    k = nb = n = 0;

                    do
                    {
                        do
                        {
                            tmp = n;

                            if ((data[j] & 64) != 0)
                            {
                                n += ((sbyte)data[j + 1]);

                                if ((n < -0x5F) || (n > 0x5F))
                                    break;
                            }
                            else
                                n = data[j + 1];

                            if (j == jl)
                            {
                                _vtm.Ornaments[l].Loop = k;
                                nb = tmp;
                            }

                            k++;

                            _vtm.Ornaments[l].Offsets[k - 1] = (sbyte)n;
                            j += 2;

                            if (k == VTModule.MaxOrnamentLength)
                                break;
                        }
                        while (j < len);

                        j = jl;
                    }
                    while ((k != VTModule.MaxOrnamentLength) && (n != nb) && (n >= -0x5F) && (n <= 0x5F));

                    _vtm.Ornaments[l].Length = k;
                }
            }
            for (i = 0; i < 32; i++)
            {
                l = _sams[i];

                if ((l > 0) && (l <= _vtm.Samples.Length))
                {
                    j = _ftc.SamplesPointers[i] + 3;

                    if ((data[j - 2] > VTModule.MaxSampleLength - 1) || (data[j - 1] + 1 > VTModule.MaxSampleLength))
                        continue;

                    _vtm.Samples[l] = new Sample();
                    _vtm.Samples[l].Loop = data[j - 2];
                    _vtm.Samples[l].Length = (byte)(data[j - 1] + 1);

                    for (k = 0; k < _vtm.Samples[l].Length; k++)
                    {
                        _vtm.Samples[l].Ticks[k] = new SampleTick();
                        _vtm.Samples[l].Ticks[k].Mixer_Noise = (data[j + k * 5] & 64) == 0;
                       
                        if (_vtm.Samples[l].Ticks[k].Mixer_Noise)
                        {
                            _vtm.Samples[l].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(data[j + k * 5] & 31);
                        
                            if ((_vtm.Samples[l].Ticks[k].Add_to_Envelope_or_Noise & 0x10) != 0)
                                _vtm.Samples[l].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[l].Ticks[k].Add_to_Envelope_or_Noise | 0xF0);

                            _vtm.Samples[l].Ticks[k].Envelope_or_Noise_Accumulation = ((sbyte)data[j + k * 5]) < 0;
                        }

                        // _vtm.Samples[l].Items[k].Add_to_Ton := WordPtr(@FTC.Index[j + k * 5 + 1]) ^ and $FFF;
                        _vtm.Samples[l].Ticks[k].AddToTone = (short)(BitConverter.ToUInt16(data, j + k * 5 + 1) & 0xFFF);

                        if ((_vtm.Samples[l].Ticks[k].AddToTone & 0x800) != 0)
                            _vtm.Samples[l].Ticks[k].AddToTone = (short)(_vtm.Samples[l].Ticks[k].AddToTone | 0xF000);

                        _vtm.Samples[l].Ticks[k].Ton_Accumulation = ((sbyte)data[j + k * 5 + 2]) < 0;
                        _vtm.Samples[l].Ticks[k].Mixer_Ton = (data[j + k * 5 + 2] & 64) == 0;

                        if ((data[j + k * 5 + 3] & 32) != 0)
                        {
                            _vtm.Samples[l].Ticks[k].Amplitude_Sliding = true;
                            _vtm.Samples[l].Ticks[k].Amplitude_Slide_Up = (data[j + k * 5 + 3] & 16) == 0;
                        }

                        _vtm.Samples[l].Ticks[k].Amplitude = (byte)(data[j + k * 5 + 3] & 15);
                        _vtm.Samples[l].Ticks[k].Envelope_Enabled = (data[j + k * 5 + 3] & 64) != 0;

                        if (!_vtm.Samples[l].Ticks[k].Mixer_Noise && _vtm.Samples[l].Ticks[k].Envelope_Enabled)
                        {
                            _vtm.Samples[l].Ticks[k].Envelope_or_Noise_Accumulation = ((sbyte)data[j + k * 5 + 3]) < 0;
                            _vtm.Samples[l].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)-((sbyte)data[j + k * 5 + 4]);
                        }
                    }
                }
            }

            return result;
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool quit = false;
            sbyte exxAF = 2;
            int a;
            int i;
            byte nt;
            int quitCounter = 0;

            do
            {
                if (++quitCounter > 65536 * 2)
                    return;
 
                if (data[_chPtr[chNum]] >= 0x00 &&
                    data[_chPtr[chNum]] <= 0x1F)
                {
                    a = data[_chPtr[chNum]];
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
                else if (data[_chPtr[chNum]] >= 0x20 &&
                    data[_chPtr[chNum]] <= 0x2F)
                {
                    i = data[_chPtr[chNum]] - 0x20;

                    if (i == 0)
                        i = 1;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)i;
                }
                else if (data[_chPtr[chNum]] == 0x30)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    _skipCounter[chNum] = 0;
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x31 &&
                    data[_chPtr[chNum]] <= 0x3E)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(data[_chPtr[chNum]] - 0x30);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)_prevOrn[chNum];
                    _chPtr[chNum]++;
                    // _vtm.Patterns[PatNum].Items[LnNum].Envelope := WordPtr(@FTC.Index[ChPtr[ChNum]]) ^;
                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = BitConverter.ToUInt16(data, _chPtr[chNum]);
                    _chPtr[chNum]++;
                }
                else if (data[_chPtr[chNum]] == 0x3F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)_prevOrn[chNum];
                }
                else if (data[_chPtr[chNum]] >= 0x40 &&
                    data[_chPtr[chNum]] <= 0x5F)
                {
                    _skipCounter[chNum] = (sbyte)(data[_chPtr[chNum]] - 0x40);
                    exxAF = 1;
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0x60 &&
                    data[_chPtr[chNum]] <= 0xCB)
                {
                    nt = (byte)(data[_chPtr[chNum]] + _cPat.Trans - 0x60);

                    if (nt > 0x5F)
                        nt = 0x5F;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)nt;
                    _skipCounter[chNum] = 0;
                    quit = true;
                }
                else if (data[_chPtr[chNum]] >= 0xCC &&
                    data[_chPtr[chNum]] <= 0xEC)
                {
                    a = data[_chPtr[chNum]] - 0xCC;
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
                else if (data[_chPtr[chNum]] == 0xED)
                {
                    exxAF = 1;
                    _chPtr[chNum]++;
                    // i := smallint(WordPtr(@FTC.Index[ChPtr[ChNum]])^);
                    i = BitConverter.ToInt16(data, _chPtr[chNum]);

                    if (i >= 0)
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)(i & 255);
                    }
                    else
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 2;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)(-i & 255);
                    }
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _chPtr[chNum]++;
                }
                else if (data[_chPtr[chNum]] == 0xEE)
                {
                    exxAF = 0;
                    _chPtr[chNum]++;
                    _tsStep[chNum] = (sbyte)data[_chPtr[chNum]];
                }
                else if (data[_chPtr[chNum]] == 0xEF)
                {
                    _chPtr[chNum]++;
                    _ns = data[_chPtr[chNum]];
                }
                else
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 11;
                    _chPtr[chNum]++;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_chPtr[chNum]];
                }

                _chPtr[chNum]++;
            }
            while (!quit);
            
            if (exxAF == 0)
            {
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 3;
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = (byte)_tsStep[chNum];
            }
        }
    }
}
