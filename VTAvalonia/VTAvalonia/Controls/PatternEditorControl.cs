using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace VTAvalonia.Controls;

/// <summary>
/// Custom pattern-editor rendering control.
/// Draws a colour-coded VT pattern grid from the pre-formatted strings produced
/// by <c>LibVtModuleService.GetPatternLines()</c>.
///
/// String format (52 chars, hex line-nums, separators on):
/// <code>
///   HH|EEEE|NN|NNN SEOV NDPP|NNN SEOV NDPP|NNN SEOV NDPP
///   01 2 3-6 7 8-9 A B--D E  F-H I  J--L M  N-P Q  R--T
/// </code>
/// Where: HH=linenum, EEEE=envelope, NN=noise, NNN=note,
///        S=sample, E=chanEnv, O=ornament, V=volume,
///        N=cmdNum, D=cmdDelay, PP=cmdParam  (all 1-hex except PP=2-hex)
/// </summary>
public sealed class PatternEditorControl : Control
{
    // ── Styled properties ─────────────────────────────────────────────────────

    public static readonly StyledProperty<IList<string>?> PatternLinesProperty =
        AvaloniaProperty.Register<PatternEditorControl, IList<string>?>(nameof(PatternLines));

    public static readonly StyledProperty<int> CursorLineProperty =
        AvaloniaProperty.Register<PatternEditorControl, int>(nameof(CursorLine), defaultValue: 0);

    public static readonly StyledProperty<int> HlStepProperty =
        AvaloniaProperty.Register<PatternEditorControl, int>(nameof(HlStep), defaultValue: 4);

    public IList<string>? PatternLines
    {
        get => GetValue(PatternLinesProperty);
        set => SetValue(PatternLinesProperty, value);
    }

    public int CursorLine
    {
        get => GetValue(CursorLineProperty);
        set => SetValue(CursorLineProperty, value);
    }

    public int HlStep
    {
        get => GetValue(HlStepProperty);
        set => SetValue(HlStepProperty, value);
    }

    // ── Rendering constants ───────────────────────────────────────────────────

    private const double FontSize = 13.0;
    // 52 chars per line; used for preferred-width hint only
    private const int CharCount = 52;
    private static readonly Typeface s_typeface =
        new(new FontFamily("Consolas,Courier New,monospace"));

    // ── Segment layout ────────────────────────────────────────────────────────
    // Positions verified against VTModule.GetPatternLineString(lineNums=true,separators=true)

    private enum Seg { LineNum, Sep, Envelope, Noise, Note, Params, Commands }

    private static readonly (int Start, int Len, Seg Kind)[] s_segs =
    [
        (0,  2, Seg.LineNum),
        (2,  1, Seg.Sep),
        (3,  4, Seg.Envelope),
        (7,  1, Seg.Sep),
        (8,  2, Seg.Noise),
        (10, 1, Seg.Sep),
        // Channel A ─────────────────────────
        (11, 3, Seg.Note),
        (15, 4, Seg.Params),      // S E O V
        (20, 4, Seg.Commands),    // N D PP
        (24, 1, Seg.Sep),
        // Channel B ─────────────────────────
        (25, 3, Seg.Note),
        (29, 4, Seg.Params),
        (34, 4, Seg.Commands),
        (38, 1, Seg.Sep),
        // Channel C ─────────────────────────
        (39, 3, Seg.Note),
        (43, 4, Seg.Params),
        (48, 4, Seg.Commands),
    ];

    // ── Colour theme — "Default" ──────────────────────────────────────────────
    // Values sourced from ColorThemes.DefaultColorThemes[0] in VortexTracker/ColorThemes.cs

    private static IBrush Rgb(uint rgb) =>
        new SolidColorBrush(Color.FromArgb(0xFF, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb));

    private static readonly IBrush s_bgBrush        = Rgb(0xFFFFFF);
    private static readonly IBrush s_selBgBrush      = Rgb(0x4256A2);
    private static readonly IBrush s_hlBgBrush       = Rgb(0xEFEFEF);

    private static readonly IBrush s_textBrush       = Rgb(0x7D7D88);
    private static readonly IBrush s_selTextBrush    = Rgb(0x798EAB);
    private static readonly IBrush s_hlTextBrush     = Rgb(0x54545C);

    private static readonly IBrush s_lineNumBrush    = Rgb(0x454455);
    private static readonly IBrush s_selLineNumBrush = Rgb(0xECEDD1);
    private static readonly IBrush s_hlLineNumBrush  = Rgb(0x414050);

