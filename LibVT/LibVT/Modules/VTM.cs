using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibVT
{
    // [StructLayout(LayoutKind.Sequential)]
    public class Position : ICloneable
    {
        // 0..42 for compatability with Pro Tracker 3.4r,
        // i.e. max 43 patterns (85 in 3.6+)
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public int[] Value;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public int[] Colors;
        public int Length;
        public int Loop;

        public object Clone()
        {
            Position position = (Position)this.MemberwiseClone();
            position.Value = (int[])this.Value.Clone();
            position.Colors = (int[])this.Colors.Clone();
            return position;
        }
    }

    public class ChannelState : ICloneable
    {
        public bool GlobalTone;
        public bool GlobalNoise;
        public bool GlobalEnvelope;
        public bool EnvelopeEnabled;
        public byte Ornament;
        public byte Sample;
        public byte Volume;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public enum NoteTableType : byte
    {
        ProTracker,   // ProTracker 3.3
        SoundTracker, // Sound Tracker
        ASM,          // ASM or PSC (1.75 MHz)
        Real,         // RealSound
        Natural,      // IvanRochin NATURAL Cmaj/Am
        Custom        // Custom
    }

    public class VTM : ICloneable
    {
        public int ChipFreq;
        public int IntFreq;
        public string Title;
        public string Author;
        public string Info;
        public bool ShowInfo;
        public NoteTableType NoteTable;
        public byte InitialDelay;
        public Position Positions;
        public Sample[] Samples;     // 32 sample for samples browser preview (check PreviewSamNum const)
        public Ornament[] Ornaments; // 16 ornament for ornaments browser preview (check PreviewOrnNum const)
        public Pattern[] Patterns;
        public Pattern ReservedPattern;
        public FeaturesLevel FeaturesLevel;
        public bool HasHeader;
        public ChannelState[] ChannelStates;
        public byte[] Data;

        public VTM()
        {
            Positions = new Position
            {
                Value = new int[256],
                Colors = new int[256]
            };

            Samples = new Sample[VTModule.MaxSampleLength + 1];
            Ornaments = new Ornament[VTModule.MaxOrnamentLength + 1];
            Patterns = new Pattern[VTModule.MaxPatternLength];
            ChannelStates = new ChannelState[3];

            for (int i = 0; i < 3; i++)
                ChannelStates[i] = new ChannelState();

            Title = "";
            Author = "";
            Info = "";
            ShowInfo = false;
            NoteTable = NoteTableType.ASM;
            InitialDelay = 3;
            Positions.Length = 0;
            Positions.Loop = 0;

            for (int i = 0; i < Samples.Length; i++)
                Samples[i] = null;

            for (int i = 0; i < Ornaments.Length; i++)
                Ornaments[i] = null;

            for (int i = 0; i <= VTModule.MaxPatternIndex; i++)
                Patterns[i] = null;

            for (int i = 0; i < Positions.Value.Length; i++)
            {
                Positions.Value[i] = 0;
                Positions.Colors[i] = 0;
            }

            ReservedPattern = new Pattern();
            ReservedPattern.Length = 2;
            ReservedPattern.Lines[0].Envelope = 0;
            ReservedPattern.Lines[1].Envelope = 0;
            ReservedPattern.Lines[0].Noise = 0;
            ReservedPattern.Lines[1].Noise = 0;
            ReservedPattern.Lines[0].Channel[0] = new ChannelLine();
            ReservedPattern.Lines[0].Channel[1] = new ChannelLine();
            ReservedPattern.Lines[0].Channel[2] = new ChannelLine();
            ReservedPattern.Lines[1].Channel[0] = new ChannelLine();
            ReservedPattern.Lines[1].Channel[1] = new ChannelLine();
            ReservedPattern.Lines[1].Channel[2] = new ChannelLine();
            ReservedPattern.Lines[0].Channel[0].Note = 36;
            ReservedPattern.Lines[1].Channel[0].Note = 36;
            ReservedPattern.Lines[0].Channel[0].Sample = 1;
            ReservedPattern.Lines[1].Channel[0].Sample = 1;
            ReservedPattern.Lines[0].Channel[0].Envelope = 15;
            ReservedPattern.Lines[1].Channel[0].Envelope = 15;
            ReservedPattern.Lines[0].Channel[0].Ornament = 1;
            ReservedPattern.Lines[0].Channel[0].Volume = 15;
            ReservedPattern.Lines[1].Channel[0].Volume = 15;

            for (int i = 0; i < 3; i++)
            {
                ChannelStates[i].GlobalTone = true;
                ChannelStates[i].GlobalNoise = true;
                ChannelStates[i].GlobalEnvelope = true;
                ChannelStates[i].Sample = 1;
                ChannelStates[i].EnvelopeEnabled = false;
                ChannelStates[i].Ornament = 0;
                ChannelStates[i].Volume = 15;
            }

            Positions.Loop = 0;
            ChipFreq = Main.DefaultChipFreq;
            IntFreq = Main.DefaultIntFreq;
            FeaturesLevel = VTModule.FeaturesLevel;
            HasHeader = VTModule.VortexModuleHeader;
        }

        public static int LoadModuleFromText(string fileName, VTM vtm1, ref VTM vtm2, ref VTM vtm3)
        {
            bool decNoise = false;
            bool patternsFound = false;
            bool samplesFound = false;
            bool ornamentsFound = false;
            VTM vtm = null;

            string line;
            string currentSection = "";
            int moduleIndex = 0;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (string.IsNullOrEmpty(line))
                            continue;

                        if (line.StartsWith("[") && line.EndsWith("]"))
                            currentSection = line.Substring(1, line.Length - 2).ToUpper();

                        if (currentSection.Equals("MODULE"))
                        {
                            switch (moduleIndex)
                            {
                                case 0:
                                    vtm = vtm1;
                                    break;
                                case 1:
                                    vtm = vtm2 = new VTM();
                                    break;
                                case 2:
                                    vtm = vtm3 = new VTM();
                                    break;
                            }

                            int result = ProcessModuleInfo(streamReader, vtm, out decNoise);

                            if (result != 0)
                                return result;

                            moduleIndex++;
                        }
                        else if (currentSection.StartsWith("ORNAMENT"))
                        {
                            if (!ProcessOrnament(streamReader, currentSection, vtm, out ornamentsFound))
                                return 2;
                        }
                        else if (currentSection.StartsWith("SAMPLE"))
                        {
                            if (!ProcessSample(streamReader, currentSection, vtm, out samplesFound, decNoise))
                                return 5;
                        }
                        else if (currentSection.StartsWith("PATTERN"))
                        {
                            if (!ProcessPattern(streamReader, currentSection, vtm, out patternsFound, decNoise))
                                return 2;
                        }
                        else if (currentSection.Equals("INFO"))
                        {
                            ProcessInfo(streamReader, vtm);
                        }
                    }
                }
            }

            if (!patternsFound)
                return -1;

            if (!samplesFound)
                return -2;

            if (!ornamentsFound)
                return -3;

            return 0;
        }

        private static int ProcessModuleInfo(StreamReader streamReader, VTM vtm, out bool decNoise)
        {
            string line;
            decNoise = false;

            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                    return 0;

                if (line.StartsWith("[") && line.EndsWith("]"))
                    return 0;

                int eqPos = line.IndexOf('=');

                if (eqPos < 1)
                    return 0;

                string key = line.Substring(0, eqPos).Trim().ToUpper();
                string value = eqPos == line.Length - 1 ? "" : line.Substring(eqPos + 1).Trim();

                switch (key)
                {
                    case "VORTEXTRACKERII":
                        vtm.HasHeader = value != "0";
                        break;
                    case "VERSION":
                        vtm.FeaturesLevel = value == "3.5" ? FeaturesLevel.PT35 : value == "3.7" ? FeaturesLevel.PT37 : FeaturesLevel.VTII_PT36;
                        break;
                    case "TITLE":
                        vtm.Title = value.Length > 32 ? value.Substring(0, 32) : value;
                        break;
                    case "AUTHOR":
                        vtm.Author = value.Length > 32 ? value.Substring(0, 32) : value;
                        break;
                    case "NOTETABLE":
                        if (!int.TryParse(value, out int noteTable))
                            return 2;

                        if (noteTable < 0 || noteTable > 4)
                            return 3;

                        vtm.NoteTable = (NoteTableType)noteTable;
                        break;
                    case "SHOWINFO":
                        vtm.ShowInfo = value == "1";
                        break;
                    case "CHIPFREQ":
                        if (!int.TryParse(value, out vtm.ChipFreq))
                            return 2;
                        break;
                    case "INTFREQ":
                        if (!int.TryParse(value, out vtm.IntFreq))
                            return 2;
                        break;
                    case "NOISE":
                        decNoise = value.ToUpper() == "DEC";
                        break;
                    case "SPEED":
                        if (!int.TryParse(value, out int speed))
                            return 2;

                        if (speed < 1 || speed > 255)
                            return 3;

                        vtm.InitialDelay = (byte)speed;
                        break;
                    case "PLAYORDER":
                        ProcessPlayOrder(value, vtm);
                        break;
                    case "COLORS":
                        ProcessColors(value, vtm);
                        break;
                }
            }

            return 0;
        }

        private static int ProcessInfo(StreamReader streamReader, VTM vtm)
        {
            string line;
            List<string> infoLines = new List<string>();

            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                    break;

                infoLines.Add(line);
            }

            vtm.Info = String.Join("\n", infoLines.ToArray());

            return 0;
        }

        private static void ProcessPlayOrder(string value, VTM vtm)
        {
            vtm.Positions = new Position
            {
                Value = new int[VTModule.MaxPatternCount],
                Colors = new int[VTModule.MaxPatternCount],
                Length = 0,
                Loop = 0
            };

            string[] parts = value.Split(',');

            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                bool isLoop = trimmed.StartsWith("L");
                if (isLoop)
                    trimmed = trimmed.Substring(1);

                if (int.TryParse(trimmed, out int patternNum) && patternNum >= 0 && patternNum <= VTModule.MaxPatternIndex)
                {
                    if (isLoop)
                        vtm.Positions.Loop = vtm.Positions.Length;

                    vtm.Positions.Value[vtm.Positions.Length++] = patternNum;
                }
            }
        }

        private static void ProcessColors(string value, VTM vtm)
        {
            string[] parts = value.Split(',');
            for (int i = 0; i < parts.Length && i < vtm.Positions.Colors.Length; i++)
            {
                if (int.TryParse(parts[i], out int color))
                    vtm.Positions.Colors[i] = color;
            }
        }

        private static bool ProcessOrnament(StreamReader streamReader, string sectionHeader, VTM vtm, out bool ornamentsFound)
        {
            ornamentsFound = false;

            // Extract the ornament index from section header "[OrnamentX]"
            if (!int.TryParse(sectionHeader.Substring("ORNAMENT".Length), out int index) || index < 1 || index > 31)
                return false;

            // Read the next line (ornament data)
            string line = streamReader.ReadLine();

            if (!IsLineValid(line))
                return false;

            // Create new ornament
            Ornament orn = new Ornament();

            // Parse ornament data
            if (!Ornament.RecognizeOrnamentString(line, orn))
                return false;

            // Store ornament
            vtm.Ornaments[index] = orn;
            ornamentsFound = true;

            return true;
        }

        private static bool ProcessSample(StreamReader streamReader, string sectionHeader, VTM vtm, out bool samplesFound, bool decNoise)
        {
            samplesFound = false;

            if (!int.TryParse(sectionHeader.Substring("SAMPLE".Length), out int index) || index < 1 || index > 31)
                return false;

            List<string> sampleLines = new List<string>();
            string line;

            do
            {
                line = streamReader.ReadLine()?.Trim();

                if (IsLineValid(line))
                    sampleLines.Add(line);
            }
            while (IsLineValid(line));

            if (sampleLines.Count == 0 || Sample.LoadSampleDataTxt(sampleLines.ToArray(), out Sample sample, decNoise) != 0)
                return false;

            vtm.Samples[index] = sample;
            samplesFound = true;

            return true;
        }

        private static bool ProcessPattern(StreamReader streamReader, string sectionHeader, VTM vtm, out bool patternsFound, bool decNoise)
        {
            patternsFound = false;

            if (!int.TryParse(sectionHeader.Substring("PATTERN".Length), out int index) || index < 0 || index > VTModule.MaxPatternIndex)
                return false;

            List<string> patternLines = new List<string>();
            string line;

            do
            {
                line = streamReader.ReadLine()?.Trim();

                if (IsLineValid(line))
                    patternLines.Add(line);
            }
            while (IsLineValid(line));

            if (patternLines.Count == 0 || Pattern.LoadPatternDataTxt(patternLines.ToArray(), out Pattern pattern, decNoise) != 0)
                return false;

            vtm.Patterns[index] = pattern;
            patternsFound = true;

            return true;
        }

        public static bool IsLineValid(string line)
        {
            return !string.IsNullOrEmpty(line) && !line.StartsWith("[");
        }

        public static void SavePattern(TextWriter textWriter, VTM vtm, int n, int tracksCursorXLeft)
        {
            string s;
            int i, l;

            bool envelopeAsNode = Main.EnvelopeAsNote;
            bool decBaseNoiseOn = Main.DecBaseNoiseOn;
            Main.EnvelopeAsNote = false;
            Main.DecBaseNoiseOn = false;
            l = VTModule.DefaultPatternLength;

            if (vtm.Patterns[n] != null)
                l = vtm.Patterns[n].Length;

            for (i = 0; i < l; i++)
            {
                s = VTModule.GetPatternLineString(vtm.Patterns[n], i, VTModule.StdChns, true, true, false, false);
                textWriter.WriteLine(s.Substring(tracksCursorXLeft + 1 - 1, s.Length - tracksCursorXLeft));
            }

            Main.EnvelopeAsNote = envelopeAsNode;
            Main.DecBaseNoiseOn = decBaseNoiseOn;
        }

        public static void SaveOrnament(TextWriter textWriter, VTM vtm, int n)
        {
            int lp, l, i;

            if (vtm.Ornaments[n] == null)
                textWriter.WriteLine("L0");
            else
            {
                Ornament ornament = vtm.Ornaments[n];
                lp = ornament.Loop;
                l = ornament.Length;

                for (i = 0; i < l; i++)
                {
                    if (i == lp)
                        textWriter.Write('L');

                    textWriter.Write((ornament.Offsets[i]).ToString());
                    
                    if (i < l)
                        textWriter.Write(',');
                }

                textWriter.WriteLine();
            }
        }

        public static void SaveSample(TextWriter textWriter, VTM vtm, int n)
        {
            int lp, l, i;

            if (vtm.Samples[n] == null)
                textWriter.WriteLine("... +000_ +00_ 0_ L");
            else
            {
                Sample sample = vtm.Samples[n];
                lp = sample.Loop;
                l = sample.Length;
                
                for (i = 0; i < l; i++)
                {
                    textWriter.Write(VTModule.GetSampleString(sample.Ticks[i], false, false));
                    
                    if (i == lp)
                        textWriter.Write(" L");
                }

                textWriter.WriteLine();
            }
        }

        public object Clone()
        {
            VTM module = new VTM();

            // Copy primitives and immutable types
            module.ChipFreq = this.ChipFreq;
            module.IntFreq = this.IntFreq;
            module.Title = this.Title;
            module.Author = this.Author;
            module.Info = this.Info;
            module.ShowInfo = this.ShowInfo;
            module.NoteTable = this.NoteTable;
            module.InitialDelay = this.InitialDelay;
            module.FeaturesLevel = this.FeaturesLevel;
            module.HasHeader = this.HasHeader;

            // Deep clone Positions (a struct with arrays)
            module.Positions = new Position
            {
                Value = (int[])this.Positions.Value.Clone(),
                Colors = (int[])this.Positions.Colors.Clone(),
                Length = this.Positions.Length,
                Loop = this.Positions.Loop
            };

            // Deep clone Samples (TSample implements ICloneable)
            module.Samples = new Sample[this.Samples.Length];
            for (int i = 0; i < this.Samples.Length; i++)
            {
                if (this.Samples[i] != null)
                    module.Samples[i] = (Sample)this.Samples[i].Clone();
                else
                    module.Samples[i] = null;
            }

            // Deep clone Ornaments (TOrnament implements ICloneable)
            module.Ornaments = new Ornament[this.Ornaments.Length];
            for (int i = 0; i < this.Ornaments.Length; i++)
            {
                if (this.Ornaments[i] != null)
                    module.Ornaments[i] = (Ornament)this.Ornaments[i].Clone();
                else
                    module.Ornaments[i] = null;
            }

            // Deep clone Patterns.
            module.Patterns = new Pattern[this.Patterns.Length];
            for (int i = 0; i < this.Patterns.Length; i++)
            {
                if (this.Patterns[i] != null)
                    module.Patterns[i] = (Pattern)this.Patterns[i].Clone();
                else
                    module.Patterns[i] = null;
            }

            // Deep clone ReservedPattern
            module.ReservedPattern = (Pattern)this.ReservedPattern.Clone();

            // Deep clone IsChans (structs; simple copy is sufficient)
            module.ChannelStates = new ChannelState[this.ChannelStates.Length];
            for (int i = 0; i < this.ChannelStates.Length; i++)
            {
                module.ChannelStates[i] = (ChannelState)this.ChannelStates[i].Clone();
            }

            return module;
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Ornaments.Length; i++)
            {
                if (Ornaments[i] == null)
                    continue;

                result += $"[Ornament{i}]\n";
                result += Ornaments[i].ToString() + "\n";
            }

            for (int i = 0; i < Samples.Length; i++)
            {
                if (Samples[i] == null)
                    continue;

                result += $"[Sample{i}]\n";
                result += Samples[i].ToString() + "\n";
            }

            for (int i = 0; i < Patterns.Length; i++)
            {
                if (Patterns[i] == null)
                    continue;

                result += $"[Pattern{i}]\n";
                result += Patterns[i].ToString() + "\n";
            }

            return result;
        }
    }
}
