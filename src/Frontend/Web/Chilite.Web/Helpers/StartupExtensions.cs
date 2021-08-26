using Blazored.LocalStorage;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Chilite.Web
{
    internal static class StartupExtensions
    {
        public static WebAssemblyHostBuilder AddRoot<T>(this WebAssemblyHostBuilder builder, string selector)
            where T : IComponent
        {
            builder.RootComponents.Add<T>(selector);
            return builder;
        }

        public static async Task<WebAssemblyHost> UseStartup(this WebAssemblyHostBuilder builder)
        {
            var startup = new Startup();
            startup.ConfigureServices(builder.Services, builder.HostEnvironment);
            var host = builder.Build();
            await startup.Configure(host.Services);
            return host;
        }

        public static async Task UseLocalization(this IServiceProvider serviceProvider)
        {
            var defaultCultrure = new CultureInfo("ru-Ru");

            var localStorage = serviceProvider.GetService<ILocalStorageService>();

            if (localStorage != null)
            {
                var culture = await localStorage
                    .GetItemAsStringAsync("lang_culture");

                if (culture != null)
                    defaultCultrure = new CultureInfo(culture);
            }

            CultureInfo.DefaultThreadCurrentCulture = defaultCultrure;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCultrure;
        }

        public static void AddAuthGrpcClient<T>(this IServiceCollection services) where T : ClientBase
        {
            services.AddScoped(provider =>
            {
                var nav = provider.GetService<NavigationManager>();
                var storage = provider.GetService<ISyncLocalStorageService>();

                if (nav != null && storage != null)
                {
                    string token = storage.GetItem<string>("token");

                    var client = (T?) Activator.CreateInstance(typeof(T), nav.GetAuthChannel(token));

                    if (client != null) 
                        return client;
                }

                return Activator.CreateInstance<T>();
            });
        }
    }
}