using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Narik.Common.Web.Infrastructure
{
    public static class HostingExtension
    {
       // private static ServiceProviderFactory _factory;
        public static IWebHostBuilder UseAndConfigNarik(this IWebHostBuilder hostBuilder)
        {
            NLog.Web.NLogBuilder.ConfigureNLog("nlog.config");
            hostBuilder.ConfigureAppConfiguration(cfg =>
            {
                cfg.AddJsonFile("narik-config.json");
            });
            return hostBuilder;
        }
    }
}
