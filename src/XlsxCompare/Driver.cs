using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace XlsxCompare
{
    /// <summary>
    /// DI-enabled entry point
    /// </summary>
    class Driver : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;

        public Driver(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: do work
            Environment.ExitCode = 0;
            _applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
