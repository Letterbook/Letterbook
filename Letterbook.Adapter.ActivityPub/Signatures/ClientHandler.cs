using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSign;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public class ClientHandler : DelegatingHandler
{
    private readonly ILogger<ClientHandler> _logger;
    private readonly IOptions<MessageSigningOptions> _signingOptions;
    private readonly IEnumerable<IClientSigner> _signers;

    public ClientHandler(ILogger<ClientHandler> logger, IOptions<MessageSigningOptions> signingOptions, IEnumerable<IClientSigner> signers)
    {
        _logger = logger;
        _signingOptions = signingOptions;
        _signers = signers;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Date ??= DateTimeOffset.Now;
        if (request.Content?.Headers.TryGetValues("Content-Digest", out var digest) == true)
            request.Headers.Add("Digest", digest);
        if (request.Options.TryGetValue(new HttpRequestOptionsKey<IEnumerable<Models.SigningKey>>(IClientSigner.SigningKeysOptionsId),
                out IEnumerable<Models.SigningKey>? keys))
        {
            // TODO: refactor before adding other signers
            // This works for mastodon, but we want to add spec-compliant signatures in the future
            
            // Ideally, this would support Accept-Signature negotiation
            // And if nothing else, we want at most a small set of signatures covering some common algorithms. Not a
            // combinatorial set of all keys and algorithms we can possibly support.
            var key = keys.FirstOrDefault(k => k.Family == Models.SigningKey.KeyFamily.Rsa);
            if (key == null)
            {
                _logger.LogWarning("Can't generate mastodon-structured signature because no RSA keys were available");
                return base.SendAsync(request, cancellationToken);
            }

            foreach (var signer in _signers)
            {
                signer.SignRequest(request, key);
            }
        }
        else
        {
            _logger.LogWarning("Can's sign request because no keys were available");
        }
        return base.SendAsync(request, cancellationToken);
    }
}