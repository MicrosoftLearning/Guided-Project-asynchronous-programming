using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace UI
{
    class GameLogger : ILogger
    {
        string filePath;

        public GameLogger(string filePath)
        {
            this.filePath = filePath;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logRecord = $"{DateTime.Now} [{logLevel.ToString()}] {formatter(state, exception)} {exception?.StackTrace}";
            var options = new FileStreamOptions() { Access = FileAccess.Write, Mode = FileMode.Append, Share = FileShare.Read };
            StreamWriter? streamWriter = null;
            var nTries = 1000;

            // Spin until when we can open the log file for writing
            while (nTries > 0)
            {
                try
                {
                    streamWriter = new StreamWriter(filePath, Encoding.UTF8, options);
                    streamWriter.WriteLine(logRecord);
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(10);
                    nTries--;
                }
                finally
                {
                    streamWriter?.Dispose();
                }
            }
        }
    }
}