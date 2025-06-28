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

using LibVT;
using RtMidi.Net;
using RtMidi.Net.Clients;
using RtMidi.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker
{
    public class MidiOut
    {
        private static MidiDeviceInfo _deviceInfo;
        private static MidiOutputClient _outputClient;

        public static bool Enabled = false;

        private static readonly byte[] _lastCCValues = new byte[3 * VTModule.MaxSoundChipCount * 128]; // 3 channels, 128 controllers
        private static int[] _lastNotes = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }; // 16 channels

        static MidiOut()
        {
        }

        public static void CreateClient(IWin32Window? owner, uint devicePort = 1)
        {
            var devices = MidiManager.GetAvailableDevices();

            if (devices.Count > 0)
            {
                try
                {
                    _deviceInfo = MidiManager.GetDeviceInfo(devicePort, MidiDeviceType.Output);

                    // Create and open the MIDI output client
                    _outputClient = new MidiOutputClient(_deviceInfo);
                    _outputClient.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(owner, $"MIDI Output Error: {ex.Message}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static void PlaybackEvent(object? sender, PlaybackEventArgs e)
        {
            if (!Enabled)
                return;

            int chipIndex = e.ChipIndex;
            SoundChip soundChip = e.SoundChip;
            AYRegisters registers = soundChip.AYRegisters;
            PlayArgs playArgs = e.PlayArgs;

            // MIDI Output for AY chip

            // Register write
            SendMidiRegister(chipIndex, 0, 0x00, registers.ToneA);      // A freq L
            SendMidiRegister(chipIndex, 0, 0x01, registers.ToneA >> 8); // A freq H
            SendMidiRegister(chipIndex, 1, 0x02, registers.ToneB);      // B freq L
            SendMidiRegister(chipIndex, 1, 0x03, registers.ToneB >> 8); // B freq H
            SendMidiRegister(chipIndex, 2, 0x04, registers.ToneC);      // C freq L
            SendMidiRegister(chipIndex, 2, 0x05, registers.ToneC >> 8); // C freq H

            SendMidiRegister(chipIndex, -1, 0x06, registers.Noise);
            SendMidiRegister(chipIndex, -1, 0x07, registers.Mixer);

            SendMidiRegister(chipIndex, 0, 0x08, registers.AmplitudeA);
            SendMidiRegister(chipIndex, 1, 0x09, registers.AmplitudeB);
            SendMidiRegister(chipIndex, 2, 0x0A, registers.AmplitudeC);

            SendMidiRegister(chipIndex, -1, 0x0B, registers.Envelope & 0xFF);        // Env freq L
            SendMidiRegister(chipIndex, -1, 0x0C, (registers.Envelope >> 8) & 0xFF); // Env freq H
            SendMidiRegister(chipIndex, -1, 0x0D, registers.EnvType);

            for (int ch = 0; ch < 3; ch++)
            {
                var channelParams = playArgs.ChannelParams[ch];
                int midiChannel = e.ChipIndex * 3 + ch;

                // Base note inferred from tone register
                //int midiNote = chan.Note + 36; // or use GetNoteByEnvelope() for freq-based pitch
                //int midiNote = GetNoteByEnvelope(chan.Ton >> 4); // AY values are 12-bit
                //int midiNote = (int)Math.Round(GetMidiNoteFromAYFreq(chan.Ton));
                int midiNote = channelParams.Note + 24;
                int velocity = 127; // (chan.Amplitude & 0x0F) * 8;

                bool shouldPlay = channelParams.SoundEnabled && velocity > 0;

                if (shouldPlay)
                {
                    if (!playArgs.NoteOnState[ch] || playArgs.LastNoteSent[ch] != midiNote)
                    {
                        if (playArgs.NoteOnState[ch])
                            SendMidiNoteOff(chipIndex, ch, playArgs.LastNoteSent[ch], 0);

                        SendMidiNoteOn(chipIndex, ch, midiNote, velocity);
                        playArgs.LastNoteSent[ch] = midiNote;
                        playArgs.NoteOnState[ch] = true;
                    }

                    // --- Pitch Bend Calculation ---
                    double actualFreq = 2000000.0 / (16.0 * channelParams.Tone); // AY tone frequency
                    double baseFreq = MidiNoteToFreq(midiNote);

                    int pitchBend = CalculatePitchBend(baseFreq, actualFreq); // -8192 to +8191

                    SendMidiPitchBend(chipIndex, ch, pitchBend);
                }
                else
                {
                    if (playArgs.NoteOnState[ch])
                    {
                        SendMidiNoteOff(chipIndex, ch, playArgs.LastNoteSent[ch], 0);
                        playArgs.NoteOnState[ch] = false;
                    }
                }
            }
        }

        public static void SendMidiRegister(int chipIndex, int channel, int register, int value)
        {
            if (channel >= 0 && channel < 9)
            {
                int midiChannel = chipIndex * 3 + channel;

                if (register >= 0x08 && register <= 0x0A)
                {
                    // Amplitude -> CC7 (volume)
                    SendMidiCC(midiChannel, 7, (byte)value);
                }
                else if (register >= 0x00 && register <= 0x05)
                {
                    // Tone -> CC74 (arbitrary)
                    SendMidiCC(midiChannel, 74, (byte)(value & 0x7F));
                }
                else if (register == 0x0D)
                {
                    // Env type -> CC4
                    SendMidiCC(midiChannel, 4, (byte)value);
                }
            }
        }

        public static void SendMidiNoteOn(int chipIndex, int channel, int note, int velocity)
        {
            if (channel >= 0 && channel < 9)
            {
                int midiChannel = chipIndex * 3 + channel;
                SendMidiNoteOn(midiChannel, note, (byte)velocity);
            }
        }

        public static void SendMidiNoteOff(int chipIndex, int channel, int note, int velocity)
        {
            if (channel >= 0 && channel < 9)
            {
                int midiChannel = chipIndex * 3 + channel;
                SendMidiNoteOff(midiChannel, note);
            }
        }

        public static void SendMidiPitchBend(int chipIndex, int channel, int value)
        {
            if (channel >= 0 && channel < 9)
            {
                int midiChannel = chipIndex * 3 + channel;
                SendMidiPitchBend(midiChannel, value);
            }
        }

        public static void InitializeMidiChannel(int channel)
        {
            if (!Enabled)
                return;

            //SendMidiCC(channel, 120, 0);

            SendMidiCC(channel, 3, 0);     // CC3  - Synth Type: 0 = Square
            SendMidiCC(channel, 4, 0);     // CC4  - Volume Env Shape: 64 = ramp down
            SendMidiCC(channel, 7, 127);   // CC7  - Volume: full

            SendMidiCC(channel, 6, 0);     // CC6  - Vibrato Rate: off
            SendMidiCC(channel, 5, 0);     // CC5  - Glide: off
            SendMidiCC(channel, 10, 0);    // CC10 - Pitch Env Shape: off
            SendMidiCC(channel, 9, 0);     // CC9  - Pitch Env Amount: off
            SendMidiCC(channel, 8, 0);     // CC8  - Noise Delay: off
            SendMidiCC(channel, 11, 64);   // CC11 - Transpose: neutral
        }

        private static float GetMidiNoteFromAYFreq(ushort ayFreq)
        {
            if (ayFreq == 0)
                return 0; // Prevent division by zero

            // AY frequency (hz) = 2MHz / (16 * ayFreq)
            double hz = 2000000.0 / (16.0 * ayFreq);
            double midi = 69 + 12 * Math.Log2(hz / 440.0);
            return (float)midi;
        }

        private static int CalculatePitchBend(double baseFreq, double actualFreq)
        {
            if (baseFreq <= 0 || actualFreq <= 0)
                return 0;

            double semitoneRatio = Math.Pow(2.0, 1.0 / 12.0);
            double ratio = actualFreq / baseFreq;
            double bendInSemitones = Math.Log(ratio) / Math.Log(semitoneRatio);

            int bend = (int)(bendInSemitones * 8192 / 2.0);
            return Math.Clamp(bend, -8192, 8191);
        }

        private static double MidiNoteToFreq(int midiNote)
        {
            return 440.0 * Math.Pow(2, (midiNote - 69) / 12.0);
        }

        public static int CalculatePitchBend(double baseFreq, double actualFreq, int semitoneRange = 2)
        {
            if (baseFreq == 0 || actualFreq == 0)
                return 8192;

            double ratio = actualFreq / baseFreq;
            double bendSemitones = 12.0 * Math.Log(ratio, 2.0);
            int bend = (int)(8192 + (bendSemitones / semitoneRange) * 8192);
            return Math.Clamp(bend, 0, 16383);
        }

        public static void SendMidiNoteOn(int channel, int note, byte velocity)
        {
            if (_lastNotes[channel] == note)
                return;

            // Turn off previous note
            if (_lastNotes[channel] >= 0)
            {
                var offNote = new MidiNote((byte)_lastNotes[channel]);
                _outputClient.SendMessage(new MidiMessageNote((MidiChannel)channel, offNote, 0, RtMidi.Net.Enums.MidiMessageType.NoteOff));
            }

            _lastNotes[channel] = note;

            var onNote = new MidiNote((byte)note);
            _outputClient.SendMessage(new MidiMessageNote((MidiChannel)channel, onNote, velocity, RtMidi.Net.Enums.MidiMessageType.NoteOn));
        }

        public static void SendMidiNoteOff(int channel, int note)
        {
            if (_lastNotes[channel] == note)
            {
                var offNote = new MidiNote((byte)note);
                _outputClient.SendMessage(new MidiMessageNote((MidiChannel)channel, offNote, 0, RtMidi.Net.Enums.MidiMessageType.NoteOff));
                _lastNotes[channel] = -1;
            }
        }

        public static void SendMidiPitchBend(int channel, int bendValue)
        {
            byte lsb = (byte)(bendValue & 0x7F);
            byte msb = (byte)((bendValue >> 7) & 0x7F);
            _outputClient.SendMessage(new MidiMessagePitchBendChange((MidiChannel)channel, lsb, msb));
        }

        public static void SendMidiCC(int channel, int controller, byte value)
        {
            int index = (channel * 128) + controller;

            if (_lastCCValues[index] == value)
                return;

            _lastCCValues[index] = value;
            _outputClient.SendMessage(new MidiMessageControlChange((MidiChannel)channel, (byte)controller, value));
        }

        public static void UpdateMidiChannel(int channel, ChannelParams channelParams, AYRegisters regs)
        {
            if (!Enabled || channel < 0 || channel > 2)
                return;

            LibVT.ChannelState channelState = VTModule.VTM.ChannelStates[channel];
            Sample sample = VTModule.VTM.Samples[channelState.Sample];

            // These are placeholder mappings — replace or refine with actual values as needed
            byte pwmFreq = 64; // Default mid-point (CC1)
            byte softDetune = (byte)Math.Clamp(channelParams.CurrentNoiseSliding + 64, 0, 127); // CC2
            byte synthType = DetectSynthType(channelParams, channelState, sample, regs); // You may map this based on Sample/Ornament state (CC3)
            byte volEnvShape = 0; // CC4
            byte glide = (byte)Math.Clamp(Math.Abs(channelParams.ToneSlideDelta >> 6), 0, 127); // CC5
            byte vibratoRate = 64; // Placeholder (CC6)
            byte vibratoDepth = (byte)Math.Clamp(Math.Abs(channelParams.CurrentToneSliding) / 10, 0, 127); // CC7
            byte noiseDelay = 0; // CC8 (no direct mapping unless you implement it)
            byte pitchEnvAmt = (byte)Math.Clamp(channelParams.CurrentEnvelopeSliding * 10, 0, 127); // CC9
            byte pitchEnvShape = 64; // CC10
            byte transpose = 64; // CC11 (centered)

            // The rest remains the same...
            SendMidiCC(channel, 3, synthType);
            // Send mapped CCs
            SendMidiCC(channel, 1, pwmFreq);
            SendMidiCC(channel, 2, softDetune);
            SendMidiCC(channel, 3, synthType);
            SendMidiCC(channel, 4, volEnvShape);
            SendMidiCC(channel, 5, glide);
            SendMidiCC(channel, 6, vibratoRate);
            SendMidiCC(channel, 7, vibratoDepth);
            SendMidiCC(channel, 8, noiseDelay);
            SendMidiCC(channel, 9, pitchEnvAmt);
            SendMidiCC(channel, 10, pitchEnvShape);
            SendMidiCC(channel, 11, transpose);
        }

        public static byte DetectSynthType(ChannelParams channelParams, LibVT.ChannelState channelState, Sample sample, AYRegisters regs)
        {
            bool hasTone = true;
            bool hasEnv = channelState.EnvelopeEnabled;
            bool hasSoftwave = false;
            bool hasNoise = false;
            bool pitchModOnly = false;

            if (sample != null && sample.Ticks != null)
            {
                for (int i = 0; i < sample.Ticks.Length; i++)
                {
                    var tick = sample.Ticks[i];
                    if (!tick.Mixer_Ton) hasTone = false;
                    if (tick.Envelope_Enabled) hasEnv = true;
                    if (!tick.Mixer_Noise) hasNoise = true;

                    // Rough detection of "softwave" PWM-style modulation
                    if (tick.Amplitude_Sliding)
                        hasSoftwave = true;

                    // Optional: detect square-only acid mode by tone slide or vibrato
                    if (channelParams.ToneSlideType != 0 || channelParams.CurrentToneSliding != 0)
                        pitchModOnly = true;
                }
            }

            // Map to synthType based on combinations
            if (hasNoise && !hasTone && !hasEnv)
                return 7; // Noise only
            if (hasTone && !hasEnv && !hasSoftwave)
                return 0; // Square voice only
            if (hasTone && hasEnv && !hasSoftwave)
                return (byte)(pitchModOnly ? 2 : 1); // Square + Envelope
            if (!hasTone && hasEnv && !hasSoftwave)
                return (byte)(regs.EnvType == 0x08 || regs.EnvType == 0x0D ? 4 : 3); // Envelope only
            if (hasTone && hasSoftwave)
                return (byte)(pitchModOnly ? 6 : 5); // Square + PWM

            // Default fallback
            return 0;
        }

        public static void DestroyClient()
        {
            _outputClient?.Close();
            _outputClient = null;
            _deviceInfo = null;
        }
    }
}
