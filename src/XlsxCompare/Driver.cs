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
        private readonly XlsxComparer _comparer;
        private readonly ResultsWriter _resultsWriter;

        public Driver(IHostApplicationLifetime applicationLifetime, ILogger<Driver> logger, XlsxComparer comparer, ResultsWriter resultsWriter)
        {
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _comparer = comparer;
            _resultsWriter = resultsWriter;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");
            try
            {
                Run(Environment.GetCommandLineArgs());
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

        private void Run(string[] args)
        {
            // TODO: use a real options parser
            _logger.LogInformation("Reading config from {Path}", args[1]);
            var opts = CompareOptions.FromJsonFile(args[1]);
            var leftPath = args[2];
            var rightPath = args[3];
            var results = _comparer.Compare(leftPath, rightPath, opts);
            _resultsWriter.Write(results, opts.ResultOptions);
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
