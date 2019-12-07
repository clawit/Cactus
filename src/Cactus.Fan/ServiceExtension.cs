using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cactus.Fan
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddFan(this IServiceCollection services, string pgConnector)
        {
            services.AddHangfire(x => x.UsePostgreSqlStorage(pgConnector));
            return services;
        }

        public static IApplicationBuilder UseFan(this IApplicationBuilder app)
        {
            app.UseHangfireServer();

            app.UseHangfireDashboard("/cactus/fan");

            return app;
        }
    }
}
