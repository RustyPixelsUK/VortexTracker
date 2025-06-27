Vortex Tracker II v1.0
(c)2000-2024 S.V.Bulba
Author Sergey Bulba
E-mail: svbulba@gmail.com
Support page: http://bulba.untergrund.net/

History
-------

The Vortex Tracker II idea is based on the Vortex Tracker. I and Roman
Scherbakov have started Vortex Tracker project at summer 2000, and the work was
stopped at autumn 2000 in not finished state. At August 2002 I decided to
restart work alone with new name Vortex Tracker II.

What is it?
-----------

Vortex Tracker II is a complete music editor for creating and editing music for
AY-3-8910, AY-3-8912 or YM2149F sound chips. Sound output is realized by sound
chips emulation through standard Windows digital sound devices, so, real sound
chips are not required. Vortex Tracker II uses standard Win32 functions and does
not require any additional libraries.

Vortex Tracker II can import ZX Spectrum music files (modules) of next types:

 1) Pro Tracker 3.xx (file mask is *.pt3);
 2) Pro Tracker 2.xx (*.pt2);
 3) Pro Tracker 1.xx (*.pt1);
 4) Flash Tracker (*.fls);
 5) Fast Tracker (*.ftc);
 6) Global Tracker 1.x (*.gtr);
 7) Pro Sound Creator 1.xx (*.psc);
 8) compiled  Pro Sound Maker modules (*.psm);
 9) compiled ASC Sound Master 0.xx modules (*.as0);
 10) compiled ASC Sound Master 1.xx modules (*.asc);
 11) non-compiled Sound Tracker modules (*.st1, *.ay);
 12) compiled Sound Tracker and Super Sonic modules (*.stc);
 13) recompiled Sound Tracker (*.st3);
 14) non-compiled Sound Tracker Pro modules (*.stf);
 15) compiled Sound Tracker Pro modules (*.stp);
 16) compiled SQ-Tracker modules (*.sqt);
 17) Amadeus (Fuxoft AY Language) modules (*.fxm, *.ay).

VT II detects module type only by filename extension (mask), and no any
additional checks are performed. These extensions are used in well-known player
called Ay_Emul. Any other extensions are analyzed as text files.

PT v3.7+ modules saved in TS-mode can be imported in VT II too, they are
converted to two single PT3 in two windows. Previous not documented PT v3.6 TS-
modules are imported after user prompt.

Any two tracker modules (except FXM) can be stored in one file with 16 bytes
identifier at the end of file. For text format identifier is not need. After
loading such pair VT II turns on Turbo-Sound mode and aligns both windows
horizontally.

