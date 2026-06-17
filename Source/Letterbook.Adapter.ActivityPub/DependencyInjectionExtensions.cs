using System.Net.Http.Headers;
using ActivityPub.Types;
using Letterbook.Adapter.ActivityPub.Signatures;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSign.Client;
using NSign.Signatures;
using AddContentDigestHandler = Letterbook.Adapter.ActivityPub.Signatures.AddContentDigestHandler;

namespace Letterbook.Adapter.ActivityPub;

public static class DependencyInjectionExtensions
{
	public static IHttpClientBuilder AddSigningClient(this IHttpClientBuilder clientBuilder)
	{
		clientBuilder.Services
			.AddTransient<AddContentDigestHandler>()
			.AddTransient<ClientHandler>()
			.AddTransient<IClientSigner, MastodonSigner>();
		return clientBuilder
			.AddHttpMessageHandler<AddContentDigestHandler>()
			.AddHttpMessageHandler<ClientHandler>();
	}

	public static IServiceCollection AddActivityPubClient(this IServiceCollection services, IConfiguration configuration)
	{
		var coreOptions = configuration.GetSection(CoreOptions.ConfigKey).Get<CoreOptions>() ?? throw ConfigException.Missing(nameof(CoreOptions));

		services.TryAddTypesModule();
		services
			.Configure<AddContentDigestOptions>(options => options.WithHash(AddContentDigestOptions.Hash.Sha256))
			.AddSingleton<IActivityPubDocument, Document>()
			.AddScoped<IVerificationKeyProvider, ActivityPubClientVerificationKeyProvider>()
			.ConfigureMessageSigningOptions(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.Method);
				options.WithMandatoryComponent(SignatureComponent.Authority);
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
				options.WithOptionalComponent(SignatureComponent.ContentDigest);
				options.WithOptionalComponent(new HttpHeaderComponent("Digest"));
				options.WithMandatoryComponent(new HttpHeaderComponent("Date"));
				options.SetParameters = component =>
				{
					component.WithCreatedNow();
					component.WithExpires(TimeSpan.FromSeconds(150));
				};
			})
			.Services
			.AddHttpClient<IActivityPubClient, Client>(client =>
			{
				client.DefaultRequestHeaders.Accept.ParseAdd(Constants.ActivityPubAccept);
				// TODO: get version from Product Version
				client.DefaultRequestHeaders.UserAgent.TryParseAdd($"Letterbook/0.0-dev ({coreOptions.DomainName})");
			})
			.AddSigningClient();

		return services;
	}
}