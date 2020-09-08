using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy.Gps
{
    public class GpsService
    {
        private readonly BackgroundWorker worker;

        private readonly Config _config;
        private readonly ILogger _logger;

        public GpsService(Config config, ILogger logger)
        {
            _config = config;
            _logger = logger;

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private const string LINE_FORMAT = "$GPGLL";

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string line;

            try
            {
                using var stream = new FileStream(_config.GpsDevice, FileMode.Open);
                using var reader = new StreamReader(stream);
                while (true)
                {
                    line = await reader.ReadLineAsync();

                    if (line.StartsWith(LINE_FORMAT))
                    {
                        var lonlat = Coordinates.Parse(line);
                        _logger.Log($"coordinates: {lonlat}");

                        await SendCoordinatesAsync(lonlat);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log("coordinates read error", isError: true);
                _logger.Log(ex.Message, isError: true);
            }
        }

        private async Task SendCoordinatesAsync(Coordinates lonlat)
        {
            using var elapsed = Elapsed.Create;

            var result = await Http.Http.PostAsync(_config.ApiUrlSendCoordinates,
                new Dictionary<string, string>
                {
                    { "d", _config.DeviceGuid },
                    { "lon", lonlat.Longitude.ToString().Replace(",", ".") },
                    { "lat", lonlat.Latitude.ToString().Replace(",", ".") }
                });

            if (result.Exception != null)
            {
                _logger.Log($"coordinates sending error", isError: true);
                _logger.Log(result.Exception.Message, isError: true);
            }
            else
            {
                _logger.Log($"coordinates sended({result.Content})", elapsed: elapsed);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(_config.GpsSendInterval.Value);

            worker.RunWorkerAsync();
        }

        public void Run()
        {
            worker.RunWorkerAsync();
        }
    }
}
