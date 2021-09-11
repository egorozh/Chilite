using Grpc.Core;
using Grpc.Net.Client;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chilite.FrontendCore
{
    public static class ChannelExtensions
    {
        public static GrpcChannel ToAuthChannel(this HttpClient httpClient, string baseUri, string token) =>
            GrpcChannel.ForAddress(baseUri,
                new GrpcChannelOptions
                {
                    HttpClient = httpClient,
                    Credentials = ChannelCredentials.Create(new SslCredentials(),
                        GetJwtCredentials(token))
                });

        public static GrpcChannel ToAuthChannel(this HttpClient httpClient, string baseUri) =>
            GrpcChannel.ForAddress(baseUri,
                new GrpcChannelOptions
                {
                    HttpClient = httpClient,
                    Credentials = ChannelCredentials.SecureSsl
                });

        private static CallCredentials GetJwtCredentials(string token) =>
            CallCredentials.FromInterceptor((_, metadata) =>
            {
                if (!string.IsNullOrEmpty(token))
                    metadata.AddJwt(token);
                
                return Task.CompletedTask;
            });

        private static void AddJwt(this Metadata metadata, string token)
            => metadata.Add("Authorization", $"Bearer {token}");
    }
}