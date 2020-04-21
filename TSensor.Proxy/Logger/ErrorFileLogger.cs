using System;
using System.IO;
using System.Text;
using System.Threading;

namespace TSensor.Proxy.Logger
{
    public class ErrorFileLogger : ILogger
    {
        private const string ERROR_FILE_NAME = "error.log";
        private const string ERROR_FILE_NAME_TEMP = "error.tmp";

        private readonly object Locker = new object();

        private readonly int MaxErrorFileSize;

        public ErrorFileLogger(Config config)
        {
            MaxErrorFileSize = config.MaxErrorFileSize;
        }

        public void Log(string message, string prefix = null, Elapsed elapsed = null, bool isError = false)
        {
            try
            {
                if (!isError)
                {
                    return;
                }

                lock (Locker)
                {
                    var log = string.Join(string.Empty,
                        new[]
                        {
                        $"{Thread.CurrentThread.ManagedThreadId}:{DateTime.Now:HH:mm:ss.f}:",
                        prefix != null ? $"{prefix} " : null,
                        $"{message}",
                        elapsed != null ? $"({elapsed})" : null
                        });

                    if (!File.Exists(ERROR_FILE_NAME))
                    {
                        File.Create(ERROR_FILE_NAME);
                    }

                    var currentSize = Encoding.UTF8.GetByteCount(log);

                    using (var writer = new StreamWriter(ERROR_FILE_NAME_TEMP))
                    {
                        using (var reader = new StreamReader(ERROR_FILE_NAME))
                        {
                            writer.WriteLine(log);

                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                var lineSize = Encoding.UTF8.GetByteCount(line);

                                if (currentSize + lineSize < MaxErrorFileSize || MaxErrorFileSize <= 0)
                                {
                                    currentSize += lineSize;

                                    writer.WriteLine(line);
                                }
                            }
                        }
                    }

                    File.Copy(ERROR_FILE_NAME_TEMP, ERROR_FILE_NAME, true);
                    File.Delete(ERROR_FILE_NAME_TEMP);
                }

            }
            catch { }
        }
    }
}
