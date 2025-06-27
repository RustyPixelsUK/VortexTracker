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