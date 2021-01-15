using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using TSensor.Proxy.Logger;

namespace TSensor.Proxy.Commands
{
    public class CommandsRepository
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger _logger;
        private readonly Config _config;


        public CommandsRepository(ILogger logger, Config config)
        {
            _logger = logger;
            _config = config;

        }

        public async Task<ControllerCommand> GetLastCommand()
        {
            var deviceGuid = _config.DeviceGuid;
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string requestUri = $"{_config.ApiUrlGetCommand}?deviceGuid={Uri.EscapeDataString(deviceGuid)}";
            
            var streamTask  = client.GetStreamAsync(requestUri);
            var command = await JsonSerializer.DeserializeAsync<ControllerCommand>(await streamTask);
            
            _logger.Log($"Got command {command.Command} with guid {command.Guid}");
            return command;
            
        }
        
    }
}