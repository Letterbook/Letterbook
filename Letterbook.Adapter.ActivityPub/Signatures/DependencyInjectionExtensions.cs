using Microsoft.Extensions.DependencyInjection;
using NSign.Client;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public static class DependencyInjectionExtensions
{
    public static IHttpClientBuilder AddMastodonSignatures(this IHttpClientBuilder clientBuilder)
    {
        clientBuilder.AddContentDigestHandler()
            .Services
            .AddTransient<SigningHandler>()
            .AddTransient<IMessageSigner, ClientMessageSigner>();

        return clientBuilder.AddHttpMessageHandler<SigningHandler>();
    }
}