using GameNest.Grpc.Games;
using GameNest.GrpcClients.Clients;
using GameNest.GrpcClients.Interfaces;
using GameNest.ServiceDefaults.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameNest.GrpcClients.Extensions
{
    public static class GrpcClientRegistrationExtensions
    {
        public static IServiceCollection AddGameGrpcClient(this IServiceCollection services, IConfiguration configuration)
        {
            var catalogAddress = configuration["Grpc:CatalogService"] ?? "https://catalogservice-api";

            services.AddGrpcClient<GameGrpcService.GameGrpcServiceClient>(options =>
            {
                options.Address = new Uri(catalogAddress);
            })
            .ConfigureChannel(channelOptions =>
            {
                channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
                channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
            })
            .AddServiceDiscovery()
            .AddGrpcResilienceHandler(ResilienceProfile.Standard);

            services.AddScoped<IGameGrpcClient, GameGrpcClient>();

            return services;
        }
    }
}