    private static readonly IBrush s_envBrush        = Rgb(0x515A6F);
    private static readonly IBrush s_selEnvBrush     = Rgb(0xFFFED9);
    private static readonly IBrush s_noiseBrush      = Rgb(0x477C80);
    private static readonly IBrush s_selNoiseBrush   = Rgb(0xFFFED9);
    private static readonly IBrush s_noteBrush       = Rgb(0x1F1C65);
    private static readonly IBrush s_selNoteBrush    = Rgb(0xFFFED9);
    private static readonly IBrush s_paramsBrush     = Rgb(0x5A5E74);
    private static readonly IBrush s_selParamsBrush  = Rgb(0xFFFED9);
    private static readonly IBrush s_cmdsBrush       = Rgb(0x536B71);
    private static readonly IBrush s_selCmdsBrush    = Rgb(0xFFFED9);
    private static readonly IBrush s_sepBrush        = Rgb(0x8E8E9F);

    // ── State ─────────────────────────────────────────────────────────────────

    private int _scrollOffset;
    private double _charW;
    private double _charH;
    private bool _charsMeasured;
    private INotifyCollectionChanged? _subscribedCollection;

    // ── Static constructor ────────────────────────────────────────────────────

    static PatternEditorControl()
    {
        AffectsRender<PatternEditorControl>(
            PatternLinesProperty, CursorLineProperty, HlStepProperty);

        PatternLinesProperty.Changed.AddClassHandler<PatternEditorControl>(
            (c, _) => c.OnPatternLinesChanged());

        CursorLineProperty.Changed.AddClassHandler<PatternEditorControl>(
            (c, _) => c.OnCursorChanged());
    }

    public PatternEditorControl()
    {
        Focusable = true;
        ClipToBounds = true;
    }

    // ── Property-change handlers ──────────────────────────────────────────────

    private void OnPatternLinesChanged()
    {
        if (_subscribedCollection != null)
        {
            _subscribedCollection.CollectionChanged -= OnCollectionChanged;
            _subscribedCollection = null;
        }

        if (PatternLines is INotifyCollectionChanged ncc)
        {
            _subscribedCollection = ncc;
            ncc.CollectionChanged += OnCollectionChanged;
        }

        _scrollOffset = 0;
        EnsureCursorVisible();
        InvalidateVisual();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
    }

    private void OnCursorChanged()
    {
        EnsureCursorVisible();
        InvalidateVisual();
    }

    // ── Scroll helpers ────────────────────────────────────────────────────────

    private int VisibleLines =>
        _charH > 0 && Bounds.Height > 0 ? (int)(Bounds.Height / _charH) : 0;

    private void EnsureCursorVisible()
    {
        int vis = VisibleLines;
        if (vis <= 0) return;

        int c = CursorLine;
        if (c < _scrollOffset)
            _scrollOffset = c;
        else if (c >= _scrollOffset + vis)
            _scrollOffset = c - vis + 1;

        ClampScroll();
    }

