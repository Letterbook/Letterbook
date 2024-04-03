using System.Diagnostics.CodeAnalysis;
using static NSign.Constants;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public class MastodonComponentBuilder : ISignatureComponentVisitor
{
	private readonly HttpRequestMessage _message;
	private List<string> _derivedParams = new List<string>();
	private List<string> _derivedParamsValues = new List<string>();
	private List<string> _headerParams = new List<string>();
	private List<string> _headerParamsValues = new List<string>();

	/* Draft 19 looks like
     * "@signature-params": ("@method" "@authority" "@path" "content-digest" "content-length" "content-type");created=1618884473;keyid="test-key-rsa-pss"
     */

	/* Mastodon/draft-cavage-8 looks like
     * headers="(request-target) host date"
     * Signature: keyId="https://my-example.com/actor#main-key",headers="(request-target) host date",signature="Y2FiYW...IxNGRiZDk4ZA=="
     */

	public string SigningDocument => BuildSigningDocument();
	public string? SigningDocumentSpec => BuildDocumentSpec();

	public MastodonComponentBuilder(HttpRequestMessage message)
	{
		_message = message;
	}

	/// <summary>
	/// Do nothing, this is not supported by draft-cavage-8
	/// </summary>
	/// <param name="component"></param>
	public void Visit(SignatureComponent component)
	{
	}

	public void Visit(HttpHeaderComponent httpHeader)
	{
		string fieldName = httpHeader.ComponentName;

		if (TryGetHeaderValues(fieldName, out var values))
		{
			// Mastodon only understands the Digest header, which is a non-standard header that was
			// replaced with content-digest in the httpsig drafts years ago
			AddHeader(
				fieldName.Equals("content-digest", StringComparison.InvariantCultureIgnoreCase) ? "digest" : fieldName,
				string.Join(", ", values));
		}
		else
		{
			if (fieldName == "host")
			{
				AddHeader(fieldName, _message.GetDerivedComponentValue(SignatureComponent.Authority));
			}
		}
	}

	/// <summary>
	/// Do nothing, this is not supported by draft-cavage-8
	/// </summary>
	/// <param name="httpHeaderDictionary"></param>
	public void Visit(HttpHeaderDictionaryStructuredComponent httpHeaderDictionary)
	{
	}

	/// <summary>
	/// Do nothing, this is not supported by draft-cavage-8
	/// </summary>
	/// <param name="httpHeaderStructuredField"></param>
	public void Visit(HttpHeaderStructuredFieldComponent httpHeaderStructuredField)
	{
	}

	/// <summary>
	/// Derived components are not really a general case thing in draft-cavage-8. But, the request target is a required
	/// component of the signature.
	/// </summary>
	/// <param name="derived"></param>
	public void Visit(DerivedComponent derived)
	{
		var method = new DerivedComponent(DerivedComponents.Method);
		switch (derived.ComponentName)
		{
			case DerivedComponents.RequestTarget:
				AddRequestTarget(
					$"{_message.GetDerivedComponentValue(method).ToLowerInvariant()} {_message.GetDerivedComponentValue(derived)}");
				break;
			case DerivedComponents.Authority:
				AddHeader("host", _message.GetDerivedComponentValue(derived));
				break;
		}
	}

	/// <summary>
	/// This is kind of the entry point to the whole builder. SignatureParams is the definition of all the other params
	/// that should make up the signing document. The Signer will visit the SignatureParamsComponent, and
	/// the builder should then visit all of its constituent components. 
	/// </summary>
	/// <param name="signatureParamsComponent"></param>
	public void Visit(SignatureParamsComponent signatureParamsComponent)
	{
		var hasTarget = false;
		foreach (SignatureComponent component in signatureParamsComponent.Components)
		{
			component.Accept(this);
			if (component is DerivedComponent { ComponentName: DerivedComponents.RequestTarget })
			{
				hasTarget = true;
			}
		}

		// draft-cavage-8 requires (I think?) including the request target in the signing document
		if (!hasTarget) new DerivedComponent(DerivedComponents.RequestTarget).Accept(this);
	}

	/// <summary>
	/// Do nothing, this is not supported by draft-cavage-8
	/// </summary>
	/// <param name="queryParam"></param>
	public void Visit(QueryParamComponent queryParam)
	{
	}

	#region Private Methods

	private bool TryGetHeaderValues(string header, [NotNullWhen(true)] out IEnumerable<string>? values)
	{
		return _message.Headers.TryGetValues(header, out values)
			   || (_message.Content != null && _message.Content.Headers.TryGetValues(header, out values));
	}
	// protected bool TryGetHeaderValues(string headerName, out IEnumerable<string> values)
	// {
	// return _message.Headers.TryGetValues(headerName, out values);
	// }

	private void AddHeader(string header, string value)
	{
		_headerParams.Add(header);
		_headerParamsValues.Add($"{header}: {value}");
	}

	private void AddRequestTarget(string value)
	{
		_derivedParams.Add("(request-target)");
		_derivedParamsValues.Add($"(request-target): {value}");
	}

	private string BuildSigningDocument()
	{
		return string.Join('\n', _derivedParamsValues.Concat(_headerParamsValues));
	}

	private string BuildDocumentSpec()
	{
		return string.Join(' ', _derivedParams.Concat(_headerParams));
	}

	#endregion
}