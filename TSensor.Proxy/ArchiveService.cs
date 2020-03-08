using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TSensor.Proxy
{
    public class ArchiveService
    {
        private readonly Config _config;
        private readonly Logger _logger;

        private readonly object Locker = new object();
        private bool IsSendRunning = false;

        private const string FILE_NAME = "current.archived";

        private readonly Timer timer;

        public ArchiveService(Config config, Logger logger)
        {
            _config = config;
            _logger = logger;

            timer = new Timer(new TimerCallback(Send), null, 0, _config.ArchiveSendInterval);
        }

        public void Write(string portName, string data)
        {
            try
            {
                lock (Locker)
                {
                    File.AppendAllLines(FILE_NAME, new[] { data });

                    _logger.Write($"{portName}: data archived");
                }

                FinishCurrentArchive();
            }
            catch (Exception ex)
            {
                _logger.Write($"{portName}: data archiving error");
                _logger.Write(ex);
            }
        }

        private void FinishCurrentArchive(bool forceMove = false)
        {
            lock (Locker)
            {
                try
                {
                    if (File.Exists(FILE_NAME))
                    {
                        var file = new FileInfo(FILE_NAME);
                        if (file.Length > _config.MaxArchiveFileSize || forceMove)
                        {
                            var newFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.archived";
                            file.MoveTo(newFileName);

                            _logger.Write($"archive moved to {newFileName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Write($"current.archive moving error");
                    _logger.Write(ex);
                }
            }
        }
        private async void Send(object state)
        {
            if (IsSendRunning)
            {
                _logger.Write("archive service already running, skip sending");
            }
            else
            {
                IsSendRunning = true;

                if (!Http.IsConnectionError)
                {
                    FinishCurrentArchive(forceMove: true);

                    try
                    {
                        foreach (var archive in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.archived"))
                        {
                            var fileName = Path.GetFileName(archive);
                            if (fileName != FILE_NAME)
                            {
                                using (var elapsed = Elapsed.Create)
                                {
                                    var result = await Http.PostAsync(_config.ApiUrlSendArchive, new Dictionary<string, string>
                                    {
                                        { "d", _config.DeviceGuid },
                                        { "a", await File.ReadAllTextAsync(archive) }
                                    });

                                    if (result.Exception != null)
                                    {
                                        _logger.Write($"sending archive {fileName} error");
                                        _logger.Write(result.Exception);
                                    }
                                    else
                                    {
                                        _logger.Write($"archive {fileName} sended", elapsed);
                                        _logger.Write($"response: {result.Content}");

                                        try
                                        {
                                            File.Delete(archive);

                                            _logger.Write($"archive {fileName} removed");
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Write($"removing archive {fileName} error");
                                            _logger.Write(ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Write($"archive service working error");
                        _logger.Write(ex);
                    }
                }
                else
                {
                    _logger.Write("last post return connection error, ignore archive sending");
                }

                IsSendRunning = false;
            }
        }

        public void Run()
        {
            while (true) { }
        }
    }
}
