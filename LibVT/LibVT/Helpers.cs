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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    public class Helpers
    {
        public static void FillChar<T>(T[] array, int fillCount, T fillValue)
        {
            for (int i = 0; i < fillCount && i < array.Length; i++)
                array[i] = fillValue;
        }

        public static void FillChar<T>(T[] array, int fillOffset, int fillCount, T fillValue)
        {
            for (int i = fillOffset; i < fillCount && i < array.Length; i++)
                array[i] = fillValue;
        }

        public static void FillChar<T>(T[,] array, int fillCount, T fillValue)
        {
            for (int i = 0; i < fillCount && i <= array.Length; i++)
                for (int j = 0; j < fillCount && j <= array.GetUpperBound(1); j++)
                    array[i, j] = fillValue;
        }

        public static byte[] ReadBytes<T>(T input) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] result = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(input, ptr, false);
                Marshal.Copy(ptr, result, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return result;
        }

        public static void WriteBytes<T>(ref T dest, byte[] data) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.Copy(data, 0, ptr, size);
                dest = Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static void Move<T>(T source, ref T dest, int count) where T : struct
        {
            var sourceBytes = ReadBytes(source);
            var destBytes = ReadBytes(dest);

            int len = Math.Min(count, Math.Min(sourceBytes.Length, destBytes.Length));
            Array.Copy(sourceBytes, destBytes, len);

            WriteBytes(ref dest, destBytes);
        }

        public static void Move<T>(T source, byte[] destBytes, int copyCount) where T : struct
        {
            byte[] sourceBytes = ReadBytes(source);
            int len = Math.Min(copyCount, Math.Min(sourceBytes.Length, destBytes.Length));
            Array.Copy(sourceBytes, destBytes, len);
        }

        public static void Move<T>(T[] sourceArray, T[] destArray, int copyCount)
        {
            Array.Copy(sourceArray, destArray, copyCount);
        }

        public static void Move<T>(T[] sourceArray, int sourceIndex, T[] destArray, int destIndex, int copyCount)
        {
            Array.Copy(sourceArray, sourceIndex, destArray, destIndex, copyCount);
        }

        public static void Move(byte[] source, int sourceIndex, ushort[] dest, int destIndex, int copyCount)
        {
            Buffer.BlockCopy(source, sourceIndex, dest, destIndex * sizeof(ushort), copyCount);
        }

        public static void Move(byte[] source, int sourceIndex, ushort[] dest, int copyCount)
        {
            Buffer.BlockCopy(source, sourceIndex, dest, 0, copyCount);
        }

        public static string CopyBytesToString(char[] source, int sourceIndex, int length)
        {
            return new string(source, sourceIndex, length);
        }

        public static string CopyBytesToString(byte[] source, int sourceIndex, int length)
        {
            return Encoding.ASCII.GetString(source, sourceIndex, length);
        }

        public static void CopyStringToByteArray(string source, int sourceIndex, byte[] destination, int destIndex, int length)
        {
            if (source == null || destination == null)
                return;

            string subString = source.Substring(sourceIndex, Math.Min(length, source.Length - sourceIndex));
            CopyStringToByteArray(subString, destination, destIndex, length);
        }

        public static void CopyStringToByteArray(string source, byte[] destination, int destIndex)
        {
            if (source == null || destination == null)
                return;

            int maxLength = destination.Length - destIndex;
            CopyStringToByteArray(source, destination, destIndex, maxLength);
        }

        public static void CopyStringToByteArray(string source, byte[] destination, int destIndex, int length)
        {
            if (source == null || destination == null)
                return;

            int copyLength = Math.Min(Math.Min(source.Length, length), destination.Length - destIndex);

            for (int i = 0; i < copyLength; i++)
                destination[destIndex + i] = (byte)source[i];
        }

        public static void Move(PatternTriple source, ushort[] dest, int destIndex)
        {
            dest[destIndex] = source.PatternA;
            dest[destIndex + 1] = source.PatternB;
            dest[destIndex + 2] = source.PatternC;
        }

        public static string CopyCharsToString(char[] source, int sourceIndex, int length)
        {
            return new string(source.Skip(sourceIndex).Take(length).ToArray());
        }

        public static string CopyCharsToString(byte[] source, int sourceIndex, int length)
        {
            return new string(source.Skip(sourceIndex).Take(length).Select(b => (char)b).ToArray());
        }

        public static byte[] ToByteArray<T>(T[,] data) where T : struct
        {
            if (data == null)
                return null;

            int totalSize = data.Length * Marshal.SizeOf(typeof(T));
            byte[] result = new byte[totalSize];

            Buffer.BlockCopy(data, 0, result, 0, totalSize);
            return result;
        }

        public static byte[] ToByteArray<T>(T[] data) where T : struct
        {
            if (data == null)
                return null;

            int totalSize = data.Length * Marshal.SizeOf(typeof(T));
            byte[] result = new byte[totalSize];

            Buffer.BlockCopy(data, 0, result, 0, result.Length);
            return result;
        }

        public static byte[] ToByteArray<T>(T data) where T : struct
        {
            int totalSize = Marshal.SizeOf(data);
            byte[] result = new byte[totalSize];
            IntPtr ptr = Marshal.AllocHGlobal(totalSize);
            Marshal.StructureToPtr(data, ptr, true);
            Marshal.Copy(ptr, result, 0, totalSize);
            Marshal.FreeHGlobal(ptr);
            return result;
        }

        /* public static byte[] ToByteArray(int[] data)
        {
            if (data == null)
                return null;
            byte[] result = new byte[data.Length * sizeof(int)];
            Buffer.BlockCopy(data, 0, result, 0, result.Length);
            return result;
        }

        public static byte[] ToByteArray(T24Bit[] data)
        {
            if (data == null)
                return null;

            byte[] result = new byte[data.Length * 3];
            for (int i = 0; i < data.Length; i++)
            {
                int offset = i * 3;
                result[offset] = data[i].B1;
                result[offset + 1] = data[i].B2;
                result[offset + 2] = data[i].B3;
            }
            return result;
        } */

        public static int GetWord(byte[] data, int index = 0)
        {
            // Assume little-endian: combine two bytes into a word.
            return data[index] | (data[index + 1] << 8);
        }

        public static ushort IntelWord(ushort value)
        {
            return (ushort)((value >> 8) | (value << 8));
        }

        public static double Frac(double value)
        {
            return value - Math.Floor(value);
        }

        public static T CastToStruct<T>(byte[] data) where T : struct
        {
            var pData = GCHandle.Alloc(data, GCHandleType.Pinned);
            var result = (T)Marshal.PtrToStructure(pData.AddrOfPinnedObject(), typeof(T));
            pData.Free();
            return result;
        }

        public static byte[] CastToArray<T>(T data) where T : struct
        {
            var result = new byte[Marshal.SizeOf(typeof(T))];
            var pResult = GCHandle.Alloc(result, GCHandleType.Pinned);
            Marshal.StructureToPtr(data, pResult.AddrOfPinnedObject(), true);
            pResult.Free();
            return result;
        }

        /* public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        } */

        public static int StrLComp(string str1, string str2, int maxLength)
        {
            // If both strings are null, consider them equal.
            if (str1 == null && str2 == null)
                return 0;

            // If only one is null, consider the null one as "less".
            if (str1 == null)
                return -1;

            if (str2 == null)
                return 1;

            int i = 0;

            // Loop up to maxLength
            while (i < maxLength)
            {
                // Get the character at the current index or '\0' if we've reached the end.
                char c1 = i < str1.Length ? str1[i] : '\0';
                char c2 = i < str2.Length ? str2[i] : '\0';

                // If the characters differ, return the difference.
                if (c1 != c2)
                    return c1 - c2;

                // If we hit the end of both strings (null terminator), they are equal.
                if (c1 == '\0')
                    break;

                i++;
            }

            // Up to maxLength characters, the strings are equivalent.
            return 0;
        }

        public static int Mod(int a, int b)
        {
            int result = a % b;
            return (result < 0) ? result + Math.Abs(b) : result;
        }

        public static bool ReadLine(StreamReader streamReader, ref string lineText, ref int lineCount)
        {
            bool result;

            do
            {
                result = !streamReader.EndOfStream;

                if (result)
                    lineText = streamReader.ReadLine();

                lineCount++;

                lineText = lineText.Trim();
            }
            while (!result && lineText != "");

            return result;
        }
    }
}
