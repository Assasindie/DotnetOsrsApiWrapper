using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetOsrsApiWrapper
{
    public static class Extensions
    {
        /// <summary>
        /// Add the DotNetOsrsWrapper a given IServiceCollection.
        /// Registers PlayerInfoService, along with an associated HttpClient
        /// </summary>
        /// <param name="services">The IServiceCollection DI Container</param>
        /// <returns></returns>
        public static IServiceCollection AddOsrsWrapper(this IServiceCollection services)
        {
            services.AddScoped<IPlayerInfoService, PlayerInfoService>();
            services.AddHttpClient<IPlayerInfoService, PlayerInfoService>();

            return services;
        }
    }
}
