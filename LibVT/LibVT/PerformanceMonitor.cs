using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVT
{
    public static class PerformanceMonitor
    {
        private static readonly Queue<long> _frameTimes = new();
        private static readonly Stopwatch _stopWatch = Stopwatch.StartNew();
        private static long _lastFrame;
        private const int MaxSamples = 30;

        public static void Frame()
        {
            long now = _stopWatch.ElapsedMilliseconds;
            long delta = now - _lastFrame;
            _lastFrame = now;

            _frameTimes.Enqueue(delta);
            if (_frameTimes.Count > MaxSamples)
                _frameTimes.Dequeue();

            long avg = _frameTimes.Count > 0 ? (long)_frameTimes.Average() : 0;
            //double fps = avg > 0 ? 1000.0 / avg : 0;

            //var cpu = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount / (_stopWatch.Elapsed.TotalMilliseconds / 1000.0);
            //var mem = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);

            //AppEvents.PostEvent(EventType.UpdatePerformanceStats, $"{fps:F0} FPS ({avg} ms) CPU {cpu:F0}% RAM {mem} MB");
            AppEvents.PostEvent(EventType.UpdatePerformanceStats, $"{avg:D3} ms");
        }
    }
}
