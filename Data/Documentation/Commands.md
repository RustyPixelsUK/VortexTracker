# Vortex Tracker Commands

## Instruments Column

This column consists of four options (not necessarily in HEX, at least for the first one):

- **Sample number**  
- **Envelope type** – Usually either:
  - `A` for triangle
  - `C` for harsh envelope
  - `F` to disable envelopes for this channel  
- **Ornament number** – Responsible for arpeggios  
- **Channel volume** – Depends on the volume setting of your sample  

---

## Effects Column

| Code   | Description |
|--------|-------------|
| `1xyz` | Tone slide up |
| `2xyz` | Tone slide down |
| `3xyz` | Portamento (slides from C-1 to the current note) |
| `4xxx` | Sample offset (starts at a specific frame of the sample) |
| `5xxx` | Ornament offset (same as above but for ornaments) |
| `6xyz` | Vibrato (interrupts and replays the note at a specific rate) |
| `9xyz` | Envelope slide up |
| `Axyz` | Envelope slide down |
| `Bxxx` | Speed |

---

## Envelope Amounts by Octave

| Note | Octave 1 | Octave 2 | Octave 3 |
|------|----------|----------|----------|
| C    | D1       | 68       | 34       |
| C#   | C5       | 63       | 31       |
| D    | BA       | 5D       | 2F       |
| D#   | B0       | 58       | 2C       |
| E    | A6       | 53       | 2A       |
| F    | 9D       | 4E       | 27       |
| F#   | 94       | 4A       | 25       |
| G    | 8C       | 46       | 23       |
| G#   | 84       | 42       | 21       |
| A    | 7C       | 3E       | 1F       |
| A#   | 75       | 3B       | 1D       |
| B    | 6F       | 37       | 1C       |
