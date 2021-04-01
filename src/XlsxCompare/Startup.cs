using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XlsxCompare
{
    class Startup
    {
        public IConfiguration Configuration { get; init; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Driver>();
        }
    }
}
