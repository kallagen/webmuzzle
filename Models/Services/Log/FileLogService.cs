using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;

namespace TSensor.Web.Models.Services.Log
{
    public class FileLogService
    {
        private static ReaderWriterLock InputErrorLocker = new ReaderWriterLock();
        private static ReaderWriterLock RawInputLocker = new ReaderWriterLock();

        private readonly string LogFilePath;

        public FileLogService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            LogFilePath = configuration.GetValue("logPath", $"{environment?.ContentRootPath}\\log\\");
        }

        public void Write(string header, string message)
        {
            switch (header)
            {
                case "inputerror":
                    Write(header, message, InputErrorLocker);
                    break;
                case "rawinput":
                    Write(header, message, RawInputLocker);
                    break;
                default: break;
            }
        }

        private void Write(string header, string message, ReaderWriterLock locker)
        {
            try
            {
                locker.AcquireWriterLock(5000);

                try
                {
                    var path = $"{LogFilePath}\\{header}";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    using var writer = File.AppendText($"{path}\\{DateTime.Now.ToString("yyyyMMdd")}.log");
                    writer.Write($"{DateTime.Now.ToString("HH:mm:ss")}: {message}");
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
