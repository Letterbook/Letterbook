using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.RegularExpressions;
using DarkLink.Web.WebFinger.Server;
using DarkLink.Web.WebFinger.Shared;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.Extensions.Options;

namespace Letterbook.Api;

public class WebfingerProvider : IResourceDescriptorProvider
{
	private readonly ILogger<WebfingerProvider> _logger;
	private readonly CoreOptions _options;
	private readonly IProfileService _profiles;

	public WebfingerProvider(ILogger<WebfingerProvider> logger, IOptions<CoreOptions> coreOptions,
		IProfileService profiles)
	{
		_logger = logger;
		_options = coreOptions.Value;
		_profiles = profiles;
	}

	public async Task<JsonResourceDescriptor?> GetResourceDescriptorAsync(Uri resource, IReadOnlyList<string> relations,
		HttpRequest request, CancellationToken cancellationToken)
	{
		var authority = _options.BaseUri().Authority;

		var match = Regex.Match(resource.AbsolutePath, "^.*@"
		                                               + authority
		                                               + "$");
		if (!match.Success)
		{
			_logger.LogInformation("Invalid Webfinger query for {Resource}", resource);
			_logger.LogDebug("Invalid Webfinger query from {UserAgent}",
				request.Headers.TryGetValue("User-Agent", out var ua) ? ua : "unknown");
			return default;
		}

		var handle = match.Value.Split('@', 2,
			StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
		if (handle == null) return default;
		var profile = await _profiles.As([]).FindProfiles(handle, authority).FirstOrDefaultAsync(cancellationToken: cancellationToken);
		if (profile != null)
		{
			var descriptor = JsonResourceDescriptor.Empty with
			{
				Subject = resource,
				Links = ImmutableList.Create(
					DarkLink.Web.WebFinger.Shared.Link.Create("self") with
					{
						Type = Core.Constants.ActivityPubAccept,
						Href = profile.FediId,
					}),
			};

			return descriptor;
		}

		return default;
	}
}