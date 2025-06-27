using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibVT
{
    public class AdditionalCommand : ICloneable
    {
        public byte Number;
        public byte Delay;
        public byte Parameter;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class ChannelLine : ICloneable
    {
        public sbyte Note;    // 0..95, -2 - Sound off (R--), -1 - No note (---)
        public byte Sample;   // 0..31
        public byte Ornament; // 0..15
        public sbyte Volume;  // 1-15 - vol, 0 - prev vol
        public byte Envelope; // 1-14 - R13, 15 - Envelope off, 0 - prev
        public AdditionalCommand AdditionalCommand;

        public ChannelLine()
        {
            Note = -1;
            Sample = 0;
            Ornament = 0;
            Volume = 0;
            Envelope = 0;
            AdditionalCommand = new AdditionalCommand();
        }

        public object Clone()
        {
            ChannelLine channelLine = (ChannelLine)this.MemberwiseClone();

            channelLine.AdditionalCommand = (AdditionalCommand)AdditionalCommand.Clone();

            return channelLine;
        }
    }

    public class Line : ICloneable
    {
        public byte Noise;
        public ushort Envelope;
        public ChannelLine[] Channel;

        public Line()
        {
            Noise = 0;
            Envelope = 0;
            Channel = new ChannelLine[3];

            for (int i = 0; i < 3; i++)
                Channel[i] = new ChannelLine();
        }

        public object Clone()
        {
            Line line = (Line)this.MemberwiseClone();

            line.Channel = new ChannelLine[3];

            for (int i = 0; i < 3; i++)
                line.Channel[i] = (ChannelLine)Channel[i].Clone();

            return line;
        }
    }

    public class Pattern : ICloneable
    {
        public int Length;
        public Line[] Lines;

        public Pattern()
        {
            Lines = new Line[VTModule.MaxPatternLength];

            for (int i = 0; i < VTModule.MaxPatternLength; i++)
            {
                Lines[i] = new Line();
                Lines[i].Envelope = 0;
                Lines[i].Noise = 0;
                Lines[i].Channel[0] = new ChannelLine();
                Lines[i].Channel[1] = new ChannelLine();
                Lines[i].Channel[2] = new ChannelLine();
            }

            Length = VTModule.DefaultPatternLength;
        }

        public static int LoadPatternDataTxt(string[] patternLines, out Pattern pattern, bool decNoise)
        {
            pattern = new Pattern();
            int length = 0;

            foreach (var patternLine in patternLines)
            {
                if (string.IsNullOrEmpty(patternLine))
                    continue;

                if (!RecognizePatternString(patternLine, pattern, decNoise, length))
                    return 6;

                if (++length == VTModule.MaxPatternLength)
                    break;
            }

            if (length == 0)
                return 6;

            pattern.Length = length;

            for (int l = length; l < VTModule.MaxPatternLength; l++)
            {
                pattern.Lines[l].Noise = 0;
                pattern.Lines[l].Envelope = 0;

                for (int ch = 0; ch < 3; ch++)
                    pattern.Lines[l].Channel[ch] = new ChannelLine();
            }

            return 0;
        }

        private static bool RecognizePatternString(string patternLine, Pattern pattern, bool decNoise, int len)
        {
            int value = 0, j;

            if (patternLine.Length != 49 && patternLine.Length != 48)
                return false;

            if (VTModule.SGetNumber(patternLine.Substring(0, 4), 65535, out value))
                pattern.Lines[len].Envelope = (ushort)value;
            else if (VTModule.SGetNote(patternLine.Substring(0, 3), out value))
                pattern.Lines[len].Envelope = (ushort)Math.Round(VTModule.GetNoteFreq(Main.NoteTableOnLoad, value) / 16.0);
            else
                return false;

            patternLine = " " + patternLine;

            if (decNoise)
            {
                if (!VTModule.SGetDecNumber(patternLine.Substring(6, 2), 31, out value))
                    return false;
            }
            else if (!VTModule.SGetNumber(patternLine.Substring(6, 2), 31, out value))
                return false;

            pattern.Lines[len].Noise = (byte)value;

            for (int ch = 0; ch < 3; ch++)
            {
                if (!VTModule.SGetNote(patternLine.Substring(9 + ch * 14, 3), out j))
                    return false;
                pattern.Lines[len].Channel[ch].Note = (sbyte)j;
                if (!VTModule.SGetNumber(patternLine.Substring(13 + ch * 14, 1), 32, out j))
                    return false;
                pattern.Lines[len].Channel[ch].Sample = (byte)j;
                if (!VTModule.SGetNumber(patternLine.Substring(14 + ch * 14, 1), 16, out j))
                    return false;
                pattern.Lines[len].Channel[ch].Envelope = (byte)j;
                if (!VTModule.SGetNumber(patternLine.Substring(15 + ch * 14, 1), 16, out j))
                    return false;
                pattern.Lines[len].Channel[ch].Ornament = (byte)j;
                if (!VTModule.SGetNumber(patternLine.Substring(16 + ch * 14, 1), 16, out j))
                    return false;
                pattern.Lines[len].Channel[ch].Volume = (sbyte)j;
                if (!VTModule.SGetNumber(patternLine.Substring(18 + ch * 14, 1), 16, out j))
                    return false;
                pattern.Lines[len].Channel[ch].AdditionalCommand.Number = (byte)j;
                if (!VTModule.SGetNumber(patternLine.Substring(19 + ch * 14, 1), 16, out j))
                    return false;
                pattern.Lines[len].Channel[ch].AdditionalCommand.Delay = (byte)j;
                if (!VTModule.SGetNumber(patternLine.Substring(20 + ch * 14, 2), 256, out j))
                    return false;
                pattern.Lines[len].Channel[ch].AdditionalCommand.Parameter = (byte)j;
            }

            return true;
        }


        public object Clone()
        {
            Pattern pattern = new Pattern
            {
                Length = this.Length
            };

            pattern.Lines = new Line[Lines.Length];

            for (int i = 0; i < this.Lines.Length; i++)
                pattern.Lines[i] = (Line)Lines[i].Clone();

            return pattern;
        }


        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Length; i++)
            {
                result += VTModule.GetPatternLineString(this, i, new int[] { 0, 1, 2 }, true, true, false, false);
                result += "\n";
            }

            return result;
        }
    }
}
