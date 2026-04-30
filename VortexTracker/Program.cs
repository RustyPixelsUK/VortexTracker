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

namespace VortexTracker
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.SetDefaultFont(new Font("Segoe UI", 9F));
            ApplicationConfiguration.Initialize();
            WaveOutAPI.Initialize();
            Globals.MainForm = new MainForm();
            Globals.OptionsForm = new OptionsForm(Globals.MainForm);
            Globals.TracksManagerForm = new TracksManagerForm(Globals.MainForm);
            Globals.ExportZXForm = new ExportZXForm(Globals.MainForm);
            Globals.GlobalTransForm = new GlobalTransForm(Globals.MainForm);
            Globals.TurboSoundForm = new TurboSoundForm(Globals.MainForm);
            Globals.ToggleSamplesForm = new ToggleSamplesForm(Globals.MainForm);
            Globals.ProgressBarForm = new ProgressBarForm(Globals.MainForm);
            Globals.ExportWavOptionsForm = new ExportWavOptionsForm(Globals.MainForm);
            Globals.TrackInfoForm = new TrackInfoForm(Globals.MainForm);
            Globals.PluginManagerForm = new PluginManagerForm(Globals.MainForm);

            Globals.MainForm.CheckCommandLine();
            Application.Run(Globals.MainForm);
            WaveOutAPI.Shutdown();
        }
    }
}