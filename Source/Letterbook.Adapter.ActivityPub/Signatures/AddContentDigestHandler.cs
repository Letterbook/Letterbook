using System.IO.Pipelines;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using NSign;
using NSign.Client;

namespace Letterbook.Adapter.ActivityPub.Signatures;

/// <summary>
/// An HttpMessageHandler that calculates digest hashes of the content and sets them in the relevant headers.
/// Adapted from <see cref="NSign.Client.AddContentDigestHandler"/>
/// </summary>
public class AddContentDigestHandler : DelegatingHandler
{
	public const string DigestHeader = "Digest";
	private readonly IOptions<AddContentDigestOptions> _options;

	public AddContentDigestHandler(IOptions<AddContentDigestOptions> options)
	{
		_options = options;
	}

	/// <inheritdoc/>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (request.Content is null) return await base.SendAsync(request, cancellationToken);

		foreach (var pair in _options.Value.Hashes.Select((alg, i) => new { alg, i }))
		{
			using var hash = GetConfiguredHash(pair.alg, out var algName);
			var digestValue = await GetContentDigestValueAsync(request.Content, hash);

			// IETF-9421 spec compliant
			request.Content.Headers.Add(Constants.Headers.ContentDigest, $"{algName}=:{digestValue}:");

			// Required by mastodon/draft-cavage-8 (note the missing colons. It's likely to be ambiguous if you try to use multiple algorithms)
			if (pair.i == 0)
				request.Content.Headers.Add(DigestHeader, $"{algName}={digestValue}");

		}
		return await base.SendAsync(request, cancellationToken);
	}

	private static async Task<string> GetContentDigestValueAsync(HttpContent content, HashAlgorithm hash)
	{
		var contentPipe = new Pipe();
		await CopyContentStream(content, contentPipe);

		await using var streamToHash = contentPipe.Reader.AsStream();
		var hashOutput = await hash.ComputeHashAsync(streamToHash);

		return Convert.ToBase64String(hashOutput, Base64FormattingOptions.None);
	}


	private static HashAlgorithm GetConfiguredHash(AddContentDigestOptions.Hash alg, out string algName)
	{
		switch (alg)
		{
			case AddContentDigestOptions.Hash.Sha256:
				algName = "sha-256";
				return SHA256.Create();

			case AddContentDigestOptions.Hash.Sha512:
				algName = "sha-512";
				return SHA512.Create();

			case AddContentDigestOptions.Hash.Unknown:
			default:
				throw new NotSupportedException($"Hash algorithm '{alg}' is not supported.");
		}
	}

	private static async Task CopyContentStream(HttpContent content, Pipe contentPipe)
	{
		// We need to get a copy of the HttpContent stream without breaking the request pipeline because the content
		// stream would otherwise be closed by the time it needs to be sent over the wire. So, we copy the stream to
		// a pipe, and read from that to calculate the hash value.
		await using var contentStream = contentPipe.Writer.AsStream();

		await content.CopyToAsync(contentStream);
	}
}