using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibVT
{

    [StructLayout(LayoutKind.Sequential)]
    public struct TSData2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Type1;
        public ushort Size1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Type2;
        public ushort Size2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] TSID;

        public TSData2(string type1, string type2, string tsid)
        {
            Type1 = Encoding.ASCII.GetBytes(type1);
            Size1 = 0;
            Type2 = Encoding.ASCII.GetBytes(type2);
            Size2 = 0;
            TSID = Encoding.ASCII.GetBytes(tsid);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TSData3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Type0;
        public ushort Size0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Type1;
        public ushort Size1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Type2;
        public ushort Size2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] TSID;

        public TSData3(string type1, string type2, string type3, string tsid)
        {
            Type0 = Encoding.ASCII.GetBytes(type1);
            Size0 = 0;
            Type1 = Encoding.ASCII.GetBytes(type2);
            Size1 = 0;
            Type2 = Encoding.ASCII.GetBytes(type3);
            Size2 = 0;
            TSID = Encoding.ASCII.GetBytes(tsid);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SongStructure
    {
        public short PSongName;
        public short PSongData;
    }

    // AY-file header and structures
    [StructLayout(LayoutKind.Sequential)]
    public struct AYFileHeader
    {
        public uint FileID;
        public uint TypeID;
        public byte FileVersion;
        public byte PlayerVersion;
        public short PSpecialPlayer;
        public short PAuthor;
        public short PMisc;
        public byte NumOfSongs;
        public byte FirstSong;
        public short PSongsStructure;
    }

    public class ZXModule
    {
        public static void Prepare(byte[] data, ref ModuleType moduleType, int length)
        {
            int i, j, k, i1, i2;

            switch (moduleType)
            {
                case ModuleType.FLSFile:
                    // Extract FLS pointers directly from the data
                    int positionsPointer = BitConverter.ToUInt16(data, 0);
                    int ornamentsPointer = BitConverter.ToUInt16(data, 2);
                    int samplesPointer = BitConverter.ToUInt16(data, 4);

                    i = ornamentsPointer - 16;

                    if (i >= 0)
                    {
                        do
                        {
                            i2 = samplesPointer + 2 - i;

                            if (i2 >= 8 && i2 < length)
                            {
                                i1 = BitConverter.ToUInt16(data, i2) - i;

                                if (i1 >= 8 && i1 < length)
                                {
                                    i2 = BitConverter.ToUInt16(data, i2 - 4) - i;

                                    if (i2 >= 6 && i2 < length)
                                    {
                                        if (i1 - i2 == 0x20)
                                        {
                                            int patternOffset = 6; // FLS_PatternsPointers[1]
                                            int patternB = BitConverter.ToUInt16(data, patternOffset + 2) - i;
                                            if (patternB > 21 && patternB < length)
                                            {
                                                int patternA = BitConverter.ToUInt16(data, patternOffset + 0) - i;
                                                if (patternA > 20 && patternA < length)
                                                {
                                                    if (data[patternA - 1] == 0)
                                                    {
                                                        i1 = patternA;
                                                        while (i1 < length && data[i1] != 255)
                                                        {
                                                            while (i1 < length)
                                                            {
                                                                byte val = data[i1];

                                                                if ((val >= 0 && val <= 0x5F) || val == 0x80 || val == 0x81)
                                                                {
                                                                    i1++;
                                                                    break;
                                                                }
                                                                else if (val >= 0x82 && val <= 0x8E)
                                                                {
                                                                    i1++;
                                                                }

                                                                i1++;
                                                            }
                                                        }

                                                        if (i1 + 1 == patternB)
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            i--;
                        }
                        while (i >= 0);
                    }

                    if (i < 0)
                    {
                        moduleType = ModuleType.Unknown;
                    }
                    else
                    {
                        // Adjust all pointer values
                        i1 = BitConverter.ToUInt16(data, 4) - i; // FLS_SamplesPointer
                        i2 = BitConverter.ToUInt16(data, 0) - i + 2; // FLS_PositionsPointer + 2

                        int ptrOffset = 0;

                        while (ptrOffset + 1 < data.Length && ptrOffset != i1)
                        {
                            ushort ptr = BitConverter.ToUInt16(data, ptrOffset);
                            ptr -= (ushort)i;
                            byte[] bytes = BitConverter.GetBytes(ptr);
                            Array.Copy(bytes, 0, data, ptrOffset, 2);
                            ptrOffset += 2;
                        }

                        ptrOffset += 2;

                        while (ptrOffset + 1 < data.Length && ptrOffset != i2)
                        {
                            ushort ptr = BitConverter.ToUInt16(data, ptrOffset);
                            ptr -= (ushort)i;
                            byte[] bytes = BitConverter.GetBytes(ptr);
                            Array.Copy(bytes, 0, data, ptrOffset, 2);
                            ptrOffset += 4;
                        }

                        // Delphi:
                        // 134, 0, 66, 0, 82, 0, 174, 0, 47, 1, 222, 1, 21, 2, 147, 2, 86, 3, 174, 3, 147, 2, 45, 4, 91, 4, 37, 5, 45, 4, 87, 5, 207, 5, 146, 6, 221, 6, 47, 1, 94, 7, 221, 6, 47, 1, 138, 7, 221, 6, 197, 7, 111, 8, 171, 8, 197, 7, 23, 9, 221, 6, 47, 1, 35, 9, 110, 9, 142, 9, 174, 9, 206, 9, 238, 9, 14, 10, 46, 10, 78, 10, 0, 1, 110, 10, 24, 8, 206, 10, 24, 8, 46, 11, 24, 8, 142, 11, 0, 1, 238, 11, 0, 1, 78, 12, 0, 1, 174, 12, 26, 6, 14, 13, 26, 6, 110, 13, 0, 1, 206, 13, 16, 16, 46, 14, 0, 1, 142, 14, 0, 1, 238, 14, 6, 9, 8, 10, 10, 1, 1, 3, 3, 2, 2, 6, 6, 6, 6, 7, 7, 4, 4, 5, 5, 1, 1, 3, 3, 2, 2, 6, 6, 6, 6, 7, 7, 4, 4, 5, 5, 1, 1, 0, 161, 108, 112, 36, 105, 64, 106, 64, 105, 64, 107, 36, 105, 64, 106, 64, 100, 64, 108, 36, 100, 64, 106, 64, 105, 64, 107, 36, 105, 64, 106, 64, 105, 64, 108, 36, 105, 64, 106, 64, 105, 64, 107, 36, 105, 64, 106, 64, 100, 64, 108, 36, 100, 64, 106, 64, 105, 64, 107, 36, 105, 64, 106, 64, 107, 36, 108, 36, 105, 64, 106, 60, 105, 60, 107, 36, 105, 60, 106, 60, 100, 60, 108, 36, 100, 60, 106, 60, 105, 60, 107, 36, 105, 60, 106, 60, 105, 60, 108, 36, 105, 62, 106, 62, 105, 62, 107, 36, 105, 62, 106, 62, 100, 62, 108, 36, 107, 36, 106, 62, 107, 36, 36, 105, 62, 107, 36, 36, 255, 161, 102, 142, 48, 16, 97, 4, 102, 16, 97, 4, 96, 113, 23, 102, 142, 36, 21, 162, 142, 40, 19, 161, 142, 48, 16, 97, 4, 102, 16, 97, 4, 96, 113, 23, 102, 142, 36, 21, 162, 142, 40, 19, 161, 142, 48, 16, 97, 4, 102, 16, 97, 4, 96, 113, 23, 102, 142, 36, 21, 162, 142, 40, 19, 161, 142, 48, 16, 97, 4, 102, 16, 97, 4, 96, 113, 23, 102, 142, 36, 21, 142, 40, 19, 96, 113, 12, 102, 142, 60, 12, 97, 0, 102, 12, 97, 0, 96, 113, 21, 102, 142, 40, 19, 162, 142, 48, 16, 161, 142, 60, 12, 97, 0, 102, 12, 97, 0, 96, 113, 21, 102, 142, 40, 19, 162, 142, 48, 16, 161, 142, 54, 14, 97, 2, 102, 14, 97, 2, 96, 113, 21, 102, 142, 40, 19, 162, 142, 42, 18, 161, 142, 54, 14, 96, 114, 12, 102, 142, 54, 14, 97, 2, 96, 113, 21, 102, 142, 40, 19, 162, 142, 42, 18, 255, 164, 98, 115, 52, 162, 47, 164, 52, 162, 47, 161, 54, 47, 162, 55, 164, 52, 162, 47, 164, 52, 162, 47, 161, 54, 47, 162, 55, 164, 48, 162, 43, 164, 48, 162, 43, 161, 48, 43, 162, 55, 164, 54, 162, 52, 164, 50, 162, 52, 161, 54, 52, 162, 55, 255, 161, 108, 112, 36, 105, 36, 106, 52, 105, 36, 107, 36, 105, 36, 106, 52, 107, 36, 108, 36, 105, 36, 106, 52, 105, 36, 107, 36, 105, 36, 106, 52, 108, 36, 36, 105, 36, 106, 48, 105, 36, 107, 36, 105, 36, 106, 48, 107, 36, 108, 36, 105, 36, 106, 48, 105, 36, 107, 36, 105, 36, 106, 48, 108, 36, 36, 105, 36, 106, 50, 105, 36, 107, 36, 105, 36, 106, 50, 107, 36, 108, 36, 105, 36, 106, 50, 105, 36, 107, 36, 105, 36, 106, 50, 108, 36, 36, 105, 36, 106, 47, 105, 36, 107, 36, 105, 36, 106, 47, 105, 36, 108, 36, 107, 36, 36, 105, 36, 107, 36, 105, 36, 107, 36, 36, 255, 161, 102, 142, 48, 16, 142, 24, 28, 142, 12, 40, 142, 48, 16, 142, 24, 28, 142, 12, 40, 142, 48, 16, 142, 12, 40, 142, 48, 16, 142, 24, 28, 142, 12, 40, 142, 48, 16, 142, 24, 28, 142, 12, 40, 142, 48, 16, 142, 12, 40, 142, 60, 12, 142, 30, 24, 142, 15, 36, 142, 60, 12, 142, 30, 24, 142, 15, 36, 142, 60, 12, 142, 15, 36, 142, 60, 12, 142, 30, 24, 142, 15, 36, 142, 60, 12, 142, 30, 24, 142, 15, 36, 142, 60, 12, 142, 15, 36, 142, 54, 14, 142, 27, 26, 142, 13, 38, 142, 54, 14, 142, 27, 26, 142, 13, 38, 142, 54, 14, 142, 13, 38, 142, 54, 14, 142, 27, 26, 142, 13, 38, 142, 54, 14, 142, 27, 26, 142, 13, 38, 142, 54, 14, 142, 13, 38, 142, 64, 11, 142, 32, 23, 142, 16, 35, 142, 64, 11, 142, 32, 23, 142, 16, 35, 142, 64, 11, 142, 16, 35, 142, 64, 11, 142, 32, 23, 142, 16, 35, 142, 64, 11, 142, 32, 23, 142, 16, 35, 142, 64, 11, 142, 16, 35, 255, 161, 97, 116, 52, 162, 52, 161, 52, 52, 45, 118, 54, 162, 54, 161, 55, 162, 52, 161, 116, 52, 50, 52, 55, 52, 162, 52, 161, 52, 52, 45, 118, 54, 162, 54, 161, 55, 162, 52, 161, 116, 52, 50, 52, 55, 57, 162, 57, 161, 57, 57, 47, 118, 54, 162, 54, 161, 55, 162, 50, 161, 116, 57, 55, 54, 52, 54, 162, 54, 161, 54, 162, 54, 161, 118, 54, 54, 52, 50, 162, 50, 161, 116, 50, 52, 54, 55, 255, 161, 108, 112, 36, 105, 36, 106, 64, 105, 36, 107, 36, 105, 36, 106, 64, 105, 36, 

                        // C#:
                        // 134, 192, 66, 192, 82, 192, 174, 192, 47, 193, 222, 193, 21, 194, 147, 194, 86, 195, 174, 195, 147, 194, 45, 196, 91, 196, 37, 197, 45, 

                        //Console.WriteLine("");
                    }
                    break;

                case ModuleType.SQTFile:
                    {
                        SQT sqt = Helpers.CastToStruct<SQT>(data);

                        i = sqt.SamplesPointer - 10;
                        j = 0;
                        k = sqt.PositionsPointer - i;

                        while (data[k] != 0)
                        {
                            j = Math.Max(j, data[k] & 0x7F); k += 2;
                            j = Math.Max(j, data[k] & 0x7F); k += 2;
                            j = Math.Max(j, data[k] & 0x7F); k += 3;
                        }

                        int patchOffset = 2;
                        int iterations = (sqt.PatternsPointer - i + (j << 1)) / 2;

                        for (k = 1; k <= iterations; k++)
                        {
                            ushort val = BitConverter.ToUInt16(data, patchOffset);
                            val -= (ushort)i;
                            byte[] bytes = BitConverter.GetBytes(val);
                            Array.Copy(bytes, 0, data, patchOffset, 2);
                            patchOffset += 2;
                        }

                        // CA 10 0A 00 1E 00 3E 00 E0 0E E0 0E FE 00 60 01 C2 01 24 02 86 02 E8 02 4A 03 AC 03 0E 04 70 04 D2 04 F4 04 16 05 38 05 5A 05 7C 05 9E 05 C0 05 E2 05 04 06 26 06 48 06 6A 06 8C 06 AE 06 D0 06 F2 06 03 07 0F 07 1B 07 27 07 37 07 46 07 5A 07 65 07 7F 07 9C 07 B4 07 CA 07 D6 07 E0 07 E3 07 FB 07

                        //Console.WriteLine("");

                        break;
                    }
            }
        }

        public static ModuleType GetTSType(byte[] tsData)
        {
            int tsInt = BitConverter.ToInt32(tsData, 0);

            switch (tsInt)
            {
                case 0x21435453:
                    return ModuleType.STCFile;
                case 0x21435341:
                    return ModuleType.ASCFile;
                case 0x21505453:
                    return ModuleType.STPFile;
                case 0x21435350:
                    return ModuleType.PSCFile;
                case 0x21534C46:
                    return ModuleType.FLSFile;
                case 0x21435446:
                    return ModuleType.FTCFile;
                case 0x21315450:
                    return ModuleType.PT1File;
                case 0x21325450:
                    return ModuleType.PT2File;
                case 0x21335450:
                    return ModuleType.PT3File;
                case 0x21545153:
                    return ModuleType.SQTFile;
                case 0x21525447:
                    return ModuleType.GTRFile;
                case 0x214D5350:
                    return ModuleType.PSMFile;
            }

            return ModuleType.Unknown;
        }

        public static ModuleType LoadAndDetect(string fileName, out int length, out ModuleType fileType1, out ModuleType fileType2, out int tsSize1, out int tsSize2, out ushort zxAddr, out int tm, out byte andSix, out string authorName, out string songName, out byte[] data)
        {
            length = 0;
            fileType1 = ModuleType.Unknown;
            fileType2 = ModuleType.Unknown;
            tsSize1 = 0;
            tsSize2 = 0;
            zxAddr = 0;
            tm = 0;
            andSix = 0;
            authorName = "";
            songName = "";
            data = null;

            ModuleType result = ModuleType.Unknown;

            string ext = Path.GetExtension(fileName).ToLower();

            switch (ext)
            {
                case ".stc":
                    result = ModuleType.STCFile;
                    break;
                case ".asc":
                    result = ModuleType.ASCFile;
                    break;
                case ".stp":
                    result = ModuleType.STPFile;
                    break;
                case ".psc":
                    result = ModuleType.PSCFile;
                    break;
                case ".fls":
                    result = ModuleType.FLSFile;
                    break;
                case ".ftc":
                    result = ModuleType.FTCFile;
                    break;
                case ".pt1":
                    result = ModuleType.PT1File;
                    break;
                case ".pt2":
                    result = ModuleType.PT2File;
                    break;
                case ".pt3":
                    result = ModuleType.PT3File;
                    break;
                case ".sqt":
                    result = ModuleType.SQTFile;
                    break;
                case ".fxm":
                case ".ay":
                    result = ModuleType.FXMFile;
                    break;
                case ".gtr":
                    result = ModuleType.GTRFile;
                    break;
                case ".psm":
                    result = ModuleType.PSMFile;
                    break;
                default:
                    break;
            }

            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    int len = 65536;

                    if (result != ModuleType.FXMFile)
                    {
                        int tsData2Size = Marshal.SizeOf(typeof(TSData2));

                        if (tsData2Size >= fileStream.Length)
                            return ModuleType.Unknown;

                        fileStream.Seek(-tsData2Size, SeekOrigin.End);

                        byte[] tsData2Bytes = binaryReader.ReadBytes(tsData2Size);
                        TSData2 tsData2 = Helpers.CastToStruct<TSData2>(tsData2Bytes);
                        string tsid = Encoding.ASCII.GetString(tsData2.TSID);

                        if (tsid == "02TS")
                        {
                            result = GetTSType(tsData2.Type1);

                            if (result == ModuleType.Unknown)
                                return ModuleType.Unknown;

                            len = tsData2.Size1;
                            fileType1 = GetTSType(tsData2.Type2);
                            tsSize1 = tsData2.Size2;
                            tsSize2 = 0;
                        }
                        else if (tsid == "03TS")
                        {
                            int tsData3Size = Marshal.SizeOf(typeof(TSData3));

                            if (tsData3Size >= fileStream.Length)
                                return ModuleType.Unknown;

                            fileStream.Seek(-tsData3Size, SeekOrigin.End);

                            byte[] tsData3Bytes = binaryReader.ReadBytes(tsData3Size);
                            TSData3 tsData3 = Helpers.CastToStruct<TSData3>(tsData3Bytes);

                            result = GetTSType(tsData3.Type0);

                            if (result == ModuleType.Unknown)
                                return ModuleType.Unknown;

                            len = tsData3.Size0;
                            fileType1 = GetTSType(tsData3.Type1);
                            tsSize1 = tsData3.Size1;
                            fileType2 = GetTSType(tsData3.Type2);
                            tsSize2 = tsData3.Size2;
                        }

                        fileStream.Seek(0, SeekOrigin.Begin);
                        data = new byte[65536];
                        length = binaryReader.Read(data, 0, len);
                    }
                    else if (ext == ".fxm")
                    {
                        tm = 0;
                        andSix = 31;
                        songName = "";
                        authorName = "";

                        fileStream.Seek(4, SeekOrigin.Begin);
                        byte[] zxAddrBytes = new byte[2];
                        int zxAddrLen = binaryReader.Read(zxAddrBytes, 0, 2);

                        if (zxAddrLen != 2)
                            return ModuleType.Unknown;

                        zxAddr = BitConverter.ToUInt16(zxAddrBytes, 0);
                        len = 65536 - zxAddr;
                        data = new byte[65536];
                        length = binaryReader.Read(data, zxAddr, len);
                    }
                    else
                    {
                        result = ModuleType.Unknown;

                        int sizeOfAYFileHeader = Marshal.SizeOf(typeof(AYFileHeader));
                        byte[] ayHeaderBytes = binaryReader.ReadBytes(sizeOfAYFileHeader);

                        AYFileHeader ayFileHeader = Helpers.CastToStruct<AYFileHeader>(ayHeaderBytes);

                        if (ayFileHeader.FileID != 0x5941585A)
                            return ModuleType.Unknown;

                        if (ayFileHeader.TypeID != 0x44414D41)
                            return ModuleType.Unknown;

                        fileStream.Seek(Helpers.IntelWord((ushort)ayFileHeader.PAuthor) + 12, SeekOrigin.Begin);

                        authorName = "";
                        char ch;

                        do
                        {
                            ch = binaryReader.ReadChar();

                            if (ch != '\0')
                                authorName += ch;
                        }
                        while (ch != '\0');

                        authorName = authorName.Trim();

                        if (authorName.Length > 32)
                            authorName = authorName.Substring(0, 32);

                        fileStream.Seek(Helpers.IntelWord((ushort)ayFileHeader.PSongsStructure) + 18, System.IO.SeekOrigin.Begin);

                        byte[] songStructureBytes = binaryReader.ReadBytes(4);
                        //int sizeOfSongStructure = Marshal.SizeOf(typeof(TSongStructure));

                        //TSongStructure songStructure = Helpers.CastToStruct<TSongStructure>(songStructureBytes);
                        SongStructure songStructure = new SongStructure();
                        songStructure.PSongName = BitConverter.ToInt16(songStructureBytes, 0);
                        songStructure.PSongData = BitConverter.ToInt16(songStructureBytes, 2);

                        int curPos = (int)fileStream.Position;
                        fileStream.Seek(Helpers.IntelWord((ushort)songStructure.PSongName) + curPos - 4, SeekOrigin.Begin);

                        string songNameLocal = "";

                        do
                        {
                            ch = binaryReader.ReadChar();

                            if (ch != '\0')
                                songNameLocal += ch;

                        }
                        while (ch != '\0');

                        songNameLocal = songNameLocal.Trim();

                        if (songNameLocal.Length > 32)
                            songNameLocal = songNameLocal.Substring(0, 32);

                        int offset = Helpers.IntelWord((ushort)songStructure.PSongData) + curPos - 2;

                        fileStream.Seek(offset, SeekOrigin.Begin);
                        zxAddr = Helpers.IntelWord(binaryReader.ReadUInt16());

                        andSix = binaryReader.ReadByte();
                        byte byt = binaryReader.ReadByte();
                        ushort wrd = binaryReader.ReadUInt16();

                        tm = Helpers.IntelWord((ushort)(byt * wrd));

                        fileStream.Seek(offset + 14, SeekOrigin.Begin);

                        len = 65536 - zxAddr;
                        data = new byte[65536];

                        length = binaryReader.Read(data, zxAddr, len);

                        result = ModuleType.FXMFile;
                    }
                }
            }

            Prepare(data, ref result, length);

            return result;
        }
    }
}
