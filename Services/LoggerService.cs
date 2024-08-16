using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Owcounter.Services
{
    public class LoggerService : IDisposable
    {
        private readonly BlockingCollection<string> _logQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _logTask;
        private readonly string _logFilePath;

        public LoggerService(string logFilePath)
        {
            _logQueue = new BlockingCollection<string>();
            _cancellationTokenSource = new CancellationTokenSource();
            _logFilePath = logFilePath;

            _logTask = Task.Run(ProcessLogQueue);
        }

        public void Log(string message)
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _logQueue.Add($"[{DateTime.Now}] {message}");
            }
        }

        private void ProcessLogQueue()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    string message = _logQueue.Take(_cancellationTokenSource.Token);
                    File.AppendAllText(_logFilePath, message + Environment.NewLine);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _logTask.Wait();
            _logQueue.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}