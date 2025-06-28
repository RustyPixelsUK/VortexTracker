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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibVT
{
    public class SampleTick : ICloneable
    {
        public short AddToTone;
        public bool Ton_Accumulation;
        public byte Amplitude;
        public bool Amplitude_Sliding;
        public bool Amplitude_Slide_Up;
        public bool Envelope_Enabled;
        public bool Envelope_or_Noise_Accumulation;
        public sbyte Add_to_Envelope_or_Noise;
        public bool Mixer_Ton;
        public bool Mixer_Noise;

        public SampleTick()
        {
            AddToTone = 0;
            Ton_Accumulation = false;
            Amplitude = 0;
            Amplitude_Sliding = false;
            Amplitude_Slide_Up = false;
            Envelope_Enabled = false;
            Envelope_or_Noise_Accumulation = false;
            Add_to_Envelope_or_Noise = 0;
            Mixer_Ton = false;
            Mixer_Noise = false;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class Sample : ICloneable
    {
        public byte Length;
        public byte Loop;
        public bool Enabled;
        public SampleTick[] Ticks;

        public Sample()
        {
            Ticks = new SampleTick[VTModule.MaxSampleLength];

            Loop = 0;
            Length = 1;
            Enabled = true;
            Ticks[0] = new SampleTick();

            for (int i = 1; i < VTModule.MaxSampleLength; i++)
                Ticks[i] = new SampleTick();
        }

        public static void ValidateSample(int sam, VTM vtm)
        {
            if (vtm.Samples[sam] == null)
            {
                vtm.Samples[sam] = new Sample
                {
                    Loop = 0,
                    Length = 1,
                    Enabled = true
                };

                vtm.Samples[sam].Ticks[0] = new SampleTick
                {
                    AddToTone = 0,
                    Ton_Accumulation = false,
                    Amplitude = 0,
                    Amplitude_Sliding = false,
                    Amplitude_Slide_Up = false,
                    Envelope_Enabled = false,
                    Envelope_or_Noise_Accumulation = false,
                    Add_to_Envelope_or_Noise = 0,
                    Mixer_Ton = false,
                    Mixer_Noise = false
                };

                for (int i = 1; i < VTModule.MaxSampleLength; i++)
                {
                    vtm.Samples[sam].Ticks[i] = new SampleTick();

                    /*
                    // Sample templates are disabled now
                    vtm.Samples[sam].Items[i] = MainForm.SampleLineTemplates[MainForm.CurrentSampleLineTemplate];
                    */
                }
            }
        }

        public static int LoadSampleDataTxt(string[] sampleLines, out Sample sample, bool decNoise)
        {
            sample = new Sample();
            int length = 0;
            int loop = -1; // Initialize loop point to -1 indicating no loop

            foreach (var line in sampleLines)
            {
                if (!RecognizeSampleString(line, sample, decNoise, length, out int lineLoop))
                    return 6;

                if (lineLoop != -1)
                    loop = lineLoop; // Update loop point if found

                if (++length == VTModule.MaxSampleLength)
                    break;
            }

            if (length == 0)
                return 6;

            sample.Loop = (byte)(loop != -1 ? loop : 0);
            sample.Length = (byte)length;
            sample.Enabled = true;

            // Fill the remaining sample slots with new samples
            for (int i = length; i < VTModule.MaxSampleLength; i++)
                sample.Ticks[i] = new SampleTick();

            return 0;
        }

        private static bool RecognizeSampleString(string str, Sample sample, bool decNoise, int length, out int loop)
        {
            str = str.Trim();
            loop = -1;

            if (string.IsNullOrEmpty(str))
                return false;

            int pos = 0, num = 0;
            sample.Ticks[length] = new SampleTick();

            // Parsing logic for Mixer_Ton
            if (!FindNextChar(str, ref pos, 't', 'T', '.'))
                return false;
            sample.Ticks[length].Mixer_Ton = str[pos++] == 'T';

            // Parsing logic for Mixer_Noise
            if (!FindNextChar(str, ref pos, 'n', 'N', '.'))
                return false;
            sample.Ticks[length].Mixer_Noise = str[pos++] == 'N';

            // Parsing logic for Envelope_Enabled
            if (!FindNextChar(str, ref pos, 'e', 'E', '.'))
                return false;
            sample.Ticks[length].Envelope_Enabled = str[pos++] == 'E';

            // Parsing logic for Add_to_Ton
            if (!GetNum(str, ref pos, decNoise, out num))
                return false;
            sample.Ticks[length].AddToTone = (short)num;

            // Parsing logic for Ton_Accumulation
            if (!FindNextChar(str, ref pos, '_', '^'))
                return false;
            sample.Ticks[length].Ton_Accumulation = str[pos++] == '^';

            // Parsing logic for Add_to_Envelope_or_Noise
            if (!GetNum(str, ref pos, decNoise, out num))
                return false;
            sample.Ticks[length].Add_to_Envelope_or_Noise = (sbyte)(num & 0x1F);
            if ((sample.Ticks[length].Add_to_Envelope_or_Noise & 0x10) != 0)
                sample.Ticks[length].Add_to_Envelope_or_Noise |= unchecked((sbyte)0xF0);

            // Parsing logic for Envelope_or_Noise_Accumulation
            if (!FindNextChar(str, ref pos, '_', '^'))
                return false;
            sample.Ticks[length].Envelope_or_Noise_Accumulation = str[pos++] == '^';

            // Parsing logic for Amplitude
            if (!GetNum(str, ref pos, decNoise, out num))
                return false;
            sample.Ticks[length].Amplitude = (byte)(num & 15);

            // Parsing logic for Amplitude_Sliding and Amplitude_Slide_Up
            if (!FindNextChar(str, ref pos, '_', '+', '-'))
                return false;
            if (str[pos] == '+' || str[pos] == '-')
            {
                sample.Ticks[length].Amplitude_Sliding = true;
                sample.Ticks[length].Amplitude_Slide_Up = str[pos] == '+';
            }
            pos++;

            if (FindNextChar(str, ref pos, 'l', 'L'))
                loop = length;

            return true;
        }

        private static bool FindNextChar(string str, ref int pos, params char[] chars)
        {
            while (pos < str.Length && Array.IndexOf(chars, str[pos]) == -1)
                pos++;

            return pos < str.Length;
        }

        private static bool GetNum(string str, ref int pos, bool decNoise, out int num)
        {
            num = 0;
            int sign = 1;
            var hexValue = new StringBuilder();

            // Move to the next valid character
            while (pos < str.Length && !Uri.IsHexDigit(str[pos]) && str[pos] != '+' && str[pos] != '-')
                pos++;

            // If we're out of bounds, return false
            if (pos >= str.Length)
                return false;

            // Check for sign
            if (str[pos] == '+' || str[pos] == '-')
            {
                sign = (str[pos] == '-') ? -1 : 1;
                pos++;
            }

            // Special handling when decNoise is true and pos is 11
            if (decNoise && pos == 11 && pos + 1 < str.Length)
            {
                if (int.TryParse(str.Substring(pos, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                {
                    num *= sign;
                    pos += 2;
                    return true;
                }
                return false;
            }

            // Read the numeric portion
            while (pos < str.Length && Uri.IsHexDigit(str[pos]))
            {
                hexValue.Append(str[pos]);
                pos++;
            }

            // If we didn't collect any digits, it's not a valid number
            if (hexValue.Length == 0)
                return false;

            // Convert the hex string to a number
            if (!int.TryParse(hexValue.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                return false;

            // Apply the sign
            num *= sign;

            return true;
        }

        public object Clone()
        {
            Sample sample = (Sample)this.MemberwiseClone();

            sample.Ticks = new SampleTick[Ticks.Length];

            for (int i = 0; i < this.Ticks.Length; i++)
                sample.Ticks[i] = (SampleTick)Ticks[i].Clone();

            return sample;
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Length; i++)
            {
                result += VTModule.GetSampleString(Ticks[i], false, false);

                if (i == Loop)
                    result += " L";

                result += "\n";
            }

            return result;
        }
    }
}
