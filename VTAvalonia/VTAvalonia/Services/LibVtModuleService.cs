using LibVT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class LibVtModuleService : IModuleService
{
    private VTM? _vtm;
    private VTM? _vtm2;
    private VTM? _vtm3;
    private int _chipCount;
    private bool _loopingEnabled = true;

    public ModuleInfo? Current { get; private set; }
    public bool IsPlaying => WaveOutAPI.IsPlaying;
    public bool LoopingEnabled => _loopingEnabled;

    public ModuleInfo CreateNew()
    {
        _vtm = new VTM();
        _vtm2 = null;
        _vtm3 = null;
        _chipCount = 1;

        // Seed one empty position pointing at pattern 0
        _vtm.Positions.Length = 1;
        _vtm.Positions.Value[0] = 0;
        _vtm.Positions.Loop = 0;

        // Seed an empty pattern 0
        _vtm.Patterns[0] = new Pattern();

        // Seed sample 1 and ornament 1 as blanks
        _vtm.Samples[1] = new Sample();
        _vtm.Ornaments[1] = new Ornament();

        Current = new ModuleInfo("", _vtm.Title, _vtm.Author, 1);
        return Current;
    }

    public Task<ModuleInfo?> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        int cnt = 0;
        VTM? vtm = null;
        VTM? vtm2 = null;
        VTM? vtm3 = null;

        var ok = WaveOutAPI.LoadTrackerModule(filePath, 0, ref cnt, ref vtm, ref vtm2, ref vtm3);
        if (!ok || vtm == null)
            return Task.FromResult<ModuleInfo?>(null);

        _vtm = vtm;
        _vtm2 = vtm2;
        _vtm3 = vtm3;
        _chipCount = cnt;

        Current = new ModuleInfo(filePath, vtm.Title, vtm.Author, cnt);
        return Task.FromResult<ModuleInfo?>(Current);
    }

    public bool TryGetLoadedModules(out VTM? vtm, out VTM? vtm2, out VTM? vtm3, out int chipCount)
    {
        vtm = _vtm;
        vtm2 = _vtm2;
        vtm3 = _vtm3;
        chipCount = _chipCount;
        return _vtm != null;
    }

    public Task<bool> SaveAsTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (_vtm == null)
            return Task.FromResult(false);

        VTModule.VTM2TextFile(filePath, _vtm, false, 0);

        if (_vtm2 != null)
            VTModule.VTM2TextFile(filePath, _vtm2, true, 0);

        if (_vtm3 != null)
            VTModule.VTM2TextFile(filePath, _vtm3, true, 0);

        return Task.FromResult(true);
    }

    public IReadOnlyList<int> GetPositions()
    {
        if (_vtm?.Positions == null)
            return Array.Empty<int>();

        var count = _vtm.Positions.Length;
        var positions = new int[count];
        Array.Copy(_vtm.Positions.Value, positions, count);
        return positions;
    }

    public IReadOnlyList<string> GetPatternLines(int patternIndex)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return Array.Empty<string>();

        var pattern = _vtm.Patterns[patternIndex];
        var length = pattern?.Length ?? VTModule.DefaultPatternLength;
        var lines = new List<string>(length);

        for (int line = 0; line < length; line++)
            lines.Add(VTModule.GetPatternLineString(pattern, line, VTModule.StdChns, true, true, false, false));

        return lines;
    }

    public bool UpdatePatternLines(int patternIndex, IReadOnlyList<string> lines)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return false;

        var parsed = Pattern.LoadPatternDataTxt(lines.ToArray(), out var pattern, false);
        if (parsed != 0)
            return false;

        _vtm.Patterns[patternIndex] = pattern;
        return true;
    }

    public int GetLoopPosition()
    {
        return _vtm?.Positions?.Loop ?? 0;
    }

    public bool UpdatePositions(IReadOnlyList<int> positions, int loopPosition)
    {
        if (_vtm?.Positions == null)
            return false;

        if (positions.Count > VTModule.MaxPatternCount)
            return false;

        for (int i = 0; i < positions.Count; i++)
        {
            if (positions[i] < 0 || positions[i] > VTModule.MaxPatternIndex)
                return false;
        }

        _vtm.Positions.Length = positions.Count;
        for (int i = 0; i < _vtm.Positions.Value.Length; i++)
            _vtm.Positions.Value[i] = i < positions.Count ? positions[i] : 0;

        _vtm.Positions.Loop = loopPosition >= 0 && loopPosition < positions.Count ? loopPosition : 0;
        return true;
    }

    public ModuleHeader? GetModuleHeader()
    {
        if (_vtm == null)
            return null;

        return new ModuleHeader(
            _vtm.Title,
            _vtm.Author,
            _vtm.InitialDelay,
            (int)_vtm.NoteTable,
            _vtm.ChipFreq,
            _vtm.IntFreq);
    }

    public bool UpdateModuleHeader(ModuleHeader header)
    {
        if (_vtm == null)
            return false;

        _vtm.Title  = header.Title.Length > 32 ? header.Title[..32] : header.Title;
        _vtm.Author = header.Author.Length > 32 ? header.Author[..32] : header.Author;
        _vtm.InitialDelay = (byte)Math.Clamp(header.InitialDelay, 1, 255);
        _vtm.NoteTable    = (NoteTableType)Math.Clamp(header.NoteTable, 0, 5);
        _vtm.ChipFreq     = header.ChipFreq;
        _vtm.IntFreq      = header.IntFreq;

        Current = Current == null
            ? new ModuleInfo("", _vtm.Title, _vtm.Author, _chipCount)
            : Current with { Title = _vtm.Title, Author = _vtm.Author };

        return true;
    }

    public bool DuplicatePosition(int positionIndex)
    {
        if (_vtm?.Positions == null)
            return false;

        var count = _vtm.Positions.Length;
        if (positionIndex < 0 || positionIndex >= count)
            return false;

        if (count >= VTModule.MaxPatternCount)
            return false;

        // Shift entries after insertion point
        for (int i = count; i > positionIndex + 1; i--)
        {
            _vtm.Positions.Value[i]  = _vtm.Positions.Value[i - 1];
            _vtm.Positions.Colors[i] = _vtm.Positions.Colors[i - 1];
        }

        _vtm.Positions.Value[positionIndex + 1]  = _vtm.Positions.Value[positionIndex];
        _vtm.Positions.Colors[positionIndex + 1] = _vtm.Positions.Colors[positionIndex];
        _vtm.Positions.Length++;
        return true;
    }

    public bool ClonePosition(int positionIndex)
    {
        if (_vtm?.Positions == null)
            return false;

        var count = _vtm.Positions.Length;
        if (positionIndex < 0 || positionIndex >= count)
            return false;

        if (count >= VTModule.MaxPatternCount)
            return false;

        // Find a free pattern slot
        int srcPattern = _vtm.Positions.Value[positionIndex];
        int newPattern = -1;
        for (int i = 0; i <= VTModule.MaxPatternIndex; i++)
        {
            if (_vtm.Patterns[i] == null)
            {
                newPattern = i;
                break;
            }
        }

        if (newPattern < 0)
            return false;

        // Deep-clone the source pattern
        _vtm.Patterns[newPattern] = _vtm.Patterns[srcPattern] != null
            ? (Pattern)_vtm.Patterns[srcPattern].Clone()
            : new Pattern();

        // Insert new position after current
        for (int i = count; i > positionIndex + 1; i--)
        {
            _vtm.Positions.Value[i]  = _vtm.Positions.Value[i - 1];
            _vtm.Positions.Colors[i] = _vtm.Positions.Colors[i - 1];
        }

        _vtm.Positions.Value[positionIndex + 1]  = newPattern;
        _vtm.Positions.Colors[positionIndex + 1] = _vtm.Positions.Colors[positionIndex];
        _vtm.Positions.Length++;
        return true;
    }

    public bool TransposePattern(int patternIndex, int semitones)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return false;

        var pattern = _vtm.Patterns[patternIndex];
        if (pattern == null)
            return false;

        for (int line = 0; line < pattern.Length; line++)
        {
            for (int ch = 0; ch < 3; ch++)
            {
                var channel = pattern.Lines[line].Channel[ch];
                if (channel.Note < 0)
                    continue; // --- or R--

                int newNote = channel.Note + semitones;
                newNote = Math.Clamp(newNote, 0, 95);
                channel.Note = (sbyte)newNote;
            }
        }

        return true;
    }

    public bool ExpandPattern(int patternIndex)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return false;

        var pattern = _vtm.Patterns[patternIndex];
        if (pattern == null)
            return false;

        int oldLength = pattern.Length;
        int newLength = oldLength * 2;

        if (newLength > VTModule.MaxPatternLength)
            return false;

        // Expand in reverse: copy line[i] to line[i*2], clear line[i*2+1]
        for (int i = oldLength - 1; i >= 0; i--)
        {
            pattern.Lines[i * 2] = (Line)pattern.Lines[i].Clone();
            var blank = pattern.Lines[i * 2 + 1];
            blank.Envelope = 0;
            blank.Noise    = 0;
            blank.Channel[0] = new ChannelLine();
            blank.Channel[1] = new ChannelLine();
            blank.Channel[2] = new ChannelLine();
        }

        pattern.Length = newLength;
        return true;
    }

    public bool CompressPattern(int patternIndex)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return false;

        var pattern = _vtm.Patterns[patternIndex];
        if (pattern == null || pattern.Length < 2)
            return false;

        int newLength = pattern.Length / 2;

        for (int i = 0; i < newLength; i++)
            pattern.Lines[i] = (Line)pattern.Lines[i * 2].Clone();

        pattern.Length = newLength;
        return true;
    }

    public bool SwapChannels(int patternIndex, bool rightDirection)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return false;

        var pattern = _vtm.Patterns[patternIndex];
        if (pattern == null)
            return false;

        for (int line = 0; line < pattern.Length; line++)
        {
            var l = pattern.Lines[line];
            var ch0 = l.Channel[0];
            var ch1 = l.Channel[1];
            var ch2 = l.Channel[2];

            if (rightDirection)
            {
                // A→B, B→C, C→A
                l.Channel[0] = ch2;
                l.Channel[1] = ch0;
                l.Channel[2] = ch1;
            }
            else
            {
                // A→C, B→A, C→B
                l.Channel[0] = ch1;
                l.Channel[1] = ch2;
                l.Channel[2] = ch0;
            }
        }

        return true;
    }

    public void ToggleLooping()
    {
        _loopingEnabled = !_loopingEnabled;
    }

    // ── Sample access ────────────────────────────────────────────────────────

    public int SampleCount => _vtm?.Samples?.Length ?? 0;

    public SampleData? GetSample(int index)
    {
        if (_vtm == null || index < 0 || index >= _vtm.Samples.Length)
            return null;

        var sample = _vtm.Samples[index];
        if (sample == null)
            return new SampleData(index, 0, 0, false, Array.Empty<string>());

        var lines = new List<string>(sample.Length);
        for (int i = 0; i < sample.Length; i++)
        {
            var prefix = i == sample.Loop ? "L " : "  ";
            lines.Add(prefix + VTModule.GetSampleString(sample.Ticks[i], false, false));
        }

        return new SampleData(index, sample.Length, sample.Loop, sample.Enabled, lines);
    }

    public bool UpdateSampleTick(int sampleIndex, int tickIndex, SampleTickData tick)
    {
        if (_vtm == null || sampleIndex < 0 || sampleIndex >= _vtm.Samples.Length)
            return false;

        var sample = _vtm.Samples[sampleIndex];
        if (sample == null || tickIndex < 0 || tickIndex >= sample.Length)
            return false;

        var t = sample.Ticks[tickIndex];
        t.Mixer_Ton = tick.MixerTone;
        t.Mixer_Noise = tick.MixerNoise;
        t.Envelope_Enabled = tick.EnvelopeEnabled;
        t.AddToTone = tick.AddToTone;
        t.Ton_Accumulation = tick.ToneAccumulation;
        t.Add_to_Envelope_or_Noise = tick.AddToEnvelopeOrNoise;
        t.Envelope_or_Noise_Accumulation = tick.EnvelopeOrNoiseAccumulation;
        t.Amplitude = tick.Amplitude;
        t.Amplitude_Sliding = tick.AmplitudeSliding;
        t.Amplitude_Slide_Up = tick.AmplitudeSlideUp;
        return true;
    }

    public bool UpdateSampleLength(int sampleIndex, int length)
    {
        if (_vtm == null || sampleIndex < 0 || sampleIndex >= _vtm.Samples.Length)
            return false;

        var sample = _vtm.Samples[sampleIndex];
        if (sample == null)
            return false;

        length = Math.Clamp(length, 1, VTModule.MaxSampleLength);
        sample.Length = (byte)length;
        if (sample.Loop >= length)
            sample.Loop = (byte)(length - 1);
        return true;
    }

    public bool UpdateSampleLoop(int sampleIndex, int loop)
    {
        if (_vtm == null || sampleIndex < 0 || sampleIndex >= _vtm.Samples.Length)
            return false;

        var sample = _vtm.Samples[sampleIndex];
        if (sample == null || loop < 0 || loop >= sample.Length)
            return false;

        sample.Loop = (byte)loop;
        return true;
    }

    public bool ClearSample(int sampleIndex)
    {
        if (_vtm == null || sampleIndex < 0 || sampleIndex >= _vtm.Samples.Length)
            return false;

        _vtm.Samples[sampleIndex] = new Sample();
        return true;
    }

    // ── Ornament access ──────────────────────────────────────────────────────

    public int OrnamentCount => _vtm?.Ornaments?.Length ?? 0;

    public OrnamentData? GetOrnament(int index)
    {
        if (_vtm == null || index < 0 || index >= _vtm.Ornaments.Length)
            return null;

        var orn = _vtm.Ornaments[index];
        if (orn == null)
            return new OrnamentData(index, 0, 0, Array.Empty<sbyte>());

        var offsets = new sbyte[orn.Length];
        Array.Copy(orn.Offsets, offsets, orn.Length);
        return new OrnamentData(index, orn.Length, orn.Loop, offsets);
    }

    public bool UpdateOrnamentOffset(int ornamentIndex, int offsetIndex, sbyte value)
    {
        if (_vtm == null || ornamentIndex < 0 || ornamentIndex >= _vtm.Ornaments.Length)
            return false;

        var orn = _vtm.Ornaments[ornamentIndex];
        if (orn == null || offsetIndex < 0 || offsetIndex >= orn.Length)
            return false;

        orn.Offsets[offsetIndex] = value;
        return true;
    }

    public bool UpdateOrnamentLength(int ornamentIndex, int length)
    {
        if (_vtm == null || ornamentIndex < 0 || ornamentIndex >= _vtm.Ornaments.Length)
            return false;

        var orn = _vtm.Ornaments[ornamentIndex];
        if (orn == null)
            return false;

        length = Math.Clamp(length, 1, VTModule.MaxOrnamentLength);
        orn.Length = length;
        if (orn.Loop >= length)
            orn.Loop = length - 1;
        return true;
    }

    public bool UpdateOrnamentLoop(int ornamentIndex, int loop)
    {
        if (_vtm == null || ornamentIndex < 0 || ornamentIndex >= _vtm.Ornaments.Length)
            return false;

        var orn = _vtm.Ornaments[ornamentIndex];
        if (orn == null || loop < 0 || loop >= orn.Length)
            return false;

        orn.Loop = loop;
        return true;
    }

    public bool ClearOrnament(int ornamentIndex)
    {
        if (_vtm == null || ornamentIndex < 0 || ornamentIndex >= _vtm.Ornaments.Length)
            return false;

        _vtm.Ornaments[ornamentIndex] = new Ornament { Length = 1, Loop = 0 };
        return true;
    }

    public PatternLineData? GetPatternLineData(int patternIndex, int lineIndex, int channelIndex = 0)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return null;

        var pattern = _vtm.Patterns[patternIndex];
        if (pattern == null || lineIndex < 0 || lineIndex >= pattern.Length)
            return null;

        channelIndex = Math.Clamp(channelIndex, 0, 2);
        var channel = pattern.Lines[lineIndex].Channel[channelIndex];
        return new PatternLineData(VTModule.NoteToStr(channel.Note), channel.Sample, channel.Ornament, channel.Volume);
    }

    public bool UpdatePatternLineData(int patternIndex, int lineIndex, PatternLineData data, int channelIndex = 0)
    {
        if (_vtm == null || patternIndex < 0 || patternIndex >= _vtm.Patterns.Length)
            return false;

        var pattern = _vtm.Patterns[patternIndex];
        if (pattern == null || lineIndex < 0 || lineIndex >= pattern.Length)
            return false;

        if (!TryParseNote(data.Note, out var note))
            return false;

        channelIndex = Math.Clamp(channelIndex, 0, 2);
        var channel = pattern.Lines[lineIndex].Channel[channelIndex];
        channel.Note     = (sbyte)Math.Clamp(note, sbyte.MinValue, sbyte.MaxValue);
        channel.Sample   = (byte)Math.Clamp(data.Sample, 0, 31);
        channel.Ornament = (byte)Math.Clamp(data.Ornament, 0, 31);
        channel.Volume   = (sbyte)Math.Clamp(data.Volume, 0, 15);
        pattern.Lines[lineIndex].Channel[channelIndex] = channel;

        return true;
    }

    private static bool TryParseNote(string noteText, out int note)
    {
        note = -1;
        if (string.IsNullOrWhiteSpace(noteText))
            return false;

        noteText = noteText.Trim().ToUpperInvariant();

        if (noteText == "---") { note = -1; return true; }
        if (noteText == "R--") { note = -2; return true; }

        if (noteText.Length != 3)
            return false;

        var noteName = noteText[..2];
        if (!int.TryParse(noteText[2..3], out var octave))
            return false;

        var index = Array.IndexOf(VTModule.Notes, noteName);
        if (index < 0)
            return false;

        note = (octave - 1) * 12 + index;
        return note >= 0;
    }
}
