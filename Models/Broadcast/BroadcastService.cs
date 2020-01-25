using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TSensor.Web.Models.Repository;

namespace TSensor.Web.Models.Broadcast
{
    public class BroadcastService : IHostedService, IDisposable
    {
        private readonly IBroadcastRepository _repository;
        private readonly IHubContext<BroadcastHub> _hubContext;

        private Timer timer;
        private readonly int delay;

        public BroadcastService(IBroadcastRepository repository, IConfiguration configuration,
            IHubContext<BroadcastHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;

            delay = configuration.GetValue("dataRequestDelay", 1);
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(state =>
            {
                var actualValues = _repository.GetActualSensorValues().ToDictionary(p => p.SensorGuid, p => p);

                _hubContext.Clients.All.SendAsync("sensorupdate", actualValues,
                    DateTime.Now.ToString());

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