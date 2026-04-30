# VortexTracker — Architecture Block Diagram

> Auto-generated from workspace analysis. Update this file as the Avalonia port progresses.

## Block Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              PLATFORM TARGET HEADS                              │
│  ┌──────────────────┐  ┌──────────────┐  ┌─────────────┐  ┌─────────────────┐ │
│  │ VTAvalonia       │  │ VTAvalonia   │  │ VTAvalonia  │  │ VortexTracker   │ │
│  │ .Desktop         │  │ .Android     │  │ .Browser    │  │ (WinForms)      │ │
│  │ net10.0          │  │              │  │ (WASM)      │  │ net8.0-windows  │ │
│  └────────┬─────────┘  └──────┬───────┘  └──────┬──────┘  └────────┬────────┘ │
└───────────┼────────────────────┼─────────────────┼──────────────────┼──────────┘
            │                    │                 │                  │
            └────────────────────┴────────┬────────┘                  │
                                          │                           │
                         ┌────────────────▼───────────────┐           │
                         │         VTAvalonia              │           │
                         │      (Avalonia UI Library)      │           │
                         │                                 │           │
                         │  Views                          │           │
                         │  ├─ MainWindow                  │           │
                         │  ├─ MainView          ░░░░░░░░  │           │
                         │  ├─ OptionsWindow     ░░░░░░░░  │           │
                         │  └─ AboutWindow                 │           │
                         │                                 │           │
                         │  ViewModels                     │           │
                         │  ├─ MainViewModel               │           │
                         │  ├─ OptionsViewModel  ░░░░░░░░  │           │
                         │  └─ AboutViewModel              │           │
                         │                                 │           │
                         │  Services (real)                │           │
                         │  ├─ LibVtModuleService    ✓     │           │
                         │  ├─ LibVtPlaybackService  ✓     │           │
                         │  ├─ AvaloniaWindowingService ✓  │           │
                         │  ├─ AvaloniaFileDialogService ✓ │           │
                         │  ├─ AvaloniaClipboardService ✓  │           │
                         │  └─ AvaloniaUIActionDispatcher✓ │           │
                         │                                 │           │
                         │  Services (Null stubs only)     │           │
                         │  ├─ IAudioOutputService  ░░░░░░ │           │
                         │  ├─ IMidiService         ░░░░░░ │           │
                         │  ├─ ISerialPortService   ░░░░░░ │           │
                         │  ├─ IPlatformPathsService ░░░░░ │           │
                         │  └─ IRegistryService     ░░░░░░ │           │
                         └──────────────┬──────────────────┘           │
                                        │                               │
            ┌───────────────────────────┼───────────────────────────────┘
            │                           │
            ▼                           ▼
┌───────────────────────┐   ┌───────────────────────────────────────────────────┐
│  VortexTracker.Core   │   │              VortexTracker (WinForms)             │
│   (net8.0)            │   │                                                   │
│                       │   │  UI / Forms                                       │
│  AppState             │   │  ├─ MainForm        (pattern editor shell)        │
│                       │   │  ├─ ChildForm        (dockable MDI child)         │
│  Service Interfaces   │   │  ├─ OptionsForm                                   │
│  ├─ IModuleService    │   │  ├─ AboutForm                                     │
│  ├─ IPlaybackService  │   │  ├─ TrackInfoForm                                 │
│  ├─ IFileDialogService│   │  ├─ TracksManagerForm                             │
│  ├─ IClipboardService │   │  ├─ TurboSoundForm                                │
│  ├─ IWindowingService │   │  ├─ ExportWavOptionsForm                          │
│  ├─ IAudioOutputSvc   │   │  ├─ ExportZXForm                                  │
│  ├─ IMidiService      │   │  ├─ GlobalTransForm                               │
│  ├─ ISerialPortSvc    │   │  ├─ ToggleSamplesForm                             │
│  └─ IPlatformPathsSvc │   │  ├─ UnloopForm                                    │
│                       │   │  ├─ FxmParamsForm                                 │
│  Action System        │   │  ├─ FileBrowser                                   │
│  ├─ UIActionCatalog   │   │  ├─ DriveSelect                                   │
│  ├─ UIActionCatalog   │   │  └─ PluginManagerForm                             │
│  │   Factory          │   │                                                   │
│  ├─ UIActionType      │   │  Rendering                                        │
│  ├─ UIActionDefinition│   │  ├─ OpenGL (custom pattern editor renderer)       │
│  ├─ UIActionState     │   │  ├─ GDI                                           │
│  └─ IUIActionDispatch │   │  ├─ Font / Texture2D                              │
└───────────┬───────────┘   │  └─ ColorThemes                                   │
            │               │                                                   │
            │               │  Win32 / Platform services                        │
            │               │  ├─ MidiOut     (Windows P/Invoke)                │
            │               │  ├─ SerialOut   (Windows P/Invoke)                │
            │               │  ├─ Win32       (Windows P/Invoke)                │
            │               │  └─ HotKeys / UIActionManager                     │
            │               │                                                   │
            │               │  Editors (custom controls)                        │
            │               │  ├─ Samples.cs                                    │
            │               │  └─ Ornaments.cs                                  │
            └───────────────┴───────────────────────────────────────────────────┘
                                        │
                                        ▼
                         ┌──────────────────────────────┐
                         │          LibVT  (net8.0)      │
                         │                              │
                         │  VTM  (module data model)    │
                         │  ├─ Pattern / Sample /        │
                         │  │   Ornament / Positions     │
                         │  └─ VTM2PT3 (export)         │
                         │                              │
                         │  AY chip emulation           │
                         │  ├─ AY / Ayumi / WaveAyumi   │
                         │  └─ SoundChip / DigiDrum /   │
                         │     SIDVoice                 │
                         │                              │
                         │  Format converters           │
                         │  PT1/PT2/PT3/STC/STP/SQT/   │
                         │  ASC/GTR/PSM/PSC/FLS/FTC/   │
                         │  FXM  → VTM                  │
                         │                              │
                         │  WaveOutAPI  (OpenAL)  ✓     │
                         │  PatternPacker               │
                         │  AppEvents (UI bridge)       │
                         │  Plugins: IHost / IPlugin    │
                         │  Main (global config state)  │
                         └──────────────────────────────┘
