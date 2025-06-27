using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVT.Plugins
{
    public enum MidiMessageType : byte
    {
        Unknown = 0,
        NoteOff = 128,
        NoteOn = 144,
        PolyphonicKeyPressure = 160,
        ControlChange = 176,
        ProgramChange = 192,
        ChannelPressure = 208,
        PitchBendChange = 224,
        SystemExclusive = 240,
        TimeCodeQuarterFrame = 241,
        SongPositionPointer = 242,
        SongSelect = 243,
        UndefinedReserved1 = 244,
        UndefinedReserved2 = 245,
        TuneRequest = 246,
        EndOfSysEx = 247,
        TimingClock = 248,
        UndefinedReserved3 = 249,
        Start = 250,
        Continue = 251,
        Stop = 252,
        UndefinedReserved4 = 253,
        ActiveSensing = 254,
        SystemReset = byte.MaxValue
    }

    public readonly struct MidiMessage
    {
        public MidiMessage(MidiMessageType type, int channel,
                           int data1, int data2, TimeSpan timestamp, int device)
        {
            Type = type;
            Channel = (byte)channel;
            Data1 = (byte)data1;
            Data2 = (byte)data2;
            Timestamp = timestamp;
            DeviceIndex = device;
        }

        public MidiMessageType Type { get; }
        public byte Channel { get; }
        public byte Data1 { get; }
        public byte Data2 { get; }
        public TimeSpan Timestamp { get; }
        public int DeviceIndex { get; }

        public override string ToString()
            => $"{Timestamp} Dev={DeviceIndex} {Type:X2} " +
               $"Ch={Channel} {Data1:X2} {Data2:X2}";
    }

    public class MidiMessageEventArgs : EventArgs
    {
        public MidiMessage Message { get; }
        public bool Handled { get; set; }

        public MidiMessageEventArgs(MidiMessage message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message.ToString();
        }
    }
}
