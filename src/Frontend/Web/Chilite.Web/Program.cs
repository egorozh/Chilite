using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Chilite.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;

namespace Chilite.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

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


            await builder.Build().RunAsync();
        }
    }
}
