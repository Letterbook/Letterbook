using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Adapter.ActivityPub.Signatures;
using Letterbook.Core.Tests.Fakes;
using NSign;
using NSign.Signatures;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

/// <summary>
/// ClientSigner does very little except call into the parent methods
/// So, the only useful tests we can do is that it constructs properly and doesn't throw 
/// </summary>
public class SignerTests
{
    private readonly ITestOutputHelper _output;
    private KeyContainer _container = new KeyContainer();
    private FakeProfile _fakeProfile;
    private Models.Profile _profile;
    private ISigner _signer;
    
    public SignerTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine($"Bogus seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile("letterbook.example");
        _profile = _fakeProfile.Generate();
        _container.SetKey(_profile.Keys.First());
        
        _signer = new ClientSigner(_container);
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_signer);
    }
    
    [Fact]
    public void TestUpdateParams()
    {
        _signer.UpdateSignatureParams(new SignatureParamsComponent());
        Assert.True(true);
    }

    [Fact]
    public async Task TestSignAsync()
    {
        var bytes = "test signing data"u8.ToArray();
        var actual = await _signer.SignAsync(bytes, CancellationToken.None);
        
        Assert.False(actual.IsEmpty);
    }

    [Fact]
    public void MissingPrivateKeyTest()
    {
        var publicKey = new Models.SigningKey()
        {
            PublicKey = _profile.Keys.First().PublicKey
        };
        var container = new KeyContainer();
        container.SetKey(publicKey);
        Assert.Throws<ClientException>(() => new ClientSigner(container));
    }
    
    [Fact]
    public void MissingPublicKeyTest()
    {
        var container = new KeyContainer();
        Assert.Throws<ClientException>(() => new ClientSigner(container));
    }
}