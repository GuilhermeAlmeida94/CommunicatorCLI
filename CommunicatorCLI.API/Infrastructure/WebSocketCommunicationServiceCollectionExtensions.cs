using CommunicatorCLI.API.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicatorCLI.API.Infrastructure
{
    public static class BusinessServiceCollectionExtensions
    {
        
        public static IServiceCollection AddWebSocketCommunication(this IServiceCollection services)
        {
            services.AddSingleton(typeof(WebSocketCommunicationService), new WebSocketCommunicationService());
            return services;
        }
    }
}