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
        /// <param name="throwMismatchException">Controls whether an exception is thrown from the service when there is a mismatch of properties between PlayerInfo and the API</param>
        /// <returns></returns>
        public static IServiceCollection AddOsrsWrapper(this IServiceCollection services, bool throwMismatchException = true)
        {
            services.AddHttpClient<IPlayerInfoService, PlayerInfoService>(x => new PlayerInfoService(x, throwMismatchException));

            return services;
        }
    }
}
