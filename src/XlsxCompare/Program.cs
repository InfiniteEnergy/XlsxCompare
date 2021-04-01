using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace XlsxCompare
{
    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                new Startup(context.Configuration)
                    .ConfigureServices(services);
            });
    }
}
