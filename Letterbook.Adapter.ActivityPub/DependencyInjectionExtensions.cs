using System.Net.Http.Headers;
using Letterbook.Adapter.ActivityPub.Signatures;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSign.Client;
using NSign.Signatures;

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

	public static IServiceCollection AddActivityPubClient(this IServiceCollection services, ConfigurationManager configuration)
	{
		var coreOptions = configuration.GetSection(CoreOptions.ConfigKey).Get<CoreOptions>()
		                  ?? throw new ConfigException(nameof(CoreOptions));
		services
			.Configure<AddContentDigestOptions>(options => options.WithHash(AddContentDigestOptions.Hash.Sha256))
			.ConfigureMessageSigningOptions(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.Method);
				options.WithMandatoryComponent(SignatureComponent.Authority);
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
				options.WithOptionalComponent(SignatureComponent.ContentDigest);
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
				client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("dotnet",
					Environment.Version.ToString(2)));
				// TODO: get version from Product Version
				client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("letterbook", "0.0-dev"));
				client.DefaultRequestHeaders.UserAgent.TryParseAdd(coreOptions.DomainName);
			})
			.AddSigningClient();

		return services;
	}
}