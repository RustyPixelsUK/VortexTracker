using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static LibVT.SQTPat;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SQT
    {
        public ushort Size;
        public ushort SamplesPointer;
        public ushort OrnamentsPointer;
        public ushort PatternsPointer;
        public ushort PositionsPointer;
        public ushort LoopPointer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SQTPat
    {
        public int Del;
        public Channel[] Chn;

        public struct Channel
        {
            public bool EnableEffects;
            public int PatChanNumber;
            public int Vol;
            public int Trans;
        }
    }

    public class SQT2VTM
    {
        private ushort[] _chPtr = new ushort[3];
        private byte[] _prevNote = new byte[3];
        private byte[] _prevOrn = new byte[3];
        private byte[] _prevSamp = new byte[3];
        private byte[] _ix21 = new byte[3];
        private ushort[] _ix27 = new ushort[3];
        private bool[] _b6ix0 = new bool[3];
        private bool[] _b7ix0 = new bool[3];
        private bool[] _envEn = new bool[3];
        private int[] _orns = new int[32];
        private bool[] _isSample = new bool[32];
        private byte _cDelay;
        private byte _envP;
        private byte _envT;
        private SQTPat _cPat;
        private byte[] _cVol = new byte[3];
        private int _nOrns;
        private byte[] _orn2Sam = new byte[16];
        private int _pos;
        private int _patMax;
        private bool[] _isPattern = new bool[VTModule.MaxPatternIndex + 1];
        private SQTPat[] _pats = new SQTPat[VTModule.MaxPatternIndex + 1];
        private SQT _sqt;
        private VTM _vtm;

        private ushort _ptr;
        private int _temp, _prOrn;
        private bool _sampleSet;

        public SQT2VTM()
        {
        }

        public bool Initialize(byte[] data, VTM vtm)
        {
            _sqt = Helpers.CastToStruct<SQT>(data);
            _vtm = vtm;

            bool result  = true;
            int i, j, k, l, lp, c, len;

            _vtm.Title = "";
            _vtm.Author = "";
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.InitialDelay = 0;

            for (i = 0; i < 16; i++)
                _vtm.Ornaments[i] = null;

            for (i = 1; i < 16; i++)
                _orn2Sam[i] = 0;

            for (i = 1; i < 32; i++)
                _orns[i] = -1;

            for (i = 1; i < 32; i++)
                _vtm.Samples[i] = null;

            for (i = 1; i < 32; i++)
                _isSample[i] = false;

            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
            {
                _vtm.Patterns[i] = null;
                _isPattern[i] = false;
            }

            _pos = 0;
            _patMax = 0;
            _nOrns = 0;

            _vtm.Positions.Loop = 0;

            while ((_pos < 256) && (data[_sqt.PositionsPointer + _pos * 7] != 0))
            {
                _cPat = new SQTPat();
                _cPat.Chn = new SQTPat.Channel[3];

                if (_sqt.PositionsPointer + _pos * 7 == _sqt.LoopPointer)
                    _vtm.Positions.Loop = _pos;

                _cPat.Chn[2].PatChanNumber = data[_sqt.PositionsPointer + _pos * 7] & 0x7F;
                _cPat.Chn[1].PatChanNumber = data[_sqt.PositionsPointer + _pos * 7 + 2] & 0x7F;
                _cPat.Chn[0].PatChanNumber = data[_sqt.PositionsPointer + _pos * 7 + 4] & 0x7F;
                _cPat.Chn[2].EnableEffects = (data[_sqt.PositionsPointer + _pos * 7] & 128) != 0;
                _cPat.Chn[1].EnableEffects = (data[_sqt.PositionsPointer + _pos * 7 + 2] & 128) != 0;
                _cPat.Chn[0].EnableEffects = (data[_sqt.PositionsPointer + _pos * 7 + 4] & 128) != 0;
                _cPat.Chn[2].Vol = data[_sqt.PositionsPointer + _pos * 7 + 1] & 15;
                _cPat.Chn[1].Vol = data[_sqt.PositionsPointer + _pos * 7 + 3] & 15;
                _cPat.Chn[0].Vol = data[_sqt.PositionsPointer + _pos * 7 + 5] & 15;

                if (data[_sqt.PositionsPointer + _pos * 7 + 1] >> 4 < 9)
                    _cPat.Chn[2].Trans = data[_sqt.PositionsPointer + _pos * 7 + 1] >> 4;
                else
                    _cPat.Chn[2].Trans = -(data[_sqt.PositionsPointer + _pos * 7 + 1] >> 4 - 9) - 1;

                if (data[_sqt.PositionsPointer + _pos * 7 + 3] >> 4 < 9)
                    _cPat.Chn[1].Trans = data[_sqt.PositionsPointer + _pos * 7 + 3] >> 4;
                else
                    _cPat.Chn[1].Trans = -(data[_sqt.PositionsPointer + _pos * 7 + 3] >> 4 - 9) - 1;

                if (data[_sqt.PositionsPointer + _pos * 7 + 5] >> 4 < 9)
                    _cPat.Chn[0].Trans = data[_sqt.PositionsPointer + _pos * 7 + 5] >> 4;
                else
                    _cPat.Chn[0].Trans = -(data[_sqt.PositionsPointer + _pos * 7 + 5] >> 4 - 9) - 1;

                _cDelay = data[_sqt.PositionsPointer + _pos * 7 + 6];

                if (_vtm.InitialDelay == 0)
                    _vtm.InitialDelay = _cDelay;

                _cPat.Del = _cDelay;
                j = _patMax;

                for (c = 0; c < _patMax; c++)
                {
                    if ((_pats[c].Chn[2].PatChanNumber == _cPat.Chn[2].PatChanNumber) &&
                        (_pats[c].Chn[2].EnableEffects == _cPat.Chn[2].EnableEffects) &&
                        (_pats[c].Chn[2].Vol == _cPat.Chn[2].Vol) &&
                        (_pats[c].Chn[2].Trans == _cPat.Chn[2].Trans) &&
                        (_pats[c].Chn[1].PatChanNumber == _cPat.Chn[1].PatChanNumber) &&
                        (_pats[c].Chn[1].EnableEffects == _cPat.Chn[1].EnableEffects) &&
                        (_pats[c].Chn[1].Vol == _cPat.Chn[1].Vol) &&
                        (_pats[c].Chn[1].Trans == _cPat.Chn[1].Trans) &&
                        (_pats[c].Chn[0].PatChanNumber == _cPat.Chn[0].PatChanNumber) &&
                        (_pats[c].Chn[0].EnableEffects == _cPat.Chn[0].EnableEffects) &&
                        (_pats[c].Chn[0].Vol == _cPat.Chn[0].Vol) &&
                        (_pats[c].Chn[0].Trans == _cPat.Chn[0].Trans) &&
                        (_pats[c].Del == _cPat.Del))
                    {
                        j = c;
                        break;
                    }
                }

                if (j == _patMax)
                {
                    _patMax++;

                    if (j < VTModule.MaxPatternCount)
                        _pats[j] = _cPat;
                }

                _vtm.Positions.Value[_pos] = j;
                _pos++;

                if ((j < VTModule.MaxPatternCount) && !_isPattern[j])
                {
                    _isPattern[j] = true;
                    _vtm.Patterns[j] = new Pattern();

                    // move(SQT.Index[CPat.Chn[0].PatChanNumber * 2 + SQT.SQT_PatternsPointer], ChPtr[0], 2);
                    // move(SQT.Index[CPat.Chn[1].PatChanNumber * 2 + SQT.SQT_PatternsPointer], ChPtr[1], 2);
                    // move(SQT.Index[CPat.Chn[2].PatChanNumber * 2 + SQT.SQT_PatternsPointer], ChPtr[2], 2);

                    _chPtr[0] = BitConverter.ToUInt16(data, _cPat.Chn[0].PatChanNumber * 2 + _sqt.PatternsPointer);
                    _chPtr[1] = BitConverter.ToUInt16(data, _cPat.Chn[1].PatChanNumber * 2 + _sqt.PatternsPointer);
                    _chPtr[2] = BitConverter.ToUInt16(data, _cPat.Chn[2].PatChanNumber * 2 + _sqt.PatternsPointer);

                    c = data[_chPtr[2]];

                    if (c > VTModule.MaxPatternLength)
                        c = VTModule.MaxPatternLength;

                    for (k = 0; k < 3; k++)
                    {
                        _chPtr[k]++;
                        _envEn[k] = false;
                        _prevSamp[k] = 0;
                        _prevOrn[k] = 255;
                        _prevNote[k] = 0;
                        _ix21[k] = 0;
                    }

                    _vtm.Patterns[j].Length = c;
                    _cVol[0] = (byte)_cPat.Chn[0].Vol;
                    _vtm.Patterns[j].Lines[0].Channel[0].Volume = (sbyte)(15 - _cVol[0]);
                    
                    if (_vtm.Patterns[j].Lines[0].Channel[0].Volume == 0)
                        _vtm.Patterns[j].Lines[0].Channel[0].Volume++;

                    _cVol[1] = (byte)_cPat.Chn[1].Vol;
                    _vtm.Patterns[j].Lines[0].Channel[1].Volume = (sbyte)(15 - _cVol[1]);

                    if (_vtm.Patterns[j].Lines[0].Channel[1].Volume == 0)
                        _vtm.Patterns[j].Lines[0].Channel[1].Volume++;

                    _cVol[2] = (byte)_cPat.Chn[2].Vol;
                    _vtm.Patterns[j].Lines[0].Channel[2].Volume = (sbyte)(15 - _cVol[2]);

                    if (_vtm.Patterns[j].Lines[0].Channel[2].Volume == 0)
                        _vtm.Patterns[j].Lines[0].Channel[2].Volume++;

                    i = 0;
                    k = 0;

                    while (i < c)
                    {
                        for (k = 2; k >= 0; k--)
                            PatternInterpreter(data, j, i, k);

                        i++;
                    }

                    if ((_vtm.Patterns[j].Lines[0].Channel[0].AdditionalCommand.Number != 11) &&
                        (_vtm.Patterns[j].Lines[0].Channel[1].AdditionalCommand.Number != 11) &&
                        (_vtm.Patterns[j].Lines[0].Channel[2].AdditionalCommand.Number != 11))
                    {
                        if (_vtm.Patterns[j].Lines[0].Channel[0].AdditionalCommand.Number == 0)
                            k = 0;
                        else if (_vtm.Patterns[j].Lines[0].Channel[1].AdditionalCommand.Number == 0)
                            k = 1;
                        else
                            k = 2;

                        _vtm.Patterns[j].Lines[0].Channel[k].AdditionalCommand.Number = 11;
                        _vtm.Patterns[j].Lines[0].Channel[k].AdditionalCommand.Parameter = (byte)_cPat.Del;
                    }
                }
            }

            _vtm.Positions.Length = _pos;

            for (i = 1; i < 32; i++)
            {
                l = _orns[i];

                if ((l > 0) && (l <= _vtm.Ornaments.Length))
                {
                    _vtm.Ornaments[l] = new Ornament();

                    // move(SQT.Index[SQT.SQT_OrnamentsPointer + i * 2], j, 2);
                    j = BitConverter.ToUInt16(data, _sqt.OrnamentsPointer + i * 2);

                    lp = data[j];
                    j++;

                    if (lp > 32)
                        lp = 32;

                    if (lp < 32)
                    {
                        len = lp + data[j];

                        if (len > 32)
                            len = 32;

                        if (len < 32)
                        {
                            _vtm.Ornaments[l].Loop = 32;
                            _vtm.Ornaments[l].Length = 32 + len - lp;
                        }
                        else
                        {
                            _vtm.Ornaments[l].Loop = lp;
                            _vtm.Ornaments[l].Length = 32;
                        }
                    }
                    else
                    {
                        len = 32;
                        k = _orn2Sam[l];

                        if (k > 0)
                        {
                            // c := WordPtr(@SQT.Index[SQT.SQT_SamplesPointer + k * 2])^;
                            c = BitConverter.ToUInt16(data, _sqt.SamplesPointer + k * 2);
                            lp = data[c];

                            c++;

                            if (lp > 32)
                                lp = 32;

                            len = lp + data[c];

                            if (len > 32)
                                len = 32;
                        }
                        if (lp < 32)
                        {
                            if (len < 32)
                            {
                                _vtm.Ornaments[l].Loop = 32;
                                _vtm.Ornaments[l].Length = 32 + len - lp;
                            }
                            else
                            {
                                _vtm.Ornaments[l].Loop = lp;
                                _vtm.Ornaments[l].Length = 32;
                            }
                        }
                        else
                        {
                            _vtm.Ornaments[l].Loop = 31;
                            _vtm.Ornaments[l].Length = 32;
                        }
                    }

                    j++;

                    for (k = 0; k < 32; k++)
                        _vtm.Ornaments[l].Offsets[k] = (sbyte)data[j + k];

                    for (k = 32; k < _vtm.Ornaments[l].Length; k++)
                        _vtm.Ornaments[l].Offsets[k] = _vtm.Ornaments[l].Offsets[k - 32 + lp];
                }
            }

            for (i = 1; i < 32; i++)
            {
                if (_isSample[i])
                {
                    _vtm.Samples[i] = new Sample();
                    // j := WordPtr(@SQT.Index[SQT.SQT_SamplesPointer + i * 2])^;
                    j = BitConverter.ToUInt16(data, _sqt.SamplesPointer + i * 2);
                    lp = data[j];

                    j++;

                    if (lp > 32)
                        lp = 32;

                    if (lp < 32)
                    {
                        len = lp + data[j];

                        if (len > 32)
                            len = 32;

                        if (len != 32)
                        {
                            _vtm.Samples[i].Loop = 32;
                            _vtm.Samples[i].Length = (byte)(32 + len - lp);
                        }
                        else
                        {
                            _vtm.Samples[i].Loop = (byte)lp;
                            _vtm.Samples[i].Length = 32;
                        }
                    }
                    else
                    {
                        _vtm.Samples[i].Loop = 32;
                        _vtm.Samples[i].Length = 33;
                    }
                    
                    j++;

                    for (k = 0; k < 32; k++)
                    {
                        _vtm.Samples[i].Ticks[k] = new SampleTick();
                        _vtm.Samples[i].Ticks[k].Amplitude = (byte)(data[j + k * 3] & 15);

                        if (_vtm.Samples[i].Ticks[k].Amplitude == 0)
                            _vtm.Samples[i].Ticks[k].Envelope_Enabled = true;

                        _vtm.Samples[i].Ticks[k].Mixer_Noise = (data[j + k * 3 + 1] & 32) != 0;
                        _vtm.Samples[i].Ticks[k].Mixer_Ton = (data[j + k * 3 + 1] & 64) != 0;

                        if (_vtm.Samples[i].Ticks[k].Mixer_Noise)
                        {
                            _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)((data[j + k * 3] & 0xF0) >> 3);

                            if ((data[j + k * 3 + 1] & 128) != 0)
                                _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise++;

                            if ((_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise & 0x10) != 0)
                                _vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise = (sbyte)(_vtm.Samples[i].Ticks[k].Add_to_Envelope_or_Noise | 0xF0);
                        }
                        if ((data[j + k * 3 + 1] & 16) != 0)
                            _vtm.Samples[i].Ticks[k].AddToTone = (short)((data[j + k * 3 + 1] & 15) << 8 + data[j + k * 3 + 2]);
                        else
                            _vtm.Samples[i].Ticks[k].AddToTone = (short)-((data[j + k * 3 + 1] & 15) << 8 + data[j + k * 3 + 2]);
                    }

                    if (lp == 32)
                        _vtm.Samples[i].Ticks[32] = new SampleTick();
                    else
                    {
                        for (k = 32; k < _vtm.Samples[i].Length; k++)
                            _vtm.Samples[i].Ticks[k] = _vtm.Samples[i].Ticks[k - 32 + lp];
                    }
                }
            }

            return result;
        }

        public void Call_LC1D1(byte[] data, byte a, int patNum, int lnNum, int chNum)
        {
            _ptr++;

            if (_b6ix0[chNum])
            {
                _chPtr[chNum] = (ushort)(_ptr + 1);
                _b6ix0[chNum] = false;
            }

            switch (a - 1)
            {
                case 0:
                    if (_cPat.Chn[chNum].EnableEffects)
                    {
                        _cVol[chNum] = (byte)(data[_ptr] & 15);
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)(15 - _cVol[chNum]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = 1;
                    }
                    break;
                case 1:
                    if (_cPat.Chn[chNum].EnableEffects)
                    {
                        _cVol[chNum] = (byte)((_cVol[chNum] + data[_ptr]) & 15);
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = (sbyte)(15 - _cVol[chNum]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Volume = 1;
                    }
                    break;
                case 2:
                    if (_cPat.Chn[chNum].EnableEffects)
                    {
                        _cVol[0] = (byte)(data[_ptr] & 15);
                        
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[0].Volume = (sbyte)(15 - _cVol[0]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[0].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[0].Volume = 1;

                        _cVol[1] = (byte)(data[_ptr] & 15);
                        
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[1].Volume = (sbyte)(15 - _cVol[1]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[1].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[1].Volume = 1;

                        _cVol[2] = (byte)(data[_ptr] & 15);

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[2].Volume = (sbyte)(15 - _cVol[2]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[2].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[2].Volume = 1;
                    }
                    break;
                case 3:
                    if (_cPat.Chn[chNum].EnableEffects)
                    {
                        _cVol[0] = (byte)((_cVol[0] + data[_ptr]) & 15);

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[0].Volume = (sbyte)(15 - _cVol[0]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[0].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[0].Volume = 1;

                        _cVol[1] = (byte)((_cVol[1] + data[_ptr]) & 15);

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[1].Volume = (sbyte)(15 - _cVol[1]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[1].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[1].Volume = 1;

                        _cVol[2] = (byte)((_cVol[2] + data[_ptr]) & 15);

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[2].Volume = (sbyte)(15 - _cVol[2]);

                        if (_vtm.Patterns[patNum].Lines[lnNum].Channel[2].Volume == 0)
                            _vtm.Patterns[patNum].Lines[lnNum].Channel[2].Volume = 1;
                    }
                    break;
                case 4:
                    if (_cPat.Chn[chNum].EnableEffects)
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 11;
                        
                        _cDelay = (byte)(data[_ptr] & 31);

                        if (_cDelay == 0)
                            _cDelay = 32;

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = _cDelay;
                    }
                    break;
                case 5:
                    if (_cPat.Chn[chNum].EnableEffects)
                    {
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 11;
                        
                        _cDelay = (byte)((_cDelay + data[_ptr]) & 31);
                        
                        if (_cDelay == 0)
                            _cDelay = 32;

                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = _cDelay;
                    }
                    break;
                case 6:
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 2;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_ptr];
                    break;
                case 7:
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Delay = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Number = 1;
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].AdditionalCommand.Parameter = data[_ptr];
                    break;
                default:
                    _envEn[chNum] = true;
                    _envT = (byte)((a - 1) & 15);

                    if (_envT == 15)
                        _envT = 7;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = _envT;

                    if (_prevOrn[chNum] != 255)
                        _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = _prevOrn[chNum];

                    _envP = data[_ptr];

                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = _envP;
                    break;
            }
        }

        public void Call_LC2A8(byte[] data, byte a, int patNum, int lnNum, int chNum)
        {
            if (_envEn[chNum] || (_prevOrn[chNum] != 0))
            {
                _sampleSet = true;
                _prOrn = _prevOrn[chNum];
                _prevOrn[chNum] = 0;
                _envEn[chNum] = false;
            
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = 0;
            }

            if ((a > 0) && (a <= 31))
                _isSample[a] = true;

            if (_prevSamp[chNum] != a)
            {
                _prevSamp[chNum] = a;
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Sample = a;
            }

            if (a != 0)
                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)_prevNote[chNum];
        }

        public void Call_LC2D9(byte[] data, byte a, int patNum, int lnNum, int chNum)
        {
            int orn;

            if (a == 0)
                return;

            orn = _orns[a];

            if (orn < 0)
            {
                if (_nOrns >= 15)
                    return;

                _nOrns++;
                orn = _nOrns;
                _orns[a] = orn;
            }

            if (_sampleSet)
            {
                _prevOrn[chNum] = (byte)_prOrn;

                if (!_envEn[chNum])
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 0;
            }

            if (_envEn[chNum] || (_prevOrn[chNum] != orn))
            {
                _prevOrn[chNum] = (byte)orn;

                if (_envEn[chNum])
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = _envT;
                    _vtm.Patterns[patNum].Lines[lnNum].Envelope = _envP;
                }
                else
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Envelope = 15;

                _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Ornament = (byte)orn;
            }
        }

        public void Call_LC283(byte[] data, int patNum, int lnNum, int chNum)
        {
            if (data[_ptr] >= 0x00 && data[_ptr] <= 0x7F)
                Call_LC1D1(data, data[_ptr], patNum, lnNum, chNum);
            else if (data[_ptr] >= 0x80 && data[_ptr] <= 0xFF)
            {
                if (((data[_ptr] >> 1) & 31) != 0)
                    Call_LC2A8(data,(byte)(((data[_ptr] >> 1) & 31)), patNum, lnNum, chNum);

                if ((data[_ptr] & 64) != 0)
                {
                    _temp = data[_ptr + 1] >> 4;

                    if ((data[_ptr] & 1) != 0)
                        _temp = _temp | 16;

                    if (_temp != 0)
                        Call_LC2D9(data, (byte)_temp, patNum, lnNum, chNum);

                    _ptr++;

                    if ((data[_ptr] & 15) != 0)
                        Call_LC1D1(data, (byte)(data[_ptr] & 15), patNum, lnNum, chNum);
                }
            }

            _ptr++;
        }

        public void Call_LC191(byte[] data, int patNum, int lnNum, int chNum)
        {
            _ptr = _ix27[chNum];
            _b6ix0[chNum] = false;

            if (data[_ptr] >= 0x00 && data[_ptr] <= 0x7F)
            {
                _ptr++;
                Call_LC283(data, patNum, lnNum, chNum);
            }
            else if (data[_ptr] >= 0x80 && data[_ptr] <= 0xFF)
                Call_LC2A8(data, (byte)(data[_ptr] & 31), patNum, lnNum, chNum);
        }

        public void PatternInterpreter(byte[] data, int patNum, int lnNum, int chNum)
        {
            byte nt;

            _sampleSet = false;

            if (_ix21[chNum] != 0)
            {
                _ix21[chNum]--;

                if (_b7ix0[chNum])
                    Call_LC191(data, patNum, lnNum, chNum);

                if ((_prevOrn[chNum] > 0) && (_prevOrn[chNum] != 255) && (_orn2Sam[_prevOrn[chNum]] == 0))
                    _orn2Sam[_prevOrn[chNum]] = _prevSamp[chNum];

                return;
            }

            _ptr = _chPtr[chNum];
            _b7ix0[chNum] = false;
            _b6ix0[chNum] = true;

            do
            {
                if (data[_ptr] >= 0x00 && data[_ptr] <= 0x5F)
                {
                    nt = (byte)(data[_ptr] + _cPat.Chn[chNum].Trans + 2);

                    if (nt > 0x5F)
                        nt = 0x5F;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)nt;
                    _prevNote[chNum] = nt;
                    _ix27[chNum] = _ptr;
                    
                    _ptr++;

                    Call_LC283(data, patNum, lnNum, chNum);

                    if (_b6ix0[chNum])
                        _chPtr[chNum] = _ptr;

                    break;
                }
                else if (data[_ptr] >= 0x60 && data[_ptr] <= 0x6E)
                {
                    Call_LC1D1(data, (byte)(data[_ptr] - 0x60), patNum, lnNum, chNum);
                    break;
                }
                else if (data[_ptr] == 0x6F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    _chPtr[chNum] = (ushort)(_ptr + 1);
                    break;
                }
                else if (data[_ptr] >= 0x70 && data[_ptr] <= 0x7F)
                {
                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = -2;
                    Call_LC1D1(data, (byte)(data[_ptr] - 0x6F), patNum, lnNum, chNum);
                    break;
                }
                else if (data[_ptr] >= 0x80 && data[_ptr] <= 0x9F)
                {
                    _chPtr[chNum] = (ushort)(_ptr + 1);

                    if ((data[_ptr] & 16) == 0)
                        _prevNote[chNum] += (byte)(data[_ptr] & 15);
                    else
                        _prevNote[chNum] -= (byte)(data[_ptr] & 15);

                    if (_prevNote[chNum] > 0x5F)
                        _prevNote[chNum] = 0x5F;

                    _vtm.Patterns[patNum].Lines[lnNum].Channel[chNum].Note = (sbyte)_prevNote[chNum];

                    Call_LC191(data, patNum, lnNum, chNum);
                    break;
                }
                else if (data[_ptr] >= 0xA0 && data[_ptr] <= 0xBF)
                {
                    _chPtr[chNum] = (ushort)(_ptr + 1);
                    _ix21[chNum] = (byte)(data[_ptr] & 15);
                    if ((data[_ptr] & 16) == 0)
                        break;

                    if (_ix21[chNum] != 0)
                        _b7ix0[chNum] = true;

                    Call_LC191(data, patNum, lnNum, chNum);
                    break;
                }
                else if (data[_ptr] >= 0xC0 && data[_ptr] <= 0xFF)
                {
                    _chPtr[chNum] = (ushort)(_ptr + 1);
                    _ix27[chNum] = _ptr;

                    Call_LC2A8(data,(byte)(data[_ptr] & 31), patNum, lnNum, chNum);
                    break;
                }
            }
            while (true);

            if ((_prevOrn[chNum] > 0) && (_prevOrn[chNum] != 255) && (_orn2Sam[_prevOrn[chNum]] == 0))
                _orn2Sam[_prevOrn[chNum]] = _prevSamp[chNum];
        }
    }
}
