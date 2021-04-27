using Blazored.LocalStorage;
using Chilite.Protos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Chilite.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
        {
            services.AddAuthGrpcClient<Account.AccountClient>();
            services.AddAuthGrpcClient<ChatRoom.ChatRoomClient>();

            services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();

            services.AddBlazoredLocalStorage();

            services.AddAuthorizationCore();
            services.AddScoped<AuthorizeApi>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
        }

        public async Task Configure(IServiceProvider serviceProvider)
        {
            await serviceProvider.UseLocalization();
        }
    }
}