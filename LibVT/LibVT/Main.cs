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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibVT
{
    public class Main
    {
        public static string ProductName = null;
        public static NoteTableType NoteTableOnLoad = 0;
        public static bool LoopAllAllowed = false;
        public static int GlobalVolume = 56;
        public static int GlobalVolumeMax = 64;
        public static Sample BuffSample = null;
        public static Ornament BuffOrnament = null;
        public static bool VTExit = false;
        public static byte[] Panoram = new byte[3];
        public static bool EnvelopeAsNote = false;
        public static bool MoveBetweenPatterns = true;
        public static bool DecBaseLinesOn = false;
        public static bool DecBaseNoiseOn = false;
        public static int ManualChipFreq = 0;
        public static int DefaultChipFreq = 1750000;
        public static int DefaultIntFreq = 48828;
        public static int ChanAllocIndex = 1;

        static Main()
        {
			ProductName= Assembly.GetExecutingAssembly().GetName().Name;

			Panoram[0] = 64;
            Panoram[1] = 128;
            Panoram[2] = 192;
		}

        public static void InitBuffSample()
        {
            BuffSample = new Sample();
            BuffSample.Length = 1;
            BuffSample.Loop = 0;
            BuffSample.Ticks = new SampleTick[VTModule.MaxSampleLength];
            BuffSample.Ticks[0] = new SampleTick();
        }

        public static void InitBuffOrnament()
        {
            BuffOrnament = new Ornament();
            BuffOrnament.Length = 1;
            BuffOrnament.Loop = 0;
            BuffOrnament.Offsets[0] = 0;
        }
    }
}
