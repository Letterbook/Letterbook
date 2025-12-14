using System.Security.Cryptography;
using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public sealed class FakeProfile : Faker<Profile>
{
	private const string TestKeyRsaPrivate =
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

	public FakeProfile() : this(new Uri(new Faker().Internet.UrlWithPath()))
	{
		// RuleFor(p => p.Id, f => f.Random.Guid());
	}

	public FakeProfile(string authority) : this(new Uri($"http://{authority}/{new Faker().Internet.UserName()}"))
	{
		// RuleFor(p => p.Id, f => f.Random.Guid());
	}

	public FakeProfile(Uri uri)
	{
		CustomInstantiator(f =>
		{
			var localId = f.Random.Uuid7();
			var builder = new UriBuilder(uri)
			{
				Fragment = null,
				Password = null,
				Path = null,
				Query = null,
				UserName = null,
			};

			builder.Path += $"actor/{localId.ToId25String()}";
			var id = builder.Uri;
			var basePath = builder.Path;
			builder.Path = basePath + "/inbox";
			var inbox = builder.Uri;
			builder.Path = basePath + "/outbox";
			var outbox = builder.Uri;
			builder.Path = "actor/shared_inbox";
			var sharedInbox = builder.Uri;
			builder.Path = basePath + "/followers";
			var followers = builder.Uri;
			builder.Path = basePath + "/following";
			var following = builder.Uri;
			var profile = Profile.CreateEmpty(id);
			{
				profile.Id = localId;
				profile.FediId = id;
				profile.Handle = f.Internet.UserName();
				profile.Inbox = inbox;
				profile.Outbox = outbox;
				profile.SharedInbox = sharedInbox;
				profile.Followers = followers;
				profile.Following = following;
				// profile.FollowersCollection = ObjectCollection<FollowerRelation>.Followers(id);
				// profile.FollowingCollection = ObjectCollection<FollowerRelation>.Following(id);
			}
			return profile;
		});

		RSA rsa = OperatingSystem.IsWindows() ? new RSACng() : new RSAOpenSsl();
		rsa.ImportFromPem(TestKeyRsaPrivate);

		RuleFor(p => p.DisplayName, (f) => f.Internet.UserName());
		RuleFor(p => p.Handle, (f, p) => p.Handle ?? $"@{f.Internet.UserName()}@{uri.Authority}");
		RuleFor(p => p.Description, (f) => f.Lorem.Paragraph());
		RuleFor(p => p.CustomFields,
			(f) => new CustomField[] { new() { Label = "UUID", Value = $"{f.Random.Guid()}" } });
		RuleFor(p => p.Keys,
			(f, profile) => new List<SigningKey>()
			{
				new SigningKey()
				{
					Created = f.Date.Past(1, DateTime.Parse("2020-01-01").ToUniversalTime()),
					Expires = DateTimeOffset.MaxValue,
					Family = SigningKey.KeyFamily.Rsa,
					Id = f.Random.Guid(),
					FediId = new Uri(uri, $"actor/{profile.GetId25()}/public_keys/0"),
					PrivateKey = rsa.ExportPkcs8PrivateKey(),
					PublicKey = rsa.ExportSubjectPublicKeyInfo(),
					KeyOrder = 0,
					Label = "Static test key"
				}
			});
		RuleFor(p => p.Headlining, (faker, profile) => new List<Audience>() { Audience.Followers(profile), Audience.FromMention(profile) });
	}

	public FakeProfile(Uri uri, Account owner) : this(uri)
	{
		CustomInstantiator(f =>
		{
			var profile = Profile.CreateIndividual(uri, $"{f.Hacker.Noun()}_{f.Random.Hexadecimal(4)}");
			profile.OwnedBy = owner;
			return profile;
		});
	}

	public FakeProfile(string authority, Account owner) : this(new Uri($"https://{authority}"), owner)
	{ }
}