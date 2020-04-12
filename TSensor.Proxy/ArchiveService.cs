using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy
{
    public class ArchiveService
    {
        private readonly Config _config;
        private readonly ILogger _logger;

        private readonly BackgroundWorker worker;

        private readonly object Locker = new object();
        private const string FILE_NAME = "current.archived";

        public ArchiveService(Config config, ILogger logger)
        {
            _config = config;
            _logger = logger;

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            await SendAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(_config.ArchiveSendInterval);

            worker.RunWorkerAsync();
        }

        public void Run()
        {
            worker.RunWorkerAsync();
        }

        public void Write(string portName, string data)
        {
            try
            {
                lock (Locker)
                {
                    File.AppendAllLines(FILE_NAME, new[] { data });

                    _logger.Log("data archived", prefix: portName);
                }

                FinishCurrentArchive();
            }
            catch (Exception ex)
            {
                _logger.Log("data archiving error", prefix: portName);
                _logger.Log(ex.Message);
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
                            var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}.archived";
                            file.MoveTo(newFileName);

                            _logger.Log($"archive moved to {newFileName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"current.archive moving error");
                    _logger.Log(ex.Message);
                }
            }
        }

        private async Task SendAsync()
        {
            if (Http.Http.IsConnectionError)
            {
                _logger.Log("last data sending return connection error, ignore archive sending");

                return;
            }

            FinishCurrentArchive(forceMove: true);

            try
            {
                foreach (var archive in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.archived"))
                {
                    var fileName = Path.GetFileName(archive);
                    if (fileName != FILE_NAME)
                    {
                        using var elapsed = Elapsed.Create;

                        var result = await Http.Http.PostAsync(_config.ApiUrlSendArchive,
                            new Dictionary<string, string>
                            {
                                { "d", _config.DeviceGuid },
                                { "a", await File.ReadAllTextAsync(archive) }
                            });

                        if (result.Exception != null)
                        {
                            _logger.Log($"sending archive {fileName} error");
                            _logger.Log(result.Exception.Message);
                        }
                        else
                        {
                            _logger.Log($"archive {fileName} sended({result.Content})", elapsed: elapsed);

                            try
                            {
                                File.Delete(archive);

                                _logger.Log($"archive {fileName} removed");
                            }
                            catch (Exception ex)
                            {
                                _logger.Log($"removing archive {fileName} error");
                                _logger.Log(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"archive service working error");
                _logger.Log(ex.Message);
            }
        }
    }
}