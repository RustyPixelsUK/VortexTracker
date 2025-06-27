using System;
using System.IO;

namespace VortexTracker
{
    public class Logger
    {
        public bool _isOpened = false;
        public Stream _logFile = null;
        public StreamWriter _streamWriter = null;

        public Logger(string fileName)
        {
            _streamWriter = new StreamWriter(fileName);
        }

        ~Logger()
        {
            _streamWriter.Close();
        }

        public void Add(string text)
        {
            _streamWriter.WriteLine(text);
        }
    }
}
