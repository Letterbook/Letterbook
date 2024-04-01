using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Letterbook.Adapter.ActivityPub.Test;

public class PemTests
{
	public static IEnumerable<object[]> TestKeys()
	{
		// var stringWriter = new StringWriter();
		// var writer = new Org.BouncyCastle.OpenSsl.PemWriter(stringWriter);
		// writer.WriteObject();
		// IAsymmetricCipherKeyPairGenerator gen = new Ed25519KeyPairGenerator();
		// gen.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
		yield return new object[] { RSA.Create().ExportSubjectPublicKeyInfoPem(), "RSA" };
		yield return new object[] { DSA.Create().ExportSubjectPublicKeyInfoPem(), "DSA" };
		yield return new object[] { ECDsa.Create().ExportSubjectPublicKeyInfoPem(), "ECDSA" };
		// yield return new object[] { gen.GenerateKeyPair(), "ed25519" };
	}

	[Theory]
	[MemberData(nameof(TestKeys))]
	public void PublicKeyTest(string pem, string expected)
	{
		using TextReader tr = new StringReader(pem);

		var reader = new Org.BouncyCastle.OpenSsl.PemReader(tr);
		var pemObject = reader.ReadObject();
		var alg = "unknown";
		AsymmetricKeyParameter kp;
		switch (pemObject)
		{
			case RsaKeyParameters rsa:
				kp = rsa;
				alg = "RSA";
				break;
			case DsaKeyParameters dsa:
				kp = dsa;
				alg = "DSA";
				break;
			case ECKeyParameters ecdsa:
				kp = ecdsa;
				alg = "ECDSA";
				break;
			case Ed25519PublicKeyParameters ed25519:
				kp = ed25519;
				alg = "ed25519";
				break;
		}

		Assert.Equal(expected, alg);
	}
}