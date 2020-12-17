using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Chilite.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chilite.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            builder.Services.AddSingleton(services =>
            {
                var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;

                ChannelBase chanel = GrpcChannel.ForAddress(baseUri,
                    new GrpcChannelOptions
                    {
                        HttpHandler = new GrpcWebHandler(new HttpClientHandler())
                    });
                return new ChatRoom.ChatRoomClient(chanel);
            });

            builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthorizeApi>();
            
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            var host = builder.Build();

            var culture = await host.Services.GetService<ILocalStorageService>().GetItemAsStringAsync("lang_culture");

            var defaultCultrure = new CultureInfo("ru-Ru");

            if (culture != null) 
                defaultCultrure = new CultureInfo(culture);

            CultureInfo.CurrentCulture = defaultCultrure;
            CultureInfo.CurrentUICulture = defaultCultrure;

            await host.RunAsync();
        }
    }
}