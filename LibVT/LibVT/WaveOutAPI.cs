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

using OpenTK.Audio.OpenAL;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ThreadState = System.Threading.ThreadState;

namespace LibVT
{
    public class WaveBuffer
    {
        public int BufferID;
        public byte[] PcmData;
    }

    public class PlayInfo
    {
        public int Position;
        public int Pattern;
        public int Line;

        public override string ToString()
        {
            return $"Position: {Position} Pattern: {Pattern} Line: {Line}";
        }
    }

    public class PlayGrid
    {
        public PlayInfo[] Module;

        public PlayGrid()
        {
            Module = new PlayInfo[3];

            for (int i = 0; i < 3; i++)
                Module[i] = new PlayInfo();
        }
    }

    public class EMultiMediaError : Exception
    {
        public EMultiMediaError(string message) :
            base(message)
        {
        }
    }

    public class WaveOutAPI
    {
        public const int WAVE_MAPPER = -1;

        public static int BufferCount = 3;
        public static int BufferLength = 0;
        public static int BufferPosition = 0;
        public static int BufferLengthMs = 100;
        public static uint TickCount = 0;
        public static bool IsPlaying = false;
        public static bool HasReset = false;
        public static int InterruptFreq = 48828;
        public static int NumberOfChannels = 2;
        public static int SampleRate = 44100;
        public static int SampleBit = 16;
        public static PlayGrid[] PlayGrid;
        public static uint PlayGridLength = 0;
        public static uint PlayGridIndex = 0;
        public static Mutex ResetMutex = null;
        public static Dictionary<int, WaveBuffer> BufferMap = new Dictionary<int, WaveBuffer>();
        public static bool LineReady = false;
        public static bool ExportStarted = false;
        public static bool ExportFinished = false;
        public static int ExportLoops = 0;
        public static Thread WOThread = null;
        //public static Thread TSThread = null;
        public static bool AudioProblem = false;
        public static Action? PlaybackEnded = null;
        public const int BufferCountDefault = 3;
        public const int BufferLengthMsDefault = 100;
        //public static AutoResetEvent TSEvent = null;
        //public static ManualResetEventSlim WOEvent = null;

        private static ALDevice _alDevice;
        private static ALContext _alContext;
        private static int _alSource;
        private static int[] _alBuffers = new int[BufferCount];
        private static int _totalSamplesPlayed = 0;

        private enum AudioCommandType
        {
            Start,
            Stop,
            Reset,
            Unreset,
            PlayModule,
            PlayPattern,
            PlayPatternLine,
            PlayLine
        }

        private sealed class AudioCommand
        {
            public AudioCommandType Type { get; init; }
            public VTM? Vtm { get; init; }
            public VTM? Vtm2 { get; init; }
            public VTM? Vtm3 { get; init; }
            public int ChipCount { get; init; }
            public bool LoopEnabled { get; init; }
            public int PatternIndex { get; init; }
            public int LineIndex { get; init; }
        }

        private static BlockingCollection<AudioCommand> _commandQueue;
        private static Thread _commandThread;
        private static int _commandThreadId;
        private static readonly object _commandThreadLock = new();

        private static Stopwatch _playbackTimer = new Stopwatch();

        public static string WODevice = null;

        private static void EnsureCommandThread()
        {
            if (_commandThread != null)
                return;

            lock (_commandThreadLock)
            {
                if (_commandThread != null)
                    return;

                _commandQueue = new BlockingCollection<AudioCommand>();
                _commandThread = new Thread(ProcessAudioCommands)
                {
                    IsBackground = true,
                    Name = "AudioCommandThread"
                };
                _commandThread.Start();
            }
        }

        private static void ProcessAudioCommands()
        {
            _commandThreadId = Environment.CurrentManagedThreadId;

            foreach (var command in _commandQueue.GetConsumingEnumerable())
                ExecuteCommand(command);
        }

        private static void EnqueueCommand(AudioCommandType commandType)
            => EnqueueCommand(new AudioCommand { Type = commandType });

        private static void EnqueueCommand(AudioCommand command)
        {
            EnsureCommandThread();

            if (Environment.CurrentManagedThreadId == _commandThreadId)
            {
                ExecuteCommand(command);
                return;
            }

            _commandQueue.Add(command);
        }

        private static void ExecuteCommand(AudioCommand command)
        {
            switch (command.Type)
            {
                case AudioCommandType.Start:
                    StartWOThreadInternal();
                    break;
                case AudioCommandType.Stop:
                    StopPlayingInternal();
                    break;
                case AudioCommandType.Reset:
                    ResetPlayingInternal();
                    break;
                case AudioCommandType.Unreset:
                    UnResetPlayingInternal();
                    break;
                case AudioCommandType.PlayModule:
                    ApplyPlayContext(command);
                    InitForAllTypes(true);
                    StartWOThreadInternal();
                    break;
                case AudioCommandType.PlayPattern:
                    ApplyPlayContext(command);
                    InitForAllTypes(false);
                    StartWOThreadInternal();
                    break;
                case AudioCommandType.PlayPatternLine:
                    ApplyPlayContext(command);
                    InitForAllTypes(false);
                    StartWOThreadInternal();
                    break;
                case AudioCommandType.PlayLine:
                    ApplyPlayContext(command);
                    InitForAllTypes(false);
                    VTModule.Pattern_PlayCurrentLine();
                    LineReady = true;
                    StartWOThreadInternal();
                    break;
            }
        }

