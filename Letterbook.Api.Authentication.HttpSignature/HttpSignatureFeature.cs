namespace Letterbook.Api.Authentication.HttpSignature;

public class HttpSignatureFeature
{
	private IList<Uri> _validatedSignatures;

	public void Add(IEnumerable<Uri> validatedSignatures)
	{
		_validatedSignatures = validatedSignatures.ToList();
	}

	public IEnumerable<Uri> GetValidatedSignatures() => _validatedSignatures.AsReadOnly();
}