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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace LibVT
{
    public enum MyMessageBoxButtons
    {
        OK,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel
    }

    public enum MyMessageBoxIcon
    {
        None = 0,
        Hand = 16,
        Question = 32,
        Exclamation = 48,
        Asterisk = 64,
        Stop = 16,
        Error = 16,
        Warning = 48,
        Information = 64
    }

    public enum EventType
    {
        None = 0,
        MessageBox,
        RedrawTracks,
        PlayingOff,
        FinalizeWO,
        SetControlsForExport,
        SetChannelsAllocation,
        SetFromAndToPosition,
        SetChildPositions,
        ChangePositions,
        RestartPlayingPosition,
        RestorePositionAndPattern,
        RerollToLine,
        RerollToLineNum,
        GetPosition,
        StopAndRestart,
        FXMDialog,
        UpdatePerformanceStats,
        ProgressBar,
        DoEvents
    };

    public class AppEventArgs : EventArgs
    {
        private readonly ManualResetEventSlim _waitHandle = new();

        public EventType EventType { get; }
        public object[] Params { get; }
        public object Result { get; set; }
        public bool IsCompleted { get; private set; }

        public AppEventArgs(EventType type, params object[] parameters)
        {
            EventType = type;
            Params = parameters;
        }

        public void Wait()
        {
            if (!IsCompleted)
                _waitHandle.Wait();
        }

        public void Complete()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                _waitHandle.Set();
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"EventType={EventType}");

            if (Params.Length > 0)
                stringBuilder.Append($", Params=[{string.Join(", ", Params.Select(p => p ?? "null"))}]");

            if (IsCompleted)
                stringBuilder.Append($", Result={Result ?? "null"}");

            return stringBuilder.ToString();
        }
    }

    public static class AppEvents
    {
        private static readonly ConcurrentQueue<AppEventArgs> _queue = new();

        /// <summary>
        /// Optional function set by the UI to check if the current thread is the UI thread.
        /// </summary>
        public static Func<bool> IsOnUIThread { get; set; }

        /// <summary>
        /// Optional delegate to dispatch and handle AppEvents synchronously when on UI thread.
        /// </summary>
        public static Action<AppEventArgs> Dispatcher { get; set; }

        public static void PostEvent(EventType type, params object[] parameters)
        {
            _queue.Enqueue(new AppEventArgs(type, parameters));
        }

        public static object SendEvent(EventType type, params object[] parameters)
        {
            var e = new AppEventArgs(type, parameters);

            if (IsOnUIThread != null && IsOnUIThread())
            {
                if (Dispatcher != null)
                {
                    Dispatcher(e);
                    return e.Result;
                }
                else
                {
                    throw new InvalidOperationException("Dispatcher must be set to handle events on UI thread.");
                }
            }

            _queue.Enqueue(e);
            e.Wait();
            return e.Result;
        }

        public static bool TryDequeue(out AppEventArgs e)
        {
            return _queue.TryDequeue(out e);
        }

        public static void ClearEvent(EventType type)
        {
            var remaining = new Queue<AppEventArgs>();

            while (_queue.TryDequeue(out var evt))
            {
                if (evt.EventType != type)
                    remaining.Enqueue(evt);
            }

            while (remaining.Count > 0)
                _queue.Enqueue(remaining.Dequeue());
        }

        public static void ClearAllEvents()
        {
            while (_queue.TryDequeue(out _)) { }
        }
    }
}