```

## Legend

| Symbol | Meaning |
|--------|---------|
| `✓` | Implemented and wired |
| `░░░░░░` | Interface exists but no real implementation yet |
| `░░░░░░` (on a View/ViewModel) | Stub — window/class exists but content is missing |

---

## Avalonia Porting Status

### 🔴 Missing service implementations (interfaces defined, zero real code)

| Interface | WinForms equivalent | Notes |
|-----------|---------------------|-------|
| `IAudioOutputService` | `WaveOutAPI` device list | Enumerate OpenAL output devices |
| `IMidiService` | `MidiOut.cs` | Cross-platform MIDI via RtMidi.Net (already a dep) |
| `ISerialPortService` | `SerialOut.cs` | `System.IO.Ports` — already cross-platform |
| `IPlatformPathsService` | `Globals.cs` paths | `Environment.GetFolderPath` — trivially cross-platform |
| `IRegistryService` | `Win32.cs` file assoc | Desktop-only; safe to no-op on non-Windows |

### 🟡 Stub ViewModels / Views that need real content

| Item | What's missing |
|------|----------------|
| `OptionsViewModel` | Every setting from `OptionsForm` (audio device, note table, MIDI, colours, hotkeys…) |
| `OptionsWindow.axaml` | All the actual controls — currently just a Close button |
| `MainView.axaml` | The **pattern editor** — biggest single task; needs a custom `ItemsControl` or `Canvas`-based renderer to replace the WinForms OpenGL/GDI renderer |

### 🟠 Dialogs / windows not yet created in Avalonia

| WinForms form | Purpose |
|---------------|---------|
| `TrackInfoForm` | Module header (title, author, speed, note table) |
| `TracksManagerForm` | Manage position list / patterns |
| `TurboSoundForm` | Dual/triple AY chip settings |
| `SamplesForm` | Sample editor |
| `OrnamentsForm` | Ornament editor |
| `ExportWavOptionsForm` | WAV export settings |
| `ExportZXForm` | ZX Spectrum binary export |
| `GlobalTransForm` | Global transpose |
| `ToggleSamplesForm` | Mute/solo samples |
| `UnloopForm` | Unroll loop for export |
| `FxmParamsForm` | FXM effect parameters |
| `FileBrowser` | Built-in file browser panel |
| `DriveSelect` | Drive picker |
| `PluginManagerForm` | Plugin load/configure |
| `ProgressBarForm` | Long-operation progress |

### 🔵 Cross-cutting concerns not yet ported

| Area | Notes |
|------|-------|
| **Hotkey system** | `HotKeys.cs` + `UIActionManager.cs` — bind `UIActionType` to keys; Avalonia uses `KeyBindings` in XAML |
| **ColorThemes** | `ColorThemes.cs` — map to Avalonia `ResourceDictionary` / dynamic theme |
| **Custom pattern renderer** | Currently OpenGL + GDI in WinForms; Avalonia equivalent would be a `CustomControl` using `DrawingContext` or SkiaSharp |
| **Plugin system** | `IPlugin`/`IHost` + `PluginManager` — needs Avalonia host glue |
| **`AppState`** | Currently a tiny two-field class — needs expanding to mirror `Main` global state as the ViewModels grow |
