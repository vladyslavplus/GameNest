using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace GameNest.OrderService.BLL.Mappings
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings(ILoggerFactory loggerFactory)
        {
            var config = new MapperConfigurationExpression();
            config.AddMaps(Assembly.GetExecutingAssembly());

            return new MapperConfiguration(config, loggerFactory);
        }
    }
}