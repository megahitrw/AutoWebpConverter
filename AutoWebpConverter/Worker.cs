using Megacorp.AutoWebpConverter.Configuration;
using Megacorp.AutoWebpConverter.Services;
using Microsoft.Extensions.Options;

namespace Megacorp.AutoWebpConverter
{ 
    public sealed class AutoWebpConverterBackgroundService(ILogger<AutoWebpConverterBackgroundService> logger, IOptions<AutoWebpConverterConfiguration> serviceConfiguration) : BackgroundService
    {
        private AutoWebpConverterService _service;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (_service == null)
                {
                    _service = new AutoWebpConverterService(logger, serviceConfiguration);
                    _service.Start();
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }

                _service.Stop();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
