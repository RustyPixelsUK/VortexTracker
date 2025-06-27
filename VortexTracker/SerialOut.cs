using LibVT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace VortexTracker
{
    public static class SerialOut
    {
        private static SerialPort? _serialPort;
        private static Channel<RegisterEventArgs>? _frameCh;

        private static Thread? _pumpThread;
        private static CancellationTokenSource? _pumpCts;

        // ──────────────────────────────────────────────────────────────────────────
        public static void StartPump()
        {
            StopPump();                                      // 1️⃣  shut down cleanly

            // 2️⃣  fresh channel for the new session
            _frameCh = Channel.CreateBounded<RegisterEventArgs>(new BoundedChannelOptions(80)
            {
                SingleWriter = false,
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });

            // 3️⃣  new serial port
            _serialPort = new SerialPort("COM7", 2_000_000);
            _serialPort.Open();

            _pumpCts = new CancellationTokenSource();
            var token = _pumpCts.Token;

            _pumpThread = new Thread(() =>
            {
                try
                {
                    PumpLoopAsync(token).GetAwaiter().GetResult();
                }
                catch
                {
                }
            })
            { IsBackground = true, Priority = ThreadPriority.Highest };

            _pumpThread.Start();
        }

        public static void StopPump()
        {
            _pumpCts?.Cancel();                     // ask the loop to exit
            _frameCh?.Writer.TryComplete();         // wake the reader if blocked

            _pumpThread?.Join();                    // wait for a clean finish

            _serialPort?.Dispose();                 // close the COM port
            _serialPort = null;
            _pumpThread = null;
            _pumpCts = null;
            _frameCh = null;
        }

        private static async Task PumpLoopAsync(CancellationToken token)
        {
            var sw = Stopwatch.StartNew();
            long nextUs = sw.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
            long lastSlot = 0;

            var reader = _frameCh!.Reader;          // channel is fresh for this run

            while (!token.IsCancellationRequested)
            {
                var f = await reader.ReadAsync(token);

                byte[] frame = new byte[17];
                frame[0] = (byte)f.ChipIndex;
                Array.Copy(f.Registers, 0, frame, 1, f.Registers.Length);
                _serialPort.Write(frame, 0, frame.Length);

                if (f.ChipIndex != 0)
                    continue;     // timing only on chip 0

                // ─── slot scheduling ────────────────────────────────────────────────
                nextUs += f.SlotUs;

                if (lastSlot != f.SlotUs) lastSlot = f.SlotUs;

                // If we ever slip behind, jump to the *next* slot boundary
                long nowUs = sw.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
                while (nextUs <= nowUs) nextUs += f.SlotUs;

                // Busy‑/sleep‑wait until the boundary
                do
                {
                    nowUs = sw.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
                    long diff = nextUs - nowUs;
                    if (diff > 2_000) Thread.Sleep(1);   // 1 ms
                    else Thread.SpinWait(75);
                }
                while (nextUs > nowUs && !token.IsCancellationRequested);
            }
        }

        public static void RegisterEvent(object? sender, RegisterEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen || _frameCh == null)
                return;

            // TryWrite is lock‑free and never throws; falls back to async await if full.
            if (!_frameCh.Writer.TryWrite(e))
                _ = _frameCh.Writer.WriteAsync(e);
        }
    }
}
