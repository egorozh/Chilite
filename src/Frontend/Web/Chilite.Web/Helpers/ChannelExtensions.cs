using Chilite.FrontendCore;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace Chilite.Web
{
    internal static class ChannelExtensions
    {
        public static GrpcChannel GetAuthChannel(this NavigationManager navigation, string token) =>
            new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            .ToAuthChannel(navigation.BaseUri, token);
    }
}