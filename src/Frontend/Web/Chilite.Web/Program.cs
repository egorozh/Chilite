using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Threading.Tasks;

namespace Chilite.Web
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = await WebAssemblyHostBuilder.CreateDefault(args)
                .AddRoot<App>("app")
                .UseStartup();

            await host.RunAsync();
        }
    }
}