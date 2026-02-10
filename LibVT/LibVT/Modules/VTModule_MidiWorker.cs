// 
// MIDI Event Worker Thread - Part of VTModule
// Handles async MIDI event processing to prevent audio thread blocking
//

using System.Collections.Concurrent;

namespace LibVT
{
    public partial class VTModule
    {
        // Dedicated MIDI event queue and worker thread
        private static readonly BlockingCollection<PlaybackEventArgs> _eventQueue 
            = new BlockingCollection<PlaybackEventArgs>(boundedCapacity: 100);
        private static Thread? _eventWorkerThread;
        private static volatile bool _eventWorkerRunning;
        
        private static void StartEventWorker()
        {
            _eventWorkerRunning = true;
            _eventWorkerThread = new Thread(EventWorkerThreadProc)
            {
                Name = "MIDI Event Worker",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _eventWorkerThread.Start();
        }
        
        private static void EventWorkerThreadProc()
        {
            while (_eventWorkerRunning)
            {
                try
                {
                    // Blocking wait for next event (100ms timeout to check shutdown flag)
                    if (_eventQueue.TryTake(out var args, millisecondsTimeout: 100))
                    {
                        try
                        {
                            PlaybackEvent?.Invoke(null, args);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"PlaybackEvent handler error: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Event worker thread error: {ex.Message}");
                }
            }
        }
        
        public static void StopEventWorker()
        {
            _eventWorkerRunning = false;
            _eventWorkerThread?.Join(TimeSpan.FromMilliseconds(1000)); // Wait up to 1 second
        }
    }
}
