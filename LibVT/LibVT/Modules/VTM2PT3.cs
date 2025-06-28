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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    public class VTM2PT3
    {
        private string[] _pt3Id = { "ProTracker 3.6 compilation of ", "Vortex Tracker II 1.0 module: " };
        private string _byId = " by ";
        private const string _emptyPatternString = "\xB1\x40\xD0\x00";
        private bool[] _patterns = new bool[VTModule.MaxPatternIndex + 1];
        private bool[] _compiledPatterns = new bool[VTModule.MaxPatternIndex + 1];
        private int[] _vtmPat2PT3Pat = new int[VTModule.MaxPatternIndex + 1];
        private int[] _patOfs = new int[Convert.ToInt32(VTModule.MaxPatternCount * 3 - 1) + 1];
        private int _maxPattern;
        private string[] _patStrs = new string[Convert.ToInt32(VTModule.MaxPatternCount * 3 - 1) + 1];
        private int[,] _patsIndexes = new int[VTModule.MaxPatternIndex + 1, 2 + 1];
        private int _patNum;
        private int _strNum;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct AlCo
        {
            public int DeltT;
            public int TnStp;
            public int TnDl;
            public int TnCurDl;
            public int Note;
            public int Sample;
            public int Ornament;
            public int Volume;
            public int SkipPrev;
            public int Skip;
            public int Envelope;
        }

        // struct to avoid bug in Alone Coder's PT3 player
        private AlCo[] _alCo = new AlCo[3];

        private bool _orn;
        private bool _sam;
        private bool _orn1;
        private bool _sam1;
        private int _prevNoise;
        private int _dl;
        private bool[] _isOrnament = new bool[16];
        private bool[] _isSample = new bool[32];
        private PT3 _pt3;
        private VTM _vtm;

        public VTM2PT3()
        {
        }

        public bool Initialize(byte[] data, PT3 pt3, VTM vtm, out int moduleSize)
        {
            _pt3 = Helpers.CastToStruct<PT3>(data);
            _vtm = vtm;
            moduleSize = 0;

            bool result = false;
            int i, i1, j, k, d;

            // Move(Pt3Id[_vtm.VortexModule_Header and (_vtm.FeaturesLevel = 1), 0], PT3.PT3_Name, 30);
            string selectedId = _pt3Id[_vtm.HasHeader && _vtm.FeaturesLevel == FeaturesLevel.VTII_PT36 ? 1 : 0];            
            Array.Copy(data, 0, _pt3.Name, 0, Math.Min(30, selectedId.Length));
            
            if (_vtm.FeaturesLevel != FeaturesLevel.VTII_PT36)
                _pt3.Name[13] = (byte)(0x35 + _vtm.FeaturesLevel);

            i = 32;

            if (i > _vtm.Title.Length)
                i = _vtm.Title.Length;

            // Move(_vtm.Title[1], PT3.PT3_Name[30], i);
            Helpers.CopyStringToByteArray(_vtm.Author, _pt3.Name, 66, i);

            j = 32 - i;

            if (j != 0)
                Helpers.FillChar<byte>(_pt3.Name, 30 + i, j, 32);

            // Move(ById, PT3.PT3_Name[62], 4);
            Helpers.CopyStringToByteArray(_byId, _pt3.Name, 62, 4);

            i = 32;

            if (i > _vtm.Author.Length)
                i = _vtm.Author.Length;

            // Move(_vtm.Author[1], PT3.PT3_Name[66], i);
            Helpers.CopyStringToByteArray(_vtm.Author, _pt3.Name, 66, i);
            Helpers.FillChar<byte>(_pt3.Name, 66 + i, 32 - i + 1, 32);

            _pt3.Table = (byte)_vtm.NoteTable;
            _pt3.Delay = _vtm.InitialDelay;
            _pt3.NumberOfPositions = (byte)_vtm.Positions.Length;
            _pt3.LoopPosition = (byte)_vtm.Positions.Loop;
            _pt3.PatternsPointer = (ushort)(0xC9 + _pt3.NumberOfPositions + 1);

            Helpers.FillChar<ushort>(_pt3.SamplePointers, 96, 0);
            
            data[0xC9 + _pt3.NumberOfPositions] = 255;
            
            for (i = 0; i <= VTModule.MaxPatternIndex; i++)
            {
                _patterns[i] = false;
                _compiledPatterns[i] = false;
            }

            _maxPattern = 0;

            for (i = 0; i < _pt3.NumberOfPositions; i++)
            {
                if (_maxPattern < _vtm.Positions.Value[i])
                    _maxPattern = _vtm.Positions.Value[i];

                _patterns[_vtm.Positions.Value[i]] = true;
                _vtmPat2PT3Pat[_vtm.Positions.Value[i]] = _vtm.Positions.Value[i];
            }

            for (i = 0; i <= _maxPattern; i++)
            {
                if (_patterns[i])
                    continue;

                for (j = i + 1; j <= _maxPattern; j++)
                    _vtmPat2PT3Pat[j]--;
            }

            for (i = 0; i < _pt3.NumberOfPositions; i++)
                data[0xC9 + i] = (byte)(_vtmPat2PT3Pat[_vtm.Positions.Value[i]] * 3);

            for (i = 0; i < 16; i++)
                _isOrnament[i] = false;

            for (i = 1; i < 32; i++)
                _isSample[i] = false;

            _strNum = 0;

            _alCo[0].Note = 0;
            _alCo[1].Note = 0;
            _alCo[2].Note = 0;
            // BUG in AlCo's pt3 player
            _alCo[0].DeltT = 0;
            _alCo[1].DeltT = 0;
            _alCo[2].DeltT = 0;
            // this all for more compatability with old ZX players
            _alCo[0].TnDl = 0;
            _alCo[1].TnDl = 0;
            _alCo[2].TnDl = 0;

            // BUG in AlCo's pt3 player
            for (i1 = 0; i1 < _pt3.NumberOfPositions; i1++)
            {
                i = _vtm.Positions.Value[i1];

                _patNum = _vtmPat2PT3Pat[i];

                if (!_compiledPatterns[i])
                {
                    _compiledPatterns[i] = true;

                    if (_vtm.Patterns[i] != null)
                    {
                        _prevNoise = 0;

                        for (k = 0; k < 3; k++)
                        {
                            _dl = _alCo[k].DeltT;
                            _alCo[k].Sample = -1;
                            _alCo[k].Ornament = -1;
                            _alCo[k].Envelope = -1;
                            _alCo[k].Volume = -1;
                            _alCo[k].SkipPrev = 255;
                            _patStrs[_strNum] = "";

                            j = 0;

                            while (j < _vtm.Patterns[i].Length)
                            {
                                // new standard in pt3.69
                                _orn = ((_vtm.Patterns[i].Lines[j].Channel[k].Envelope != 0) ||
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Ornament != 0)) &&
                                    ((_vtm.Patterns[i].Lines[j].Channel[k].Ornament != _alCo[k].Ornament) ||
                                    ((_alCo[k].Ornament != 0) && (_vtm.Patterns[i].Lines[j].Channel[k].Note == -1)));
                                
                                if (_orn)
                                    _isOrnament[_vtm.Patterns[i].Lines[j].Channel[k].Ornament] = true;

                                _sam = (_vtm.Patterns[i].Lines[j].Channel[k].Note != -1) &&
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Sample != 0) &&
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Sample != _alCo[k].Sample);

                                if (_sam)
                                {
                                    _isSample[_vtm.Patterns[i].Lines[j].Channel[k].Sample] = true;
                                    _alCo[k].Sample = _vtm.Patterns[i].Lines[j].Channel[k].Sample;
                                }

                                _orn1 = _orn;
                                _sam1 = _sam;

                                if (_sam && _orn && (_vtm.Patterns[i].Lines[j].Channel[k].Envelope != 0))
                                {
                                    // new standard in pt3.69
                                    if (_vtm.Patterns[i].Lines[j].Channel[k].Envelope == 15)
                                    {
                                        if (_alCo[k].Envelope != 0)
                                        {
                                            _sam1 = false;
                                            _orn1 = false;

                                            _patStrs[_strNum] = _patStrs[_strNum] +
                                                ((char)0xF0 + _vtm.Patterns[i].Lines[j].Channel[k].Ornament) +
                                                ((char)_vtm.Patterns[i].Lines[j].Channel[k].Sample * 2);
                                        }
                                    }
                                    else
                                    {
                                        _sam1 = false;
                                        _orn1 = false;

                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)0x10 + _vtm.Patterns[i].Lines[j].Channel[k].Envelope) +
                                            (_vtm.Patterns[i].Lines[j].Envelope >> 8) +
                                            (char)_vtm.Patterns[i].Lines[j].Envelope +
                                            ((char)_vtm.Patterns[i].Lines[j].Channel[k].Sample * 2);
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)0x40 + _vtm.Patterns[i].Lines[j].Channel[k].Ornament);
                                    }
                                }

                                if (_sam1)
                                    _patStrs[_strNum] = _patStrs[_strNum] +
                                        ((char)0xD0 + _vtm.Patterns[i].Lines[j].Channel[k].Sample);
 
                                if (_orn1)
                                {
                                    _patStrs[_strNum] = _patStrs[_strNum] +
                                        ((char)0x40 + _vtm.Patterns[i].Lines[j].Channel[k].Ornament);
                                    
                                    if (_vtm.Patterns[i].Lines[j].Channel[k].Envelope >= 1 &&
                                        _vtm.Patterns[i].Lines[j].Channel[k].Envelope <= 14)
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)0xB1 + _vtm.Patterns[i].Lines[j].Channel[k].Envelope) +
                                            (_vtm.Patterns[i].Lines[j].Envelope >> 8) +
                                            (char)_vtm.Patterns[i].Lines[j].Envelope;
                                    else if ((_vtm.Patterns[i].Lines[j].Channel[k].Envelope == 15) &&
                                        _alCo[k].Envelope != 0)
                                        _patStrs[_strNum] = _patStrs[_strNum] + '\xB0';
                                }

                                if (!_orn && (_vtm.Patterns[i].Lines[j].Channel[k].Envelope > 0))
                                {
                                    if (_vtm.Patterns[i].Lines[j].Channel[k].Envelope != 15)
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)0xB1 + _vtm.Patterns[i].Lines[j].Channel[k].Envelope) +
                                            (_vtm.Patterns[i].Lines[j].Envelope >> 8) +
                                            (char)_vtm.Patterns[i].Lines[j].Envelope;
                                    else if (_alCo[k].Envelope != 0)
                                        _patStrs[_strNum] = _patStrs[_strNum] + '\xB0';
                                }

                                if (_orn)
                                    _alCo[k].Ornament = _vtm.Patterns[i].Lines[j].Channel[k].Ornament;

                                if (_vtm.Patterns[i].Lines[j].Channel[k].Envelope != 0)
                                    _alCo[k].Envelope = (_vtm.Patterns[i].Lines[j].Channel[k].Envelope < 15 ? 1 : 0);

                                if (_vtm.Patterns[i].Lines[j].Channel[k].Volume != 0)
                                {
                                    if (_vtm.Patterns[i].Lines[j].Channel[k].Volume != _alCo[k].Volume)
                                    {
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)0xC0 + _vtm.Patterns[i].Lines[j].Channel[k].Volume);
                                        _alCo[k].Volume = _vtm.Patterns[i].Lines[j].Channel[k].Volume;
                                    }
                                }

                                if ((k == 1) && (_vtm.Patterns[i].Lines[j].Noise != _prevNoise))
                                {
                                    _prevNoise = _vtm.Patterns[i].Lines[j].Noise;
                                    _patStrs[_strNum] = _patStrs[_strNum] +
                                        ((char)0x20 + _vtm.Patterns[i].Lines[j].Noise);
                                }

                                switch (_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number)
                                {
                                    case 1:
                                    case 2:
                                        _patStrs[_strNum] = _patStrs[_strNum] + '\x01';
                                        _alCo[k].TnDl = _vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Delay;
                                        _alCo[k].TnCurDl = _alCo[k].TnDl;
                                        if (_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number == 1)
                                            _alCo[k].TnStp = _vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Parameter;
                                        else
                                            _alCo[k].TnStp = -_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Parameter;
                                        break;
                                    case 3:
                                        if ((_vtm.Patterns[i].Lines[j].Channel[k].Note >= 0) ||
                                            ((_vtm.Patterns[i].Lines[j].Channel[k].Note != -2) &&
                                            (_vtm.FeaturesLevel >= FeaturesLevel.VTII_PT36)))
                                        {
                                            _patStrs[_strNum] = _patStrs[_strNum] + '\x02';
                                            _alCo[k].TnDl = -_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Delay;
                                            _alCo[k].TnCurDl = -_alCo[k].TnDl;
                                            _dl = _alCo[k].DeltT;

                                            if (_vtm.Patterns[i].Lines[j].Channel[k].Note >= 0)
                                                _dl += VTModule.GetNoteFreq(_vtm.NoteTable, _vtm.Patterns[i].Lines[j].Channel[k].Note) -
                                                    VTModule.GetNoteFreq(_vtm.NoteTable, _alCo[k].Note);

                                            if (_dl >= 0)
                                                _alCo[k].TnStp = _vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Parameter;
                                            else
                                                _alCo[k].TnStp = -_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Parameter;

                                            _alCo[k].DeltT = _dl;
                                        }
                                        break;
                                    case 4:
                                    case 5:
                                    case 6:
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number - 1);
                                        break;
                                    case 9:
                                    case 10:
                                        _patStrs[_strNum] = _patStrs[_strNum] + '\x08';
                                        break;
                                    case 11:
                                        if (_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Parameter != 0)
                                            _patStrs[_strNum] = _patStrs[_strNum] + '\x09';
                                        break;
                                }

                                if ((_vtm.Patterns[i].Lines[j].Channel[k].Note == -2) ||
                                    ((_vtm.Patterns[i].Lines[j].Channel[k].Note >= 0) &&
                                    !(_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number >= 1 &&
                                    _vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number <= 3)))
                                {
                                    _dl = 0;
                                    _alCo[k].TnDl = 0;
                                    _alCo[k].DeltT = 0;
                                }

                                _alCo[k].Skip = 0;
                                d = j;

                                do
                                {
                                    if (_alCo[k].TnDl != 0)
                                    {
                                        _alCo[k].TnCurDl--;

                                        if (_alCo[k].TnCurDl == 0)
                                        {
                                            _alCo[k].TnCurDl = Math.Abs(_alCo[k].TnDl);
                                            _alCo[k].DeltT -= _alCo[k].TnStp;

                                            if (_alCo[k].TnDl < 0)
                                            {
                                                if (((_alCo[k].DeltT >= 0) && (_alCo[k].TnStp < 0)) ||
                                                    ((_alCo[k].DeltT <= 0) && (_alCo[k].TnStp >= 0)))
                                                {
                                                    _alCo[k].TnDl = 0;
                                                    _alCo[k].DeltT = 0;
                                                }
                                            }
                                        }
                                    }

                                    _alCo[k].Skip++;
                                    j++;

                                    // new standard in pt3.69
                                }
                                while ((j < _vtm.Patterns[i].Length) &&
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Note == -1) &&
                                    !(_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number == 11 &&
                                    _vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Parameter != 0) &&
                                    (_vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number == 0 ||
                                    _vtm.Patterns[i].Lines[j].Channel[k].AdditionalCommand.Number == 11) &&
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Volume == 0) &&
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Envelope < 1 ||
                                    _vtm.Patterns[i].Lines[j].Channel[k].Envelope > 14) &&
                                    !(_vtm.Patterns[i].Lines[j].Channel[k].Envelope == 15 &&
                                    (_alCo[k].Envelope != 0 ||
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Ornament == 0 &&
                                    _alCo[k].Ornament != 0))) &&
                                    (_vtm.Patterns[i].Lines[j].Channel[k].Ornament == 0) &&
                                    ((k != 1) || (_vtm.Patterns[i].Lines[d].Noise == _vtm.Patterns[i].Lines[j].Noise)));

                                if (_alCo[k].Skip != _alCo[k].SkipPrev)
                                {
                                    _patStrs[_strNum] = _patStrs[_strNum] + '\xB1' + ((char)_alCo[k].Skip);
                                    _alCo[k].SkipPrev = _alCo[k].Skip;
                                }

                                if (_vtm.Patterns[i].Lines[d].Channel[k].Note == -2)
                                    _patStrs[_strNum] = _patStrs[_strNum] + '\xC0';
                                else if (_vtm.Patterns[i].Lines[d].Channel[k].Note == -1)
                                    _patStrs[_strNum] = _patStrs[_strNum] + '\xD0';
                                else
                                {
                                    _alCo[k].Note = _vtm.Patterns[i].Lines[d].Channel[k].Note;
                                    _patStrs[_strNum] = _patStrs[_strNum] + (char)(0x50 + _alCo[k].Note);
                                }
                                switch (_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Number)
                                {
                                    case 1:
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Delay) +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter) + '\x00';
                                        break;
                                    case 2:
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Delay) +
                                            ((char)-_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter) + '\xFF';
                                        break;
                                    case 3:
                                        if ((_vtm.Patterns[i].Lines[d].Channel[k].Note >= 0) ||
                                            ((_vtm.Patterns[i].Lines[d].Channel[k].Note != -2) &&
                                            (_vtm.FeaturesLevel >= FeaturesLevel.VTII_PT36)))
                                        {
                                            if (_dl >= 0)
                                                _patStrs[_strNum] = _patStrs[_strNum] +
                                                    ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Delay) +
                                                    (char)_dl + (_dl >> 8) + ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter) + '\x00';
                                            else
                                                _patStrs[_strNum] = _patStrs[_strNum] +
                                                    ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Delay) +
                                                    (char)-_dl + (-_dl >> 8) + ((char)-_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter) + '\xFF';
                                        }
                                        break;
                                    case 4:
                                    case 5:
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter);
                                        break;
                                    case 6:
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter >> 4) +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter & 15);
                                        break;
                                    case 9:
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Delay) +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter) + '\x00';
                                        break;
                                    case 10:
                                        _patStrs[_strNum] = _patStrs[_strNum] +
                                            ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Delay) +
                                            ((char)-_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter) + '\xFF';
                                        break;
                                    case 11:
                                        if (_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter != 0)
                                            _patStrs[_strNum] = _patStrs[_strNum] +
                                                ((char)_vtm.Patterns[i].Lines[d].Channel[k].AdditionalCommand.Parameter);
                                        break;
                                }

                                _dl = _alCo[k].DeltT;
                            }

                            _patStrs[_strNum] = _patStrs[_strNum] + '\x00';
                            _patsIndexes[_patNum, k] = _strNum;

                            for (d = 0; d < _strNum; d++)
                            {
                                if (_patStrs[d] == _patStrs[_strNum])
                                {
                                    _patsIndexes[_patNum, k] = d;
                                    _strNum--;
                                    break;
                                }
                            }
                            _strNum++;
                        }
                    }
                    else
                    {
                        _patStrs[_strNum] = _emptyPatternString;
                        _patsIndexes[_patNum, 0] = _strNum;
                        _patsIndexes[_patNum, 1] = _strNum;
                        _patsIndexes[_patNum, 2] = _strNum;

                        for (d = 0; d < _strNum; d++)
                        {
                            if (_patStrs[d] == _emptyPatternString)
                            {
                                _patsIndexes[_patNum, 0] = d;
                                _patsIndexes[_patNum, 1] = d;
                                _patsIndexes[_patNum, 2] = d;

                                _strNum--;
                                break;
                            }
                        }

                        _strNum++;
                    }
                }
            }

            _patNum = _pt3.PatternsPointer + 6 * (_vtmPat2PT3Pat[_maxPattern] + 1);

            for (i = 0; i < _strNum; i++)
            {
                if (_patNum > 65536 - 3 - _patStrs[i].Length)
                    return result;

                _patOfs[i] = _patNum;

                // Move(PatStrs[i][1], PT3.Index[PatNum], Length(PatStrs[i]));
                Helpers.CopyStringToByteArray(_patStrs[i], data, _patNum);
                _patNum += _patStrs[i].Length;
            }

            j = _pt3.PatternsPointer;

            for (i = 0; i <= _maxPattern; i++)
            {
                if (_patterns[i])
                {
                    for (k = 0; k < 3; k++)
                    {
                        // WordPtr(@PT3.Index[j]) ^ := PatOfs[PatsIndexes[VTMat2PT3Pat[i], k]];
                        ushort value = (ushort)_patOfs[_patsIndexes[_vtmPat2PT3Pat[i], k]];
                        byte[] wordBytes = BitConverter.GetBytes(value);
                        data[j] = wordBytes[0];
                        data[j + 1] = wordBytes[1];

                        j += 2;
                    }
                }
            }
            for (i = 1; i < 32; i++)
            {
                if (_isSample[i])
                {
                    if (_patNum >= 65536 - 2 - 3)
                        return result;

                    _pt3.SamplePointers[i] = (ushort)_patNum;

                    if (_vtm.Samples[i] != null)
                        data[_patNum] = _vtm.Samples[i].Loop;
                    else
                        data[_patNum] = 0;

                    _patNum++;

                    if (_vtm.Samples[i] != null)
                        data[_patNum] = _vtm.Samples[i].Length;
                    else
                        data[_patNum] = 1;

                    _patNum++;

                    if (_patNum > 65536 - data[_patNum - 1] * 4 - 3)
                        return result;

                    if (_vtm.Samples[i] != null)
                    {
                        for (j = 0; j < _vtm.Samples[i].Length; j++)
                        {
                            d = 0;

                            if (!_vtm.Samples[i].Ticks[j].Envelope_Enabled)
                                d = 1;

                            d += (_vtm.Samples[i].Ticks[j].Add_to_Envelope_or_Noise) & 31 << 1;

                            if (_vtm.Samples[i].Ticks[j].Amplitude_Sliding)
                            {
                                d |= 0x80;

                                if (_vtm.Samples[i].Ticks[j].Amplitude_Slide_Up)
                                    d |= 0x40;
                            }

                            data[_patNum] = (byte)d;

                            _patNum++;

                            d = _vtm.Samples[i].Ticks[j].Amplitude;

                            if (!_vtm.Samples[i].Ticks[j].Mixer_Ton)
                                d |= 0x10;

                            if (!_vtm.Samples[i].Ticks[j].Mixer_Noise)
                                d |= 0x80;

                            if (_vtm.Samples[i].Ticks[j].Envelope_or_Noise_Accumulation)
                                d |= 0x20;

                            if (_vtm.Samples[i].Ticks[j].Ton_Accumulation)
                                d |= 0x40;

                            data[_patNum] = (byte)d;

                            _patNum++;

                            // WordPtr(@PT3.Index[PatNum])^ := _vtm.Samples[i].Items[j].Add_to_Ton;
                            short value = (short)_vtm.Samples[i].Ticks[j].AddToTone;
                            byte[] wordBytes = BitConverter.GetBytes(value);
                            data[_patNum] = wordBytes[0];
                            data[_patNum + 1] = wordBytes[1];

                            _patNum += 2;
                        }
                    }
                    else
                    {
                        // DWordPtr(@PT3.Index[PatNum])^ := $9001;
                        byte[] dwordBytes = BitConverter.GetBytes(0x9001);
                        data[_patNum] = dwordBytes[0];
                        data[_patNum + 1] = dwordBytes[1];
                        data[_patNum + 2] = dwordBytes[2];
                        data[_patNum + 3] = dwordBytes[3];

                        _patNum += 4;
                    }
                }
            }

            if (_patNum > 65536 - 3)
                return result;

            _pt3.OrnamentPointers[0] = (ushort)_patNum;
            data[_patNum++] = 0;
            data[_patNum++] = 1;
            data[_patNum++] = 0;

            for (i = 1; i < 16; i++)
            {
                if (_isOrnament[i])
                {
                    if (_patNum >= 65536 - 2)
                        return result;

                    _pt3.OrnamentPointers[i] = (ushort)_patNum;

                    if (_vtm.Ornaments[i] != null)
                        data[_patNum] = (byte)_vtm.Ornaments[i].Loop;
                    else
                        data[_patNum] = 0;

                    _patNum++;

                    if (_vtm.Ornaments[i] != null)
                        data[_patNum] = (byte)_vtm.Ornaments[i].Length;
                    else
                        data[_patNum] = 1;

                    _patNum++;

                    if (_patNum > 65536 - data[_patNum - 1])
                        return result;

                    if (_vtm.Ornaments[i] != null)
                    {
                        for (j = 0; j < _vtm.Ornaments[i].Length; j++)
                        {
                            data[_patNum] = (byte)_vtm.Ornaments[i].Offsets[j];
                            _patNum++;
                        }
                    }
                    else
                    {
                        data[_patNum] = 0;
                        _patNum++;
                    }
                }
            }

            moduleSize = _patNum;
            
            result = true;
            
            return result;
        }
    }
}
