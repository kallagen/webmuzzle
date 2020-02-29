using System;
using System.IO;

namespace TSensor.FillValues
{
    public class LoggerService
    {
        private readonly string FileName;
        private readonly bool IsError;

        public LoggerService(string fileName)
        {
            FileName = fileName;

            if (string.IsNullOrEmpty(fileName))
            {
                IsError = true;
            }
            else
            {
                if (!File.Exists(fileName))
                {
                    try
                    {
                        File.Create(FileName);
                    }
                    catch
                    {
                        IsError = true;
                    }
                }
            }
        }

        public void Write(object obj)
        {
            if (!IsError)
            {
                try
                {
                    using (var writer = new StreamWriter(FileName, true))
                    {
                        writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: {obj}");
                    }
                }
                catch { }
            }
        }
    }
}