        private static void ApplyPlayContext(AudioCommand command)
        {
            AY.SetDefault(AY.SampleRateDefault, AY.NumberOfChannelsDefault, AY.SampleBitDefault);
            AY.SetSampleRate(SampleRate);
            AY.SetBitRate(SampleBit);
            AY.SetNChans(NumberOfChannels);
            AY.SetBuffers(BufferLengthMs, BufferCount);

            AY.ChipCount = command.ChipCount;
            AY.LoopAllowed = command.LoopEnabled;
            AY.ActiveModule = command.Vtm!;
            AY.PlayingModule[0] = command.Vtm!;
            AY.PlayingModule[1] = command.Vtm2;
            AY.PlayingModule[2] = command.Vtm3;

            switch (command.Type)
            {
                case AudioCommandType.PlayModule:
                    AY.PlayMode = PlayModes.PlayModule;
                    for (int i = 0; i < AY.ChipCount; i++)
                    {
                        if (AY.PlayingModule[i] == null) continue;
                        VTModule.Module_SetPointer(AY.PlayingModule[i], i);
                        VTModule.Module_SetDelay((sbyte)AY.PlayingModule[i].InitialDelay);
                        VTModule.Module_SetCurrentPosition(0);
                    }
                    break;
                case AudioCommandType.PlayPattern:
                    AY.PlayMode = PlayModes.PlayPattern;
                    for (int i = 0; i < AY.ChipCount; i++)
                    {
                        if (AY.PlayingModule[i] == null) continue;
                        VTModule.Module_SetPointer(AY.PlayingModule[i], i);
                        VTModule.Module_SetDelay((sbyte)AY.PlayingModule[i].InitialDelay);
                        VTModule.Module_SetCurrentPosition(0);
                        VTModule.Module_SetCurrentPattern(command.PatternIndex);
                    }
                    break;
                case AudioCommandType.PlayPatternLine:
                    AY.PlayMode = PlayModes.PlayPattern;
                    for (int i = 0; i < AY.ChipCount; i++)
                    {
                        if (AY.PlayingModule[i] == null) continue;
                        VTModule.Module_SetPointer(AY.PlayingModule[i], i);
                        VTModule.Module_SetDelay((sbyte)AY.PlayingModule[i].InitialDelay);
                        VTModule.Module_SetCurrentPattern(command.PatternIndex);
                        VTModule.Pattern_SetCurrentLine(command.LineIndex);
                    }
                    break;
                case AudioCommandType.PlayLine:
                    AY.PlayMode = PlayModes.PlayLine;
                    VTModule.Module_SetPointer(AY.PlayingModule[0], 0);
                    VTModule.Module_SetCurrentPattern(command.PatternIndex);
                    VTModule.Pattern_SetCurrentLine(command.LineIndex);
                    break;
            }
        }
        private static void ALCheck()
        {
            ALError alError = AL.GetError();
            if (alError != ALError.NoError)
            {
                Debug.WriteLine($"OpenAL Error: {alError}");
            }
        }

        public static void SkipRedraw()
        {
            if (AY.PlayMode == PlayModes.PlayModule || AY.PlayMode == PlayModes.PlayPattern)
                AppEvents.ClearEvent(EventType.RedrawTracks);
        }

        /* public static void StopTrackSlider()
        {
            if (TSEvent == null)
                return;

            TSEvent.Set();

            while (TSThread.IsAlive)
                Thread.Sleep(1);

            TSThread = null;

            TSEvent.Dispose();
            TSEvent = null;
        } */

        public static void WaitForWOThreadExit()
        {
            Debug.WriteLine("WaitForWOThreadExit");

            if (WOThread == null)
                return;

            WOThread.Join(5000);

            //if (WOThread.IsAlive)
            //    WOEvent.Wait();

            WOThread = null;
        }

        public static void StopPlaying()
        {
            EnqueueCommand(AudioCommandType.Stop);
        }

        private static void StopPlayingInternal()
        {
            Debug.WriteLine("StopPlaying");

            if (!IsPlaying)
                return;

            IsPlaying = false;

            // Wait for the WO thread to finish all AL calls before destroying the context.
            WOThread?.Join(3000);

            StopOpenAL();
            UnResetPlayingInternal();
            AY.ClearRegisters();
            AY.ClearSpec();
        }