    private void ClampScroll()
    {
        int count = PatternLines?.Count ?? 0;
        int vis   = Math.Max(1, VisibleLines);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, count - vis));
    }

    // ── Render ────────────────────────────────────────────────────────────────

    public override void Render(DrawingContext ctx)
    {
        EnsureCharsMeasured();

        var bounds = Bounds;
        ctx.FillRectangle(s_bgBrush, new Rect(bounds.Size));

        var lines = PatternLines;
        if (lines == null || lines.Count == 0 || _charH <= 0)
            return;

        int hlStep = HlStep;
        int cursor = CursorLine;
        int count  = lines.Count;
        // Render one extra line to avoid a gap at the bottom when partially scrolled
        int vis    = VisibleLines + 1;
        double y   = 0;

        for (int i = 0; i < vis; i++)
        {
            int li = _scrollOffset + i;
            if (li >= count) break;

            bool isSel = li == cursor;
            bool isHl  = !isSel && hlStep > 0 && li % hlStep == 0;

            IBrush rowBg = isSel ? s_selBgBrush : (isHl ? s_hlBgBrush : s_bgBrush);
            ctx.FillRectangle(rowBg, new Rect(0, y, bounds.Width, _charH));

            DrawLine(ctx, lines[li], isSel, isHl, y);
            y += _charH;
        }
    }

    private void DrawLine(DrawingContext ctx, string text, bool isSel, bool isHl, double y)
    {
        if (text.Length == 0) return;

        IBrush baseFg = isSel ? s_selTextBrush : (isHl ? s_hlTextBrush : s_textBrush);

        var ft = new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            s_typeface,
            FontSize,
            baseFg);

        foreach (var (start, len, kind) in s_segs)
        {
            if (start + len > text.Length) continue;
            var brush = PickBrush(kind, text, start, len, isSel, isHl);
            if (brush != null)
                ft.SetForegroundBrush(brush, start, len);
        }

        ctx.DrawText(ft, new Point(0, y));
    }

    private static IBrush? PickBrush(
        Seg kind, string text, int start, int len, bool isSel, bool isHl)
    {
        return kind switch
        {
            Seg.LineNum  => isSel ? s_selLineNumBrush : (isHl ? s_hlLineNumBrush : s_lineNumBrush),
            Seg.Sep      => s_sepBrush,
            Seg.Envelope => AllEmpty(text, start, len) ? null : (isSel ? s_selEnvBrush  : s_envBrush),
            Seg.Noise    => AllEmpty(text, start, len) ? null : (isSel ? s_selNoiseBrush : s_noiseBrush),
            Seg.Note     => IsEmptyNote(text, start)   ? null : (isSel ? s_selNoteBrush  : s_noteBrush),
            Seg.Params   => AllEmpty(text, start, len) ? null : (isSel ? s_selParamsBrush : s_paramsBrush),
            Seg.Commands => AllEmpty(text, start, len) ? null : (isSel ? s_selCmdsBrush   : s_cmdsBrush),
            _            => null,
        };
    }

    /// <summary>Returns true when all characters in [start, start+len) are dots or spaces.</summary>
    private static bool AllEmpty(string s, int start, int len)
    {
        for (int i = start; i < start + len; i++)
            if (s[i] != '.' && s[i] != ' ') return false;
        return true;
    }

    /// <summary>Returns true for the "no note" placeholder "---".</summary>
    private static bool IsEmptyNote(string s, int start) =>
        start + 3 <= s.Length && s[start] == '-' && s[start + 1] == '-' && s[start + 2] == '-';

    // ── Character measurement ─────────────────────────────────────────────────

    private void EnsureCharsMeasured()
    {
        if (_charsMeasured) return;

        var ft = new FormattedText(
            "W",
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            s_typeface,
            FontSize,
            Brushes.White);

        _charW = ft.Width;
        _charH = ft.Height;
        _charsMeasured = true;
    }

    // ── Input handling ────────────────────────────────────────────────────────

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
        if (_charH <= 0) return;

        int li    = _scrollOffset + (int)(e.GetPosition(this).Y / _charH);
        int count = PatternLines?.Count ?? 0;
        if (li >= 0 && li < count)
            CursorLine = li;

        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        _scrollOffset += e.Delta.Y > 0 ? -3 : 3;
        ClampScroll();
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        int count = PatternLines?.Count ?? 0;
        if (count == 0) return;

        switch (e.Key)
        {
            case Key.Up:
                if (CursorLine > 0) CursorLine--;
                e.Handled = true;
                break;
            case Key.Down:
                if (CursorLine < count - 1) CursorLine++;
                e.Handled = true;
                break;
            case Key.PageUp:
                CursorLine = Math.Max(0, CursorLine - Math.Max(1, VisibleLines - 1));
                e.Handled = true;
                break;
            case Key.PageDown:
                CursorLine = Math.Min(count - 1, CursorLine + Math.Max(1, VisibleLines - 1));
                e.Handled = true;
                break;
            case Key.Home when (e.KeyModifiers & KeyModifiers.Control) != 0:
                CursorLine = 0;
                e.Handled = true;
                break;
            case Key.End when (e.KeyModifiers & KeyModifiers.Control) != 0:
                CursorLine = count - 1;
                e.Handled = true;
                break;
        }
    }

    // ── Measure ───────────────────────────────────────────────────────────────

    protected override Size MeasureOverride(Size available)
    {
        EnsureCharsMeasured();

        // Preferred width = full pattern line + small margin
        double w = double.IsInfinity(available.Width)
            ? (_charW > 0 ? _charW * CharCount + 8 : 640)
            : available.Width;

        // Preferred height:
        //  · Infinite available (auto-sizing window)  → report the full pattern height so
        //    the window opens large enough, but cap at a sensible maximum.
        //  · Finite available (normal layout pass)    → take all the space we're given.
        double h;
        if (double.IsInfinity(available.Height))
        {
            int lineCount = PatternLines?.Count ?? 0;
            double fullH  = _charH > 0 && lineCount > 0 ? _charH * lineCount : 400;
            h = Math.Min(fullH, 600);   // cap so auto-size windows don't go off-screen
        }
        else
        {
            h = Math.Max(available.Height, _charH > 0 ? _charH * 4 : 60);
        }

        return new Size(w, h);
    }
}
