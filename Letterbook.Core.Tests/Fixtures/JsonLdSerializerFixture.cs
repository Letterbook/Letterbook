// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using ActivityPub.Types;
using ActivityPub.Types.Conversion;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Core.Tests.Fixtures;

/// <summary>
///     Provides a pre-initialized <see cref="IJsonLdSerializer" /> instance for use in tests.
///     All loaded assemblies are registered.
///     This should be used to avoid the processing load of populating the caches from scratch on every single test.
/// </summary>
/// <seealso href="https://xunit.net/docs/shared-context#collection-fixture" />
[UsedImplicitly]
public sealed class JsonLdSerializerFixture
{
	public bool WriteIndented
	{
		get => JsonLdSerializer.SerializerOptions.WriteIndented;
		set
		{
			// This avoid a crash when set in a test constructor
			if (JsonLdSerializer.SerializerOptions is { WriteIndented: true, IsReadOnly: false })
				JsonLdSerializer.SerializerOptions.WriteIndented = value;
		}
	}

	public JsonLdSerializerFixture()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.TryAddTypesModule();
		serviceCollection.Configure<JsonLdSerializerOptions>(options =>
		{
			options.DefaultJsonSerializerOptions.WriteIndented = true;
		});
		var serviceProvider = serviceCollection.BuildServiceProvider();
		JsonLdSerializer = serviceProvider.GetRequiredService<IJsonLdSerializer>();
	}

	public IJsonLdSerializer JsonLdSerializer { get; }
}