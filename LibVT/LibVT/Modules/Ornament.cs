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
