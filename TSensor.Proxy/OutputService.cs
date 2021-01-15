using System.Collections.Generic;
using System.Threading.Tasks;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy
{
    public class OutputService
    {
        private readonly Config _config;
        private readonly ILogger _logger;
        private readonly ArchiveService _archiveService;

        private readonly string _port;

        private void Log(string message, Elapsed elapsed = null, bool isError = false)
        {
            _logger.Log(message, prefix: _port, elapsed, isError);
        }

        public OutputService(string port, Config config, ILogger logger, ArchiveService archiveService)
        {
            _config = config;
            _logger = logger;
            _archiveService = archiveService;

            _port = port;
        }

        /// <summary>
        /// Отправляет данные полученные с контроллера в бэкенд
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public async Task Process(string strData)
        {
            using var elapsed = Elapsed.Create;
            var eventDate = elapsed.Start.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");

            if (_config.IsApiOutputMode)
            {
                var result = await Http.Http.PostAsync(_config.ApiUrlSendValue,
                    new Dictionary<string, string>
                    {
                        { "v", strData },
                        { "d", eventDate },
                        { "g", _config.DeviceGuid }
                    });

                if (result.Exception != null)
                {
                    Log("sending error", isError: true);
                    Log(result.Exception.Message, isError: true);

                    _archiveService.Write(_port, $"{_config.DeviceGuid};{eventDate};{strData}");
                }
                else
                {
                    Log($"data sended({result.Content})", elapsed);
                }
            }
            else if (_config.IsFileOutputMode)
            {
                _archiveService.Write(_port, $"{_config.DeviceGuid};{eventDate};{strData}");
            }
        }
    }
}