Vortex Tracker II saves result only in one format is Pro Tracker 3.xx (*.pt3).
You can play these files in different players-emulators (the most known is
Windows and Linux emulator Ay_Emul), on real ZX Spectrum in many players (Little
Viewer, Quick Commander, Real Commander, BestView, Pusher, ZXAmp, The Viewer and
so on) or by built-in playing routines. Also you can include YM-sound into your
PC-programs (see Ay_Emul sources, YM-Engine or SquareTone at
http://bulba.untergrund.net/).

During editing you can save work versions of module in text format. It allows to
save all temporary not used ornaments, samples and patterns. Also, text format
is easy editable in any text editor. Of course, text format is only one chance
to save your music, if PT3 saving is not available due size limitations (65536
bytes).

When saving any of the TS-pair modules in Turbo-Sound mode, the second one is
also added to the file. The next order is observed at the same time: the module
created or opened first (its window has a smaller number) is written first.

In fact, Vortex Tracker II is PC version of ZX Spectrum Pro Tracker 3.xx. The
most compatible version is Pro Tracker v3.6x-3.7x of Alone Coder (a.k.a. Dima
Bystrov). Vortex Tracker II is fully compatible with any Pro Tracker v3.5x in
"ProTracker 3.5" mode.

All supported formats are converted to Pro Tracker 3 compatible format,
therefore some information can be lost, because of ZX music formats are very
badly compatible between each other. More information about converting you can
see in 'Tracker limitations.ru.txt' file.

New 3xxx interpretation is supported in last Pro Tracker 3 versions from Alone
Coder (v3.6 and higher).

During editing total time length and current time position are calculated
automatically (for demomakers).

Note: new 3xxx interpretation changes behavior of ASC modules import also.

After launch Vortex Tracker II opens the modules specified in the command line,
feature can be used to configure the editor launching in file shells such as Far
Manager or to configure file associations.

The paths to the last opened or saved modules, patterns, samples and ornaments
are remembered. At the first launch, the Modules (or Modules for test),
Patterns, Samples or Ornaments folders become such, if they are found in the
program directory.

The project creation history can be found in the file History.txt.

Known issues
------------

There are a number of errors/flaws that the author is aware of, and it is not
necessary to send bug reports about.

1. Unused patterns (missing in the Position List) are deleted during saving, as
a result of which the pattern numbers may change, unused samples and ornaments
are also deleted (without renumbering). Save intermediate revisions in text
format to avoid losing a temporarily (possibly accidentally) unused pattern,
sample, or ornament.
2. The table #1 is used from the second note for PSM and SQT files, which leads
to the loss of notes #5E and #5F of the original module (they turn into B-8).
3. Some settings cannot be changed during playback, or when the audio output
device is busy (during editing). Click the 'Stop playing/Free device' button.
4. Some settings take effect after a period of time equal to the total length of
the buffers.
5. Some settings take effect immediately, but with a small click associated with
the temporary suspension of playback.
6. Playback in Loop all mode does not move to the next module if any modal
window was opened (for example, Options or About Vortex Tracker II).
7. Configuring the main window toolbar by the pop-up menu hides the elements,
but returning them back can happen in an unexpected place. Restarting the
application helps.
8. Considerable time is spent on changing the channel allocation with a large
number of open modules due to the rejection of MDI. This is due to the fact that
rearranging the buttons in places leads to iterate the parent window all
elements.
9. The editable modules themselves are not windows, so, unlike for MDI child
windows, there is no system optimization, and the MDI simulation works slowly,
which is also especially noticeable with a large number of open at the same time
modules: slow redrawing when changing the size and position of elements/modules.

Short manual
------------

You can override default hot keys via config file edition or in the Options
dialog.

Next keys are used by default:

 Pattern editor keys
 ~~~~~~~~~~~~~~~~~~~

1. In a note cell

1.1. NoteKeys:

                          Q 2 W 3 E R 5 T 6 Y 7 U I 9 O 0 P [ = ]
  Z S X D C V G B H N J M , L . ; /

  | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | |
  | C#| D#| | F#| G#| A#| | C#| D#| | F#| G#| A#| | C#| D#| | F#|
  C   D   E F   G   A   B C   D   E F   G   A   B C   D   E F   G

So fortepiano keys are emulated with range more than two octaves.

1.2. Shift+NoteKeys - input note one octave higher.

1.3. Shift+Ctrl+NoteKeys - input note one octave lower (useful for Envelope as
note mode to enter 0th octave notes).

1.4. A - for sound off in channel command (R--).

1.5. K - for delete note (---).

1.6. From 1 to 8 at NumPad - choose current octave number.

During inputting numbers from NumPad NumLock must be turned on.

2. Pattern navigation (cursor control)

2.1. Up, Down, Left, Right - move cursor in four directions.

2.2. Alt+Up, Alt+Down - move the cursor up or down by the absolute value of the
autostep.

2.3. PageUp, PageDown - move cursor to page up or down.

2.4. Home, End - move cursor to begin or end of line.

2.5. Ctrl+Right, Ctrl+Left - quick cursor moving among columns.

2.6. Ctrl+PageUp, Ctrl+PageDown - move cursor to top or bottom of column.

2.7. Ctrl+Home, Ctrl+End - move to top of first or bottom of last column.

2.8. F9-F12 - quick jump to the beginning, first, second and third quarters.

2.9. Mouse clicks - move cursor to desired cell.

2.10.Mouse wheel - scroll pattern up/down.

3. Pattern's area selection

3.1. Shift+cursor control keys (i.2.) - select rectangular area of pattern.

3.2. Ctrl+A, Ctrl+5 on NumPad - select all pattern.

3.3. Moving mouse with pushed left button.

3.4. Shift+mouse click - select from cursor to desired cell.

3.5. Shift+mouse wheel - not reset/add to selection.

4. Jumps from edit pattern to other objects

4.1. Tab - jump from one window control to other in predefined order.

4.2. Shift+Tab - same in reverse order.

4.3. ` (back apostrophe, usually over Tab key) - quick switch between pattern
and position list editors.

4.4. Double-clicking on a non-zero sample or ornament - proceeds to
viewing/editing the corresponding sample/ornament.

5. Deleting

5.1. BackSpace - delete current track line (with moving).
                                                          
5.2. Ctrl+BackSpace, Ctrl+Y - delete current pattern line (with moving).

5.3. Delete - clear selection's values or value at cursor position.

5.4. Ctrl+Delete - clear pattern line values.

5.5. Numpad / - Shrink pattern command: delete each 2nd pattern line with moving
and decreasing pattern size.

6. Inserting empty lines

6.1. Insert - insert empty track line (with moving).

6.2. Ctrl+I - insert empty pattern line (with moving).

6.3. Numpad * - Expand pattern command: double pattern size with inserting empty
pattern lines between old lines.

7. Working with selection

7.1. Ctrl+C, Ctrl+Insert - copy selection to clipboard.

7.2. Ctrl+X, Shift+Delete - same with clearing selection values.

7.3. Ctrl+V, Shift+Insert - inserting from clipboard (to right and to down from
cursor or from upper left corner of selection). In case of selection is defined,
insertion not exceeds its bounds.

7.4. Ctrl+Alt+V - same as usual inserting from clipboard, but without replacing
non-empty values with empty ones (i.e. merging).

7.5. Numpad + and Numpad - - transpose 1 semitone up and down.

7.6. Ctrl+Numpad + and Ctrl+Numpad - - transpose 1 octave up and down.

7.7. To transpose 3 or 5 semitones up or down, you can use the pop-up menu or
set the hotkeys in the Options window.

7.8. Alt+Left, Alt+Right - swap the current and left/right channels. Before
calling, it is enough to put the cursor in one of the channels (if the cursor is
in the envelope or noise period tracks, it is assumed that the leftmost channel
is selected), or select a lines range. In the first case, the notes with the
parameters of the current line will be swapped, in the second one of the
selected lines range.

8. Test playing of pattern's part

8.1. Any pushed key during inputting data - play current line.

8.2. Pushed Enter key - play pattern from current cursor position until key is
pushed. If any looping is on, pattern will be playing cyclically.

9. Other

9.1. 0 on NumPad - on/off Auto Envelope.

9.2. Space - on/off AutoStep.

9.3. Ctrl+0..9 - quick setting of the autostep value. The AutoStep mode is also
enabling.

9.4. Shift+Backspace - if AutoStep is on do step backward.

Right mouse button can call pop-up menu, which duplicates some key combinations
(useful if you are using keyboard without Numpad, like in notebooks). In
addition, you can access commands that do not have hotkeys set by default via
this menu:

 Split pattern. You need to put the cursor in the desired pattern line before
 calling it. As a result, the current pattern size will decrease to this line
 and a second one with the remaining lines will be created. If the split pattern
 was not in the positions list, then the second one will simply be displayed.
 Otherwise, all instances found in the positions list will be replaced with new
 pairs of patterns.

 Pack pattern. The command works either with the selection area lines, or with
 the entire pattern, if there is no selection. The command deletes the empty
 lines and places (or updates) the Tempo setting commands in the remaining ones
 so that the sound of neither this pattern nor following it in the positions
 list does not change. To calculate the initial Tempo of the selected lines
 block, a reverse search is performed for the last previously set Tempo change
 special command. If it is not in the current pattern, then the previous
 patterns in the positions list are viewed. If this command does not appear in
 them either, or if an arbitrary pattern is edited (that is, it does not match
 the current one in position list), then the module initial speed is taken as
 the initial speed.

 Common keys
 ~~~~~~~~~~~

1. Play controls

1.1. F5 - play module from current position.

1.2. F6 - play module from start.

1.3. F7 - play current pattern from current line.

1.4. F8 - play current pattern from start.

1.5. Esc - stop playing and go to edit pattern; also use to free sound device.

1.6. Ctrl+L - module and pattern loop playing on/off.

1.7. Ctrl+Alt+L - loop playing among all opened modules on/off. If only one
module is opened or pattern is playing, it works as usual loop.

1.8. The rotation of the mouse wheel adjusts the volume (it works if the mouse
cursor is not pointed at controls that also respond to this rotation).

2. Sound chip emulation

2.1. Ctrl+Alt+C - toggle chip type (AY-3-8910/12 or YM2149F).

2.2. Ctrl+Alt+A - selects the channel layout from a user-defined list (by
default Mono, ABC, ACB or BAC). Other layouts are selected in the Options
dialog, and the list can also be configured there.

3. Editing

3.1. Ctrl+E - toggle autoenvelope mode to autocalculating envelope period by its
type and current note.

3.2. Ctrl+R - toggle autostep (edit spacing).

3.3. Ctrl+T - call/close Track Manager dialog.

3.4. Ctrl+Alt+T - call/close Global Transposition dialog.

3.5. Ctrl+M - call/close the Toggle Samples tool. You can use it to mute
individual samples or see which samples are not used in the module (the
checkboxes are disabled). This information is updated during editing and when
switching windows.

3.6. Up/Down in the text fields with arrows to increase or decrease the value by
one.

3.7. PageUp/PageDown in the pattern length text field to increase or decrease
the value by the amount of the highlighting step.

3.8. Ctrl+P - toggle auto fill parameters from test line when imputting note in
pattern editor.

3.9. Alt+Digits - set corresponding octave number in patterns editor and in all
test lines simultaneously.

4. Standard keys

4.1. Alt+F4 - close Vortex Tracker II.

4.2. Ctrl+F4, Ctrl+W - close active window with module.

4.3. Ctrl+F6, Ctrl+Tab - cyclic choosing of opened modules.

4.4. Ctrl+Shift+F6, Ctrl+Shift+Tab - cyclic choosing of modules in the reverse order.

4.5. Ctrl+O, double-click on the main window background - call open dialog.

4.6. Ctrl+S - save module.

4.7. Tab, Shift+Tab - cyclical jumps between window controls (forward and
backward).

4.8. Alt+BackSpace - undo last change.

4.9. Alt+Enter - redo last change.

 Positions list editor keys
 ~~~~~~~~~~~~~~~~~~~~~~~~~~

The keys can be redefined (or set, if no by default) in the Options dialog. The
following are used by default:

1. ` - jump to/from pattern editor.

2. Left, Right or left mouse button - select position (during playback selected
position will restart playing).

3. Home - jump to 1st position.

4. End - jump to last position if playback, to the first empty cell otherwise.

5. Shift+cursor control (items 2-4) - select cells range.

6. Ctrl+A, Ctrl+Num5 - select all positions.

7. Right mouse button - select position (if clicked non-selected cell) and call
pop-up menu with commands.

8. Ctrl+C - copy selected cells range to the clipboard.

9. Ctrl+X - cut to the clipboard (just copy and call Delete positions command).

10. Ctrl+V - paste from the clipboard previously copied or cutted fragment. The
positions will move apart at the paste point. If the paste is performed outside
the filled cells, then the list will just be extended from the end.

11. Ctrl+Alt+V - merging the contents of the clipboard (pasting inside the
selection if more than one cell is selected, or starting from the selected cell
to the next ones, without moving apart). If the last filled cells are selected,
the merging may go beyond the selection (the length of the list will increase).

12. L - set loop position.

13. From 0 to 9 - enter pattern number for the selected position.

14. Delete - Delete positions command: delete selected positions with moving.

15. Insert - Add positions command. New patterns are created in the required
amount. If one or more non-empty positions are selected, the same number of
patterns are inserted immediately after the selection with a moving apart. If an
empty cell is selected, then it and the empty cells preceding it are filled with
new patterns. The length of the new patterns corresponds to the length of the
pattern before the place where new positions were added, or 64 if the list was
empty.

16. Duplicate positions command. Creates a corresponding number positions with
the same patterns immediately after the selected ones with moving apart.

17. Clone positions command. It works similarly to the Duplicate positions
command, but fills positions with pre-created patterns with the same content as
the original ones.

18. 'Change patterns length...' command. It is intended for group resizing of
patterns: the size entered in the query dialog will be applied to all patterns
in the selected positions.

19. Renumber patterns command. Assigns new numbers to patterns in the order of
use in the positions list.

 The sample editor keys
 ~~~~~~~~~~~~~~~~~~~~~~

Up, Down, Left, Right, PageUp, PageDown, Home, End, Ctrl+Right, Ctrl+Left,
Ctrl+PageUp, Ctrl+PageDown, Ctrl+Home, Ctrl+End for navigation.

Shift+navigation keys described above for area selection.

In any position of editing sample:

T - toggle on/off tone mask
N - toggle on/off noise mask
M - toggle on/off envelope mask
` - jump to/from test line.

In 'TNE' columns:
Space - toggle on/off corresponding mask

In any '+' and '-' columns:
Space - toggle sign
Shift+'=', '=', Numpad '+' - change sign to '+'
'-', Numpad '-' - change sign to '-'
Shift+6 ('^') - turning on accumulation in corresponding column '^'
Shift+'-' ('_') - turning off accumulation in corresponding column '_'
0-9,A-F - enter hexadecimal numbers

In any '^' and '_' columns:
Space - on/off accumulation
Shift+6 ('^') - turning on accumulation '^'
Shift+'-' ('_') - turning off accumulation '_'
0-9,A-F - enter hexadecimal numbers

In last column (volume control) '+', '-' and '_':
Space - toggle three variants
Shift+'-' ('_') - don't change sample volume '_'
'-',  Numpad '-' - decrease sample volume by one '-'
'+',  Numpad '+' - increase sample volume by one '+'
0-9,A-F - enter hexadecimal numbers

In the number fields:
0-9,A-F - enter hexadecimal numbers
Space - change sign, in amplitude column - volume control
Shift+'=', '=', Numpad '+' - change sign to '+'
'-', Numpad '-' - change sign to '-'
Shift+6 ('^') - turning on accumulation '^'
Shift+'-' ('_') - turning off accumulation '_'

Any non-digital key or mouse clicking reset number inputting counter.

In the tone shifts as notes mode (shifts relative to the base note, which is set
in the sample test line, switching modes is done with the C# button), editing
the tone shifts column can be done using the note/octaves input keys, including
MIDI keyboard; in the latter case, the cursor can be positioned not only in the
tone shift input field.

By default, right mouse button (RMB) clicking in amplitude visualization field
('*' symbols) to choose corresponding amplitude. You can change it to the left
mouse button (LMB) in the Oprions dialog.

Moving mouse with clicked same mouse button in amplitude visualization field to
draw amplitude.

Middle mouse button (MMB) click in some sample cells to toggle corresponding
value.

Right-clicking on the selection opens a pop-up menu.

Shift+mouse click to select area from cursor to click point.

Ctrl+C/Ctrl+X to copy/cut the selected fragment to the clipboard.

Ctrl+V to paste from the clipboard from the cursor position or inside the
selection. Heterogeneous data can be inserted, for example, tone deviation signs
or values can be inserted into noise/envelope deviation signs or values.

If there is a pattern fragment in the clipboard, then it can also be inserted
into the sample, while rendering is performed: generating a new sample that will
sound like the original pattern fragment.

Delete to clear the selected area or what is under the cursor.

BackSpace, Ctrl+Y to delete the sample's current line with a shift.

Insert, Ctrl+I to insert an empty line to the sample with a shift.

 The ornament editor keys
 ~~~~~~~~~~~~~~~~~~~~~~~~

Up, Down, Left, Right, PageUp, PageDown, Home, End, Ctrl+PageUp, Ctrl+PageDown,
Ctrl+Home, Ctrl+End for navigation.

Shift+navigation keys described above for area selection.

` - jump to/from test line.

0-9 - input decimal numbers

Space - toggle number sign

Shift+'=', Numpad '+' - set '+' sign

'-', Numpad '-' - set '-' sign

In the ornament as notes mode (shifts relative to the base note, which is set in
the ornament test line, switching modes is done with the C# button), values are
entered using the note/octaves input keys or from a MIDI keyboard.

Use middle mouse click to toggle sign of corresponding value.

Right-clicking on the selection opens a pop-up menu.

Shift+mouse click to select area from cursor to click point.

Ctrl+C/Ctrl+X to copy/cut the selected fragment to the clipboard.

Ctrl+V to paste from the clipboard from the cursor position or inside the
selection.

Delete to clear the selected area or what is under the cursor.

BackSpace, Ctrl+Y to delete the ornament's current tick with a shift.

Insert, Ctrl+I to insert an zero tick to the ornament with a shift.

 Plugin for editing an ornament
 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

If orgen.exe file is available, you can click the OrGen button to launch the
orgen plugin by Shiru Otaku. Use the plugin to edit ornaments in an alternative
way.

 Test fields on patterns, sample and ornament tabsheets
 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

There are test lines for quick checking the sound of a given combination of
parameters (note, sample, envelopes, etc.) on the patterns, samples and
ornaments tabsheets.

Navigation and editing same as in patterns editor. Selecting, copying, pasting
from the clipboard also works (you can copy data from the pattern to the test
line and vice versa). Pressing ` (reverse apostrophe) leads to the transition to
the editing fields of the pattern, sample or ornament (depending on the selected
tabsheet).

You can find a group of Auto Parameters buttons above test line on the patterns
tabsheet, with which you can control the automatic filling of parameters when
you enter a note in the fields of the pattern editor. For example, if the S and
C buttons are pressed (sample and special command), then the sample number and
special command with its parameters will be auto set for a note, when you enter
it in any channel.

On the samples tabsheet, there is a button "Do recalcing sample tone shifts on
changing base note" before the test line. If it is pressed, then each time a
sample test line note is changed, the tone offsets will be recalculated so that
the final sample pitch relative to this new note does not change. In the Tone
shifts as a note mode, the result of the constant sample pitch can not only be
heard, but also seen.

 Other elements to edit
 ~~~~~~~~~~~~~~~~~~~~~~

Title and author strings maximum length is 32 chars.

Highlight step for pattern lines can be adjusted in patterns sheet. If AutoHlt
is on, step is autoselected from 3, 4 and 5 depending of pattern length. The
size of the highlighting step is also used in the text field of the pattern
length editor with the PageUp and PageDown keys.

On the samples and ornaments tabsheet, the highlight step is determined by the
module's initial playback speed. If the highlighting is not appropriate, it can
be turned off in the settings (turned off by default).

By clicking on the Unroll button of the samples' tab, you can unroll the
accumulations in the sample. A new sample will be generated after pushing, the
length of which may differ from the original one, and if all the lines sound the
same inside the last unrolled loop, then it is reduced to one line (usually
silent after the amplitude accumulations was unrolled). The algorithm can finish
work early if the new sample has reached the maximum length allowed in PT3.
Unfortunately, when unrolling envelope slides, the algorithm quickly runs into
the limitations of the PT3 format (from -16 to +15), while the tone and noise
slides unrolled completely.

Save command available only after starting editing of song.

 Using a MIDI keyboard to enter notes and their volumes
 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The MIDI keyboard is controlled by the toolbar buttons.

You can going over the connected keyboards with the MIDI button (shortcut is
Ctrl+Alt+M the default). The first one proposed by the system (or previously
stored in the configuration file) is connected automatically. With this button,
you can disable both the already connected keyboard and the connection standby
mode at all. Information about which keyboard is connected, as well as about
whether standby mode is enabled, is displayed in the status bar at the bottom of
the main window when the cursor hovers over this button.

The V button is designed to control the volume inputting from the MIDI keyboard:
if it is in pushed state, the pressing key velocity will be converted to values
from 1 to F of the volume column of the track or test line.

The current MIDI configuration (including the device name) is saved to the
configuration file when the application is closed.

If tracks are active, then the input of notes and volumes from the MIDI keyboard
occurs in them, if the samples' or ornaments' tab is active, then the input
occurs in the corresponding test lines (or directly into the active
sample/ornament, if the tone shifts as notes mode is enabled), otherwise this
data is enterring into the test line of the patterns' tab. By default, volume
input is disabled (only notes are entered). The cursor does not have to be in
the note input field, it is enough that it is located somewhere inside the
desired channel or the envelope period track.

 Notes on the capabilities of PT3 editors of different versions
 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

You can load VT II modules into ZX Spectrum Pro Tracker 3.69x-3.7x. It can load
modules with VT II header and with *.pt3 file extension.

For playing module by standard ZX Spectrum playing routine don't select song
speed smaller than 2 (3 for old players), or better use VT II built-in player
without such limitations.

Header types
------------

Current version can save three header types in PT3-file.

1) 'Vortex Tracker II 1.0 module: '

This header means, that module can contain all VT II abilities, including new
3xxx interpretation. However, module can be played with old PT3-players without
any problems in most cases.

2) 'ProTracker 3.6 compilation of '

This header means same abilities, as previous one. It need for some players,
which require old header style.

3) 'ProTracker 3.7 compilation of '

In addition to PT 3.6 abilities, module can contain glissando commands 1.xx and
2.xx (timedelta=0). In this case glissando not works as usually, instead single
tone frequency changing by xx down or up is performed.

4) 'ProTracker 3.5 compilation of '

This header means, that module must be played with old 3xxx command
interpretation and 1.xx and 2.xx special commands must be ignored.

AutoStep using
--------------

During editing tracks step of autoscrolling can be set. It works after the most
typical operations: typing note, sample, numbers, inserting/deleting/clearing
lines and so on. Tracks can be autoscrolled up (positive step) and down
(negative step). To fast switch autostep option use Space or Ctrl+R, and also
quick turn on the desired step with the Ctrl+0..9 keys. The default step for new
modules is 1 (you can configure it in the Options dialog).

You can use this feature for unusual tasks, like inserting same data from
clipboard with given step, or fast changing patterns size in 2 times (both
increasing and decreasing). In last case just set step to 2 or 1 and use Ctrl+I
or Ctrl+BackSpace several times (for last better use Shrink/Expand commands of
popup menu).

Sometimes user does not remember that AutoStep is on, so, he can press
Shift+Backspace to do step backward.

You can also use the keys Alt+Up and Alt+Down to move the cursor up or down by
the autostep absolute value (works even when the autostep is disabled).

Turbo-Sound mode
----------------

From the end of 90s some people tried to popularize standards of two sound chips
connection to ZX Spectrum. Known schemes are Quadro-AY, Turbo-AY and
Turbo-Sound. One of way to use it is to play two different modules
simultaneously (each through own chip). Vortex Tracker II allows playing any two
opened modules simultaneously. Active window module sounds through the first
sound chip, and module selected in list of opened modules through the second
chip (call list by pushing corresponding button of module window). By default,
second chip is off ("2nd soundchip is disabled" button label appears).

For more usability VT II synchronizes tracks of two modules in both play and
edit tracks modes (including cursors position), activates 2nd window after
reaching tracks editor cursor right or left position, during saving any of
TS-pair module, 2nd is added to result file (firstly created saved first in the
file), during loading TS-modules (including special format from PT v3.6+)
creates TS-pair and aligns it horizontally. Also, the resize/close of any window
in pair leads to same actions in second one.

Examples of playing on ZX can be found in ZX-magazine InfoGuide #8.

During exporting to ZX in TS-mode used special TS-player for ZX Spectrum.

File->Options... menu
---------------------

Calls the Options window (you can also call by clicking on the corresponding
button on the panel). All settings are applied immediately, but at any time you
can click the Cancel button (or just close the window) and return to the
settings that were before the Options window was called. In order for the
settings to be remembered, click OK.

'Design' tab sheet

By clicking on the 'Design Items' fields, you can customize the fonts and colors
of the edited pattern, sample, ornament or their testing lines items (be guided
by the hints that appear when you hover the cursor). The font selection dialog
displays only monospaced fonts such as Courier, Courier New, Fyxedsys, Lucida
Console. Due to the fact that all characters in such fonts have the same size in
width, it becomes possible to fast print tracks and to calculate coordinates for
displaying the cursor and selection.

Here you can also configure the initial ones: the number of visible lines of the
track (from 3 to 64), the number of the note table, auto step value, the
decimal/hexadecimal system for displaying line numbers and noise periods columns
in the pattern, as well as modes for displaying tone shifts (in the sample and
ornament) and the envelope periods (in the tracks) as notes. In the future,
these parameters can be changed individually for each window, while when the
window size changes, the number of lines in tracks, samples and ornaments is
synchronously recalculated.

'Chip emulation' tab sheet

Choose chip type, chip and interrupt frequency, channels allocation (both
hearable and visual), and one of resample algorithm. Some changes can be heard
after time shown in bottom of tab sheet. Some musicians use tricks, which can
sound rightly only if FIR-filter is selected (checked by default). Of course,
FIR-filter requires more processor time. So, if your system cannot produce solid
sound, select Averager or decrease bitrate or sample rate at 'Digital Sound'
tabsheet. In Turbo-Sound mode used identical setting for both chips.

'Compatibility' tab sheet

These are global compatibility options. If you need to adjust only current
module, see corresponding tab sheet on its window.

 Features level

  - Pro Tracker 3.5 - old behavior of 3xxx command. Also ASC modules will be
       imported for playing with old PT3 players.
  - Vortex Tracker II (PT 3.6) - new 3xxx command interpretation. It affects to
       playing and ASC modules import.
  - Pro Tracker 3.7 - allows using of 1.xx and 2.xx special commands.
  - Try detect - allow VT to detect it. For PT3 are used header analyzing (see
       rules above). For PT2 is used 'Pro Tracker 3.5'. For all other - 'Vortex
       Tracker II (PT 3.6)'.

 Save with header

  You can recommend to VT II save one of header type. Anyway, VT II uses known
  rules (see 'Header types').

'Digital sound' tab sheet

This is wave out sound options. All options are not available during playing. To
stop playing press corresponding button of the sheet.

 Sample rate

  Samples frequency, more value for more quality. If some frequency is not
  supported by sound card can be error messages or sound quality will be worse.

 Bit rate

  Sample size, more value for more quality.

 Channels

  Mono or Stereo.

 Device

  'Wave mapper' as default.

 Buffers

  Buffer size and number of buffers. Try to find optimal values for your system.
  Smaller buffer size for quicker reaction. More buffers for stable sound. Total
  length of buffers are calculated at low side of group. Extremely small buffers
  (even stable-sounding ones) can cause tracks to twitch when redrawing, because
  the visualization buffer contains even fewer ticks.

'Keys' tab sheet

You can override almost all hotkeys, or set them for those actions for which
they were not defined by default. Also here, almost any key on the keyboard can
be associated with any note or octave number. Select the desired category, then
the desired action in the table and click Define shortcut or Delete shortcut.

Conflicts are checked immediately after capturing a new combination (some note
keys or their combinations with Shift or Ctrl+Shift may be occupied by other
hotkeys, as well as some global combinations may overlap with local ones). If
the matches are critical, the confirmation dialog suggests deleting them,
otherwise you can simply ignore the match and add it anyway.

You can restore the default values at any time (the Default for all button) or
press the global Cancel button to abort the changes and close the Options
window.

'Other' tab sheet

 Application priority

  Select normal or high priority.

 Language

  Select the application interface language. The "auto/en" option selects the
  system language (by default). You can either enter your language's <id>, or
  select from the list. In the 'languages' folder, you can find files for
  translating standard messages into 23 languages. Also in this folder you can
  find the VT.pot template, which can be used to translate the interface into
  the language you need. It is enough to rename it to 'VT.<id>.po', and edit it
  in a text editor or in any PO file editor. If you want your translation to be
  included in the Vortex Tracker II distribution, send it to the author of
  Vortex Tracker II.

 Recalculate envelope periods on note table change

  This option allows automatic envelope periods recalculation when changing the
  note table number. Envelopes are recalculated according to the converting
  tables of periods to notes and back (they are also used in the displaying
  envelope periods as notes mode), so if the period is greater than #00FF or
  lower than C-0, or is not used with a cyclic envelope type (8, A, C or E), it
  is not recalculated.

 Accept MIDI messages in background

  Allows you to work with the MIDI keyboard in the background (that is, when the
  application is not active or minimized on the taskbar, in the latter case, the
  input from the pattern editor switches to the test line). This option will be
  useful when using virtual MIDI keyboards that take up focus. However, at the
  same time, several Vortex Tracker II applications running simultaneously will
  also simultaneously process the same MIDI keyboard messages. This option is
  disabled by default.

 Show pop-up hint for tracks/sample/ornament editor

  Three checkboxes allowing to display hints for the tracks, sample and ornament
  editors not only in the status bar at the bottom of the main window, but also
  as a pop-up window. These hint windows are bulky, so the options are disabled
  by default.

 Highlight lines in sample and ornament editors

  Turn on the lines highlighting in the ornament and sample editors with a step
  of the playback speed (Tempo).

 Don't ask when operation cannot be undo

  Disable warnings about the inability to undo the operation. Operations such as
  transposing the entire module or Swap Channels in Tracks Manager will be
  performed without additional confirmation from the user. In order not to make
  mistakes, follow the hints that are specially added for such cases (both in
  the status bar and pop-up).

 Use left mouse button (LMB) to draw amplitude

  By default, the amplitude is drawn in the right area of the sample editor with
  the right mouse button (to avoid accidental editing when working with the left
  area).
  
Tracks manager
--------------

To call press Ctrl+T or choose corresponding Edit menu option. You can copy any
part of any pattern to any place of any pattern.

In Location 1 and Location 2 group adjust pattern number, first line number and
channel number. In Area group set number of lines, and if you need check noise
and envelope tracks.

To copy one location to another simply press corresponding button in Copy group.
Also you can move or swap locations (see Move and Swap groups).

Also you can transpose any location to desired number of semitones (see
Transposition group). Positive values for up and negative for down. If you check
Envelope track (Area group), it'll be transposed too.

The Move and Swap operations cannot be undo, so the corresponding request is
shown before they are performed. It can be disabled in the settings. In order
not to make a mistake, follow the hints (in the status bar as well as the
pop-up).

Global Transposition
--------------------

To call press Ctrl+Alt+T or choose corresponding Edit menu option. You can
transpose one or more tracks of whole module or of selected pattern. This dialog
allows avoid multiple using of Tracks manager. Adjusting and using same as in
Tracks manager.

The entire module transposition operation cannot be undo, therefore, by analogy
with Tracks manager, a warning request appears, which can be disabled in the
settings.

Menu File->Save and Files->Save as...
-------------------------------------

In appeared save dialog select in dropdown list file type for saving module:
either work text format (TXT) or Pro Tracker 3 (PT3) for final compilation.
During saving PT3 format, VT II removes all not used samples, ornaments and
patterns.

Menu File->Exports->Save in SNDH (Atari ST)
-------------------------------------------

Saving in SNDH format for playing on Atari ST (or in SNDH-players and emulators
of Atari ST). There is universal MC68000 player used in SNDH, it supports all
note and volume tables (starting from PT 3.3), PT3 module version is analyzed
during initialization. Player is based on Ay_Emul procedures, volume and note
tables are packed by Ivan Roshin's method. Player size is about 9 Kb; start
address is not fixed. Dialog is appeared before saving, where you can input year
of creation (or leave it empty), and also here you can disable or enable the
ability to pack SNDH with PACKICE.

Menu File->Exports->Save with ZX Spectrum player
------------------------------------------------

Saving with ZX Spectrum player. Supported formats: HOBETA with player ($c),
HOBETA without player ($m), .AY of EMUL subtype, SCL and TAP. .AY-format not
allows using 0 address. Player can be adjusted: you can disable looping of
module, check 'Disable loop' for that. In SCL and TAP formats, player and module
both are saved separately (in two different files). It is better than HOBETA,
because variables area between player and module is not saved. Player features
and instructions can be found in ZXPlayer.txt file. Source text of player can be
found in archive with VTII sources, and also at http://bulba.untergrund.net/

If the exported module is associated with another (Turbo-Sound mode), then both
are exported, while the same order of modules in the file is observed as with
normal saving, and a special TS player is used, about the capabilities of which
you can read in the file ZXTSPlayer.txt.

Μενώ File->Exports->Convert to PSG...
-------------------------------------

Allows you to convert the module into a register stream in PSG format. PSG files
for ordinary modules are the same as when converting to Ay_Emul, and 2 PSG files
are saved for TS modules (Ay_Emul can't doing this yet). The idea is taken from
VT II v2.6. Search in the menu File-> Exports->Convert to PSG... Unlike Ay_Emul,
playback does not stop when converting to PSG. The T/N/E mute buttons and the
Toggle samples checkboxes are also ignored.

Μενώ File->Exports->Convert to WAV...
-------------------------------------

You can generate a WAV file with the parameters set in the Options window for
emulation and digital audio. The conversion process can be stopped by pressing
Esc or the cancel button next to the progress bar that appears. The current
playback does not stop during the conversion, and the functionality of the
application is partially preserved (visualization, hints, volume control with
the mouse wheel, switching to the next window playback, etc.).

Configuration file
------------------

At startup, Vortex Tracker II searches for the VT.cfg configuration file first
in the program folder, and if it does not find it, in the AppData folder of the
current user (usually C:\Users \<user name>\AppData\Local\Vortex Tracker II),
and automatically creates it at a second path with default settings if its
absent.

You can move the VT.cfg file to the program folder (or create an empty one
there) if for some reason you don't want it to be in AppData. It can also be
protected from changes (for example, by setting the Read Only attribute or
editing access rights in the file system), the changes made in the editor
settings will not be saved at the exit of the program in this case.

Thanks to
---------

- Roman Scherbakov (a.k.a. V_Soft) for idea and picture.
- Konstantin Yeliseyev (a.k.a. Hacker KAY) for AY and YM level tables.
- Dmitry Bystrov (a.k.a. Alone Coder) for information about Pro Tracker 3.xx.
- Roman Petrov (a.k.a. Megus) for ideas about ideal AY tracker.
- Macros for testing, test files, wishes about interface and for support.
- Shiru Otaku for plug-in, testing, wishes and bug-reports.
- Polaris for wishes and test modules.
- Black Groove (a.k.a. Key-Jee) for bug-reports, wishes and test modules.
- Ilya Abrosimov (a.k.a. EA) for bug-reports and wishes.
- Pavel A. Sukhodolsky for help and formats discussion.
- Asi for bugreports and wishes.
- Denis Seleznev for icon pictures.
- Spectre for help in debugging and wishes about ZX PT3-player.
- Ivan Roshin for help in writing new ZX PT3-player.
- Jecktor for adapting PT3-player sources to XAS.
- HalfElf for using in xLook Far Manager plug-in.
- Karbofos for testing, suggestions and test modules.
- Ch41ns4w for wishes about TS-mode and about design.
- Znahar for another branch of VT II with good ideas.
- TAD for suggestions, bug-reports and test modules.
- MMCM for suggestions.
- Nik-O for suggestions.
- To Ivan Pirog and the Vortex Tracker II v2.6 team for ideas and instruments
collection.
- To Alexander Boolba for closing windows icon, some functions code and ideas.
- To Benjamin Gerard for the original SNDH files packer source (PACK-ICE) in
MC68000 assembler published in unice68_pack.c.
- Lee Bee for suggestions.

Special thanks to people who wrote or write music in Vortex Tracker II:
4Mat
AER
Akasaka
Alex Rostov
Alexander Boolba
Alone Coder
And
ant1
Aprisobal
Asi
Astamur Panov
AZ
Bey Elder
Beyker
Biozoom
Black Fox
Bloobop
Bonysoft
buzzkej
Byteman
casecoma
Cat-Man
Ch41ns4w
Chip Champion
Cj Echo
Crazy Pixel
Creator
Darkman007
DDp
dead8088
Deetsay
Destr
Dippy
Dj Denson
Dj Kot
Dj Max
DoctorGentleman
EA
Elfh
Ellvis
emook
Esau
Factor6
Firestarter
Frank Triggs
Fubukimaru
funute
Garvalf
Gasman
Gibson
gotoandplay
gyms
isihlabathi
Jangler
Jass
Jerrs
Johnny McGibbitts
Joker
JosSs
Joyrex
jrlepage
Jumperror
Justinas
Kakos_nonos
Karbofos
kas29
Key-Jee
kfaraday
Klim
Kriss
kubikami
Kulor
KUVO
Kyv
Lamer Pinky
Lee Bee
Lee du-Caine aka dC Audio
LiSU
m9m
MAD_DEL
MadMax of KNA
Makinavaja
mborik
Megus
Mic of MPS
Misha Pertsovsky
Mmcm
Mofobaru
Molodcov Alexander
MovieMovies1
neppie
nikhotmsk
Nik-O
Nooly
NVitia
OldFartGamer
Orson
Pak-Zer0
Pator
petet
Pigu
PSB
Quiet
rainwarrior
REAKTIVZ
Riskej
Robyn
Rolemusic
Rubedo
ruguevara
Ryurik
Samanasuke
Sarek
Sayk
Scalesmann
Sentinel
Sharik
Shiru Otaku
Siril
Splinter
Stiffler
Striker of Megasoft
Sultan Paraiso
TAD
tayle
th4 D34D
Tobikomi
Tosha
TristEndo
usagi
V0yager
Vadimatorik
Vedem
Voxel
Wally
wbc
X-Agon
Yerzmyey
z00m
Z23
Ziutek
ZJ Alex Clap
Zlew
Znahar
ZoundMakkr
zxastafiew
and you ;)

Distribution
------------

Vortex Tracker II is free program. There are two kind of original distribution:
binary (VT.exe with documentation) and sources (source files as Lazarus project
with documentation). You can use and distribute sources freely, simply credit me
somewhere in your projects, where you include all or part of the sources and
(or) my algorithms.

Sergey Bulba

24 of August 2002 - 4 of July 2024
