using Cactus.Bus.Fan;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cactus.Bus
{
    public static class BusExtension
    {
        public static IServiceCollection AddBus(this IServiceCollection services, string pgConnector, Uri mqUri, string bus, bool durable = true)
        {
            BusManager.Instance = new RabbitBus.RabbitBus(mqUri, bus, durable);
            services.AddFan(pgConnector);
            return services;
        }

        public static IApplicationBuilder UseBus(this IApplicationBuilder app)
        {
            app.UseFan();
            return app;
        }
    }
}
