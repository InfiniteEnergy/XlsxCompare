using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace XlsxCompare
{
    /// <summary>
    /// DI-enabled entry point
    /// </summary>
    class Driver : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger _logger;

        public Driver(IHostApplicationLifetime applicationLifetime, ILogger<Driver> logger)
        {
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");
            try
            {
                // TODO: do work
                Environment.ExitCode = 0;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "uncaught error");
                Environment.ExitCode = -1;
            }
            finally
            {
                _logger.LogInformation("Finished");
            }

            _applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
