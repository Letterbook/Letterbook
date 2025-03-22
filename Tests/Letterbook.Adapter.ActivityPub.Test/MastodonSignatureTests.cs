using System.Net.Http.Headers;
using System.Security.Cryptography;
using Bogus;
using Letterbook.Adapter.ActivityPub.Signatures;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSign;
using static NSign.Constants;
using NSign.Signatures;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;


public class MastodonSignatureTests
{
	private readonly ITestOutputHelper _output;
	private readonly ServiceCollection _serviceCollection;
	private readonly ILogger<MastodonSigner> _logger;
	private readonly ILogger<MastodonVerifier> _verifierLogger;
	private RSA _rsa;
	private Models.SigningKey _signingKey;

	#region TestKeys

#pragma warning disable CS0414 // Field is assigned but its value is never used
	private string _testKeyRsaPublic =
#pragma warning restore CS0414 // Field is assigned but its value is never used
		"""
        -----BEGIN RSA PUBLIC KEY-----
        MIIBCgKCAQEAzf5EIIQ6LHWujJzGlNA2txC5174T6WIXQBTsu/n02/dEqL6kEZIV
        +/0QthIqRowdbuQTHfgE8qmooeSL6H6teNeaUOTsyJWnMxDsFarUDVZmZHzJy1Nf
        09j+BG32myD9459OddlM1Us5PVu3k+xK4eKMbx+pMu2eUzuZVsg7xDu2KZbJBbDJ
        3SD+SuNwbLmG72KnOYmCyp6W84ZVf6d2ji8IMF2k/mC2xUBRtJfohgCZTcnToeog
        kY6G1qq9sxen29oNiycc/WcrmL6AEx6ImGx35yOdxvOPB5FYUjzB7bn8muSV5JBU
        sZhLTLZ8le287KgO34OTCLnMeYnse51BGQIDAQAB
        -----END RSA PUBLIC KEY-----
        """;

	private string _testKeyRsaPrivate =
		"""
        -----BEGIN RSA PRIVATE KEY-----
        MIIEpAIBAAKCAQEAzf5EIIQ6LHWujJzGlNA2txC5174T6WIXQBTsu/n02/dEqL6k
        EZIV+/0QthIqRowdbuQTHfgE8qmooeSL6H6teNeaUOTsyJWnMxDsFarUDVZmZHzJ
        y1Nf09j+BG32myD9459OddlM1Us5PVu3k+xK4eKMbx+pMu2eUzuZVsg7xDu2KZbJ
        BbDJ3SD+SuNwbLmG72KnOYmCyp6W84ZVf6d2ji8IMF2k/mC2xUBRtJfohgCZTcnT
        oeogkY6G1qq9sxen29oNiycc/WcrmL6AEx6ImGx35yOdxvOPB5FYUjzB7bn8muSV
        5JBUsZhLTLZ8le287KgO34OTCLnMeYnse51BGQIDAQABAoIBAQCxG4tBlc5aiXfg
        x65pJjfU39mZJ4EBKOgqnZMI75jaQtfSac6wmLS0Klni4O1eKHvp6siQ/LxsUvh8
        8P5lj/zgKCcypBD9SMYvvr3sxyp4qS9x+GSbn3yFrUyBTHY53HzN5xtTcdiAjqOR
        ILlOwluDqP/rTwJvmiOFFnn5RkE0rkMIrNu55xGm4LP/j1NlAMXXzpQJxkWQC2jK
        OIB+hcC3Eo04NC+AnWGy3vQF7qavf0LkVpg+cDjtc+uPnk5nnljh3WsR/duuRLy6
        dQnJComrXjlU98NSySfp2ZWQ3FAIqudzZsybmTf8YmKsXMdcOm1uyXI5FmI4LrgN
        wMGTMCGNAoGBAO3oB+B4Qj3ulI6z+Iltm1rBPZVLQ9ndYmhhETmeeyhw3F3PrV3A
        SdGTjyF5erl7TfMyv6hLezxvxO0F/oqA9PcUINWq/RUQpaDy2h7/5Nj4ZF+aaP3N
        vxmtXGS/N/cTCtTuiyg3MLSc5iZFlHUqZR3GOqlpc4DHBGVtx+V6G5FjAoGBAN2o
        5UK5VZ15Qfsy3Aqeb6lXILa35GjVUJrNwBxQzN0K0kxpA7sD5SdxacXrNCuAfLDt
        9YkeqrhKdH8B4akd7sSIixhgD/HoVDd8rAlz/uNlbICwkSu0N7Z3cfKWHP2I2dom
        YW8C2yeDX3mUUoChTefqDDhbjyCPlc64qrNgx8pTAoGABejpqS3Tl25Bynm2BtPu
        NAbw3LCN8u+I7kbbAq9pJ8wF7V6nU9je/JHJ0G8QGWNywEPWdvvJB+tO3QR1GkOx
        0iFx31zsBIXxV1oxCOwaZzlkZOuVCBkAUkb4MJh/b2fNsRRr6IfWceYj4XeBBJgV
        AvRVqLex4tUOyuY7PPwXizkCgYApI0ZhSspD7pQ7TaYe5mas0/nDT3+5oRVTlan4
        11oeD/sVqUvC8qcd2eOakttc03zQzWkLaMCgcVkLlUrPOznsCbde770l1UuD/MRl
        AL0mcNVhUOOwa8MHP0XLkuaQe0yLOyJMGwiXXb9jbg6dxtIRn5NjBkMa9OsFsaok
        nmqV9wKBgQDlbohqEBOUIJrn+X6aw+Hffg3PQ1Pa//OPH3rixeo82OGODRozh5x5
        vR3iE5UnokwNVw54uaVxOAua417Stems54faYmsVOaqNUFoFRD1b4r1iF80Q2qY6
        f0EeAt13B99rqdgXE5DLGf7PppP2q/Z3zmR1w/tQv8x2HgJPkVHeXw==
        -----END RSA PRIVATE KEY-----
        """;

