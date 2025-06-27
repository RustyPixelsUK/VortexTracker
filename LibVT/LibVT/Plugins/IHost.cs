using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVT.Plugins
{
    public interface IHost
    {
        // Module
        abstract VTM GetCurrentModule();

        // Patterns
        abstract Pattern GetCurrentPattern();

        // Samples
        abstract void PlaySample();
        abstract void RedrawSamples();
        abstract Sample GetCurrentSample();
        abstract Sample GetPreviousSample();
        abstract Sample GetNextSample();

        // Ornaments
        abstract void PlayOrnament();
        abstract void RedrawOrnaments();
        abstract Ornament GetCurrentOrnament();
        abstract Ornament GetPreviousOrnament();
        abstract Ornament GetNextOrnament();

        // MIDI
        abstract void SendMidiRegister(int chipIndex, int channel, int register, int value);
        abstract void SendMidiNoteOn(int chipIndex, int channel, int note, int velocity);
        abstract void SendMidiNoteOff(int chipIndex, int channel, int note, int velocity);
        abstract void SendMidiPitchBend(int chipIndex, int channel, int value);
        abstract void SendMidiNoteOn(int channel, int note, byte velocity);
        abstract void SendMidiNoteOff(int channel, int note);
        abstract void SendMidiPitchBend(int channel, int bendValue);
        abstract void SendMidiCC(int channel, int controller, byte value);
    }
}
