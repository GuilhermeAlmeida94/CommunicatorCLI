using CommunicatorCLI.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;

namespace CommunicatorCLI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(DeviceService), new DeviceService());
            services.AddCors(opt => opt.AddDefaultPolicy(p => p.AllowAnyOrigin()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.UseCors();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var deviceService = (DeviceService)app.ApplicationServices.GetService(typeof(DeviceService));
                        await deviceService.FilterMessageByWebSocket(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
            app.UseStaticFiles();
        }
    }
}