	#endregion

	public MastodonSignatureTests(ITestOutputHelper output)
	{
		_output = output;
		_serviceCollection = new ServiceCollection();
		_logger = Mock.Of<ILogger<MastodonSigner>>();
		_verifierLogger = Mock.Of<ILogger<MastodonVerifier>>();
		_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
		var faker = new Faker();

		_rsa = OperatingSystem.IsWindows() ? new RSACng() : new RSAOpenSsl();
		_rsa.ImportFromPem(_testKeyRsaPrivate);

		_signingKey = new Models.SigningKey()
		{
			Created = DateTimeOffset.UnixEpoch,
			Expires = DateTimeOffset.MaxValue,
			Family = Models.SigningKey.KeyFamily.Rsa,
			Id = faker.Random.Guid(),
			KeyOrder = 0,
			FediId = new Uri("http://letterbook.example/test/key#0"),
			Label = "Test key",
			PrivateKey = _rsa.ExportPkcs8PrivateKey(),
			PublicKey = _rsa.ExportSubjectPublicKeyInfo()
		};
	}

	[Fact(DisplayName = "Should sign a request")]
	public void TestSign()
	{
		_serviceCollection.AddOptions<MessageSigningOptions>()
			.Configure(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
			});
		var provider = _serviceCollection.BuildServiceProvider();

		var signer = new MastodonSigner(_logger, provider.GetRequiredService<IOptionsSnapshot<MessageSigningOptions>>());
		var actual = signer.SignRequest(new HttpRequestMessage(HttpMethod.Get, "http://example.com"), _signingKey);

