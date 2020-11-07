using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TSensor.Web.Models.Repository;
using TSensor.Web.Models.Services;
using TSensor.Web.Models.Services.Log;

namespace TSensor.Web.Models.Broadcast
{
    public class BroadcastSensorValuesService : IHostedService, IDisposable
    {
        private readonly IBroadcastRepository _repository;
        private readonly IHubContext<BroadcastSensorValuesHub> _hubContext;
        private readonly FileLogService _logService;

        private Timer timer;
        private readonly int delay;

        public BroadcastSensorValuesService(IBroadcastRepository repository, IConfiguration configuration,
            FileLogService logService, IHubContext<BroadcastSensorValuesHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
            _logService = logService;

            delay = configuration.GetValue("dataRequestDelaySeconds", 3);
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(state =>
            {
                try
                {
                    var actualValues = _repository.GetActualSensorValues().ToDictionary(p => p.SensorGuid, p => p);

                    _hubContext.Clients.All.SendAsync("sensorupdate", actualValues, DateTime.Now.TicksJs());
                }
                catch (Exception exception)
                {
                    _logService.Write(LogCategory.Exception, exception.ToString());
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(delay));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}