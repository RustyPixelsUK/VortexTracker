using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;

namespace LibVT
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string applicationPath = Path.GetDirectoryName(assembly.Location);
            string demoSongsPath = Path.Combine(applicationPath, "DemoSongs");
            string testModulesPath = Path.Combine(applicationPath, "TestModules");

            //string fileName = Path.Combine(demoSongsPath, "2018_EA_demosong.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2018_FatalSnipe_CriticalV.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2018_mmcm_Dreaming_of_Summer_ts.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2018_nq_skrju_demosong.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2019_EA_Road_to_Summer.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2019_Fatalsnipe_Vortex_animation.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2019_Kakos_Nonos_Vortex_Power.vt2");
            string fileName = Path.Combine(demoSongsPath, "2019_MmcM_Conversions.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2019_MmcM_ft_nq_NEStle_for_ears.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2019_MmcM_Strange_movements.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2019_nq_TESTOTUNOHARDOCORE.vt2");
            //string fileName = Path.Combine(demoSongsPath, "2019_wbcbz7_you_shouldnt_quit_chipping.vt2");

            //string fileName = Path.Combine(testModulesPath, @"ASC Sound Master\GOOD-N.asc");
            //string fileName = Path.Combine(testModulesPath, @"ASC Sound Master\MARTANGO.asc");
            //string fileName = Path.Combine(testModulesPath, @"ASC Sound Master\Red,red.asc");
            //string fileName = Path.Combine(testModulesPath, @"ASC Sound Master\SAXSOLO.asc");
            //string fileName = Path.Combine(testModulesPath, @"ASC Sound Master\SKY_SURF.asc");
            //string fileName = Path.Combine(testModulesPath, @"ASC Sound Master\VA-BANK.asc");
            //string fileName = Path.Combine(testModulesPath, @"Fast Tracker\Nostalgy.ftc");
            //string fileName = Path.Combine(testModulesPath, @"Fast Tracker\PA-PA-PA.ftc");
            //string fileName = Path.Combine(testModulesPath, @"Flash Tracker\Beratron.fls");
            //string fileName = Path.Combine(testModulesPath, @"Flash Tracker\Floppys.fls");
            //string fileName = Path.Combine(testModulesPath, @"Fuxoft AY Language\Fuxoft - Indiana Jones 3.fxm");
            //string fileName = Path.Combine(testModulesPath, @"Fuxoft AY Language\Patrik Rak - Terra Cresta.fxm");
            //string fileName = Path.Combine(testModulesPath, @"Fuxoft AY Language\View_To_A_Kill.ay");
            //string fileName = Path.Combine(testModulesPath, @"Global Tracker\HYMN.gtr");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\019-bg-cryin world.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\042-bg-compilated_dreams.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\051-bg-feels_like_flyin.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\BQK.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\IIkrolik.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\Infini!.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\Kruk2_23.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Listen only with filter\Kruk2_28.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Pro Sound Creator 1.00-1.07\BUZZ16_1.psc"); // *
            //string fileName = Path.Combine(testModulesPath, @"Pro Sound Creator 1.00-1.07\Condomed Track 4.psc");
            //string fileName = Path.Combine(testModulesPath, @"Pro Sound Creator 1.00-1.07\Sairoos - Inbeatween.psc");
            //string fileName = Path.Combine(testModulesPath, @"Pro Sound Maker\Dexus - a2.psm");
            //string fileName = Path.Combine(testModulesPath, @"Pro Sound Maker\Dexus - m4.psm");
            //string fileName = Path.Combine(testModulesPath, @"Pro Sound Maker\or2.psm");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 1.xx\Intro by TWIN'S.pt1");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 2.xx\MEGAHERZ.pt2");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 2.xx\RAGE.pt2");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 3.xx\Chuta.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 3.xx\m_entryO.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 3.xx\NeOtpusk.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 3.xx\TrafOfD.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Pro Tracker 3.xx\UckA_!a.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by AlCo\SNA1+.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\!ndiffer.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\!_can_be.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\After2.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Backhome.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Beg!nsum.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Fret.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Golubka.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Ma9w!nds.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Minimal.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Morensh.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Mysong.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Naylc-o2.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Oblaka.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Perseve.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Reaofno!.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Red_cap.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Soulomat.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by macros\Specific.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sent by Shiru Otaku\smario.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Sound Tracker\Sergey Bulba - Again.stc");
            //string fileName = Path.Combine(testModulesPath, @"Sound Tracker Pro\Hacker KAY & Nikita - Whisper.stp");
            //string fileName = Path.Combine(testModulesPath, @"Sound Tracker Pro\ZXRevu2_3.stp");
            //string fileName = Path.Combine(testModulesPath, @"SQ-Tracker\Factor6 - giannacrk.sqt");
            //string fileName = Path.Combine(testModulesPath, @"SQ-Tracker\Matej Kovalcik - Nothing Else Matters.sqt");
            //string fileName = Path.Combine(testModulesPath, @"SQ-Tracker\TDM - freedom.sqt");
            //string fileName = Path.Combine(testModulesPath, @"SQ-Tracker\X-agon_of_Phantasy-Breath-of-air-MOD.sqt");
            //string fileName = Path.Combine(testModulesPath, @"SQ-Tracker\X-agon_of_Phantasy-Check-it.sqt");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\BallQuest2.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\BallQuest3.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\DncDick.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\Factor6 - Cover of 'Hung Up' by Madonna (2xAY ACB).sqt");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\long_day.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\Minik.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\onestp.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\panic.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\p_apostl.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\sayYeah!.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\Shiru Otaku");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\smvenuTS.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\WeBberTS.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\X-agon_of_Phantasy-Breath-of-air-6chan-MOD.sqt");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\Shiru Otaku\cast.txt");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\Shiru Otaku\ddr.txt");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\Shiru Otaku\jts.txt");
            //string fileName = Path.Combine(testModulesPath, @"Turbo-Sound\Shiru Otaku\pb.txt");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\1stVTII.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Back2MyStyle.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\EnvTest.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Karboflex.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Kristina.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Kroshka.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Lena.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Macho.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\NoPower.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Oil.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\Tears.pt3");
            //string fileName = Path.Combine(testModulesPath, @"Vortex Tracker II\try.pt3");

            VTM vtm = null;
            VTM vtm2 = null;
            VTM vtm3 = null;

            AY.SetDefault(AY.SampleRateDefault, AY.NumberOfChannelsDefault, AY.SampleBitDefault);
            AY.SetSampleRate(WaveOutAPI.SampleRate);
            AY.SetBitRate(WaveOutAPI.SampleBit);
            AY.SetNChans(WaveOutAPI.NumberOfChannels);
            AY.SetBuffers(WaveOutAPI.BufferLengthMs, WaveOutAPI.BufferCount);

            int iter = 0;
            int cnt = 0;

            WaveOutAPI.LoadTrackerModule(fileName, iter, ref cnt, ref vtm, ref vtm2, ref vtm3);
            WaveOutAPI.Initialize();

            AY.ChipCount = cnt;

            AY.ActiveModule = vtm;
            AY.PlayingModule[0] = vtm;
            AY.PlayingModule[1] = vtm2;
            AY.PlayingModule[2] = vtm3;

            for (int i = 0; i < AY.ChipCount; i++)
            {
                VTModule.Module_SetPointer(AY.PlayingModule[i], i);
                VTModule.Module_SetDelay((sbyte)AY.PlayingModule[i].InitialDelay);
                VTModule.Module_SetCurrentPosition(0);
            }

            //WaveOutAPI.InitForAllTypes(true);
            WaveOutAPI.StartWOThread();
            //WaveOutAPI.WOThreadFunc(IntPtr.Zero);

            Console.WriteLine("\nPress any key to exit the process...");
            Console.ReadKey();
        }
    }
}
