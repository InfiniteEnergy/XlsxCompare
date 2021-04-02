using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace XlsxCompare
{
    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

        static IHostBuilder CreateHostBuilder(string[] args)
            => Host
                .CreateDefaultBuilder(args)
                .ConfigureLogging(builder =>
                {
                    builder.AddSimpleConsole(opts =>
                    {
                        opts.TimestampFormat = "[HH:mm:ss] ";
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    new Startup(context.Configuration)
                        .ConfigureServices(services);
                })
        ;
    }
}
