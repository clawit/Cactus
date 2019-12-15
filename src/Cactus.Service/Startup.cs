using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cactus.Service.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cactus.Bus;

namespace Cactus.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddBus("Host=192.168.31.102;Port=5432;DataBase=hangfire;Username=postgres;Password=n8f39gjk2j2rh83r7gv4wfh;Timeout=300",
                new Uri("amqp://activator:activator@192.168.31.102:5672"), "Cactus.Bus");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseBus();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<BusGrpcService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            //var _bus = new Bus.RabbitBus.RabbitBus(new Uri("amqp://activator:activator@61.160.212.32:35672"), "Cactus.Bus");
            //_bus.Subscribe("Event", async (c, p) => {
            //    Console.WriteLine($"Packet Time:{p.Data}");
            //    Console.WriteLine($"Now:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            //    return await Task.FromResult(true);

            //});
            //var packet = new Protocol.Model.Packet()
            //{
            //    Service = "Output",
            //    Command = "Print",
            //    Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            //};
            //packet.Args.Add(new string[] { "arg1", "arg2" });
            //packet.Options.Add(new Dictionary<string, string>() { { "opt1", "1" }, { "opt2", "2" } });
            //packet.Options.Add("TriggerAt", DateTime.Now.AddMinutes(2).ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //_bus.Publish("Event", packet);

        }
    }
}
