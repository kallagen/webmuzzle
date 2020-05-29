using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TSensor.Web.Models.Security
{
    public class LicenseService : IHostedService
    {
        private Timer timer;
        private const int DELAY = 327;

        private readonly LicenseManager _licenseManager;

        public LicenseService(LicenseManager licenseManager)
        {
            _licenseManager = licenseManager;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(state =>
            {
                try
                {
                    _licenseManager.Update(DELAY);
                }
                catch (Exception exception)
                {
                }
            }, null, TimeSpan.FromSeconds(DELAY), TimeSpan.FromSeconds(DELAY));

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
