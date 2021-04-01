using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace XlsxCompare
{
    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(Configure)
            .ConfigureServices(Configure)
        ;

        private static void Configure(HostBuilderContext context, IServiceCollection services)
        {
            new Startup(context.Configuration)
                    .ConfigureServices(services);
        }

        static void Configure(ILoggingBuilder opts)
        {
            opts.AddSimpleConsole(Configure);
        }

        static void Configure(SimpleConsoleFormatterOptions opts)
        {
            opts.TimestampFormat = "[HH:mm:ss] ";
        }
    }
}
