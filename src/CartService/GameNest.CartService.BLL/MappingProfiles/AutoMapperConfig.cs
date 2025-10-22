using AutoMapper;
using Microsoft.Extensions.Logging;

namespace GameNest.CartService.BLL.MappingProfiles
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings(ILoggerFactory loggerFactory)
        {
            var config = new MapperConfigurationExpression();
            config.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            return new MapperConfiguration(config, loggerFactory);
        }
    }
}
