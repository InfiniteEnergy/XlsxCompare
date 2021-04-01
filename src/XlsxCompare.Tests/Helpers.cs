using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace XlsxCompare.Tests
{
    static class Helpers
    {
        private static readonly ILoggerFactory loggerFactory = LoggerFactory
            .Create(builder =>
            {
                builder.AddSimpleConsole(opts =>
                {
                    opts.ColorBehavior = LoggerColorBehavior.Disabled;
                });
            });

        public static ILogger<T> CreateLogger<T>() => loggerFactory.CreateLogger<T>();
    }
}
