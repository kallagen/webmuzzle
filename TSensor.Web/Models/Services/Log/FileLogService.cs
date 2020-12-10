using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace TSensor.Web.Models.Services.Log
{
    public class FileLogService
    {
        private static readonly Dictionary<LogCategory, ReaderWriterLock> LockCollection =
            new Dictionary<LogCategory, ReaderWriterLock> {
                { LogCategory.InputError, new ReaderWriterLock() },
                { LogCategory.RawInput, new ReaderWriterLock() },
                { LogCategory.Exception, new ReaderWriterLock() },
                { LogCategory.SystemException, new ReaderWriterLock() },
                { LogCategory.SmsLog, new ReaderWriterLock() },
                { LogCategory.SmsException, new ReaderWriterLock() },
                { LogCategory.EmailLog, new ReaderWriterLock() },
                { LogCategory.EmailException, new ReaderWriterLock() },
                { LogCategory.LiquidLevel, new ReaderWriterLock() },
            };

        private readonly string LogFilePath;

        private string Delim =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/" : "\\";

        public FileLogService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            LogFilePath = configuration.GetValue("logPath", $"{environment?.ContentRootPath}{Delim}log{Delim}");
        }

        public void Write(LogCategory category, string message)
        {
            try
            {
                if (!LockCollection.ContainsKey(category))
                {
                    return;
                }

                var locker = LockCollection[category];

                locker.AcquireWriterLock(5000);

                try
                {
                    var path = $"{LogFilePath}{Delim}{category}";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    File.AppendAllText(
                        $"{path}{Delim}{DateTime.Now:yyyyMMdd}.log",
                        $"{DateTime.Now:HH:mm:ss}: {message}");
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
            }
            catch { }
        }
    }
}
