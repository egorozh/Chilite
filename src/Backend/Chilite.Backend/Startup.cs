using System;
using Chilite.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chilite.Backend
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddDbContext<ChatDbContext>(options => options
                .UseSqlite("Data Source=chat.db"), ServiceLifetime.Singleton);

            services.AddSingleton<ChatRoomManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseGrpcWeb();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb();
                endpoints.MapGrpcService<ChatRoomService>().EnableGrpcWeb();

                endpoints.MapFallbackToFile("index.html");
            });

            serviceProvider.GetService<ChatDbContext>().Database.EnsureCreated();
        }
    }
}
