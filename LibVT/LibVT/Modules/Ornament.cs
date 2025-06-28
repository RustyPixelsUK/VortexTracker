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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVT
{
    public class Ornament : ICloneable
    {
        public sbyte[] Offsets;
        public int Length;
        public int Loop;
        public bool CopyAll;

        public Ornament()
        {
            Offsets = new sbyte[VTModule.MaxOrnamentLength];
        }

        public static bool RecognizeOrnamentString(string orst, Ornament ornament)
        {
            int lp = 0, l = 0, i = 0, j, sl = orst.Length;

            while (i < sl && l < VTModule.MaxOrnamentLength)
            {
                // Skip characters that are not digits, '-' or '+', or 'L'
                while (i < sl && !(char.IsDigit(orst[i]) || orst[i] == '-' || orst[i] == '+' || orst[i] == 'L'))
                {
                    i++;
                }

                if (i < sl)
                {
                    if (orst[i] == 'L')
                    {
                        lp = l;
                        i++;
                    }
                    else
                    {
                        // Found a number; mark the start of the token
                        j = i;
                        // If there's a sign, include it.
                        if (i < sl && (orst[i] == '-' || orst[i] == '+'))
                        {
                            i++;
                        }

                        // Continue until we hit a non-digit character.
                        while (i < sl && char.IsDigit(orst[i]))
                        {
                            i++;
                        }

                        string token = orst.Substring(j, i - j);

                        if (!sbyte.TryParse(token, out ornament.Offsets[l]))
                            return false;

                        l++;
                    }
                }
            }

            bool result = (l != 0);

            if (result)
            {
                ornament.Length = l;
                ornament.Loop = lp;
                for (int k = l; k < VTModule.MaxOrnamentLength; k++)
                    ornament.Offsets[k] = 0;
            }

            //Debug.WriteLine(ornament.ToString());

            return result;
        }

        public object Clone()
        {
            Ornament clone = (Ornament)this.MemberwiseClone();
            clone.Offsets = (sbyte[])Offsets.Clone();

            return clone;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < Length; i++)
            {
                if (i == Loop)
                    sb.Append("L");

                sb.Append(Offsets[i]);

                if (i < Length - 1)
                    sb.Append(",");
            }

            return sb.ToString() + "\n";
        }
    }
}
