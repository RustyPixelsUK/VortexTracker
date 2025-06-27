using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PSM
    {
        public ushort PositionsPointer;
        public ushort SamplesPointer;
        public ushort OrnamentsPointer;
        public ushort PatternsPointer;
        // 0..(65535 - 8): 65528 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65528)]
        public byte[] Remark;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PSMPat
    {
        public int Numb;
        public int Trans;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ch
    {
        public byte Dl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public ushort[] Ptr;
    }

    public class PSM2VTM
    {
        private int _pos;
        private int _patMax;
        private PSMPat[] _pats = new PSMPat[VTModule.MaxPatternIndex + 1];
        private Ch _ch;
        private ushort[] _retAddress = new ushort[3];
        private byte[] _retCnt = new byte[3];
        private byte[] _vol = new byte[3];
        private byte[] _envType = new byte[3];
        private byte[] _envDiv = new byte[3];
        private byte[] _prevSam = new byte[3];
        private sbyte[] _nt = new sbyte[3];
        private sbyte[] _skipCounter = new sbyte[3];
        private sbyte[] _skp = new sbyte[3];
        private sbyte[] _ornTick = new sbyte[3];
        private sbyte[] _orn = new sbyte[3];
        private sbyte[] _sam = new sbyte[3];
        private int _maxSample;
        private int _maxOrnament;
        private int[,] _samples = new int[14 + 1, 15 + 1];
        private int[] _ornaments = new int[32];
        private bool[] _wasEn = new bool[3];
        private PSMPat _cPat;
        private PSM _psm;
        private VTM _vtm;

        public PSM2VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _psm = Helpers.CastToStruct<PSM>(data);
            _vtm = vtm;

            bool result = true;
            int i, j, k, l, len;
            ushort w = 0, wo;
            byte b0, b1, b2;
            byte ls, lc, lp;

            i = _psm.PositionsPointer;
            string s = "";

            if ((i > 8) && (i <= 65536 - 8))
            {
                i -= 8;
                // move(PSM.PSM_Remark, s[1], i);
                s = Encoding.ASCII.GetString(_psm.Remark, 0, i);

                if (s == "psm1\0")
                    s = "";
                else if ((i > 5) && (s.Substring(1 - 1, 5) == "psm1\0"))
                    s = s.Substring(6 - 1, i - 5);
            }

            s = s.Trim();

            if (s.Length > 32)
            {
                if (s.Length > 32)
                {
                    _vtm.Author = s.Substring(32); // Extract characters from index 32 onwards
                    s = s.Substring(0, 32); // Trim `s` to 32 characters

                    if (_vtm.Author.Length > 32)
                        _vtm.Author = _vtm.Author.Substring(0, 32); // Ensure `_vtm.Author` is at most 32 characters
                }
            }
            else
                _vtm.Author = "";

            _vtm.Title = s;
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.Positions.Loop = 0;

            for (i = 0; i < 256; i++)
                _vtm.Positions.Value[i] = 0;

            for (i = 0; i < 16; i++)
                _vtm.Ornaments[i] = null;

            for (i = 1; i < 32; i++)
                _vtm.Samples[i] = null;

            _maxSample = 0;

            for (i = 0; i < 3; i++)
                _sam[i] = -1;

            for (i = 0; i < 15; i++)
            {
                for (j = 0; j < 16; j++)
                    _samples[i, j] = 0;
            }

            _maxOrnament = 0;

            for (i = 0; i < 32; i++)
                _ornaments[i] = 0;

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
                _vtm.Patterns[i] = null;

            for (k = 0; k < 3; k++)
            {
                _nt[k] = -128;
                _vol[k] = 15;
            }

            _patMax = 0;
            _pos = 0;

            while (_pos < 256)
            {
                _cPat.Numb = data[_psm.PositionsPointer + _pos * 2];
                _cPat.Trans = ((sbyte)data[_psm.PositionsPointer + _pos * 2 + 1]);
            
                if (_cPat.Numb == 255)
                {
                    if (_cPat.Trans != -1)
                        _vtm.Positions.Loop = ((byte)_cPat.Trans);

                    break;
                }

                j = _patMax;
                
                if (_patMax > VTModule.MaxPatternIndex)
                    return result;

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
                        _wasEn[k] = false;
                        _prevSam[k] = 0;
                        _retCnt[k] = 0;
                        _nt[k] = (sbyte)(((byte)_nt[k]) | 128);
                        _skipCounter[k] = 1;
                    }

                    // Move(PSM.Index[PSM.PSM_PatternsPointer + CPat.Numb * 7], Ch, 7);
                    byte[] tchBytes = new byte[Marshal.SizeOf(typeof(Ch))];
                    Array.Copy(data, _psm.PatternsPointer + _cPat.Numb * 7, tchBytes, 0, tchBytes.Length);
                    _ch = Helpers.CastToStruct<Ch>(tchBytes);

                    if (_pos == 1)
                        _vtm.InitialDelay = _ch.Dl;

                    i = 0;
                    bool quit = false;

                    while ((i < VTModule.MaxPatternLength) && !quit)
                    {
                        for (k = 2; k >= 0; k--)
                        {
                            _skipCounter[k]--;

                            if (_skipCounter[k] == 0)
                            {
                                if ((k == 2) && (data[_ch.Ptr[2]] == 255))
                                {
                                    i--;
                                    quit = true;
                                    break;
                                }

                                PatternInterpreter(data,j, i, k);
                            }
                        }

                        i++;
                    }

                    _vtm.Patterns[j].Lines[0].Channel[0].AdditionalCommand.Number = 11;
                    _vtm.Patterns[j].Lines[0].Channel[0].AdditionalCommand.Parameter = _ch.Dl;
                    _vtm.Patterns[j].Length = i;
                }
            }

            _vtm.Positions.Length = _pos;

            if (_vtm.Positions.Loop >= _pos)
                _vtm.Positions.Loop = _pos - 1;

            for (i = 0; i < 32; i++)
            {
                j = _ornaments[i];

                if (j != 0)
                {
                    _vtm.Ornaments[j] = new Ornament();
                    // wo := WordPtr(@PSM.Index[PSM.PSM_OrnamentsPointer + i * 2])^;
                    wo = BitConverter.ToUInt16(data, _psm.OrnamentsPointer + i * 2);
                    k = data[wo] & 31;

                    _vtm.Ornaments[j].Length = k + 1;
                   
                    if ((sbyte)data[wo + 1] >= 0)
                    {
                        _vtm.Ornaments[j].Length = k + 2;
                        _vtm.Ornaments[j].Loop = k + 1;
                        _vtm.Ornaments[j].Offsets[k + 1] = 0;
                    }
                    else
                        _vtm.Ornaments[j].Loop = data[wo + 1] & 31;

                    if (_vtm.Ornaments[j].Loop >= _vtm.Ornaments[j].Length)
                        _vtm.Ornaments[j].Loop = _vtm.Ornaments[j].Length - 1;

                    for (l = 0; l <= k; l++)
                        _vtm.Ornaments[j].Offsets[l] = (sbyte)data[wo + 2 + l];
                }
            }

            for (i = 0; i < 15; i++)
            {
                for (j = 0; j < 16; j++)
                {
                    k = _samples[i, j];

                    if ((k != 0) && (_vtm.Samples[k] == null))
                    {
                        _vtm.Samples[k] = new Sample();
                        // wo := WordPtr(@PSM.Index[PSM.PSM_SamplesPointer + i * 2])^;
                        wo = BitConverter.ToUInt16(data, _psm.SamplesPointer + i * 2);

                        len = data[wo] & 31;
                        
                        for (l = 0; l <= len; l++)
                        {
                            _vtm.Samples[k].Ticks[l] = new SampleTick();
                            b0 = data[wo + 2 + l * 3];
                            b1 = data[wo + 2 + l * 3 + 1];
                            b2 = data[wo + 2 + l * 3 + 2];
                            w = (ushort)(((b1 & 7) << 8) + b2);

                            if ((b1 & 4) != 0)
                                w = (ushort)(w | 0xF800);

                            _vtm.Samples[k].Ticks[l].AddToTone = (short)w;
                            _vtm.Samples[k].Ticks[l].Ton_Accumulation = true;
                            _vtm.Samples[k].Ticks[l].Amplitude = (byte)(b0 & 15);

                            if (j > 0)
                                _vtm.Samples[k].Ticks[l].Amplitude = (byte)(_vtm.Samples[k].Ticks[l].Amplitude | 16);

                            _vtm.Samples[k].Ticks[l].Mixer_Noise = (sbyte)b0 >= 0;
                            _vtm.Samples[k].Ticks[l].Mixer_Ton = (b0 & 0x10) == 0;

                            if (_vtm.Samples[k].Ticks[l].Mixer_Noise)
                                _vtm.Samples[k].Ticks[l].Add_to_Envelope_or_Noise = (sbyte)(b1 >> 3);
                        }

                        b0 = data[wo];
                        b1 = data[wo + 1];
                        
                        if ((b1 & 0xE0) == 0)
                        {
                            len++;
                            _vtm.Samples[k].Ticks[len] = new SampleTick();
                            _vtm.Samples[k].Loop = (byte)len;
                        }
                        else
                        {
                            if ((b0 & 0x20) == 0)
                                b0 = (byte)(b0 >> 6);
                            else
                                b0 = (byte)-((b0 >> 6) + 1);

                            b2 = (byte)(b1 & 31);
                            _vtm.Samples[k].Loop = (byte)b2;
                            ls = (byte)(len - b2);

                            if (b0 != 0)
                            {
                                do
                                {
                                    b2 = _vtm.Samples[k].Loop;
                                    lc = (byte)(b1 >> 5);
                                    lp = (byte)(len + 1);

                                    while ((len < VTModule.MaxSampleLength - 1) && (lc > 0))
                                    {
                                        l = 0;

                                        while ((l <= ls) && (len + l < VTModule.MaxSampleLength - 1))
                                        {
                                            _vtm.Samples[k].Ticks[len + l + 1] = _vtm.Samples[k].Ticks[b2 + l];
                                            
                                            if (j > 0)
                                                _vtm.Samples[k].Ticks[len + l + 1].Amplitude += (byte)b0;
                                            else if ((sbyte)b0 < -1)
                                            {
                                                _vtm.Samples[k].Ticks[len + l + 1].Amplitude += (byte)b0;
                                                
                                                if (((sbyte)_vtm.Samples[k].Ticks[len + l + 1].Amplitude) < 0)
                                                    _vtm.Samples[k].Ticks[len + l + 1].Amplitude = 0;
                                            }
                                            else if ((sbyte)b0 > 1)
                                            {
                                                _vtm.Samples[k].Ticks[len + l + 1].Amplitude += (byte)b0;
                                                
                                                if ((_vtm.Samples[k].Ticks[len + l + 1].Amplitude > 15))
                                                    _vtm.Samples[k].Ticks[len + l + 1].Amplitude = 15;
                                            }

                                            l++;
                                        }

                                        len += l;
                                        lc--;
                                    }
                                    
                                    if (lp != len + 1)
                                        _vtm.Samples[k].Loop = (byte)lp;
                                    
                                    if ((j == 0) && ((sbyte)b0 == -1))
                                    {
                                        _vtm.Samples[k].Ticks[lp].Amplitude_Sliding = true;
                                        _vtm.Samples[k].Ticks[lp].Amplitude_Slide_Up = false;
                                        break;
                                    }
                                    else if ((j == 0) && (b0 == 1))
                                    {
                                        _vtm.Samples[k].Ticks[lp].Amplitude_Sliding = true;
                                        _vtm.Samples[k].Ticks[lp].Amplitude_Slide_Up = true;
                                        break;
                                    }
                                }
                                while (len < VTModule.MaxSampleLength - 1);
                            }
                        }

                        _vtm.Samples[k].Length = (byte)(len + 1);

                        if (_vtm.Samples[k].Loop >= _vtm.Samples[k].Length)
                            _vtm.Samples[k].Loop = (byte)(_vtm.Samples[k].Length - 1);

                        if (j > 0)
                        {
                            for (l = 0; l <= len; l++)
                            {
                                _vtm.Samples[k].Ticks[l].Amplitude += (byte)(j - 15);
                                
                                if (((sbyte)_vtm.Samples[k].Ticks[l].Amplitude) < 0)
                                    _vtm.Samples[k].Ticks[l].Amplitude = 0;
                                else if (_vtm.Samples[k].Ticks[l].Amplitude >= 16)
                                {
                                    _vtm.Samples[k].Ticks[l].Amplitude = 15;
                                    _vtm.Samples[k].Ticks[l].Envelope_Enabled = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            bool quit = false;
            ushort patAddr;
            byte b;
            int i;

            patAddr = _ch.Ptr[chNum];

            if (_retCnt[chNum] != 0)
            {
                _retCnt[chNum]--;

                if (_retCnt[chNum] == 0)
                    patAddr = _retAddress[chNum];
            }

            do
            {
                if (data[patAddr] >= 0x00 && data[patAddr] <= 0x5F)
                {
                    if (_nt[chNum] < 0)
                        _nt[chNum] = (sbyte)(_pats[patNum].Trans + 48 - data[patAddr]);
                    else
                        _nt[chNum] -= (sbyte)data[patAddr];

                    if (_nt[chNum] < 0)
                        _nt[chNum] += 96;

                    b = (byte)(_nt[chNum] + 2);

                    if ((sbyte)b < 0)
                        b = 0;
                    else if (b > 95)
                        b = 95;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)b;

                    if (_ornTick[chNum] < 0)
                        _ornTick[chNum] = (sbyte)(_ornTick[chNum] & 0xE0);
                    else
                        _ornTick[chNum] = (sbyte)(_ornTick[chNum] & 0xC0);

                    if (((_ornTick[chNum] & 0x40) != 0) && (_orn[chNum] >= 33))
                    {
                        _wasEn[chNum] = true;
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = 15;

                        if (_envType[chNum] >= 0xB1)
                        {
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(_envType[chNum] - 0xB1 + 8);

                            if (_envDiv[chNum] >= 0xF1)
                                _vtm.Patterns[patNum].Lines[lnNum].Envelope = (ushort)((_envDiv[chNum] & 15) << 8);
                            else
                                _vtm.Patterns[patNum].Lines[lnNum].Envelope = _envDiv[chNum];

                            _ornTick[chNum] |= 0x40;
                        }
                        else
                        {
                            b = (byte)(_envType[chNum] - 0xA1);
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(((b & 3) << 1) | 8);
                            b = (byte)((b & 12) * 3 + _nt[chNum]);

                            if (b >= 48)
                            {
                                b -= 48;

                                if (b >= 48)
                                    b -= 48;
                            }

                            if (b > 45)
                                b = 45;

                            _vtm.Patterns[patNum].Lines[lnNum].Envelope = VTModule.PT3NoteTable_ST[b + 48 + 2];
                        }
                    }
                    quit = true;
                }
                else if (data[patAddr] == 0x60)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    quit = true;
                }
                else if (data[patAddr] >= 0x61 && data[patAddr] <= 0x6F)
                {
                    _sam[chNum] = (sbyte)(data[patAddr] - 0x61);
                }
                else if (data[patAddr] >= 0x70 && data[patAddr] <= 0x8F)
                {
                    _orn[chNum] = (sbyte)(data[patAddr] - 0x70);
                    _ornTick[chNum] = 0;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
     
                    b = (byte)_ornaments[_orn[chNum]];
                    
                    if (b == 0)
                    {
                        if (_maxOrnament < 15)
                        {
                            _maxOrnament++;
                            _ornaments[_orn[chNum]] = _maxOrnament;
                            b = (byte)_maxOrnament;
                        }
                    }

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = b;
                }
                else if (data[patAddr] == 0x90)
                {
                    quit = true;
                }
                else if (data[patAddr] >= 0x91 && data[patAddr] <= 0x9F)
                {
                    _vol[chNum] = (byte)(data[patAddr] - 0x90);
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)_vol[chNum];
                }
                else if (data[patAddr] == 0xA0)
                {
                    _ornTick[chNum] = (sbyte)data[patAddr];
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = 0;
                }
                else if (data[patAddr] >= 0xA1 && data[patAddr] <= 0xB0)
                {
                    _orn[chNum] = 33;
                    _ornTick[chNum] |= 0x40;
                    _envType[chNum] = data[patAddr];
                }
                else if (data[patAddr] >= 0xB1 && data[patAddr] <= 0xB7)
                {
                    _wasEn[chNum] = true;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = 15;
                    _envType[chNum] = data[patAddr];
                    patAddr++;
                    _envDiv[chNum] = data[patAddr];
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = (byte)(_envType[chNum] - 0xB1 + 8);
                    
                    if (_envDiv[chNum] >= 0xF1)
                        _vtm.Patterns[patNum].Lines[lnNum].Envelope = (ushort)((_envDiv[chNum] & 15) << 8);
                    else
                        _vtm.Patterns[patNum].Lines[lnNum].Envelope = _envDiv[chNum];

                    _ornTick[chNum] |= 0x40;
                }
                else if (data[patAddr] >= 0xB8 && data[patAddr] <= 0xF8)
                {
                    _skp[chNum] = (sbyte)(data[patAddr] - 0xB7);
                }
                else if (data[patAddr] == 0xF9)
                {
                    _retAddress[chNum] = (ushort)(patAddr + 3);
                    _retCnt[chNum] = data[((ushort)patAddr + 2)];
                    // PatAddr := WordPtr(@PSM.Index[PatAddr])^ - 1;
                    patAddr = (ushort)(BitConverter.ToUInt16(data, patAddr) - 1);
                }
                else if (data[patAddr] >= 0xFA && data[patAddr] <= 0xFB)
                {
                    _orn[chNum] = (sbyte)(data[patAddr] - 0xFA + 32);
                }
                else
                {
                    quit = true;
                }

                patAddr++;
            }
            while (!quit);

            if (_sam[chNum] >= 0)
            {
                b = (byte)(((_ornTick[chNum] & 0x40) != 0 ? 1 : 0) * _vol[chNum]);
                
                if (_samples[_sam[chNum], b] == 0)
                {
                    if (_maxSample < 31)
                    {
                        _maxSample++;
                        _samples[_sam[chNum], b] = _maxSample;
                    }
                    else
                    {
                        for (i = 15; i >= 0; i--)
                        {
                            if (_samples[_sam[chNum], i] != 0)
                            {
                                _samples[_sam[chNum], b] = _samples[_sam[chNum], i];
                                break;
                            }
                        }
                    }
                }

                b = (byte)_samples[_sam[chNum], b];

                if ((_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note != -1) && (_prevSam[chNum] != b))
                {
                    _prevSam[chNum] = b;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = b;
                }
            }
            
            if (_wasEn[chNum] && ((_ornTick[chNum] & 0x40) == 0))
            {
                _wasEn[chNum] = false;
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)_vol[chNum];
            }

            _ch.Ptr[chNum] = patAddr;
            _skipCounter[chNum] = _skp[chNum];
        }
    }
}