        public static bool AllBuffersDone()
        {
            AL.GetSource(_alSource, ALGetSourcei.BuffersQueued, out int queued);

            return (queued == 0);
        }

        public static void ClearBuffers()
        {
            for (int i = 0; i < BufferCount; i++)
            {
                int bufferID = _alBuffers[i];
                WaveBuffer waveBuffer = BufferMap[bufferID];

                Array.Clear(waveBuffer.PcmData);
            }
        }

        public static int WOThreadFunc()
        {
            Debug.WriteLine("WOThreadFunc Start");

            bool resetMutex = false;

            ResetOpenALBuffers();

            try
            {
                while (IsPlaying)
                {
                    ResetMutex.WaitOne();
                    resetMutex = true;

                    Stopwatch sw = Stopwatch.StartNew();

                    // Process finished buffers
                    AL.GetSource(_alSource, ALGetSourcei.BuffersProcessed, out int processed);

                    //if (processed > 0)
                    //    WOEvent.Set();

                    while (processed-- > 0)
                    {
                        int bufferID = AL.SourceUnqueueBuffer(_alSource);

                        AL.GetBuffer(bufferID, ALGetBufferi.Size, out int sizeInBytes);

                        int sampleSize = (SampleBit / 8) * NumberOfChannels;
                        int sampleCount = sizeInBytes / sampleSize;
                        _totalSamplesPlayed += sampleCount;

                        WaveBuffer waveBuffer = BufferMap[bufferID];

                        AY.MakeBuffer(waveBuffer.PcmData);

                        AL.BufferData(bufferID, GetALFormat(NumberOfChannels, SampleBit), ref waveBuffer.PcmData[0], waveBuffer.PcmData.Length, SampleRate);
                        AL.SourceQueueBuffer(_alSource, bufferID);
                    }

                    resetMutex = false;
                    ResetMutex.ReleaseMutex();

                    if (AY.RealEndAll && !HasReset && AllBuffersDone())
                        break;

                    if (!IsPlaying)
                        break;
                    
                    sw.Stop();
                    //WOEvent.WaitOne(BufLen_ms);
                    WaitForOpenALBuffers(0); // sw.ElapsedMilliseconds);
                    //Thread.Sleep(BufLen_ms);
                    PerformanceMonitor.Frame();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in WOThreadFunc: " + ex.Message);

                if (resetMutex)
                {
                    resetMutex = false;
                    ResetMutex.ReleaseMutex();
                }

                AppEvents.PostEvent(EventType.PlayingOff);
            }
            finally
            {
                Debug.WriteLine("WOThreadFunc finally reached");

                if (resetMutex)
                {
                    try { ResetMutex.ReleaseMutex(); }
                    catch (ApplicationException) { }
                }

                // If we exit the loop, finalize
                AppEvents.PostEvent(EventType.FinalizeWO);
                VTModule.UnlimitedDelay = false;
                IsPlaying = false;
                PlaybackEnded?.Invoke();
            }

            Debug.WriteLine("WOThreadFunc End");

            return 0;
        }

        private static void ResetOpenALBuffers()
        {
            AL.SourceStop(_alSource);

            // Unqueue remaining buffers
            AL.GetSource(_alSource, ALGetSourcei.BuffersQueued, out int queued);

            while (queued-- > 0)
                AL.SourceUnqueueBuffer(_alSource);

            // Queue the buffers
            for (int i = 0; i < BufferCount; i++)
            {
                int bufferID = _alBuffers[i];

                if (!BufferMap.TryGetValue(bufferID, out WaveBuffer waveBuffer))
                    return;

                AY.MakeBuffer(waveBuffer.PcmData);

                AL.BufferData(waveBuffer.BufferID, GetALFormat(NumberOfChannels, SampleBit), ref waveBuffer.PcmData[0], waveBuffer.PcmData.Length, SampleRate);
                AL.SourceQueueBuffer(_alSource, waveBuffer.BufferID);
            }

            _totalSamplesPlayed = 0;
            //_playbackTimer.Restart();
            AL.SourcePlay(_alSource);
        }

        private static void WaitForOpenALBuffers(long processingTimeMs)
        {
            int remainingWait = (int)Math.Max(0, BufferLengthMs - processingTimeMs);

            DateTime timeout = DateTime.UtcNow.AddMilliseconds(remainingWait);

            while (DateTime.UtcNow < timeout)
            {
                if (!IsPlaying)
                    break;

                AL.GetSource(_alSource, ALGetSourcei.BuffersProcessed, out int processed);

                if (processed > 0)
                {
                    //WOEvent.Set();
                    break;
                }

                Thread.Sleep(1);
            }
        }

        private static uint GetAccurateSamplePosition()
        {
            double elapsedSeconds = _playbackTimer.Elapsed.TotalSeconds;
            uint samplesPlayed = (uint)(elapsedSeconds * SampleRate);
            return samplesPlayed;
        }

        private static ALFormat GetALFormat(int channels, int bitsPerSample)
        {
            // For example, 16-bit stereo:
            if (channels == 2 && bitsPerSample == 16)
                return ALFormat.Stereo16;
            if (channels == 1 && bitsPerSample == 16)
                return ALFormat.Mono16;
            // etc. for 8-bit or 32-bit if needed
            throw new NotSupportedException("Only 16-bit mono/stereo supported here.");
        }

        public static void StartWOThread()
        {
            EnqueueCommand(AudioCommandType.Start);
        }

        public static void EnqueuePlayModule(VTM vtm, VTM? vtm2, VTM? vtm3, int chipCount, bool loopEnabled)
            => EnqueueCommand(new AudioCommand
            {
                Type = AudioCommandType.PlayModule,
                Vtm = vtm, Vtm2 = vtm2, Vtm3 = vtm3,
                ChipCount = chipCount, LoopEnabled = loopEnabled
            });

        public static void EnqueuePlayPattern(VTM vtm, VTM? vtm2, VTM? vtm3, int chipCount, bool loopEnabled, int patternIndex)
            => EnqueueCommand(new AudioCommand
            {
                Type = AudioCommandType.PlayPattern,
                Vtm = vtm, Vtm2 = vtm2, Vtm3 = vtm3,
                ChipCount = chipCount, LoopEnabled = loopEnabled,
                PatternIndex = patternIndex
            });

        public static void EnqueuePlayPatternLine(VTM vtm, VTM? vtm2, VTM? vtm3, int chipCount, bool loopEnabled, int patternIndex, int lineIndex)
            => EnqueueCommand(new AudioCommand
            {
                Type = AudioCommandType.PlayPatternLine,
                Vtm = vtm, Vtm2 = vtm2, Vtm3 = vtm3,
                ChipCount = chipCount, LoopEnabled = loopEnabled,
                PatternIndex = patternIndex, LineIndex = lineIndex
            });

        public static void EnqueuePlayLine(VTM vtm, int patternIndex, int lineIndex)
            => EnqueueCommand(new AudioCommand
            {
                Type = AudioCommandType.PlayLine,
                Vtm = vtm, Vtm2 = null, Vtm3 = null,
                ChipCount = 1, LoopEnabled = false,
                PatternIndex = patternIndex, LineIndex = lineIndex
            });

        private static void StartWOThreadInternal()
        {
            if (WOThreadActive())
                return;

            Debug.WriteLine("StartWOThread =======================>");

            // 2) Configure AY frequencies
            AY.SetAYFreq(AY.ActiveModule.ChipFreq);
            AY.SetIntFreq(AY.ActiveModule.IntFreq);

            ExportStarted = false;
            AudioProblem = false;

            //StartTrackSlider();

            // 3) Initialize OpenAL device and context
            _alDevice = ALC.OpenDevice(WODevice);
            _alContext = ALC.CreateContext(_alDevice, (int[])null);

            ALC.MakeContextCurrent(_alContext);

            // 4) Generate a single source
            _alSource = AL.GenSource();

            // 5) Create N AL buffers
            _alBuffers = AL.GenBuffers(BufferCount);

            // 6) Compute how many samples per buffer (based on BufLen_ms)
            int sampleSize = (SampleBit / 8) * NumberOfChannels;
            int samplesPerBuffer = (SampleRate * BufferLengthMs) / 1000;
            BufferLength = samplesPerBuffer;
            BufferPosition = samplesPerBuffer; // used in your code

            BufferMap.Clear();

            // 7) Fill + queue each AL buffer with initial data
            for (int i = 0; i < BufferCount; i++)
            {
                if (BufferMap.ContainsKey(_alBuffers[i]))
                    continue;

                // Create a buffer of PCM data
                WaveBuffer waveBuffer = new WaveBuffer()
                {
                    BufferID = _alBuffers[i],
                    PcmData = new byte[samplesPerBuffer * sampleSize]
                };

                BufferMap.Add(waveBuffer.BufferID, waveBuffer);

                //AL.BufferData(waveBuffer.BufferID, GetALFormat(NumberOfChannels, SampleBit), ref waveBuffer.PcmData[0], waveBuffer.PcmData.Length, SampleRate);
                //AL.SourceQueueBuffer(_alSource, waveBuffer.BufferID);
            }

            //AL.SourcePlay(_alSource);

            // 9) Initialize AyumiChip

            for (int i = 0; i < AY.ChipCount; i++)
            {
                AY.AyumiChip[i] = new Ayumi();
                AY.AyumiChip[i].Configure(AY.EmulatingChip == ChipType.YM, AY.AYFreq, SampleRate, AY.DCType);
                AY.AyumiChip[i].SetDCCutoff(AY.DCCutOff);
                AY.AyumiChip[i].SetPan(0, Main.Panoram[0] / 255.0, false);
                AY.AyumiChip[i].SetPan(1, Main.Panoram[1] / 255.0, false);
                AY.AyumiChip[i].SetPan(2, Main.Panoram[2] / 255.0, false);
            }

            // 10) Mark playing + spawn the worker thread
            IsPlaying = true;
            HasReset = false;

            AY.SongClock.Restart();

            //WOEvent.Reset();

            WOThread = new Thread(() => WOThreadFunc());
            WOThread.Name = "WOThread";
            WOThread.IsBackground = true;
            WOThread.Start();
        }

        public static void StopOpenAL()
        {
            Debug.WriteLine("StopOpenAL");

            /* for (int i = 0; i < NumberOfBuffers; i++)
            {
                int bufferID = _alBuffers[i];
                TWaveBuffer waveBuffer = BufferMap[bufferID];
                Array.Clear(waveBuffer.PcmData);
            } */

            // Immediately stop audio playback
            AL.SourceStop(_alSource);

            // Unqueue remaining buffers
            AL.GetSource(_alSource, ALGetSourcei.BuffersQueued, out int queued);

            while (queued-- > 0)
                AL.SourceUnqueueBuffer(_alSource);

            // Delete buffers, source
            AL.DeleteSource(_alSource);
            AL.DeleteBuffers(_alBuffers);

            // Destroy context + device
            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(_alContext);
            ALC.CloseDevice(_alDevice);
        }

        public static void WOThreadFinalization()
        {
            Debug.WriteLine("WOThreadFinalization");

            // Possibly null out your AY objects
            for (int i = 0; i < AY.ChipCount; i++)
                AY.AyumiChip[i] = null;

            //StopTrackSlider();

            // Stop the source
            //AL.SourceStop(_alSource);

            // Unqueue any remaining buffers
            //AL.GetSource(_alSource, ALGetSourcei.BuffersQueued, out int queued);
            //while (queued-- > 0)
            //    AL.SourceUnqueueBuffer(_alSource);

            // Delete buffers, source
            //AL.DeleteSource(_alSource);
            //AL.DeleteBuffers(_alBuffers);

            // Destroy context + device
            //ALC.MakeContextCurrent(ALContext.Null);
            //ALC.DestroyContext(_alContext);
            //ALC.CloseDevice(_alDevice);

            IsPlaying = false;
            HasReset = false;
            AudioProblem = false;
        }

        public static void ResetAYChipEmulation(int chip)
        {
            SoundChip soundChip = AY.SoundChip[chip];
            soundChip.AYRegisters.Clear();
            soundChip.SetEnvelopeRegister(0);
            soundChip.FirstPeriod = false;
            soundChip.Amplitude = 0;
            soundChip.SetMixerRegister(0);
            soundChip.SetAmplitudeA(0);
            soundChip.SetAmplitudeB(0);
            soundChip.SetAmplitudeC(0);
            AY.IntFlag = false;
            AY.TikCount.Re = 0;
            AY.CurrentTik = 0;
            soundChip.EnvelopeCounter.Re = 0;
            soundChip.ToneCounterA.Re = 0;
            soundChip.ToneCounterB.Re = 0;
            soundChip.ToneCounterC.Re = 0;
            soundChip.NoiseCounter.Re = 0;
            soundChip.ToneA = 0;
            soundChip.ToneB = 0;
            soundChip.ToneC = 0;
            AY.ChannelL = 0;
            AY.ChannelR = 0;
            AY.TickCounter = 0;
            AY.Tik.Re = (uint)AY.DelayInTiks;
            soundChip.Noise.Seed = 0xFFFF;
            soundChip.Noise.Value = 0;

            soundChip.ClearSpec();

            if (AY.AyumiChip[chip] != null)
                AY.AyumiChip[chip].ResetChip();
        }

        public static void InitForAllTypes(bool all)
        {
            LineReady = false;
            PlayGridIndex = 0;
            TickCount = 0;

            for (int i = 0; i < AY.ChipCount; i++)
            {
                ResetAYChipEmulation(i);
                AY.RealEnd[i] = false;
            }

            AY.RealEndAll = false;

            if (AY.RenderEngine == 0 && AY.FilterEnabled)
            {
                Helpers.FillChar<int>(AY.DelayLineL, (AY.FilterLength + 1) * 4, 0);
                Helpers.FillChar<int>(AY.DelayLineR, (AY.FilterLength + 1) * 4, 0);
            }

            for (int i = AY.ChipCount - 1; i >= 0; i--)
            {
                if (AY.PlayingModule[i] == null)
                    continue;

                VTModule.Module_SetPointer(AY.PlayingModule[i], i);
                VTModule.InitTrackerParameters(all);
            }
        }

        public static bool WOThreadActive()
        {
            return WOThread != null && WOThread.IsAlive;
        }

        public static void ResetPlaying()
        {
            EnqueueCommand(AudioCommandType.Reset);
        }

        private static void ResetPlayingInternal()
        {
            if (HasReset)
                return;

            Debug.WriteLine("ResetPlaying");

            HasReset = true;
            VTModule.UnlimitedDelay = false;
            AudioProblem = false;

            ResetMutex.WaitOne();

            //Thread.BeginCriticalRegion();
            //AL.SourceStop(_alSource);
            //Thread.EndCriticalRegion();

            /* while (!AllBuffersDone())
            {
                Debug.WriteLine("AllBuffersDone");
                Thread.Sleep(0);
            } */

            PlayGridIndex = 0;
            TickCount = 0;

            AY.ClearRegisters();
            AY.ClearSpec();
        }

        public static void UnResetPlaying()
        {
            EnqueueCommand(AudioCommandType.Unreset);
        }

        private static void UnResetPlayingInternal()
        {
            if (!HasReset)
                return;

            Debug.WriteLine("UnResetPlaying");

            ResetOpenALBuffers();
            //WOEvent.Set();

            AudioProblem = false;
            HasReset = false;
            //ReleaseMutex(ResetMutex);
            try
            {
                ResetMutex.ReleaseMutex();
            }
            catch(ApplicationException)
            {
            }
        }

        public static void WriteWaveHeader(Stream stream, int dataLength, int sampleRate, short bitsPerSample, short channels)
        {
            if (sampleRate <= 0 || !(bitsPerSample is 8 or 16 or 24 or 32) || !(channels is 1 or 2))
                throw new ArgumentException("Invalid parameters");

            int blockAlign = channels * bitsPerSample / 8;
            int bytesPerSec = sampleRate * blockAlign;
            int riffLen = dataLength + 36;   // 4 ("WAVE") + 24 ("fmt ") + 8 ("data") + data

            Span<byte> bytes = stackalloc byte[44];   // canonical PCM header is always 44 bytes

            Encoding.ASCII.GetBytes("RIFF").CopyTo(bytes);
            BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(4), riffLen);
            Encoding.ASCII.GetBytes("WAVE").CopyTo(bytes.Slice(8));

            Encoding.ASCII.GetBytes("fmt ").CopyTo(bytes.Slice(12));      // id
            BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(16), 16); // chunk size   (16 for PCM)
            BinaryPrimitives.WriteInt16LittleEndian(bytes.Slice(20), 1);  // format tag   (1 = PCM)
            BinaryPrimitives.WriteInt16LittleEndian(bytes.Slice(22), channels);
            BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(24), sampleRate);
            BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(28), bytesPerSec);
            BinaryPrimitives.WriteInt16LittleEndian(bytes.Slice(32), (short)blockAlign);
            BinaryPrimitives.WriteInt16LittleEndian(bytes.Slice(34), bitsPerSample);

            Encoding.ASCII.GetBytes("data").CopyTo(bytes.Slice(36));
            BinaryPrimitives.WriteInt32LittleEndian(bytes.Slice(40), dataLength);

            stream.Write(bytes);
        }

        // From ChildForm.cs
        public static bool ConvertTrackerModule(string fileName, byte[] data, ModuleType fileType, int zxAddr, int tm, int andSix, string songName, string authorName, VTM vtm, ref VTM vtm2, ref VTM vtm3)
        {
            string errMsg = "";
            bool result = true;
            int i, j;
            i = 0;

            switch (fileType)
            {
                case ModuleType.Unknown:
                    i = VTM.LoadModuleFromText(fileName, vtm, ref vtm2, ref vtm3);

                    if (i != 0)
                        result = false;

                    if (i == -1)
                    {
                        AppEvents.SendEvent(EventType.MessageBox, "Patterns Not Found", "Text Module Loader Error", MyMessageBoxButtons.OK, MyMessageBoxIcon.Exclamation);
                        //MessageBox.Show(this, "Patterns not found", "Text module loader error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    if (i == -2)
                    {
                        AppEvents.SendEvent(EventType.MessageBox, "Samples Not Found", "Text Module Loader Error", MyMessageBoxButtons.OK, MyMessageBoxIcon.Exclamation);
                        //MessageBox.Show(("Samples not found", "Text module loader error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    if (i == -3)
                    {
                        AppEvents.SendEvent(EventType.MessageBox, "Ornaments Not Found", "Text Module Loader Error", MyMessageBoxButtons.OK, MyMessageBoxIcon.Exclamation);
                        //MessageBox.Show(this, "Ornaments not found", "Text module loader error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    if (i > 0)
                    {
                        AppEvents.SendEvent(EventType.MessageBox, $"Error in Line {i}", "Text Module Loader Error", MyMessageBoxButtons.OK, MyMessageBoxIcon.Exclamation);
                        //MessageBox.Show((ErrorStrings[i] + " (line: " + (TModule.TxtLine).ToString() + ')' as string), "Text module loader error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    if (!result)
                        return result;

                    if (vtm2 != null)
                    {
                        vtm2.ShowInfo = vtm.ShowInfo;
                        vtm2.Info = vtm.Info;
                    }

                    if (vtm3 != null)
                    {
                        vtm3.ShowInfo = vtm.ShowInfo;
                        vtm3.Info = vtm.Info;
                    }
                    break;
                case ModuleType.PT2File:
                    PT22VTM pt22vtm = new PT22VTM();
                    pt22vtm.Initialize(data, vtm);
                    break;
                case ModuleType.PT1File:
                    PT12VTM pt12VTM = new PT12VTM();
                    pt12VTM.Initialize(data, vtm);
                    break;
                case ModuleType.STCFile:
                    STC2VTM stc2VTM = new STC2VTM();
                    stc2VTM.Initialize(data, i, vtm);
                    break;
                case ModuleType.STPFile:
                    STP2VTM stp2VTM = new STP2VTM();
                    stp2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.SQTFile:
                    SQT2VTM sqt2VTM = new SQT2VTM();
                    sqt2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.ASCFile:
                    ASC2VTM asc2VTM = new ASC2VTM();
                    asc2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.PSCFile:
                    PSC2VTM psc2VTM = new PSC2VTM();
                    psc2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.FLSFile:
                    FLS2VTM fls2VTM = new FLS2VTM();
                    fls2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.GTRFile:
                    GTR2VTM gTR2VTM = new GTR2VTM();
                    gTR2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.FTCFile:
                    FTC2VTM ftc2VTM = new FTC2VTM();
                    ftc2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.FXMFile:
                    FXM2VTM fxm2VTM = new FXM2VTM();
                    fxm2VTM.Initialize(data, zxAddr, tm, andSix, songName, authorName, vtm);
                    break;
                case ModuleType.PSMFile:
                    PSM2VTM psm2VTM = new PSM2VTM();
                    psm2VTM.Initialize(data, vtm);
                    break;
                case ModuleType.PT3File:
                    PT32VTM pt32VTM = new PT32VTM();
                    pt32VTM.Initialize(data, data.Length, vtm, ref vtm2);
                    //SavedAsText = false;
                    break;
            }

            // Validate loaded module
            for (j = 0; j < vtm.Patterns.Length; j++)
            {
                // Check for incorrect pattern length
                if (vtm.Patterns[j] != null && (vtm.Patterns[j].Length <= 0 || vtm.Patterns[j].Length > VTModule.MaxPatternLength))
                {
                    errMsg = $"Incorrect Pattern[{j}] Length: {vtm.Patterns[j].Length}";
                    result = false;
                    break;
                }
            }

            if (result)
            {
                for (j = 0; j < vtm.Positions.Length; j++)
                {
                    // Check for incorrect position value
                    if (vtm.Positions.Value[j] > VTModule.MaxPatternIndex || vtm.Positions.Value[j] < 0)
                    {
                        errMsg = $"Incorrect position[{j}] Pattern Number: {vtm.Positions.Value[j]}";
                        result = false;
                        break;
                    }

                    // Check if pattern exists
                    if (vtm.Patterns[vtm.Positions.Value[j]] == null)
                    {
                        errMsg = $"Position[{j}] Refers to Non-existent Pattern";
                        result = false;
                        break;
                    }
                }
            }

            // Check for incorrect sample length
            if (result)
            {
                for (j = 0; j < vtm.Samples.Length; j++)
                {
                    if (vtm.Samples[j] != null && (vtm.Samples[j].Length <= 0 || vtm.Samples[j].Length > VTModule.MaxSampleLength))
                    {
                        errMsg = $"Incorrect sample[{j}] Length: {vtm.Samples[j].Length}";
                        result = false;
                        break;
                    }
                }
            }

            // Check for incorrect ornament length
            if (result)
            {
                for (j = 0; j < vtm.Ornaments.Length; j++)
                {
                    if (vtm.Ornaments[j] != null && (vtm.Ornaments[j].Length <= 0 || vtm.Ornaments[j].Length > VTModule.MaxOrnamentLength))
                    {
                        errMsg = $"Incorrect Ornament[{j}] Length: {vtm.Ornaments[j].Length}";
                        result = false;
                        break;
                    }
                }
            }
            // Check for incorrect sample tone value
            // if Result then
            // for j := Low(VTM.Samples) to High(VTM.Samples) do
            // for k := 0 to MaxSamLen-1 do
            // if (VTM.Samples[j] <> nil) and ((VTM.Samples[j].Items[k].Add_to_Ton > $FFF) or (VTM.Samples[j].Items[k].Add_to_Ton < -$FFF)) then begin
            // Result := False;
            // Break;
            // end;
            // Check for incorrect speed
            if (vtm.InitialDelay == 0)
            {
                errMsg = $"Initial delay = 0";
                result = false;
            }

            // Check for incorrect tone table
            if ((byte)vtm.NoteTable > 4)
            {
                errMsg = $"Invalid Ton Table (>5) = {vtm.NoteTable}";
                result = false;
            }

            if (!result)
            {
                AppEvents.SendEvent(EventType.MessageBox, $"Module Loading Error {errMsg}", Main.ProductName, MyMessageBoxButtons.OK, MyMessageBoxIcon.Warning);
                //MessageBox.Show(this, "Module loading error", "Vortex Tracker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return result;
        }

        public static bool LoadTrackerModule(string fileName, int iter, ref int cnt, ref VTM vtm, ref VTM vtm2, ref VTM vtm3)
        {
            bool result = true;
            int length;
            string s;
            Stream stream;
            byte[] data;
            int tm;
            int tsSize1;
            int tsSize2;
            byte andSix;
            ushort zxAddr;
            string authorName;
            string songName;
            ModuleType fileType;
            ModuleType fileType1;
            ModuleType fileType2;

            //SavedAsText = true;
            //UndoWorking = true;
            if (!File.Exists(fileName))
            {
                AppEvents.SendEvent(EventType.MessageBox, $"Can\'t open file", $"File not found: \"{fileName}\"", MyMessageBoxButtons.OK, MyMessageBoxIcon.Warning);
                //MessageBox.Show(("File not found: \"" + Name + '\"' as string), "Can\'t open file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (iter == 0)
            {
                fileType = ZXModule.LoadAndDetect(fileName, out length, out fileType1, out fileType2, out tsSize1, out tsSize2, out zxAddr, out tm, out andSix, out authorName, out songName, out data);
                vtm ??= new VTM();
                result = ConvertTrackerModule(fileName, data, fileType, zxAddr, tm, andSix, songName, authorName, vtm, ref vtm2, ref vtm3);

                if (!result)
                    return result;

                if (vtm2 == null && vtm3 == null)
                    cnt = 1;
                else if (vtm3 == null)
                    cnt = 2;
                else
                    cnt = 3;

                if (vtm2 == null && fileType1 != ModuleType.Unknown && tsSize1 != 0)
                {
                    using (FileStream stream2 = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        BinaryReader reader2 = new BinaryReader(stream2);
                        reader2.BaseStream.Seek(length, SeekOrigin.Begin);
                        byte[] data2 = reader2.ReadBytes(tsSize1);
                        reader2.Close();

                        if (data2.Length == tsSize1)
                        {
                            ZXModule.Prepare(data2, ref fileType1, tsSize1);

                            if (fileType1 != ModuleType.Unknown)
                            {
                                vtm2 = new VTM();
                                VTM dummy = null;
                                ConvertTrackerModule(fileName, data2, fileType1, zxAddr, tm, andSix, songName, authorName, vtm2, ref dummy, ref dummy);

                                if (dummy != null)
                                    VTModule.FreeVTM(ref dummy);

                                cnt++;

                                // If third submodule exists
                                if (fileType2 != ModuleType.Unknown && tsSize2 != 0)
                                {
                                    using (FileStream stream3 = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                                    {
                                        BinaryReader reader3 = new BinaryReader(stream3);
                                        reader3.BaseStream.Seek(length + tsSize1, SeekOrigin.Begin);
                                        byte[] data3 = reader3.ReadBytes(tsSize2);
                                        reader3.Close();

                                        if (data3.Length == tsSize2)
                                        {
                                            ZXModule.Prepare(data3, ref fileType2, tsSize2);

                                            if (fileType2 != ModuleType.Unknown)
                                            {
                                                vtm3 = new VTM();
                                                dummy = null;
                                                ConvertTrackerModule(fileName, data3, fileType2, zxAddr, tm, andSix, songName, authorName, vtm3, ref dummy, ref dummy);
                                                cnt++;
                                                if (dummy != null)
                                                    VTModule.FreeVTM(ref dummy);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (iter == 1)
                vtm = vtm2;
            else if (iter == 2)
                vtm = vtm3;

            VTModule.Module_SetPointer(vtm, iter); // iter 0..2 chip 0..2

            //Debug.WriteLine(vtm.ToString());

            return result;
        }

        public static int ALSource
        {
            get { return _alSource; }
        }

        public static int TotalSamplesPlayed
        {
            get { return _totalSamplesPlayed; }
        }

        public static void Initialize()
        {
            //WOEvent = new ManualResetEventSlim(false);
            ResetMutex = new Mutex(false, "VTIII_Reset_" + Process.GetCurrentProcess().Id.ToString());
            EnsureCommandThread();
        }

        public static void Shutdown()
        {
            //WOEvent.Dispose();
            ResetMutex.Dispose();
            if (_commandQueue != null)
            {
                _commandQueue.CompleteAdding();
                _commandThread?.Join(2000);
                _commandQueue.Dispose();
                _commandQueue = null;
                _commandThread = null;
                _commandThreadId = 0;
            }
        }
    }
}
