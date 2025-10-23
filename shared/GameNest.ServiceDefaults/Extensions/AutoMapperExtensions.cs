using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace GameNest.ServiceDefaults.Extensions
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Registers AutoMapper with profiles from the specified assembly.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assembly">Assembly containing AutoMapper profiles (typically BLL layer)</param>
        public static IServiceCollection AddAutoMapperWithLogging(
            this IServiceCollection services,
            Assembly assembly)
        {
            services.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(assembly);
                }, loggerFactory);

                return config.CreateMapper();
            });

            return services;
        }

        /// <summary>
        /// Registers AutoMapper with profiles from multiple assemblies.
        /// </summary>
        public static IServiceCollection AddAutoMapperWithLogging(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(assemblies);
                }, loggerFactory);

                return config.CreateMapper();
            });

            return services;
        }
    }
}
