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
// Version 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TVPattern
    {
        public byte Noise;
        public ushort Envelope;
        public ChannelLine[] Channel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TVSample
    {
        public SampleTick ST;
        public ushort CAddr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TVOrnament
    {
        public sbyte OT;
        public ushort CAddr;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct ChParams
    {
        public ushort AddressInPattern;
        public ushort[] Stek;
        public byte Note;
        public byte PrevNote;
        public byte NoteSkipCounter;
        public byte PrevOrn;
        public byte PrevSam;
        public sbyte Transposit;
        public int CurTick;
        public ushort SamplePointer;
        public ushort OrnamentPointer;
        public byte FXM_Mixer;
        public bool B0e;
        public bool B1e;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Instrument
    {
        public ushort SamplePtr;
        public ushort OrnPtr;
        public byte Mixer;
        public byte SamNum;
        public byte OrnNum;
    }

    public class FxmParamsEventArgs : EventArgs
    {
        public int Length;
        public int LoopInterrupt;
        public int InitialTempo;
        public int PatternSize;
        public int GlobalTranspose;
        public int AmadAndSix;

        public FxmParamsEventArgs()
        {
        }
    }

    public class FXM2VTM
    {
        private ChParams[] _chParams = new ChParams[3];
        private Instrument[] _instruments;
        private int _maxInstr;
        private int[] _skipFreqs = new int[256];
        private int _tick;
        private int _delay;
        private int _curDelay;
        //private int _delayCnt;
        //private int _skipCounter;
        private int _pat;
        private int _line;
        private int _line2;
        private int _minSkip;
        //private int _minChan;
        private int _vpLen;
        private TVPattern[] _virtualPattern;
        private TVSample[] _virtualSample = new TVSample[VTModule.MaxSampleLength];
        private TVOrnament[] _virtualOrnament = new TVOrnament[VTModule.MaxOrnamentLength];
        private byte _noiseBase;
        private byte _ns;
        private int _samCnt;
        private int _samLoop;
        private int _ornLoop;
        private int _ornLen;
        private int _samLen;
        private int _vol;
        private int _maxOrn;
        private int _maxSam;
        private ushort _samAddr;
        private ushort _ornAddr;
        private int _lp;
        private int _patSz;
        private int _ntCorr;
        private int _zxAddr;
        private int _tm;
        private int _amadAndSix;
        private VTM _vtm;

        public FXM2VTM()
        {
        }

        public bool Initialize(byte[] data, int zxAddr, int tm, int andSix, string songName, string authorName, VTM vtm)
        {
            _zxAddr = zxAddr;
            _tm = tm;
            _amadAndSix = andSix;
            _vtm = vtm;

            bool result = true;
            bool flg;
            int i, j, k, tmp, ov;

            _lp = 0;
            _patSz = 64;

            if (_tm == 0)
                GetTime(data);

            tmp = 0;

            if (_tm < 0)
            {
                _tm = -_tm;
                tmp = 1;
            }

            i = 0;

            for (i = 0; i < 3; i++)
            {
                // Address_In_Pattern := WordPtr(@FXM.Index[ZXAddr + i * 2])^;
                _chParams[i].AddressInPattern = BitConverter.ToUInt16(data, _zxAddr + i * 2);
                Array.Resize(ref _chParams[i].Stek, 0);
                _chParams[i].FXM_Mixer = 8;
                _chParams[i].Transposit = 0;
                _chParams[i].SamplePointer = 0;
                _chParams[i].OrnamentPointer = 0;
            }

            Array.Resize(ref _instruments, 1);
            _instruments[0] = new Instrument();
            _instruments[0].SamplePtr = 0;
            _instruments[0].OrnPtr = 0;
            _instruments[0].Mixer = 8;
            _instruments[0].SamNum = 0;
            _instruments[0].OrnNum = 0;
            _maxInstr = 0;

            for (i = 1; i < 256; i++)
                _skipFreqs[i] = 0;

            _ntCorr = -1;
            _tick = 0;

            do
            {
                for (j = 0; j < 3; j++)
                {
                    do
                    {
                        if (data[_chParams[j].AddressInPattern] >= 0x00 &&
                            data[_chParams[j].AddressInPattern] <= 0X7F)
                        {
                            if (data[_chParams[j].AddressInPattern] - 1 + _chParams[j].Transposit == 0)
                                _ntCorr = 0;

                            _chParams[j].AddressInPattern++;
                            i = data[_chParams[j].AddressInPattern];

                            if (i != 0)
                                _skipFreqs[i]++;

                            _chParams[j].AddressInPattern++;
                            flg = false;

                            for (i = 0; i <= _maxInstr; i++)
                            {
                                if ((_instruments[i].SamplePtr == _chParams[j].SamplePointer) &&
                                    (_instruments[i].OrnPtr == _chParams[j].OrnamentPointer) &&
                                    (_instruments[i].Mixer == _chParams[j].FXM_Mixer))
                                {
                                    flg = true;
                                    break;
                                }
                            }

                            if (!flg)
                            {
                                _maxInstr++;
                                Array.Resize(ref _instruments, _maxInstr + 1);
                                _instruments[_maxInstr].SamplePtr = _chParams[j].SamplePointer;
                                _instruments[_maxInstr].OrnPtr = _chParams[j].OrnamentPointer;
                                _instruments[_maxInstr].Mixer = _chParams[j].FXM_Mixer;
                                _instruments[_maxInstr].SamNum = 0;
                                _instruments[_maxInstr].OrnNum = 0;
                            }

                            break;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x80)
                        {
                            // Address_In_Pattern := WordPtr(@Index[Address_In_Pattern + 1])^;
                            _chParams[j].AddressInPattern = BitConverter.ToUInt16(data, _chParams[j].AddressInPattern + 1);

                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x81)
                        {
                            i = _chParams[j].Stek.Length;
                            Array.Resize(ref _chParams[j].Stek, i + 1);
                            _chParams[j].Stek[i] = (ushort)(_chParams[j].AddressInPattern + 3);
                            // Address_In_Pattern := WordPtr(@Index[Address_In_Pattern + 1])^
                            _chParams[j].AddressInPattern = BitConverter.ToUInt16(data, _chParams[j].AddressInPattern + 1);
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x82)
                        {
                            i = _chParams[j].Stek.Length;
                            Array.Resize(ref _chParams[j].Stek, i + 2);
                            _chParams[j].AddressInPattern++;
                            _chParams[j].Stek[i] = data[_chParams[j].AddressInPattern];
                            _chParams[j].AddressInPattern++;
                            _chParams[j].Stek[i + 1] = _chParams[j].AddressInPattern;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x83)
                        {
                            i = _chParams[j].Stek.Length;
                            _chParams[j].Stek[i - 2]--;
                            if ((_chParams[j].Stek[i - 2] & 255) != 0)
                            {
                                _chParams[j].AddressInPattern = _chParams[j].Stek[i - 1];
                            }
                            else
                            {
                                Array.Resize(ref _chParams[j].Stek, i - 2);
                                _chParams[j].AddressInPattern++;
                            }
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x84)
                        {
                            _chParams[j].AddressInPattern++;
                            // PlParams.FXM.Noise_Base := Index[Address_In_Pattern];
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x85)
                        {
                            _chParams[j].AddressInPattern++;
                            _chParams[j].FXM_Mixer = (byte)(data[_chParams[j].AddressInPattern] & 9);
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x86)
                        {
                            _chParams[j].AddressInPattern++;
                            // OrnamentPointer := WordPtr(@Index[Address_In_Pattern])^;
                            _chParams[j].OrnamentPointer = BitConverter.ToUInt16(data, _chParams[j].AddressInPattern);
                            _chParams[j].AddressInPattern += 2;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x87)
                        {
                            _chParams[j].AddressInPattern++;
                            // SamplePointer := WordPtr(@Index[Address_In_Pattern])^;
                            _chParams[j].SamplePointer = BitConverter.ToUInt16(data, _chParams[j].AddressInPattern);
                            _chParams[j].AddressInPattern += 2;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x88)
                        {
                            _chParams[j].AddressInPattern++;
                            _chParams[j].Transposit = (sbyte)data[_chParams[j].AddressInPattern];
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x89)
                        {
                            i = _chParams[j].Stek.Length;
                            _chParams[j].AddressInPattern = _chParams[j].Stek[i - 1];
                            Array.Resize(ref _chParams[j].Stek, i - 1);
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x8A)
                        {
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x8B)
                        {
                            // b0e := True;
                            // b1e := False
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x8C)
                        {
                            // b0e := False;
                            // b1e := False
                            _chParams[j].AddressInPattern += 3;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x8D)
                        {
                            _chParams[j].AddressInPattern++;
                            // PlParams.FXM.Noise_Base :=
                            // (PlParams.FXM.Noise_Base + Index[Address_In_Pattern])
                            // and PlParams.FXM.amad_andsix;
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x8E)
                        {
                            _chParams[j].AddressInPattern++;
                            _chParams[j].Transposit = (sbyte)(_chParams[j].Transposit + data[_chParams[j].AddressInPattern]);
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x8F)
                        {
                            i = _chParams[j].Stek.Length;
                            Array.Resize(ref _chParams[j].Stek, i + 1);
                            _chParams[j].Stek[i] = (ushort)_chParams[j].Transposit;
                            _chParams[j].AddressInPattern++;
                        }
                        else if (data[_chParams[j].AddressInPattern] == 0x90)
                        {
                            i = _chParams[j].Stek.Length;
                            _chParams[j].Transposit = (sbyte)_chParams[j].Stek[i - 1];
                            Array.Resize(ref _chParams[j].Stek, i - 1);
                            _chParams[j].AddressInPattern++;
                        }
                        else
                        {
                            _chParams[j].AddressInPattern++;
                        }
                    }
                    while (true);
                }
                _tick++;
            }
            while (_tick != _tm);
            
            _delay = 1;
            _tick = _skipFreqs[1];
            
            for (i = 2; i < 256; i++)
            {
                if (_skipFreqs[i] > _tick)
                {
                    _delay = i;
                    _tick = _skipFreqs[i];
                }
            }

            FxmParamsEventArgs e = new FxmParamsEventArgs
            {
                Length = tmp == 0 ? _tm : 0,
                LoopInterrupt = tmp == 0 ? _lp : 0,
                InitialTempo = _delay,
                PatternSize = _patSz,
                GlobalTranspose = _ntCorr,
                AmadAndSix = _amadAndSix
            };

            AppEvents.SendEvent(EventType.FXMDialog, e);

            _tm = e.Length; // Length
            _lp = e.LoopInterrupt; // Loop interrupt
            _delay = e.InitialTempo; // Initial tempo (5)
            _patSz = e.PatternSize; // Pattern size (64)
            _ntCorr = e.GlobalTranspose; // Global transpose (-1)
            _amadAndSix = e.AmadAndSix; // amad_andsix (31)

            _maxSam = 0;
            _maxOrn = 0;

            for (i = 1; i <= _maxInstr; i++)
            {
                j = 0;
                flg = false;
                _samCnt = 1;
                _samLoop = _ornLoop = 255;
                _ornLen = _samLen = _vol = 0;
                ov = 0;
                tmp = _instruments[i].Mixer;

                _samAddr = _instruments[i].SamplePtr;
                _ornAddr = _instruments[i].OrnPtr;

                while (j < VTModule.MaxSampleLength)
                {
                    _virtualSample[j].CAddr = _samAddr;
                    _virtualSample[j].ST = new SampleTick();
                    _virtualSample[j].ST.Ton_Accumulation = true;
                    _virtualOrnament[j].CAddr = _ornAddr;

                    if (_samAddr != 0)
                    {
                        _samCnt--;

                        if (_samCnt == 0)
                        {
                            do
                            {
                                if (data[_samAddr] >= 0x00 &&
                                    data[_samAddr] <= 0x1D)
                                {
                                    _vol = data[_samAddr];
                                    _virtualSample[j].ST.Amplitude = (byte)_vol;
                                    _samAddr++;
                                    _samCnt = data[_samAddr];
                                    _samAddr++;
                                    break;
                                }
                                else if (data[_samAddr] == 0x80)
                                {
                                    // SamAddr := WordPtr(@Index[SamAddr + 1])^;
                                    _samAddr = BitConverter.ToUInt16(data, _samAddr + 1);
                                    _samLen = j;

                                    for (k = 0; k < j; k++)
                                    {
                                        if (_virtualSample[k].CAddr == _samAddr)
                                        {
                                            _samLoop = k;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    _vol = data[_samAddr] - 0x32;
                                    _virtualSample[j].ST.Amplitude = (byte)_vol;
                                    _samAddr++;
                                    _samCnt = 1;
                                    break;
                                }
                            }
                            while (true);
                        }
                        else
                        {
                            _virtualSample[j].ST.Amplitude = (byte)_vol;
                        }
                    }
                    else
                    {
                        _virtualSample[j].ST.Amplitude = 0;
                    }

                    if (_ornAddr != 0)
                    {
                        bool quit = false;

                        do
                        {
                            switch (data[_ornAddr])
                            {
                                case 0x80:
                                    // OrnAddr := WordPtr(@Index[OrnAddr + 1])^;
                                    _ornAddr = BitConverter.ToUInt16(data, _ornAddr + 1);
                                    _ornLen = j;

                                    for (k = 0; k < j; k++)
                                    {
                                        if (_virtualOrnament[k].CAddr == _ornAddr)
                                        {
                                            _ornLoop = k;
                                            break;
                                        }
                                    }
                                    break;
                                case 0x82:
                                    _ornAddr++;
                                    flg = true;
                                    break;
                                case 0x83:
                                    _ornAddr++;
                                    flg = false;
                                    break;
                                case 0x84:
                                    _ornAddr++;
                                    tmp ^= 9;
                                    break;
                                default:
                                    if (flg)
                                    {
                                        ov += ((sbyte)data[_ornAddr]);
                                        _virtualOrnament[j].OT = (sbyte)ov;
                                    }
                                    else
                                    {
                                        _virtualOrnament[j].OT = (sbyte)ov;
                                        _virtualSample[j].ST.AddToTone = ((sbyte)data[_ornAddr]);
                                    }
                                    _ornAddr++;
                                    quit = true;
                                    break;
                            }
                        }
                        while (!quit);
                    }
                    else
                    {
                        _virtualOrnament[j].OT = 0;
                        _virtualSample[j].ST.AddToTone = 0;
                    }

                    _virtualSample[j].ST.Mixer_Ton = (tmp & 1) == 0;
                    _virtualSample[j].ST.Mixer_Noise = (tmp & 8) == 0;
                    j++;
                }

                if (_samLen == 0)
                    _samLen = VTModule.MaxSampleLength;

                if (_samLoop == 255)
                {
                    _samLoop = _samLen - 1;
                    _virtualSample[_samLoop].ST.Ton_Accumulation = false;
                }

                if (_ornLen == 0)
                    _ornLen = VTModule.MaxOrnamentLength;

                if (_ornLoop == 255)
                    _ornLoop = _ornLen - 1;

                flg = false;

                for (j = 0; j < _ornLen; j++)
                {
                    if (_virtualOrnament[j].OT != 0)
                    {
                        flg = true;
                        break;
                    }
                }

                if (flg)
                {
                    flg = false;

                    for (k = 1; k <= _maxOrn; k++)
                    {
                        flg = (_vtm.Ornaments[k].Length == _ornLen) && (_vtm.Ornaments[k].Loop == _ornLoop);
                        
                        if (flg)
                        {
                            for (j = 0; j < _ornLen; j++)
                            {
                                flg = _vtm.Ornaments[k].Offsets[j] == _virtualOrnament[j].OT;
                                
                                if (!flg)
                                    break;
                            }
                        }

                        if (flg)
                        {
                            _instruments[i].OrnNum = (byte)k;
                            break;
                        }
                    }

                    if (!flg && (_maxOrn < 15))
                    {
                        _maxOrn++;
                        _instruments[i].OrnNum = (byte)_maxOrn;
                        _vtm.Ornaments[_maxOrn] = new Ornament();
                        _vtm.Ornaments[_maxOrn].Loop = _ornLoop;
                        _vtm.Ornaments[_maxOrn].Length = _ornLen;

                        for (j = 0; j < _ornLen; j++)
                            _vtm.Ornaments[_maxOrn].Offsets[j] = _virtualOrnament[j].OT;
                    }
                }

                flg = false;
                
                for (k = 1; k <= _maxSam; k++)
                {
                    flg = (_vtm.Samples[k].Length == _samLen) &&
                        (_vtm.Samples[k].Loop == _samLoop);
                    
                    if (flg)
                    {
                        for (j = 0; j < _samLen; j++)
                        {
                            flg = (_vtm.Samples[k].Ticks[j].AddToTone == _virtualSample[j].ST.AddToTone) &&
                                (_vtm.Samples[k].Ticks[j].Amplitude == _virtualSample[j].ST.Amplitude) &&
                                (_vtm.Samples[k].Ticks[j].Mixer_Ton == _virtualSample[j].ST.Mixer_Ton) &&
                                (_vtm.Samples[k].Ticks[j].Mixer_Noise == _virtualSample[j].ST.Mixer_Noise);
                            
                            if (!flg)
                                break;
                        }
                    }

                    if (flg)
                    {
                        _instruments[i].SamNum = (byte)k;
                        break;
                    }
                }

                if (!flg && (_maxSam < 31))
                {
                    _maxSam++;
                    _instruments[i].SamNum = (byte)_maxSam;
                    
                    if ((_samLoop > VTModule.MaxSampleLength - 1) || (_samLen > VTModule.MaxSampleLength))
                        continue;

                    _vtm.Samples[_maxSam] = new Sample();
                    _vtm.Samples[_maxSam].Loop = (byte)_samLoop;
                    _vtm.Samples[_maxSam].Length = (byte)_samLen;

                    for (j = 0; j < _samLen; j++)
                        _vtm.Samples[_maxSam].Ticks[j] = _virtualSample[j].ST;
                }
            }

            _vtm.InitialDelay = (byte)_delay;
            _vtm.NoteTable = NoteTableType.SoundTracker;
            _vtm.Title = songName;
            _vtm.Author = authorName;

            _vpLen = _delay * _patSz;

            Array.Resize(ref _virtualPattern, _vpLen);

            _noiseBase = 0;

            for (i = 0; i < 3; i++)
            {
                // Address_In_Pattern := WordPtr(@FXM.Index[ZXAddr + i * 2])^;
                _chParams[i].AddressInPattern = BitConverter.ToUInt16(data, _zxAddr + i * 2);
                _chParams[i].Transposit = 0;
                _chParams[i].NoteSkipCounter = 1;
                _chParams[i].PrevOrn = 255;
                _chParams[i].PrevSam = 255;
                _chParams[i].SamplePointer = 0;
                _chParams[i].OrnamentPointer = 0;
                _chParams[i].FXM_Mixer = 8;
                _chParams[i].PrevNote = 255;
                _chParams[i].B0e = false;
                _chParams[i].B1e = false;
                Array.Resize(ref _chParams[i].Stek, 0);
            }

            _pat = -1;
            _line2 = _patSz;
            _curDelay = _delay;
            _tick = 0;

            while (_pat < VTModule.MaxPatternCount)
            {
                if (_tick >= _tm)
                {
                    _vtm.Patterns[_pat].Length = _line2;
                    _pat++;
                    break;
                }

                for (i = 0; i < _vpLen; i++)
                {
                    _virtualPattern[i].Envelope = 0;
                    _virtualPattern[i].Channel = new ChannelLine[3];

                    for (j = 0; j < 3; j++)
                        _virtualPattern[i].Channel[j] = new ChannelLine();
                }

                _line = 0;
                
                while (_line < _vpLen)
                {
                    for (j = 0; j < 3; j++)
                        PatternInterpreter(data, j);

                    _virtualPattern[_line].Noise = _noiseBase;
                    _line++;
                }

                _line = 0;

                while ((_pat < VTModule.MaxPatternCount) && (_line < _vpLen) && (_tick < _tm))
                {
                    while (_line2 >= _patSz)
                    {
                        _line2 -= _patSz;
                        _pat++;

                        if (_pat >= VTModule.MaxPatternCount)
                            break;

                        ClearPat();
                    }
                    if ((_tick != 0) && (_line2 != 0) && (_tick == _lp))
                    {
                        _vtm.Patterns[_pat].Length = _line2;
                        _line2 = 0;
                        _pat++;

                        if (_pat >= VTModule.MaxPatternCount)
                            break;

                        _vtm.Positions.Loop = _pat;
                        ClearPat();
                    }
                    if (_pat >= VTModule.MaxPatternCount)
                        break;

                    for (j = 0; j < 3; j++)
                        _vtm.Patterns[_pat].Lines[_line2].Channel[j] = _virtualPattern[_line].Channel[j];

                    _ns = _virtualPattern[_line].Noise;
                    _vtm.Patterns[_pat].Lines[_line2].Noise = _ns;
                    _minSkip = 0;

                    do
                    {
                        _line++;
                        _tick++;
                        _minSkip++;
                        flg = _line >= _vpLen;

                        if (!flg)
                        {
                            for (j = 0; j < 3; j++)
                            {
                                if (_virtualPattern[_line].Channel[j].Note != -1)
                                {
                                    flg = true;
                                    break;
                                }
                            }
                        }

                        if (!flg)
                        {
                            for (j = 0; j < 3; j++)
                            {
                                if (_virtualPattern[_line].Channel[j].Envelope == 15)
                                {
                                    flg = true;
                                    break;
                                }
                            }
                        }

                        if (!flg)
                        {
                            for (j = 0; j < 3; j++)
                            {
                                if (_virtualPattern[_line].Channel[j].Ornament != 0)
                                {
                                    flg = true;
                                    break;
                                }
                            }
                        }

                        if (!flg)
                            flg = _virtualPattern[_line].Noise != _ns;

                    }
                    while (!flg);
                    
                    if (_delay > _minSkip)
                        i = _minSkip;
                    else
                    {
                        i = _tick % _delay;

                        if (i == 0)
                            i = _delay;

                        _line -= _minSkip - i;
                        _tick -= _minSkip - i;
                    }

                    if ((i != _curDelay) || (_line2 == 0))
                    {
                        _curDelay = i;
                        _vtm.Patterns[_pat].Lines[_line2].Channel[0].AdditionalCommand.Number = 11;
                        _vtm.Patterns[_pat].Lines[_line2].Channel[0].AdditionalCommand.Parameter = (byte)i;
                    }

                    _line2++;
                }
            }

            _vtm.Positions.Length = _pat;

            for (i = 0; i < _pat; i++)
                _vtm.Positions.Value[i] = i;

            result = _delay == 0;

            return result;
        }

        public bool LoopFound(byte[] data, ushort j11, ushort j22, ushort j33)
        {
            bool result;
            uint j1,  j2, j3;
            byte a1, a2, a3;
            bool f71, f72, f73, f61, f62, f63;
            ushort[] fxms1 = new ushort[0];
            ushort[] fxms2 = new ushort[0];
            ushort[] fxms3 = new ushort[0];
            int k;
            int tr;
            // j1:= WordPtr(@FXM.Index[ZXAddr]) ^;
            // j2:= WordPtr(@FXM.Index[ZXAddr + 2]) ^;
            // j3:= WordPtr(@FXM.Index[ZXAddr + 4]) ^;
            j1 = BitConverter.ToUInt16(data, _zxAddr);
            j2 = BitConverter.ToUInt16(data, _zxAddr + 2);
            j3 = BitConverter.ToUInt16(data, _zxAddr + 4);
            a1 = a2 = a3 = 1;
            f71 = f72 = f73 = f61 = f62 = f63 = false;
            tr = 0;

            do
            {
                if ((j1 == j11) && (j2 == j22) && (j3 == j33))
                {
                    result = true;
                    _lp = tr;
                    return result;
                }

                a1--;

                if (a1 == 0)
                {
                    f71 = false;
                    f61 = false;

                    do
                    {
                        if ((data[j1] >= 0x00 && data[j1] <= 0x7F) ||
                            (data[j1] >= 0x8F && data[j1] <= 0xFF))
                        {
                            j1++;
                            a1 = data[j1];
                            j1++;
                            break;
                        }
                        else if (data[j1] == 0x80)
                        {
                            // j1 := WordPtr(@FXM.Index[j1 + 1])^;
                            j1 = BitConverter.ToUInt16(data, (int)j1 + 1);
                            f71 = true;
                        }
                        else if (data[j1] == 0x81)
                        {
                            k = fxms1.Length;
                            Array.Resize(ref fxms1, k + 1);
                            fxms1[k] = (ushort)(j1 + 3);
                            // j1 := WordPtr(@FXM.Index[j1 + 1])^
                            j1 = BitConverter.ToUInt16(data, (int)j1 + 1);
                        }
                        else if (data[j1] == 0x82)
                        {
                            if ((j1 == j11) && (j2 == j22) && (j3 == j33))
                            {
                                result = true;
                                _lp = tr;
                                return result;
                            }

                            k = fxms1.Length;
                            Array.Resize(ref fxms1, k + 2);
                            j1++;
                            fxms1[k] = data[j1];
                            j1++;
                            fxms1[k + 1] = (ushort)j1;
                        }
                        else if (data[j1] == 0x83)
                        {
                            k = fxms1.Length;
                            fxms1[k - 2]--;

                            if ((fxms1[k - 2] & 255) != 0)
                            {
                                j1 = fxms1[k - 1];
                                f61 = true;
                            }
                            else
                            {
                                Array.Resize(ref fxms1, k - 2);
                                j1++;
                            }
                        }
                        else if (data[j1] == 0x84 ||
                                 data[j1] == 0x85 ||
                                 data[j1] == 0x88 ||
                                 data[j1] == 0x8D ||
                                 data[j1] == 0x8E)
                        {
                            j1 += 2;
                        }
                        else if (data[j1] == 0x86 ||
                                 data[j1] == 0x87 ||
                                 data[j1] == 0x8C)
                        {
                            j1 += 3;
                        }
                        else if (data[j1] == 0x89)
                        {
                            k = fxms1.Length;
                            j1 = fxms1[k - 1];
                            Array.Resize(ref fxms1, k - 1);
                        }
                        else if (data[j1] == 0x8A ||
                                 data[j1] == 0x8B)
                        {
                            j1++;
                        }
                    }
                    while (true);
                }

                a2--;

                if (a2 == 0)
                {
                    f72 = false;
                    f62 = false;

                    do
                    {
                        if ((data[j2] >= 0x00 && data[j2] <= 0x7F) ||
                            (data[j2] >= 0x8F && data[j2] <= 0xFF))
                        {
                            j2++;
                            a2 = data[j2];
                            j2++;
                            break;
                        }
                        else if (data[j2] == 0x80)
                        {
                            // j2 := WordPtr(@FXM.Index[j2 + 1])^;
                            j2 = BitConverter.ToUInt16(data, (int)j2 + 1);
                            f72 = true;
                        }
                        else if (data[j2] == 0x81)
                        {
                            k = fxms2.Length;
                            Array.Resize(ref fxms2, k + 1);
                            fxms2[k] = (ushort)(j2 + 3);
                            // j2 := WordPtr(@FXM.Index[j2 + 1])^
                            j2 = BitConverter.ToUInt16(data, (int)j2 + 1);
                        }
                        else if (data[j2] == 0x82)
                        {
                            if ((j1 == j11) && (j2 == j22) && (j3 == j33))
                            {
                                result = true;
                                _lp = tr;
                                return result;
                            }

                            k = fxms2.Length;
                            Array.Resize(ref fxms2, k + 2);
                            j2++;
                            fxms2[k] = data[j2];
                            j2++;
                            fxms2[k + 1] = (ushort)j2;
                        }
                        else if (data[j2] == 0x83)
                        {
                            k = fxms2.Length;
                            fxms2[k - 2]--;
                            if ((fxms2[k - 2] & 255) != 0)
                            {
                                j2 = fxms2[k - 1];
                                f62 = true;
                            }
                            else
                            {
                                Array.Resize(ref fxms2, k - 2);
                                j2++;
                            }
                        }
                        else if (data[j2] == 0x84 ||
                                 data[j2] == 0x85 ||
                                 data[j2] == 0x88 ||
                                 data[j2] == 0x8D ||
                                 data[j2] == 0x8E)
                        {
                            j2 += 2;
                        }
                        else if (data[j2] == 0x86 ||
                                 data[j2] == 0x87 ||
                                 data[j2] == 0x8C)
                        {
                            j2 += 3;
                        }
                        else if (data[j2] == 0x89)
                        {
                            k = fxms2.Length;
                            j2 = fxms2[k - 1];
                            Array.Resize(ref fxms2, k - 1);
                        }
                        else if (data[j2] == 0x8A ||
                                 data[j2] == 0x8B)
                        {
                            j2++;
                        }
                    }
                    while (true);
                }

                a3--;

                if (a3 == 0)
                {
                    f73 = false;
                    f63 = false;

                    do
                    {
                        if ((data[j3] >= 0x00 && data[j3] <= 0x7F) ||
                            (data[j3] >= 0x8F && data[j3] <= 0xFF))
                        {
                            j3++;
                            a3 = data[j3];
                            j3++;
                            break;
                        }
                        else if (data[j3] == 0x80)
                        {
                            // j3 := WordPtr(@FXM.Index[j3 + 1])^;
                            j3 = BitConverter.ToUInt16(data, (int)j3 + 1);
                            f73 = true;
                        }
                        else if (data[j3] == 0x81)
                        {
                            k = fxms3.Length;
                            Array.Resize(ref fxms3, k + 1);
                            fxms3[k] = (ushort)(j3 + 3);
                            // j3 := WordPtr(@FXM.Index[j3 + 1])^;
                            j3 = BitConverter.ToUInt16(data, (int)j3 + 1);
                        }
                        else if (data[j3] == 0x82)
                        {
                            if ((j1 == j11) && (j2 == j22) && (j3 == j33))
                            {
                                result = true;
                                _lp = tr;
                                return result;
                            }

                            k = fxms3.Length;
                            Array.Resize(ref fxms3, k + 2);
                            j3++;
                            fxms3[k] = data[j3];
                            j3++;
                            fxms3[k + 1] = (ushort)j3;
                        }
                        else if (data[j3] == 0x83)
                        {
                            k = fxms3.Length;
                            fxms3[k - 2]--;

                            if ((fxms3[k - 2] & 255) != 0)
                            {
                                j3 = fxms3[k - 1];
                                f63 = true;
                            }
                            else
                            {
                                Array.Resize(ref fxms3, k - 2);
                                j3++;
                            }
                        }
                        else if (data[j3] == 0x84 ||
                                 data[j3] == 0x85 ||
                                 data[j3] == 0x88 ||
                                 data[j3] == 0x8D ||
                                 data[j3] == 0x8E)
                        {
                            j3 += 2;
                        }
                        else if (data[j3] == 0x86 ||
                                 data[j3] == 0x87 ||
                                 data[j3] == 0x8C)
                        {
                            j3 += 3;
                        }
                        else if (data[j3] == 0x89)
                        {
                            k = fxms3.Length;
                            j3 = fxms3[k - 1];
                            Array.Resize(ref fxms3, k - 1);
                        }
                        else if (data[j3] == 0x8A ||
                                 data[j3] == 0x8B)
                        {
                            j3++;
                        }
                    }
                    while (true);
                }
                tr++;
            }
            while (!((f71 && (f72 || f62) && (f73 || f63)) ||
                ((f71 || f61) && f72 && (f73 || f63)) ||
                ((f71 || f61) && (f72 || f62) && f73)));

            result = false;
            
            return result;
        }

        public void GetTime(byte[] data)
        {
            int k = 0;
            uint j1, j2, j3;
            sbyte a1, a2, a3;
            bool f71, f72, f73;
            bool f61, f62, f63;
            ushort j11, j22, j33;
            ushort[] fxms1 = new ushort[0];
            ushort[] fxms2 = new ushort[0];
            ushort[] fxms3 = new ushort[0];
            // j1:= WordPtr(@Index[ZXAddr]) ^;
            // j2:= WordPtr(@Index[ZXAddr + 2]) ^;
            // j3:= WordPtr(@Index[ZXAddr + 4]) ^;
            j1 = BitConverter.ToUInt16(data, _zxAddr);
            j2 = BitConverter.ToUInt16(data, _zxAddr + 2);
            j3 = BitConverter.ToUInt16(data, _zxAddr + 4);
            a1 = a2 = a3 = 1;
            f71 = f72 = f73 = f61 = f62 = f63 = false;
            j11 = j22 = j33 = 0;

            do
            {
                a1--;

                if (a1 == 0)
                {
                    f71 = false;
                    f61 = false;

                    do
                    {
                        if ((data[j1] >= 0x00 && data[j1] <= 0x7F) ||
                            (data[j1] >= 0x8F && data[j1] <= 0xFF))
                        {
                            j1++;
                            a1 = (sbyte)data[j1];
                            j1++;
                            break;
                        }
                        else if (data[j1] == 0x80)
                        {
                            // j1 := WordPtr(@Index[j1 + 1])^;
                            j1 = BitConverter.ToUInt16(data, (int)j1 + 1);
                            j11 = (ushort)j1;
                            f71 = true;
                        }
                        else if (data[j1] == 0x81)
                        {
                            k = fxms1.Length;
                            Array.Resize(ref fxms1, k + 1);
                            fxms1[k] = (ushort)(j1 + 3);
                            // j1 := WordPtr(@Index[j1 + 1])^;
                            j1 = BitConverter.ToUInt16(data, (int)j1 + 1);
                        }
                        else if (data[j1] == 0x82)
                        {
                            k = fxms1.Length;
                            Array.Resize(ref fxms1, k + 2);
                            j1++;
                            fxms1[k] = data[j1];
                            j1++;
                            fxms1[k + 1] = (ushort)j1;
                        }
                        else if (data[j1] == 0x83)
                        {
                            k = fxms1.Length;
                            fxms1[k - 2]--;

                            if ((fxms1[k - 2] & 255) != 0)
                            {
                                j1 = fxms1[k - 1];
                                j11 = (ushort)(j1 - 2);
                                f61 = true;
                            }
                            else
                            {
                                Array.Resize(ref fxms1, k - 2);
                                j1++;
                            }
                        }
                        else if (data[j1] == 0x84 ||
                                 data[j1] == 0x85 ||
                                 data[j1] == 0x88 ||
                                 data[j1] == 0x8D ||
                                 data[j1] == 0x8E)
                        {
                            j1 += 2;
                        }
                        else if (data[j1] == 0x86 ||
                                 data[j1] == 0x87 ||
                                 data[j1] == 0x8C)
                        {
                            j1 += 3;
                        }
                        else if (data[j1] == 0x89)
                        {
                            k = fxms1.Length;
                            j1 = fxms1[k - 1];
                            Array.Resize(ref fxms1, k - 1);
                        }
                        else if (data[j1] == 0x8A ||
                                 data[j1] == 0x8B)
                        {
                            j1++;
                        }
                    }
                    while (true);
                }

                a2--;

                if (a2 == 0)
                {
                    f72 = false;
                    f62 = false;

                    do
                    {
                        if ((data[j2] >= 0x00 && data[j2] <= 0x7F) ||
                            (data[j2] >= 0x8F && data[j2] <= 0xFF))
                        {
                            j2++;
                            a2 = (sbyte)data[j2];
                            j2++;
                            break;
                        }
                        else if (data[j2] == 0x80)
                        {
                            // j2 := WordPtr(@Index[j2 + 1])^;
                            j2 = BitConverter.ToUInt16(data, (int)j2 + 1);
                            j22 = (ushort)j2;
                            f72 = true;
                        }
                        else if (data[j2] == 0x81)
                        {
                            k = fxms2.Length;
                            Array.Resize(ref fxms2, k + 1);
                            fxms2[k] = (ushort)(j2 + 3);
                            // j2 := WordPtr(@Index[j2 + 1])^;
                            j2 = BitConverter.ToUInt16(data, (int)j2 + 1);
                        }
                        else if (data[j2] == 0x82)
                        {
                            k = fxms2.Length;
                            Array.Resize(ref fxms2, k + 2);
                            j2++;
                            fxms2[k] = data[j2];
                            j2++;
                            fxms2[k + 1] = (ushort)j2;
                        }
                        else if (data[j2] == 0x83)
                        {
                            k = fxms2.Length;
                            fxms2[k - 2]--;
                            if ((fxms2[k - 2] & 255) != 0)
                            {
                                j2 = fxms2[k - 1];
                                j22 = (ushort)(j2 - 2);
                                f62 = true;
                            }
                            else
                            {
                                Array.Resize(ref fxms2, k - 2);
                                j2++;
                            }
                        }
                        else if (data[j2] == 0x84 ||
                                 data[j2] == 0x85 ||
                                 data[j2] == 0x88 ||
                                 data[j2] == 0x8D ||
                                 data[j2] == 0x8E)
                        {
                            j2 += 2;
                        }
                        else if (data[j2] == 0x86 ||
                                 data[j2] == 0x87 ||
                                 data[j2] == 0x8C)
                        {
                            j2 += 3;
                        }
                        else if (data[j2] == 0x89)
                        {
                            k = fxms2.Length;
                            j2 = fxms2[k - 1];
                            Array.Resize(ref fxms2, k - 1);
                        }
                        else if (data[j2] == 0x8A ||
                                 data[j2] == 0x8B)
                        {
                            j2++;
                        }
                    }
                    while (true);
                }

                a3--;

                if (a3 == 0)
                {
                    f73 = false;
                    f63 = false;

                    do
                    {
                        if ((data[j3] >= 0x00 && data[j3] <= 0x7F) ||
                            (data[j3] >= 0x8F && data[j3] <= 0xFF))
                        {
                            j3++;
                            a3 = (sbyte)data[j3];
                            j3++;
                            break;
                        }
                        else if (data[j3] == 0x80)
                        {
                            // j3 := WordPtr(@Index[j3 + 1])^;
                            j3 = BitConverter.ToUInt16(data, (int)j3 + 1);
                            j33 = (ushort)j3;
                            f73 = true;
                        }
                        else if (data[j3] == 0x81)
                        {
                            k = fxms3.Length;
                            Array.Resize(ref fxms3, k + 1);
                            fxms3[k] = (ushort)(j3 + 3);
                            // j3 := WordPtr(@Index[j3 + 1])^;
                            j3 = BitConverter.ToUInt16(data, (int)j3 + 1);
                        }
                        else if (data[j3] == 0x82)
                        {
                            k = fxms3.Length;
                            Array.Resize(ref fxms3, k + 2);
                            j3++;
                            fxms3[k] = data[j3];
                            j3++;
                            fxms3[k + 1] = (ushort)j3;
                        }
                        else if (data[j3] == 0x83)
                        {
                            k = fxms3.Length;
                            fxms3[k - 2]--;
                            if ((fxms3[k - 2] & 255) != 0)
                            {
                                j3 = fxms3[k - 1];
                                j33 = (ushort)(j3 - 2);
                                f63 = true;
                            }
                            else
                            {
                                Array.Resize(ref fxms3, k - 2);
                                j3++;
                            }
                        }
                        else if (data[j3] == 0x84 ||
                                 data[j3] == 0x85 ||
                                 data[j3] == 0x88 ||
                                 data[j3] == 0x8D ||
                                 data[j3] == 0x8E)
                        {
                            j3 += 2;
                        }
                        else if (data[j3] == 0x86 ||
                                 data[j3] == 0x87 ||
                                 data[j3] == 0x8C)
                        {
                            j3 += 3;
                        }
                        else if (data[j3] == 0x89)
                        {
                            k = fxms3.Length;

                            if (k <= 0)
                                return;

                            j3 = fxms3[k - 1];
                            Array.Resize(ref fxms3, k - 1);
                        }
                        else if (data[j3] == 0x8A ||
                                 data[j3] == 0x8B)
                        {
                            j3++;
                        }
                    }
                    while (true);
                }

                _tm++;

                if (_tm > 180000)
                {
                    _tm = -14999;
                    break;
                }
            }
            while (!((
                (f71 && (f72 || f62) && (f73 || f63)) ||
                ((f71 || f61) && f72 && (f73 || f63)) ||
                ((f71 || f61) && (f72 || f62) && f73)) &&
                LoopFound(data, j11, j22, j33)));

            _tm--;
        }

        public bool PatternInterpreter(byte[] data, int chan)
        {
            bool result;
            byte b;
            int i;

            _chParams[chan].NoteSkipCounter--;

            if (_chParams[chan].NoteSkipCounter != 0)
            {
                result = false;
                // GetRegisters(chan)
                return result;
            }
            else
            {
                do
                {
                    if (data[_chParams[chan].AddressInPattern] >= 0x00 &&
                        data[_chParams[chan].AddressInPattern] <= 0x7F)
                    {
                        if (data[_chParams[chan].AddressInPattern] != 0)
                        {
                            _chParams[chan].Note = (byte)(data[_chParams[chan].AddressInPattern] - 1 + _chParams[chan].Transposit);
                            b = (byte)(_chParams[chan].Note + _ntCorr);
                            
                            if ((sbyte)b < 0)
                                b = 0;
                            else if (b > 95)
                                b = 95;

                            // Ton := FXM_Table[b];
                            // b3e := False;
                        }
                        else
                            b = 254;

                        if (!_chParams[chan].B1e || (b == 254) || (_chParams[chan].PrevNote != b))
                        {
                            _chParams[chan].PrevNote = b;
                            _virtualPattern[_line].Channel[chan].Note = (sbyte)b;
                        }
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].NoteSkipCounter = data[_chParams[chan].AddressInPattern];
                        _chParams[chan].AddressInPattern++;
                        
                        for (i = 0; i <= _maxInstr; i++)
                        {
                            if ((_instruments[i].SamplePtr == _chParams[chan].SamplePointer) &&
                                (_instruments[i].OrnPtr == _chParams[chan].OrnamentPointer) &&
                                (_instruments[i].Mixer == _chParams[chan].FXM_Mixer))
                            {
                                if (_instruments[i].SamNum != _chParams[chan].PrevSam)
                                {
                                    _chParams[chan].PrevSam = _instruments[i].SamNum;
                                    _virtualPattern[_line].Channel[chan].Sample = _instruments[i].SamNum;
                                    _virtualPattern[_line].Channel[chan].Note = (sbyte)b;
                                }

                                if (_instruments[i].OrnNum != _chParams[chan].PrevOrn)
                                {
                                    _chParams[chan].PrevOrn = _instruments[i].OrnNum;
                                    _virtualPattern[_line].Channel[chan].Ornament = _instruments[i].OrnNum;
                                    
                                    if (_instruments[i].OrnNum == 0)
                                        _virtualPattern[_line].Channel[chan].Envelope = 15;
                                }
                                break;
                            }
                        }

                        if (!_chParams[chan].B1e)
                        {
                            _chParams[chan].B1e = _chParams[chan].B0e;
                            // Point_In_Sample := SamplePointer;
                            // Volume := Index[Point_In_Sample];
                            // Inc(Point_In_Sample);
                            // Sample_Tik_Counter := Index[Point_In_Sample];
                            // Inc(Point_In_Sample);
                            // RealGetRegisters(Chan)
                            // else
                            // GetRegisters(Chan)
                        }

                        result = true;
                        return result;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x80)
                    {
                        // Address_In_Pattern := WordPtr(@Index[Address_In_Pattern + 1])^;
                        _chParams[chan].AddressInPattern = BitConverter.ToUInt16(data, _chParams[chan].AddressInPattern + 1);
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x81)
                    {
                        i = _chParams[chan].Stek.Length;
                        Array.Resize(ref _chParams[chan].Stek, i + 1);
                        _chParams[chan].Stek[i] = (ushort)(_chParams[chan].AddressInPattern + 3);
                        // Address_In_Pattern := WordPtr(@Index[Address_In_Pattern + 1])^
                        _chParams[chan].AddressInPattern = BitConverter.ToUInt16(data, _chParams[chan].AddressInPattern + 1);
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x82)
                    {
                        i = _chParams[chan].Stek.Length;
                        Array.Resize(ref _chParams[chan].Stek, i + 2);
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].Stek[i] = data[_chParams[chan].AddressInPattern];
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].Stek[i + 1] = _chParams[chan].AddressInPattern;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x83)
                    {
                        i = _chParams[chan].Stek.Length;
                        _chParams[chan].Stek[i - 2]--;
                        if ((_chParams[chan].Stek[i - 2] & 255) != 0)
                        {
                            _chParams[chan].AddressInPattern = _chParams[chan].Stek[i - 1];
                        }
                        else
                        {
                            Array.Resize(ref _chParams[chan].Stek, i - 2);
                            _chParams[chan].AddressInPattern++;
                        }
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x84)
                    {
                        _chParams[chan].AddressInPattern++;
                        _noiseBase = data[_chParams[chan].AddressInPattern];
                        _chParams[chan].AddressInPattern++;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x85)
                    {
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].FXM_Mixer = (byte)(data[_chParams[chan].AddressInPattern] & 9);
                        _chParams[chan].AddressInPattern++;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x86)
                    {
                        _chParams[chan].AddressInPattern++;
                        // OrnamentPointer := WordPtr(@Index[Address_In_Pattern])^;
                        _chParams[chan].OrnamentPointer = BitConverter.ToUInt16(data, _chParams[chan].AddressInPattern);
                        _chParams[chan].AddressInPattern += 2;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x87)
                    {
                        _chParams[chan].AddressInPattern++;
                        // SamplePointer := WordPtr(@Index[Address_In_Pattern])^;
                        _chParams[chan].SamplePointer = BitConverter.ToUInt16(data, _chParams[chan].AddressInPattern);
                        _chParams[chan].AddressInPattern += 2;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x88)
                    {
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].Transposit = (sbyte)data[_chParams[chan].AddressInPattern];
                        _chParams[chan].AddressInPattern++;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x89)
                    {
                        i = _chParams[chan].Stek.Length;
                        _chParams[chan].AddressInPattern = _chParams[chan].Stek[i - 1];
                        Array.Resize(ref _chParams[chan].Stek, i - 1);
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x8A)
                    {
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].B0e = true;
                        _chParams[chan].B1e = false;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x8B)
                    {
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].B0e = false;
                        _chParams[chan].B1e = false;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x8C)
                    {
                        _chParams[chan].AddressInPattern += 3;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x8D)
                    {
                        _chParams[chan].AddressInPattern++;
                        _noiseBase = (byte)((_noiseBase + data[_chParams[chan].AddressInPattern]) & _amadAndSix);
                        _chParams[chan].AddressInPattern++;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x8E)
                    {
                        _chParams[chan].AddressInPattern++;
                        _chParams[chan].Transposit = (sbyte)(_chParams[chan].Transposit + data[_chParams[chan].AddressInPattern]);
                        _chParams[chan].AddressInPattern++;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x8F)
                    {
                        i = _chParams[chan].Stek.Length;
                        Array.Resize(ref _chParams[chan].Stek, i + 1);
                        _chParams[chan].Stek[i] = (ushort)_chParams[chan].Transposit;
                        _chParams[chan].AddressInPattern++;
                    }
                    else if (data[_chParams[chan].AddressInPattern] == 0x90)
                    {
                        i = _chParams[chan].Stek.Length;
                        _chParams[chan].Transposit = (sbyte)_chParams[chan].Stek[i - 1];
                        Array.Resize(ref _chParams[chan].Stek, i - 1);
                        _chParams[chan].AddressInPattern++;
                    }
                    else
                    {
                        _chParams[chan].AddressInPattern++;
                    }
                }
                while (true);
            }

            return result;
        }

        public void ClearPat()
        {
            _vtm.Patterns[_pat] = new Pattern();
            _vtm.Patterns[_pat].Length = _patSz;
        }
    }
}
