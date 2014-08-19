using System;
using System.IO;

namespace SimplOS.Log
{
    public class Logger
    {
        private readonly StreamWriter _writer;

        public Logger(string path)
        {
            _writer = new StreamWriter(path) {AutoFlush = true, NewLine = Environment.NewLine};
        }

        public void LogD(string d)
        {
            _writer.WriteLine("[Verbose]  " + d + " @ " + DateTime.Now.ToLocalTime());
        }

        public void LogE(string d)
        {
            _writer.WriteLine("[Error]  " + d + " @ " + DateTime.Now.ToLocalTime());
        }

        public void Finish()
        {
            if (_writer != null)
            {
                _writer.Flush();
                _writer.Close();
            }
        }
    }
}