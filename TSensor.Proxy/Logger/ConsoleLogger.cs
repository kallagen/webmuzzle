using System;
using System.Threading;

namespace TSensor.Proxy.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, string prefix = null, Elapsed elapsed = null, bool isError = false)
        {
            Console.WriteLine(
                string.Join(string.Empty,
                    new[]
                    {
                        $"{Thread.CurrentThread.ManagedThreadId}:{DateTime.Now:HH:mm:ss.f}:",
                        prefix != null ? $"{prefix} " : null,
                        $"{message}",
                        elapsed != null ? $"({elapsed})" : null
                    }));
        }
    }
}
