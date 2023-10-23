using NSign;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public class MastodonComponentBuilder : ISignatureComponentBuildVisitor
{
    private readonly MessageContext _context;
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
    
    public string SignatureInput => BuildSigningDocument();
    public string? SignatureParamsValue => BuildDocumentSpec();

    public MastodonComponentBuilder(MessageContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Do nothing, this is not supported by draft-cavage-8
    /// </summary>
    /// <param name="component"></param>
    public void Visit(SignatureComponent component)
    { }
    
    public void Visit(HttpHeaderComponent httpHeader)
    {
        bool bindRequest = httpHeader.BindRequest;
        string fieldName = httpHeader.ComponentName;

        if (TryGetHeaderValues(bindRequest, fieldName, out IEnumerable<string> values))
        {
            AddHeader(fieldName, String.Join(", ", values));
        }
    }
    
    public void Visit(HttpHeaderDictionaryStructuredComponent httpHeaderDictionary)
    {
        // TODO: needed?
        // bool bindRequest = httpHeaderDictionary.BindRequest;
        // string fieldName = httpHeaderDictionary.ComponentName;
        //
        // if (TryGetHeaderValues(bindRequest, fieldName, out IEnumerable<string> values) &&
        //     values.TryGetStructuredDictionaryValue(httpHeaderDictionary.Key, out ParsedItem? lastValue))
        // {
        //     Debug.Assert(lastValue.HasValue, "lastValue must have a value.");
        //
        //     AddInputWithKey(httpHeaderDictionary,
        //         lastValue.Value.Value.SerializeAsString() +
        //         lastValue.Value.Parameters.SerializeAsParameters());
        //     return;
        // }
        //
        // throw new SignatureComponentMissingException(httpHeaderDictionary);

        throw new NotImplementedException();
    }
    
    public void Visit(HttpHeaderStructuredFieldComponent httpHeaderStructuredField)
    {
        // TODO: needed?
        // bool bindRequest = httpHeaderStructuredField.BindRequest;
        // string fieldName = httpHeaderStructuredField.ComponentName;
        //
        // if (TryGetHeaderValues(bindRequest, fieldName, out IEnumerable<string> values))
        // {
        //     // Let's look up the type of the structured field so we know how to parse it.
        //     if (!_context.HttpFieldOptions.StructuredFieldsMap.TryGetValue(fieldName, out StructuredFieldType type))
        //     {
        //         throw new UnknownStructuredFieldComponentException(httpHeaderStructuredField);
        //     }
        //
        //     if (!type.TryParseStructuredFieldValue(values, out StructuredFieldValue structuredValue))
        //     {
        //         throw new StructuredFieldParsingException(fieldName, type);
        //     }
        //
        //     AddHeader(fieldName, structuredValue.Serialize());
        // }
        // else
        // {
        //     throw new SignatureComponentMissingException(httpHeaderStructuredField);
        // }
        throw new NotImplementedException();
    }

    /// <summary>
    /// Derived components are not really a general case thing in draft-cavage-8. But, the request target is a required
    /// component of the signature.
    /// </summary>
    /// <param name="derived"></param>
    public void Visit(DerivedComponent derived)
    {
        if (derived.ComponentName == Constants.DerivedComponents.RequestTarget)
            AddRequestTarget(_context.GetDerivedComponentValue(derived)!);
    }

    /// <summary>
    /// This is kind of the entry point to the whole builder. SignatureParams is the definition of all the other params
    /// that should make up the signing document. The Signer will visit the SignatureParamsComponent, and
    /// the builder should then visit all of its constituent components. 
    /// </summary>
    /// <param name="signatureParamsComponent"></param>
    public void Visit(SignatureParamsComponent signatureParamsComponent)
    {
        // TODO: visit and build everything
        var hasTarget = false;
        foreach (SignatureComponent component in signatureParamsComponent.Components)
        {
            component.Accept(this);
            if (component is DerivedComponent dc && dc.ComponentName == Constants.DerivedComponents.RequestTarget)
            {
                hasTarget = true;
            }
        }
        
        // cavage-draft-8 requires (I think?) including the request target in the signing document
        if (!hasTarget) new DerivedComponent(Constants.DerivedComponents.RequestTarget).Accept(this);
        
    }

    /// <summary>
    /// Do nothing, this is not supported by draft-cavage-8
    /// </summary>
    /// <param name="queryParam"></param>
    public void Visit(QueryParamComponent queryParam)
    {
    }

    #region Private Methods

    protected bool TryGetHeaderValues(bool bindRequest, string headerName, out IEnumerable<string> values)
    {
        if (bindRequest)
        {
            return TryGetRequestHeaderValues(headerName, out values);
        }
        else
        {
            return TryGetHeaderValues(headerName, out values);
        }
    }

    protected bool TryGetHeaderValues(string headerName, out IEnumerable<string> values)
    {
        values = _context.GetHeaderValues(headerName);
        return values.Any();
    }

    protected bool TryGetRequestHeaderValues(string headerName, out IEnumerable<string> values)
    {
        values = _context.GetRequestHeaderValues(headerName);
        return values.Any();
    }
    
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