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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace LibVT
{
    public enum  ProgressType
    {
        Initialize,
        Update,
        Complete
    };

    public class ProgressBarEventArgs : EventArgs
    {
        public ProgressType Type;
        public string Text;
        public int Minimum;
        public int Maximum;
        public int Step;
        public int Value;

        public ProgressBarEventArgs(string text, int minimum, int maximum, int step, int value)
        {
            Type = ProgressType.Initialize;
            Text = text;
            Minimum = minimum;
            Maximum = maximum;
            Step = step;
            Value = value;
        }

        public ProgressBarEventArgs(int value)
        {
            Type = ProgressType.Update;
            Value = value;
        }

        public ProgressBarEventArgs()
        {
            Type = ProgressType.Complete;
        }
    }

    public class WaveAyumi
    {
        const int AudioBufferSize = 4096;

        public static int SampleRate = 44100;
        public static int BitRate = 16;
        public static int NumChannels = 2;
        public static bool IsYm = true;
        private static int PrevChanAlloc;
        //private static FileStream FileStream;
        //private static FileStream TMPFileStream;
        private static double IsrCounter;
        public static double IsrStep;
        //private static TExport ExportModal;
        private static int RealBufferSize = 0;
        public static int FromPosition;
        public static int ToPosition;
        private static int CurPosition;
        public static int PositionCount;
        private static bool PrevLoopAllowed;
        public static bool PlayAll;
        public static bool ExportSelected;
        public static bool ExportSeparate;
        //private static TChildWin LeadWindow;
        public static int[] PrevPosNum = new int[3];
        public static int[] PrevPatNum = new int[3];

        private static bool IsPlaying_Old;
        private static bool HasReset_Old;

        //private static string TMPFileName;
        //private static Ayumi[] Aymui = new Ayumi[3];
        private static string CurName, Names;

        private static int CurChan;
        public static bool Separate;
        private static bool[] StoredMix = new bool[VTModule.MaxSoundChipCount * 3 * 3];
        //private static string Fn1, Fn2;

        private static byte[] AudioBuffer;
        private static int BytesPerSample;
        private static int FrameBytes;
        private static int TotalBytes;

        public static string CreateWaveAyumi(string fileName)
        {
            int i;
            Ayumi[] ayumi = new Ayumi[3];
            int exportLoops = WaveOutAPI.ExportLoops;

            AppEvents.SendEvent(EventType.SetControlsForExport, false);

            PlayAll = ExportSelected;

            if (ExportSeparate)
            {
                Separate = true;
                NumChannels = 1;
            }
            else
                Separate = false;

            CurChan = 0;
            Names = "";

MainLoop:

            PrevPatNum[1] = 0;
            PrevPosNum[1] = 0;
            PrevPatNum[2] = 0;
            PrevPosNum[2] = 0;

            IsrCounter = 1;
            CurPosition = 0;

            PrevChanAlloc = Main.ChanAllocIndex;
            PrevLoopAllowed = AY.LoopAllowed;

            // If mono, then set ABC
            if (Main.ChanAllocIndex == 0)
                AppEvents.SendEvent(EventType.SetChannelsAllocation, 1);

            if (WaveOutAPI.IsPlaying)
                WaveOutAPI.StopPlaying();

            AY.PlayMode = PlayModes.PlayModule;

            // Set chip & audio params for export
            for (i = 0; i < AY.ChipCount; i++)
            {
                if (AY.PlayingModule[i] == null)
                    continue;

                ayumi[i] = new Ayumi();
                ayumi[i].Configure(IsYm, AY.AYFreq, SampleRate, AY.DCType);
                ayumi[i].SetPan(0, Main.Panoram[0] / 255.0, false);
                ayumi[i].SetPan(1, Main.Panoram[1] / 255.0, false);
                ayumi[i].SetPan(2, Main.Panoram[2] / 255.0, false);
            }

            // Set from and to position
            AppEvents.SendEvent(EventType.SetFromAndToPosition);

            // Init pointer, position, delay
            WaveOutAPI.InitForAllTypes(true);

            for (i = 0; i < AY.ChipCount; i++)
            {
                VTModule.Module_SetPointer(AY.PlayingModule[i], i);
                VTModule.Module_SetDelay((sbyte)AY.PlayingModule[i].InitialDelay);
            }

            IsPlaying_Old = WaveOutAPI.IsPlaying;
            HasReset_Old = WaveOutAPI.HasReset;
            WaveOutAPI.IsPlaying = true;
            WaveOutAPI.HasReset = true;
            AppEvents.SendEvent(EventType.RestartPlayingPosition, FromPosition);
            WaveOutAPI.IsPlaying = IsPlaying_Old;
            WaveOutAPI.HasReset = HasReset_Old;

            AppEvents.SendEvent(EventType.ProgressBar, new ProgressBarEventArgs("Export to wav...", 0, PlayAll ? PositionCount + ((PositionCount - AY.PlayingModule[0].Positions.Loop) * WaveOutAPI.ExportLoops) : PositionCount * (WaveOutAPI.ExportLoops + 1), 1, 0));

            // Prepare memory stream and buffer
            string tempFileName = fileName + ".tmp";

            using (var tempFileStream = new FileStream(tempFileName, FileMode.Create))
            {
                int progress = 0;

                WaveOutAPI.ExportStarted = true;
                WaveOutAPI.ExportFinished = false;

                do
                {
                    if (Main.VTExit) // || Win32.GetAsyncKeyState(Keys.Escape) < 0)
                    {
                        AppEvents.SendEvent(EventType.ProgressBar, new ProgressBarEventArgs());

                        for (i = 0; i < ayumi.Length; i++)
                            ayumi[i] = null;

                        File.Delete(tempFileName);

                        WaveOutAPI.ExportStarted = false;

                        if (Main.VTExit)
                            return null;

                        AY.LoopAllowed = PrevLoopAllowed;

                        AppEvents.SendEvent(EventType.SetChildPositions);
                        AppEvents.SendEvent(EventType.SetChannelsAllocation, PrevChanAlloc);
                        AppEvents.SendEvent(EventType.SetControlsForExport, true);

                        return null;
                    }

                    BytesPerSample = BitRate / 8;
                    FrameBytes = BytesPerSample * NumChannels;
                    TotalBytes = AudioBufferSize * FrameBytes;

                    AudioBuffer = new byte[TotalBytes];

                    // Make buffer and save to memory stream
                    FillBuffer(ayumi);

                    tempFileStream.Write(AudioBuffer, 0, FrameBytes * RealBufferSize);

                    if (CurPosition != VTModule.PlayArgs[0].PositionIndex)
                    {
                        AppEvents.SendEvent(EventType.ProgressBar, new ProgressBarEventArgs(progress++));
                        CurPosition = VTModule.PlayArgs[0].PositionIndex;
                    }

                    if ((progress % 2) == 0)
                        AppEvents.SendEvent(EventType.DoEvents);
                }
                while (!WaveOutAPI.ExportFinished);

                AppEvents.SendEvent(EventType.ProgressBar, new ProgressBarEventArgs());

                for (i = 0; i < ayumi.Length; i++)
                    ayumi[i] = null;

                // Save wav
                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    WaveOutAPI.WriteWaveHeader(fileStream, (int)tempFileStream.Length, SampleRate, (short)BitRate, (short)NumChannels);

                    tempFileStream.Position = 0;
                    tempFileStream.CopyTo(fileStream);
                }
            }

            File.Delete(tempFileName);

            if (Separate)
            {
                SwapMixer();

                CurChan++;

                if (ayumi[2] != null && CurChan < 9)
                    goto MainLoop;

                if (ayumi[1] != null && CurChan < 6)
                    goto MainLoop;

                if (CurChan < 3)
                    goto MainLoop;
            }

            AppEvents.SendEvent(EventType.SetChildPositions);
            AppEvents.SendEvent(EventType.SetChannelsAllocation, PrevChanAlloc);
            AppEvents.SendEvent(EventType.SetControlsForExport, true);

            AY.LoopAllowed = PrevLoopAllowed;

            WaveOutAPI.ExportStarted = true;
            WaveOutAPI.ExportStarted = false;

            AY.ClearRegisters();

            return null;
        }

        public static void SetAyumiParams(Ayumi ayumi, int chipIndex)
        {
            if (ayumi == null)
                return;

            AYRegisters regs = AY.SoundChip[chipIndex].AYRegisters;

            ayumi.SetTone(0, regs.ToneA);
            ayumi.SetTone(1, regs.ToneB);
            ayumi.SetTone(2, regs.ToneC);

            ayumi.SetNoise(regs.Noise);

            ayumi.SetMixer(0, regs.Mixer & 1, (regs.Mixer >> 3) & 1, regs.AmplitudeA >> 4);
            ayumi.SetMixer(1, (regs.Mixer >> 1) & 1, (regs.Mixer >> 4) & 1, (regs.AmplitudeB >> 4) & 1);
            ayumi.SetMixer(2, (regs.Mixer >> 2) & 1, (regs.Mixer >> 5) & 1, (regs.AmplitudeC >> 4) & 1);

            ayumi.SetVolume(0, regs.AmplitudeA & 0x0F);
            ayumi.SetVolume(1, regs.AmplitudeB & 0x0F);
            ayumi.SetVolume(2, regs.AmplitudeC & 0x0F);

            ayumi.SetEnvelope(regs.Envelope);

            if (regs.EnvType != 0xFF)
                ayumi.SetEnvelopeShape(regs.EnvType);

            regs.EnvType = 0xFF;
        }

        public static void FillBuffer(Ayumi[] ayumi)
        {
            int j;
            double left, right;
            const double maxPeak = 1.6;
            const double minPeak = -1.6;

            RealBufferSize = 0;

            for (int i = 0; i < AudioBufferSize; i++)
            {
                IsrCounter += IsrStep;

                if (IsrCounter >= 1)
                {
                    IsrCounter--;

                    for (int c = 0; c < AY.ChipCount; c++)
                    {
                        if (ayumi[c] == null || AY.PlayingModule[c] == null)
                            continue;

                        VTModule.Module_SetPointer(AY.PlayingModule[c], c);

                        if (VTModule.Pattern_PlayCurrentLine() == PlayLineResult.PatternEnded)
                        {
                            AppEvents.SendEvent(EventType.ChangePositions, c);

                            if (!WaveOutAPI.ExportFinished)
                                VTModule.Pattern_PlayCurrentLine();
                        }

                        SetAyumiParams(ayumi[c], c);
                    }
                }

                ayumi[0].Process();
                ayumi[0].RemoveDC();

                if (ayumi[1] != null && AY.PlayingModule[1] != null)
                {
                    ayumi[1].Process();
                    ayumi[1].RemoveDC();
                }

                if (ayumi[2] != null && AY.PlayingModule[2] != null)
                {
                    ayumi[2].Process();
                    ayumi[2].RemoveDC();
                }

                left = ayumi[0].Left;
                right = ayumi[0].Right;

                if (ayumi[1] != null && AY.PlayingModule[1] != null)
                {
                    left += ayumi[1].Left;
                    right += ayumi[1].Right;
                }

                if (ayumi[2] != null && AY.PlayingModule[2] != null)
                {
                    left += ayumi[2].Left;
                    right += ayumi[2].Right;
                    left /= 1.2;
                    right /= 1.2;
                }

                if (ayumi[1] != null)
                {
                    left /= 1.2;
                    right /= 1.2;
                }

                left = Math.Clamp(left, minPeak, maxPeak);
                right = Math.Clamp(right, minPeak, maxPeak);

                int frameOffset = i * FrameBytes;

                switch (BitRate)
                {
                    case 8: // 8-bit is unsigned PCM
                        if (NumChannels == 1)
                        {
                            AudioBuffer[frameOffset] = (byte)Math.Round(((left + right) / 2) * 127 + 128);
                        }
                        else // stereo
                        {
                            AudioBuffer[frameOffset + 0] = (byte)Math.Round(left * 127 + 128);
                            AudioBuffer[frameOffset + 1] = (byte)Math.Round(right * 127 + 128);
                        }
                        break;

                    case 16: // signed little-endian
                        if (NumChannels == 1)
                        {
                            short m = (short)Math.Round(((left + right) / 2) * 0x4FFF);
                            BinaryPrimitives.WriteInt16LittleEndian(AudioBuffer.AsSpan(frameOffset), m);
                        }
                        else
                        {
                            short l = (short)Math.Round(left * 0x4FFF);
                            short r = (short)Math.Round(right * 0x4FFF);
                            var span = AudioBuffer.AsSpan(frameOffset);
                            BinaryPrimitives.WriteInt16LittleEndian(span.Slice(0), l);
                            BinaryPrimitives.WriteInt16LittleEndian(span.Slice(2), r);
                        }
                        break;

                    case 24: // 24-bit signed PCM – manual byte writes
                        if (NumChannels == 1)
                        {
                            int m = (int)Math.Round(((left + right) / 2) * 0x4FFFFF);
                            AudioBuffer[frameOffset + 0] = (byte)(m & 0xFF);
                            AudioBuffer[frameOffset + 1] = (byte)((m >> 8) & 0xFF);
                            AudioBuffer[frameOffset + 2] = (byte)((m >> 16) & 0xFF);
                        }
                        else
                        {
                            int l = (int)Math.Round(left * 0x4FFFFF);
                            int r = (int)Math.Round(right * 0x4FFFFF);

                            int o = frameOffset;
                            AudioBuffer[o + 0] = (byte)(l & 0xFF);
                            AudioBuffer[o + 1] = (byte)((l >> 8) & 0xFF);
                            AudioBuffer[o + 2] = (byte)((l >> 16) & 0xFF);

                            AudioBuffer[o + 3] = (byte)(r & 0xFF);
                            AudioBuffer[o + 4] = (byte)((r >> 8) & 0xFF);
                            AudioBuffer[o + 5] = (byte)((r >> 16) & 0xFF);
                        }
                        break;

                    case 32: // signed 32-bit little-endian
                        if (NumChannels == 1)
                        {
                            int m = (int)Math.Round(((left + right) / 2) * 0x4FFFFFFF);
                            BinaryPrimitives.WriteInt32LittleEndian(AudioBuffer.AsSpan(frameOffset), m);
                        }
                        else
                        {
                            int l = (int)Math.Round(left * 0x4FFFFFFF);
                            int r = (int)Math.Round(right * 0x4FFFFFFF);

                            var span = AudioBuffer.AsSpan(frameOffset);
                            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(0), l);
                            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(4), r);
                        }
                        break;
                }

                RealBufferSize = i;

                if (WaveOutAPI.ExportFinished)
                    break;
            }
        }

        public static void SwapMixer()
        {
            for (int chip = 0; chip < VTModule.MaxSoundChipCount; chip++)
            {
                if (AY.PlayingModule[chip] == null)
                    continue;  // skip if no chip present

                for (int channel = 0; channel < 3; channel++)
                {
                    int k = 3 * (chip * VTModule.MaxSoundChipCount + channel);

                    bool tm = StoredMix[k];
                    StoredMix[k] = AY.PlayingModule[chip].ChannelStates[channel].GlobalTone;
                    AY.PlayingModule[chip].ChannelStates[channel].GlobalTone = tm;

                    tm = StoredMix[k + 1];
                    StoredMix[k + 1] = AY.PlayingModule[chip].ChannelStates[channel].GlobalNoise;
                    AY.PlayingModule[chip].ChannelStates[channel].GlobalNoise = tm;

                    tm = StoredMix[k + 2];
                    StoredMix[k + 2] = AY.PlayingModule[chip].ChannelStates[channel].GlobalEnvelope;
                    AY.PlayingModule[chip].ChannelStates[channel].GlobalEnvelope = tm;
                }
            }
        }

        public static void SoloMixer(int ch)
        {
            for (int i = 0; i < 3 * VTModule.MaxSoundChipCount; i++)
            {
                bool solo = (i == ch);
                StoredMix[i * 3] = solo;       // tone
                StoredMix[i * 3 + 1] = solo;   // noise
                StoredMix[i * 3 + 2] = solo;   // envelope
            }
        }
    }
}
