using System.Security.Cryptography;
using Letterbook.Adapter.ActivityPub.Signatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSign;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Test;

public class SignatureExperiments
{
    private readonly ServiceCollection _serviceCollection;
    private readonly ILogger<MastodonSigner> _logger;

    public SignatureExperiments()
    {
        _serviceCollection = new ServiceCollection();
        _logger = Mock.Of<ILogger<MastodonSigner>>();
    }
    
    [Fact]
    public void Experiment()
    {
        _serviceCollection.AddOptions<MessageSigningOptions>()
            .Configure((options =>
            {
                options.WithMandatoryComponent(SignatureComponent.RequestTarget);
            }));
        var provider = _serviceCollection.BuildServiceProvider();
        
        // using var scope = provider.CreateScope();
        var rsa = Models.SigningKey.Rsa(0, new Uri("http://letterbook.example"));

        var signer = new MastodonSigner(_logger, provider.GetRequiredService<IOptionsSnapshot<MessageSigningOptions>>());
        var actual = signer.SignRequest(rsa, "test", new HttpRequestMessage(HttpMethod.Get, "http://example.com"),
            "test-sig");
        
        Assert.True(actual.Headers.Contains("Signature"));
        Assert.True(actual.Headers.Contains("Signature-Input"));
    }
}