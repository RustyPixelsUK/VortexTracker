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
using LibVT.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker
{
    public class PluginHost : IHost
    {
        public VTM GetCurrentModule()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            return activeForm.VTM;
        }

        public Pattern GetCurrentPattern()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            return activeForm.VTM.Patterns[activeForm.PatternIndex];
        }

        public void PlaySample()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.SamplePreview();
        }

        public void RedrawSamples()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.Samples.Redraw();
        }

        public Sample GetCurrentSample()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            return activeForm.VTM.Samples[activeForm.SampleIndex];
        }

        public Sample GetPreviousSample()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.ChangeSample(activeForm.SampleIndex - 1, true, true);
            return activeForm.VTM.Samples[activeForm.SampleIndex];
        }

        public Sample GetNextSample()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.ChangeSample(activeForm.SampleIndex + 1, true, true);
            return activeForm.VTM.Samples[activeForm.SampleIndex];
        }

        public void PlayOrnament()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.OrnamentPreview();
        }

        public void RedrawOrnaments()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.Ornaments.Redraw();
        }

        public Ornament GetCurrentOrnament()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            return activeForm.VTM.Ornaments[activeForm.OrnamentIndex];
        }

        public Ornament GetPreviousOrnament()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.ChangeOrnament(activeForm.OrnamentIndex - 1, true, true);
            return activeForm.VTM.Ornaments[activeForm.OrnamentIndex];
        }

        public Ornament GetNextOrnament()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.ChangeOrnament(activeForm.OrnamentIndex + 1, true, true);
            return activeForm.VTM.Ornaments[activeForm.OrnamentIndex];
        }

        public void SendMidiRegister(int chipIndex, int channel, int register, int value)
        {
            MidiOut.SendMidiRegister(chipIndex, channel, register, value);
        }

        public void SendMidiNoteOn(int chipIndex, int channel, int note, int velocity)
        {
            MidiOut.SendMidiNoteOn(chipIndex, channel, note, velocity);
        }

        public void SendMidiNoteOff(int chipIndex, int channel, int note, int velocity)
        {
            MidiOut.SendMidiNoteOff(chipIndex, channel, note, velocity);
        }

        public void SendMidiPitchBend(int chipIndex, int channel, int value)
        {
            MidiOut.SendMidiPitchBend(chipIndex, channel, value);
        }

        public void SendMidiNoteOn(int channel, int note, byte velocity)
        {
            MidiOut.SendMidiNoteOn(channel, note, velocity);
        }

        public void SendMidiNoteOff(int channel, int note)
        {
            MidiOut.SendMidiNoteOff(channel, note);
        }

        public void SendMidiPitchBend(int channel, int bendValue)
        {
            MidiOut.SendMidiPitchBend(channel, bendValue);
        }

        public void SendMidiCC(int channel, int controller, byte value)
        {
            MidiOut.SendMidiCC(channel, controller, value);
        }
    }
}
