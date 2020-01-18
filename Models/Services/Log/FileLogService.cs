using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TSensor.Web.Models.Services.Log
{
    public class FileLogService
    {
        private readonly string LogFilePath;

        public FileLogService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            LogFilePath = configuration.GetValue("logPath", $"{environment?.ContentRootPath}\\log\\");
        }

        public void Write(string header, string message)
        {
            var path = $"{LogFilePath}\\{header}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using var writer = File.AppendText($"{path}\\{DateTime.Now.ToString("yyyyMMdd")}.log");
            writer.Write($"{DateTime.Now.ToString("HH:mm:ss")}: {message}");
        }
    }
}
