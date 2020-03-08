using System;
using System.Threading;

namespace TSensor.Proxy
{
    public class Logger
    {
        private readonly object Locker = new object();

        public void Write(object obj, Elapsed elapsed = null, string prefix = null)
        {
            lock (Locker)
            {
                var log = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (prefix != null)
                {
                    log += $" {prefix}";
                }

                log += $" {obj}";

                if (elapsed != null)
                {
                    log += $" ({elapsed} ms)";
                }

                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}:{log}");
            }
        }
    }
}