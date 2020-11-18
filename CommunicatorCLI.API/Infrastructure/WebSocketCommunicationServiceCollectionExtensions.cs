using CommunicatorCLI.API.Services;
using CommunicatorCLI.API.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicatorCLI.API.Infrastructure
{
    public static class BusinessServiceCollectionExtensions
    {
        public static IServiceCollection AddWebSocketCommunication(this IServiceCollection services, IStorage storage)
        {
            services.AddSingleton(typeof(WebSocketCommunicationService), new WebSocketCommunicationService(storage));
            return services;
        }
    }
}