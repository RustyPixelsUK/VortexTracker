using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    public class Win32
    {
        public const ushort WAVE_FORMAT_PCM = 0x0001;

        public enum MMRESULT : uint
        {
            MMSYSERR_NOERROR = 0,
            MMSYSERR_ERROR = 1,
            MMSYSERR_BADDEVICEID = 2,
            MMSYSERR_NOTENABLED = 3,
            MMSYSERR_ALLOCATED = 4,
            MMSYSERR_INVALHANDLE = 5,
            MMSYSERR_NODRIVER = 6,
            MMSYSERR_NOMEM = 7,
            MMSYSERR_NOTSUPPORTED = 8,
            MMSYSERR_BADERRNUM = 9,
            MMSYSERR_INVALFLAG = 10,
            MMSYSERR_INVALPARAM = 11,
            MMSYSERR_HANDLEBUSY = 12,
            MMSYSERR_INVALIDALIAS = 13,
            MMSYSERR_BADDB = 14,
            MMSYSERR_KEYNOTFOUND = 15,
            MMSYSERR_READERROR = 16,
            MMSYSERR_WRITEERROR = 17,
            MMSYSERR_DELETEERROR = 18,
            MMSYSERR_VALNOTFOUND = 19,
            MMSYSERR_NODRIVERCB = 20,
            WAVERR_BADFORMAT = 32,
            WAVERR_STILLPLAYING = 33,
            WAVERR_UNPREPARED = 34
        }

        public const int WAVE_MAPPER = -1;

        [StructLayout(LayoutKind.Explicit)]
        public struct MMTime
        {
            [FieldOffset(0)]
            public UInt32 wType;
            [FieldOffset(4)]
            public UInt32 Ms;
            [FieldOffset(4)]
            public UInt32 Sample;
            [FieldOffset(4)]
            public UInt32 Cb;
            [FieldOffset(4)]
            public UInt32 Ticks;
            [FieldOffset(4)]
            public Byte SmpteHour;
            [FieldOffset(5)]
            public Byte SmpteMin;
            [FieldOffset(6)]
            public Byte SmpteSec;
            [FieldOffset(7)]
            public Byte SmpteFrame;
            [FieldOffset(8)]
            public Byte SmpteFps;
            [FieldOffset(9)]
            public Byte SsmpteDummy;
            [FieldOffset(10)]
            public Byte SmptePad0;
            [FieldOffset(11)]
            public Byte SmptePad1;
            [FieldOffset(4)]
            public UInt32 MidiSongPtrPos;
        }

        public const int TIME_SAMPLES = 0x0002;

        public delegate void WaveDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WAVEHDR wavhdr, int dwParam2);

        [StructLayout(LayoutKind.Sequential)]
        public struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        [Flags]
        public enum WaveHdrFlags : uint
        {
            WHDR_DONE = 1,
            WHDR_PREPARED = 2,
            WHDR_BEGINLOOP = 4,
            WHDR_ENDLOOP = 8,
            WHDR_INQUEUE = 16
        }

        [Flags()]
        public enum WaveOpenFlags : uint
        {
            CALLBACK_NULL = 0x0,
            CALLBACK_WINDOW = 0x10000,
            CALLBACK_THREAD = 0x20000,
            CALLBACK_FUNCTION = 0x30000,
            CALLBACK_EVENT = 0x50000,
            WAVE_FORMAT_QUERY = 0x1,
            WAVE_ALLOWSYNC = 0x2,
            WAVE_MAPPED = 0x4,
            WAVE_FORMAT_DIRECT = 0x8,
            WAVE_MAPPED_DEFAULT_COMMUNICATION_DEVICE = 0x10
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WAVEHDR
        {
            public IntPtr lpData;
            public uint dwBufferLength;
            public uint dwBytesRecorded;
            public IntPtr dwUser;
            public WaveHdrFlags dwFlags;
            public uint dwLoops;
            public IntPtr lpNext;
            public uint reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WAVEOUTCAPS
        {
            public short wMid;
            public short wPid;
            public int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public int dwFormats;
            public short wChannels;
            public short wReserved;
            public int dwSupport;
        }

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutGetDevCaps(IntPtr hwo, ref WAVEOUTCAPS pwoc, uint cbwoc);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutGetNumDevs();
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutPrepareHeader(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutUnprepareHeader(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutWrite(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WAVEFORMATEX lpFormat, IntPtr dwCallback, IntPtr dwInstance, WaveOpenFlags dwFlags);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutReset(IntPtr hWaveOut);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutClose(IntPtr hWaveOut);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutPause(IntPtr hWaveOut);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutRestart(IntPtr hWaveOut);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutGetPosition(IntPtr hWaveOut, out MMTime lpInfo, int uSize);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutSetVolume(IntPtr hWaveOut, int dwVolume);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern MMRESULT waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);
        [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint waveOutGetErrorText(MMRESULT mmrError, StringBuilder pszText, uint cchText);
    }
}
