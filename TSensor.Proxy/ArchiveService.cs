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
        private readonly string FileName;

        public ArchiveService(Config config, ILogger logger)
        {
            _config = config;
            _logger = logger;

            FileName = FilePath(FILE_NAME);

            if (config.IsApiOutputMode)
            {
                worker = new BackgroundWorker();
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            }
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

        private string FilePath(string fileName)
        {
            return $"archived{(_config.IsLinux ? "/" : "\\")}{fileName}";
        }

        private string FolderPath(string folderName)
        {
            return $"{folderName}{(_config.IsLinux ? "/" : "\\")}archived";
        }

        public void Write(string portName, string data)
        {
            try
            {
                lock (Locker)
                {
                    File.AppendAllLines(FileName, new[] { data });

                    _logger.Log("data archived", prefix: portName);
                }

                FinishCurrentArchive();
            }
            catch (Exception ex)
            {
                _logger.Log("data archiving error", prefix: portName, isError: true);
                _logger.Log(ex.Message, isError: true);
            }
        }

        private void FinishCurrentArchive(bool forceMove = false)
        {
            lock (Locker)
            {
                try
                {
                    if (File.Exists(FileName))
                    {
                        var file = new FileInfo(FileName);
                        if (file.Length > _config.MaxArchiveFileSize || forceMove)
                        {
                            var newFileName = FilePath($"{DateTime.Now:yyyyMMddHHmmss}.archived");
                            file.MoveTo(newFileName);

                            _logger.Log($"archive moved to {newFileName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"current.archive moving error", isError: true);
                    _logger.Log(ex.Message, isError: true);
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
                foreach (var archive in Directory.GetFiles(FolderPath(Directory.GetCurrentDirectory()), "*.archived"))
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
                            _logger.Log($"sending archive {fileName} error", isError: true);
                            _logger.Log(result.Exception.Message, isError: true);
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
                                _logger.Log($"removing archive {fileName} error", isError: true);
                                _logger.Log(ex.Message, isError: true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"archive service working error", isError: true);
                _logger.Log(ex.Message, isError: true);
            }
        }
    }
}