		Assert.True(actual.Headers.Contains("Signature"));
		Assert.True(actual.Headers.Contains("Signature-Input"));
	}

	[Fact(DisplayName = "Should verify own signatures")]
	public void TestSignAndVerify()
	{
		_serviceCollection.AddOptions<MessageSigningOptions>()
			.Configure(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
			});
		var provider = _serviceCollection.BuildServiceProvider();

		var signer = new MastodonSigner(_logger, provider.GetRequiredService<IOptionsSnapshot<MessageSigningOptions>>());
		var actual = signer.SignRequest(new HttpRequestMessage(HttpMethod.Get, "http://example.com"), _signingKey);

		var verifier = new MastodonVerifier(_verifierLogger);
		Assert.Equal(VerificationResult.SuccessfullyVerified, verifier.VerifyRequestSignature(actual, _signingKey));
	}

	[Fact(DisplayName = "Should verify own signatures with all components")]
	public void TestSignAndVerifyConfig()
	{
		var b = new ConfigurationBuilder();
		b.AddInMemoryCollection(new Dictionary<string, string?>()
		{
			[$"{CoreOptions.ConfigKey}:{nameof(CoreOptions.DomainName)}"] = "test.letterbook.example"
		});
		_serviceCollection.AddActivityPubClient(b.Build());
		var provider = _serviceCollection.BuildServiceProvider();

		var signer = new MastodonSigner(_logger, provider.GetRequiredService<IOptionsSnapshot<MessageSigningOptions>>());
		var message = new HttpRequestMessage(HttpMethod.Post, "http://example.com");
		message.Content = new StringContent("hello world");
		// precomputed digests of "hello world" string content
		message.Content.Headers.Add("Content-Digest", "sha-256=:uU0nuZNNPgilLlLX2n2r+sSE7+N6U4DukIj3rOLvzek=:");
		message.Content.Headers.Add("Digest", "sha-256=uU0nuZNNPgilLlLX2n2r+sSE7+N6U4DukIj3rOLvzek=");

		var actual = signer.SignRequest(message, _signingKey);

		var verifier = new MastodonVerifier(_verifierLogger);
		Assert.Equal(VerificationResult.SuccessfullyVerified, verifier.VerifyRequestSignature(actual, _signingKey));
	}

	[Fact(DisplayName = "Should handle host/@authority")]
	public void TestSignAndVerifyHost()
	{
		_serviceCollection.AddOptions<MessageSigningOptions>()
			.Configure(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
				options.WithMandatoryComponent(SignatureComponent.Authority);
			});
		var provider = _serviceCollection.BuildServiceProvider();

		var signer = new MastodonSigner(_logger, provider.GetRequiredService<IOptionsSnapshot<MessageSigningOptions>>());
		var actual = signer.SignRequest(new HttpRequestMessage(HttpMethod.Get, "http://example.com"), _signingKey);

		var verifier = new MastodonVerifier(_verifierLogger);
		Assert.Equal("mastodon=(request-target) host", actual.Headers.GetValues("Signature-Input").First());
		Assert.Equal(VerificationResult.SuccessfullyVerified, verifier.VerifyRequestSignature(actual, _signingKey));
	}

	[Theory(DisplayName = "Should allow root-URI key ID with and without trailing slash")]
	[InlineData("http://letterbook.example#0")]
	[InlineData("http://letterbook.example/#0")]
	public void TestTrailingSlash(string keyId)
	{
		_serviceCollection.AddOptions<MessageSigningOptions>()
			.Configure(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
			});
		var provider = _serviceCollection.BuildServiceProvider();

		var signingKey = new Models.SigningKey()
		{
			Created = _signingKey.Created,
			Expires = _signingKey.Expires,
			Family = _signingKey.Family,
			Id = _signingKey.Id,
			KeyOrder = _signingKey.KeyOrder,
			FediId = new Uri(keyId),
			Label = _signingKey.Label,
			PrivateKey = _signingKey.PrivateKey,
			PublicKey = _signingKey.PublicKey
		};

		var actual = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
		actual.Headers.Add(Headers.SignatureInput, "mastodon=(request-target)");
		actual.Headers.Add(Headers.Signature, $"keyId=\"{keyId}\",headers=\"(request-target)\",signature=\"HyB9GWpVPsGH1IhoEXYvFMaFT7lPv/kmgMwR4XDCuekUcP9bDEtlMX02sACtibD7vT8K+548bnsQcxVvJcFhJiEjINX4C10vl8f6PGEdzFNh6vDe851Ro5kmEVj70IRdqmD3UoxL9H5d9GSkaMZGVH6O4OcwYnT244K3SkaUUorVlkNVDr0TrKRqDx509J6Dj0eR/zGPuYeqFb4G7iYxITxDZ50BZ/cm83ar9zEL3U4hnuR/dgeh4UZGD6RmEKbKUflt/o8fcoanosQuG4YT684lbWamryJ+DXh1rjgNRV/bKehFM9l+6Mf9CpSSFTOmD0ATet4cUQqtH0G4XSKcSA==\"");

		var verifier = new MastodonVerifier(_verifierLogger);
		Assert.Equal(VerificationResult.SuccessfullyVerified, verifier.VerifyRequestSignature(actual, signingKey));
	}

	[Theory(DisplayName = "Should ignore other derived components")]
	[InlineData(DerivedComponents.Method)]
	[InlineData(DerivedComponents.Scheme)]
	[InlineData(DerivedComponents.Path)]
	[InlineData(DerivedComponents.Query)]
	public void TestComponents(string name)
	{
		_serviceCollection.AddOptions<MessageSigningOptions>()
			.Configure(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
				options.WithMandatoryComponent(new DerivedComponent(name));
			});
		var provider = _serviceCollection.BuildServiceProvider();

		var signer = new MastodonSigner(_logger, provider.GetRequiredService<IOptionsSnapshot<MessageSigningOptions>>());
		var actual = signer.SignRequest(new HttpRequestMessage(HttpMethod.Get, "http://example.com"), _signingKey);
		Assert.Equal("mastodon=(request-target)", actual.Headers.GetValues("Signature-Input").First());

		var verifier = new MastodonVerifier(_verifierLogger);
		Assert.Equal(VerificationResult.SuccessfullyVerified, verifier.VerifyRequestSignature(actual, _signingKey));
	}

	[Theory(DisplayName = "Should include headers")]
	[InlineData(Headers.ContentLength)]
	[InlineData(Headers.ContentType)]
	[InlineData("date")]
	public void TestHeaders(string name)
	{
		_serviceCollection.AddOptions<MessageSigningOptions>()
			.Configure(options =>
			{
				options.WithMandatoryComponent(SignatureComponent.RequestTarget);
				options.WithMandatoryComponent(new HttpHeaderComponent(name));
			});
		var provider = _serviceCollection.BuildServiceProvider();

		var content = new StringContent("hello, world");
		var message = new HttpRequestMessage(HttpMethod.Post, "http://example.com")
		{
			Content = content,
		};
		message.Headers.Date = DateTimeOffset.UnixEpoch;
		message.Content.Headers.ContentLength = 100;
		message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
		var signer = new MastodonSigner(_logger, provider.GetRequiredService<IOptionsSnapshot<MessageSigningOptions>>());
		var actual = signer.SignRequest(message, _signingKey);
		Assert.Equal($"mastodon=(request-target) {name}", actual.Headers.GetValues("Signature-Input").First());

		var verifier = new MastodonVerifier(_verifierLogger);
		Assert.Equal(VerificationResult.SuccessfullyVerified, verifier.VerifyRequestSignature(actual, _signingKey));
	}
}